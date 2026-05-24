using System.Text;

namespace Runiq.Agents.Runtime;

/// <summary>
/// Agent sistem yönergelerini runtime context bilgileriyle zenginleştirir.
/// </summary>
internal static class AgentInstructionsBuilder
{
    /// <summary>
    /// Agent'ın temel yönergelerini, bağlı context space ve skill yönergeleriyle birleştirir.
    /// </summary>
    public static string Build(
        Agent agent,
        AgentRuntimeContext runtimeContext)
    {
        ArgumentNullException.ThrowIfNull(agent);
        ArgumentNullException.ThrowIfNull(runtimeContext);

        if (!runtimeContext.HasContext)
        {
            return agent.Instructions;
        }

        var builder = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(agent.Instructions))
        {
            builder.AppendLine(agent.Instructions.Trim());
            builder.AppendLine();
        }

        builder.AppendLine("## Runiq Context Spaces");
        builder.AppendLine();
        builder.AppendLine("The following context spaces are attached to this agent run.");
        builder.AppendLine("Use the provided skill instructions as operational guidance.");
        builder.AppendLine("Sources are listed as available context only. Do not claim to have read source contents unless their contents are explicitly provided by a tool, message, or future context capability.");
        builder.AppendLine();

        foreach (var contextSpace in runtimeContext.ContextSpaces)
        {
            builder.AppendLine($"### Context Space: {contextSpace.Name}");
            builder.AppendLine($"Id: {contextSpace.Id}");

            if (!string.IsNullOrWhiteSpace(contextSpace.Description))
            {
                builder.AppendLine($"Description: {contextSpace.Description}");
            }

            if (contextSpace.Sources.Count > 0)
            {
                builder.AppendLine();
                builder.AppendLine("Available sources:");

                foreach (var source in contextSpace.Sources)
                {
                    builder.Append("- ");
                    builder.Append(source.Name);
                    builder.Append(" (");
                    builder.Append(source.Kind);
                    builder.Append(')');

                    if (!string.IsNullOrWhiteSpace(source.Description))
                    {
                        builder.Append(": ");
                        builder.Append(source.Description);
                    }

                    builder.AppendLine();
                }
            }

            builder.AppendLine();
        }

        if (runtimeContext.Skills.Count > 0)
        {
            builder.AppendLine("## Runiq Skills");
            builder.AppendLine();
            builder.AppendLine("The following skills are active instructions for this run.");
            builder.AppendLine();

            foreach (var skill in runtimeContext.Skills)
            {
                builder.AppendLine($"### Skill: {skill.Name}");
                builder.AppendLine($"Id: {skill.Id}");

                if (!string.IsNullOrWhiteSpace(skill.Description))
                {
                    builder.AppendLine($"Description: {skill.Description}");
                }

                if (!string.IsNullOrWhiteSpace(skill.Version))
                {
                    builder.AppendLine($"Version: {skill.Version}");
                }

                if (skill.Tags.Count > 0)
                {
                    builder.AppendLine($"Tags: {string.Join(", ", skill.Tags)}");
                }

                builder.AppendLine($"Source: {skill.SourceId}/{skill.RelativePath}");
                builder.AppendLine();
                builder.AppendLine(skill.Instructions.Trim());
                builder.AppendLine();
            }
        }

        return builder.ToString().Trim();
    }
}