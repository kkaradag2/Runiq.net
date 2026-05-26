namespace Runiq.Teams.Models.Teams;

/// <summary>
/// Bir agent takımında yer alan agent referansını ve bu agent'ın takım içindeki rolünü temsil eder.
/// </summary>
public sealed class TeamMember
{
    /// <summary>
    /// Takım üyesi tanımını oluşturur.
    /// </summary>
    /// <param name="agentId">Takımda çalıştırılacak kayıtlı agent kimliği.</param>
    /// <param name="role">Agent'ın takım içindeki rol adı.</param>
    /// <param name="instructions">Bu üyeye özel ek yürütme yönergesi.</param>
    public TeamMember(
        string agentId,
        string role,
        string? instructions = null)
    {
        if (string.IsNullOrWhiteSpace(agentId))
        {
            throw new ArgumentException("Agent id cannot be empty.", nameof(agentId));
        }

        if (string.IsNullOrWhiteSpace(role))
        {
            throw new ArgumentException("Role cannot be empty.", nameof(role));
        }

        AgentId = agentId.Trim();
        Role = role.Trim();
        Instructions = string.IsNullOrWhiteSpace(instructions)
            ? null
            : instructions.Trim();
    }

    /// <summary>
    /// Takımda çalıştırılacak kayıtlı agent kimliği.
    /// </summary>
    public string AgentId { get; }

    /// <summary>
    /// Agent'ın takım içindeki rol adı.
    /// </summary>
    public string Role { get; }

    /// <summary>
    /// Bu üyeye özel ek yürütme yönergesi.
    /// </summary>
    public string? Instructions { get; }
}