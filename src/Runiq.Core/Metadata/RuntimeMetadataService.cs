using Runiq.Agents;
using Runiq.Agents.Tools;
using Runiq.ContextSpaces.Models;

namespace Runiq.Core.Metadata;

/// <summary>
/// Host uygulamada DI container'a register edilmiş agent, tool ve context space kayıtlarını metadata DTO modellerine map eder.
/// </summary>
internal sealed class RuntimeMetadataService : IRuntimeMetadataService
{
    private readonly IEnumerable<Agent> _agents;
    private readonly IReadOnlyList<AgentToolRegistration> _registeredTools;
    private readonly IReadOnlyList<ContextSpace> _contextSpaces;

    public RuntimeMetadataService(
        IEnumerable<Agent> agents,
        IReadOnlyList<AgentToolRegistration>? registeredTools = null,
        IReadOnlyList<ContextSpace>? contextSpaces = null)
    {
        _agents = agents;
        _registeredTools = registeredTools ?? [];
        _contextSpaces = contextSpaces ?? [];
    }

    public IReadOnlyList<AgentMetadataDto> GetAgents()
    {
        var contextSpacesById = _contextSpaces.ToDictionary(
            contextSpace => contextSpace.Id,
            StringComparer.OrdinalIgnoreCase);

        return _agents
            .Select(agent => new AgentMetadataDto(
                Id: agent.Id,
                Name: agent.Name,
                Instructions: agent.Instructions,
                Model: agent.Model,
                ReasoningEffort: agent.ReasoningEffort,
                Verbosity: agent.Verbosity,
                Tools: agent.Tools.Select(MapAgentTool).ToList(),
                ContextSpaces: agent.ContextSpaceIds
                    .Where(contextSpacesById.ContainsKey)
                    .Select(contextSpaceId => MapAgentContextSpace(
                        contextSpacesById[contextSpaceId]))
                    .ToList()))
            .ToList();
    }

    public IReadOnlyList<ToolMetadataDto> GetTools()
    {
        var agents = _agents.ToArray();

        return _registeredTools
            .Select(tool => new ToolMetadataDto(
                Name: tool.Name,
                DisplayName: FormatDisplayName(tool.Name),
                Description: tool.Description,
                TypeName: tool.ToolType.Name,
                InputType: tool.InputType.Name,
                OutputType: tool.OutputType.Name,
                HasInput: ToolJsonSchemaGenerator.HasInput(tool.InputType),
                InputSchema: ToolJsonSchemaGenerator.CreateSchema(tool.InputType),
                OutputSchema: ToolJsonSchemaGenerator.CreateSchema(tool.OutputType),
                AttachedAgents: agents
                    .Where(agent => agent.Tools.Any(agentTool =>
                        agentTool.Name.Equals(tool.Name, StringComparison.OrdinalIgnoreCase) &&
                        agentTool.ToolType == tool.ToolType))
                    .Select(agent => new ToolAttachedAgentMetadataDto(
                        Id: agent.Id,
                        Name: agent.Name))
                    .ToList()))
            .ToList();
    }

    public IReadOnlyList<ContextSpaceMetadataDto> GetContextSpaces()
    {
        var agents = _agents.ToArray();

        return _contextSpaces
            .Select(contextSpace => new ContextSpaceMetadataDto(
                Id: contextSpace.Id,
                Name: contextSpace.Name,
                Description: contextSpace.Description,
                Sources: contextSpace.Sources
                    .Select(source => new ContextSpaceSourceMetadataDto(
                        Id: source.Id,
                        Name: source.Name,
                        Kind: source.Kind.ToString(),
                        Description: source.Description))
                    .ToList(),
                AttachedAgents: agents
                    .Where(agent => agent.ContextSpaceIds.Any(contextSpaceId =>
                        contextSpaceId.Equals(contextSpace.Id, StringComparison.OrdinalIgnoreCase)))
                    .Select(agent => new ContextSpaceAttachedAgentMetadataDto(
                        Id: agent.Id,
                        Name: agent.Name))
                    .ToList()))
            .ToList();
    }

    private static AgentContextSpaceMetadataDto MapAgentContextSpace(
        ContextSpace contextSpace)
    {
        return new AgentContextSpaceMetadataDto(
            Id: contextSpace.Id,
            Name: contextSpace.Name,
            Description: contextSpace.Description);
    }

    private static AgentToolMetadataDto MapAgentTool(AgentToolRegistration tool)
    {
        return new AgentToolMetadataDto(
            Name: tool.Name,
            Description: tool.Description,
            InputType: tool.InputType.Name,
            OutputType: tool.OutputType.Name);
    }

    private static string FormatDisplayName(string value)
    {
        return string.Join(
            " ",
            value
                .Replace("-", " ")
                .Replace("_", " ")
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(part => char.ToUpperInvariant(part[0]) + part[1..]));
    }
}