using Runiq.Agents;
using Runiq.Agents.Tools;


namespace Runiq.Core.Metadata;

/// <summary>
/// Host uygulamada DI container'a register edilmiş agent kayıtlarını metadata DTO modellerine map eder.
/// </summary>
internal sealed class RuntimeMetadataService : IRuntimeMetadataService
{
    private readonly IEnumerable<Agent> _agents;

    public RuntimeMetadataService(IEnumerable<Agent> agents)
    {
        _agents = agents;
    }

    public IReadOnlyList<AgentMetadataDto> GetAgents()
    {
        return _agents
            .Select(agent => new AgentMetadataDto(
                Id: agent.Id,
                Name: agent.Name,
                Instructions: agent.Instructions,
                Model: agent.Model,
                ReasoningEffort: agent.ReasoningEffort,
                Verbosity: agent.Verbosity,
                Tools: agent.Tools.Select(MapTool).ToList()))
            .ToList();
    }

    private static AgentToolMetadataDto MapTool(AgentToolRegistration tool)
    {
        return new AgentToolMetadataDto(
            Name: tool.Name,
            Description: tool.Description,
            InputType: tool.InputType.Name,
            OutputType: tool.OutputType.Name);
    }
}
