using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Runiq.Agents;
using Runiq.Agents.Tools;
using System.Text.Json;

namespace Runiq.Core.Agents;

/// <summary>
/// Runiq agent execution endpoint'lerini ASP.NET Core uygulamasına ekler.
/// </summary>
public static class AgentEndpointExtensions
{
    private static readonly JsonSerializerOptions StreamJsonOptions = new(JsonSerializerDefaults.Web);
    /// <summary>
    /// Studio tarafından kullanılan agent execution endpoint'lerini map eder.
    /// </summary>
    public static IEndpointRouteBuilder MapRuniqAgentApi(
        this IEndpointRouteBuilder endpoints,
        string pathPrefix = "/runiq/api")
    {
        var group = endpoints.MapGroup(pathPrefix);

        group.MapPost("/agents/{agentId}/chat", async (
            string agentId,
            AgentChatRequest request,
            IEnumerable<Agent> agents,
            CancellationToken cancellationToken) =>
        {
            var agent = agents.FirstOrDefault(
                item => string.Equals(item.Id, agentId, StringComparison.OrdinalIgnoreCase));

            if (agent is null)
            {
                return Results.NotFound(new AgentChatResponse(
                    IsSuccess: false,
                    Message: null,
                    ErrorCode: "AgentNotFound",
                    ErrorMessage: $"Agent '{agentId}' was not found."));
            }

            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return Results.BadRequest(new AgentChatResponse(
                    IsSuccess: false,
                    Message: null,
                    ErrorCode: "MessageRequired",
                    ErrorMessage: "Message is required."));
            }

            var result = await agent.ExecuteAsync(request.Message, cancellationToken);

            return Results.Ok(new AgentChatResponse(
                IsSuccess: result.IsSuccess,
                Message: result.Message,
                ErrorCode: result.ErrorCode,
                ErrorMessage: result.ErrorMessage));
        });

        group.MapPost("/agents/{agentId}/chat/stream", async (
            string agentId,
            AgentChatRequest request,
            IEnumerable<Agent> agents,
            HttpContext httpContext,
            CancellationToken cancellationToken) =>
        {
            var agent = agents.FirstOrDefault(
                item => string.Equals(item.Id, agentId, StringComparison.OrdinalIgnoreCase));

            if (agent is null)
            {
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

                await httpContext.Response.WriteAsJsonAsync(
                    new AgentChatResponse(
                        IsSuccess: false,
                        Message: null,
                        ErrorCode: "AgentNotFound",
                        ErrorMessage: $"Agent '{agentId}' was not found."),
                    cancellationToken);

                return;
            }

            if (string.IsNullOrWhiteSpace(request.Message))
            {
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

                await httpContext.Response.WriteAsJsonAsync(
                    new AgentChatResponse(
                        IsSuccess: false,
                        Message: null,
                        ErrorCode: "MessageRequired",
                        ErrorMessage: "Message is required."),
                    cancellationToken);

                return;
            }

            httpContext.Response.ContentType = "text/event-stream; charset=utf-8";
            httpContext.Response.Headers.CacheControl = "no-cache";
            httpContext.Response.Headers.Connection = "keep-alive";
            var toolInvoker = new AgentToolInvoker(httpContext.RequestServices);

            await foreach (var executionEvent in agent.ExecuteStreamAsync(request.Message,
                toolInvoker,
                cancellationToken))
            {
                var streamEvent = AgentChatStreamEventMapper.FromExecutionEvent(executionEvent);
                var payload = JsonSerializer.Serialize(streamEvent, StreamJsonOptions);

                await httpContext.Response.WriteAsync($"data: {payload}\n\n", cancellationToken);
                await httpContext.Response.Body.FlushAsync(cancellationToken);
            }

            await httpContext.Response.WriteAsync("data: [DONE]\n\n", cancellationToken);
            await httpContext.Response.Body.FlushAsync(cancellationToken);
        });

        return endpoints;
    }
}