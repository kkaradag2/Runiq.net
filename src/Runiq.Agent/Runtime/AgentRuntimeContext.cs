using Runiq.ContextSpaces.Models;
using Runiq.ContextSpaces.Models.Skills;

namespace Runiq.Agents.Runtime;

/// <summary>
/// Bir agent çalıştırması sırasında çözümlenen context space ve skill bilgilerini temsil eder.
/// </summary>
public sealed record AgentRuntimeContext(
    IReadOnlyList<ContextSpace> ContextSpaces,
    IReadOnlyList<ContextSpaceSkill> Skills)
{
    /// <summary>
    /// Çalıştırma için herhangi bir context bilgisinin çözülüp çözülmediğini belirtir.
    /// </summary>
    public bool HasContext => ContextSpaces.Count > 0 || Skills.Count > 0;
}