using System.Runtime.CompilerServices;
using Runiq.Agents.Configuration;
using Runiq.Agents.Models;
using Runiq.Agents.Providers;
using Runiq.Agents.Providers.OpenAI;
using Runiq.Agents.Tools;

namespace Runiq.Agents;

/// <summary>
/// Runiq runtime içinde çalıştırılabilir bir AI agent tanımını temsil eder.
/// </summary>
public class Agent
{

    private readonly List<AgentToolRegistration> _tools = [];

    /// <summary>
    /// Agent'a code-first olarak eklenmiş tool kayıtlarını döner.
    /// </summary>
    public IReadOnlyList<AgentToolRegistration> Tools => _tools;

    public string Id { get; }

    public string Name { get; }

    public string Instructions { get; }

    public string Model { get; }

    public string ProviderName => ModelReference.ProviderName;

    public string ModelName => ModelReference.ModelName;

    public string? ApiKey { get; }

    public string ReasoningEffort { get; }

    public string Verbosity { get; }

    public ProviderOptions? Provider { get; }

    public ModelReference ModelReference { get; }


    public Agent(
        string id,
        string name,
        string instructions,
        string model,
        string? apiKey = null,
        ProviderOptions? provider = null,
            string reasoningEffort = "minimal",
    string verbosity = "low")
    {
        Id = ValidateRequired(id, nameof(id));
        Name = ValidateRequired(name, nameof(name));
        Instructions = instructions ?? string.Empty;
        Model = ValidateRequired(model, nameof(model));
        ModelReference = ModelReference.Parse(Model);
        ApiKey = apiKey;
        Provider = provider;
        ReasoningEffort = ValidateReasoningEffort(reasoningEffort);
        Verbosity = ValidateVerbosity(verbosity);
    }


    /// <summary>
    /// Agent cevabını tek seferlik tamamlanmış çıktı olarak üretir.
    /// </summary>
    public async Task<AgentExecutionResult> ExecuteAsync(
        string input,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return AgentExecutionResult.Failure(
                errorCode: "InputRequired",
                errorMessage: "Agent input cannot be empty.");
        }

        var validationFailure = ValidateProviderRuntime();

        if (validationFailure is not null)
        {
            return validationFailure;
        }

        var endpoint = ProviderDefaults.ResolveUrl(this);
        var providerDefault = ProviderDefaults.Get(ProviderName);

        return providerDefault.Protocol switch
        {
            ProviderProtocol.OpenAICompatible => await ExecuteOpenAICompatibleAsync(
                endpoint,
                input,
                cancellationToken),

            ProviderProtocol.Ollama => await ExecuteOllamaAsync(
                endpoint,
                input,
                cancellationToken),

            _ => AgentExecutionResult.Failure(
                errorCode: "UnsupportedProviderProtocol",
                errorMessage: $"Provider protocol '{providerDefault.Protocol}' is not supported.")
        };
    }

    /// <summary>
    /// Agent cevabını parça parça üretir.
    /// Studio tarafındaki canlı cevap akışı için kullanılır.
    /// </summary>
    public async IAsyncEnumerable<AgentExecutionEvent> ExecuteStreamAsync(
        string input,
        AgentToolInvoker? toolInvoker = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            yield break;
        }

        var validationFailure = ValidateProviderRuntime();

        if (validationFailure is not null)
        {
            yield return AgentExecutionEvent.Failed(validationFailure.ErrorMessage ?? "Agent stream request failed.");
            yield break;
        }

        var endpoint = ProviderDefaults.ResolveUrl(this);
        var providerDefault = ProviderDefaults.Get(ProviderName);

        switch (providerDefault.Protocol)
        {
            case ProviderProtocol.OpenAICompatible:
                await foreach (var executionEvent in ExecuteOpenAICompatibleStreamAsync(
                               endpoint,
                               input,
                               toolInvoker,
                               cancellationToken))
                {
                    yield return executionEvent;
                }

                yield break;

            case ProviderProtocol.Ollama:
                await foreach (var chunk in ExecuteOllamaStreamAsync(
                                   endpoint,
                                   input,
                                   cancellationToken))
                {
                    if (!string.IsNullOrEmpty(chunk))
                    {
                        yield return AgentExecutionEvent.AssistantDelta(chunk);
                    }
                }

                yield return AgentExecutionEvent.Completed();
                yield break;

            default:
                yield return AgentExecutionEvent.Failed($"Provider protocol '{providerDefault.Protocol}' is not supported.");
                yield break;
        }
    }

    private Task<AgentExecutionResult> ExecuteOpenAICompatibleAsync(
        Uri endpoint,
        string input,
        CancellationToken cancellationToken = default)
    {
        if (IsNativeOpenAIProvider())
        {
            return OpenAIResponsesClient.ExecuteAsync(
                agent: this,
                endpoint: endpoint,
                input: input,
                cancellationToken: cancellationToken);
        }

        return OpenAICompatibleClient.ExecuteAsync(
            agent: this,
            endpoint: endpoint,
            input: input,
            cancellationToken: cancellationToken);
    }

    private async IAsyncEnumerable<AgentExecutionEvent> ExecuteOpenAICompatibleStreamAsync(
        Uri endpoint,
        string input,
          AgentToolInvoker? toolInvoker = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (IsNativeOpenAIProvider())
        {
            await foreach (var executionEvent in OpenAIResponsesClient.StreamAsync(
                               agent: this,
                               endpoint: endpoint,
                               input: input,
                               toolInvoker: toolInvoker,
                               cancellationToken: cancellationToken))
            {
                yield return executionEvent;
            }

            yield break;
        }

        await foreach (var chunk in OpenAICompatibleClient.StreamAsync(
                           agent: this,
                           endpoint: endpoint,
                           input: input,
                           cancellationToken: cancellationToken))
        {
            if (!string.IsNullOrEmpty(chunk))
            {
                yield return AgentExecutionEvent.AssistantDelta(chunk);
            }
        }

        yield return AgentExecutionEvent.Completed();
    }


    private Task<AgentExecutionResult> ExecuteOllamaAsync(
        Uri endpoint,
        string input,
        CancellationToken cancellationToken = default)
    {
        var message =
            $"Agent '{Name}' will call Ollama endpoint '{endpoint}' with model '{ModelName}'. Input: {input}";

        return Task.FromResult(AgentExecutionResult.Success(message));
    }

    private async IAsyncEnumerable<string> ExecuteOllamaStreamAsync(
        Uri endpoint,
        string input,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var result = await ExecuteOllamaAsync(endpoint, input, cancellationToken);

        if (result.IsSuccess && !string.IsNullOrWhiteSpace(result.Message))
        {
            yield return result.Message;
        }
    }

    private AgentExecutionResult? ValidateProviderRuntime()
    {
        var providerDefault = ProviderDefaults.Get(ProviderName);
        var hasCustomUrl = !string.IsNullOrWhiteSpace(Provider?.Url);

        if (providerDefault.RequiresApiKey &&
            !hasCustomUrl &&
            string.IsNullOrWhiteSpace(ApiKey))
        {
            return AgentExecutionResult.Failure(
                errorCode: "ApiKeyMissing",
                errorMessage: $"Agent '{Id}' uses default provider endpoint for '{ProviderName}' but ApiKey is missing.");
        }

        return null;
    }

    private bool IsNativeOpenAIProvider()
    {
        return string.Equals(
            ProviderName,
            "openai",
            StringComparison.OrdinalIgnoreCase);
    }

    private static string ValidateRequired(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{parameterName} cannot be empty.", parameterName);
        }

        return value.Trim();
    }

    private static string ValidateReasoningEffort(string value)
    {
        var normalized = ValidateRequired(value, nameof(ReasoningEffort)).ToLowerInvariant();

        return normalized switch
        {
            "minimal" => normalized,
            "low" => normalized,
            "medium" => normalized,
            "high" => normalized,
            _ => throw new ArgumentException(
                "Reasoning effort must be one of: minimal, low, medium, high.",
                nameof(ReasoningEffort))
        };
    }

    private static string ValidateVerbosity(string value)
    {
        var normalized = ValidateRequired(value, nameof(Verbosity)).ToLowerInvariant();

        return normalized switch
        {
            "low" => normalized,
            "medium" => normalized,
            "high" => normalized,
            _ => throw new ArgumentException(
                "Verbosity must be one of: low, medium, high.",
                nameof(Verbosity))
        };
    }

    /// <summary>
    /// Agent'a yeni bir tool kaydı ekler.
    /// </summary>
    /// <param name="tool">Eklenecek tool kaydıdır.</param>
    internal void AddToolRegistration(AgentToolRegistration tool)
    {
        ArgumentNullException.ThrowIfNull(tool);

        if (_tools.Any(existing =>
                existing.Name.Equals(tool.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                $"Agent '{Id}' already has a tool named '{tool.Name}'.");
        }

        _tools.Add(tool);
    }

}