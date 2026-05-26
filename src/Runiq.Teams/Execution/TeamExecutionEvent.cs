namespace Runiq.Teams.Models.Execution;

/// <summary>
/// Agent team yürütmesi sırasında oluşan tek bir runtime event bilgisini taşır.
/// </summary>
public sealed record TeamExecutionEvent(
    TeamExecutionEventType Type,
    string TeamId,
    string? TeamName = null,
    string? MemberAgentId = null,
    string? MemberRole = null,
    string? Content = null,
    string? ErrorCode = null,
    string? ErrorMessage = null)
{
    /// <summary>
    /// Takım yürütmesi başladı event'i oluşturur.
    /// </summary>
    public static TeamExecutionEvent TeamStarted(
        string teamId,
        string teamName)
    {
        return new TeamExecutionEvent(
            Type: TeamExecutionEventType.TeamStarted,
            TeamId: NormalizeRequired(teamId, nameof(teamId)),
            TeamName: NormalizeRequired(teamName, nameof(teamName)));
    }

    /// <summary>
    /// Takım üyesi yürütmesi başladı event'i oluşturur.
    /// </summary>
    public static TeamExecutionEvent MemberStarted(
        string teamId,
        string memberAgentId,
        string memberRole)
    {
        return new TeamExecutionEvent(
            Type: TeamExecutionEventType.MemberStarted,
            TeamId: NormalizeRequired(teamId, nameof(teamId)),
            MemberAgentId: NormalizeRequired(memberAgentId, nameof(memberAgentId)),
            MemberRole: NormalizeRequired(memberRole, nameof(memberRole)));
    }

    /// <summary>
    /// Takım üyesinden gelen kısmi cevap event'i oluşturur.
    /// </summary>
    public static TeamExecutionEvent MemberDelta(
        string teamId,
        string memberAgentId,
        string memberRole,
        string content)
    {
        return new TeamExecutionEvent(
            Type: TeamExecutionEventType.MemberDelta,
            TeamId: NormalizeRequired(teamId, nameof(teamId)),
            MemberAgentId: NormalizeRequired(memberAgentId, nameof(memberAgentId)),
            MemberRole: NormalizeRequired(memberRole, nameof(memberRole)),
            Content: NormalizeRequired(content, nameof(content)));
    }

    /// <summary>
    /// Takım üyesi yürütmesi tamamlandı event'i oluşturur.
    /// </summary>
    public static TeamExecutionEvent MemberCompleted(
        string teamId,
        string memberAgentId,
        string memberRole,
        string? content = null)
    {
        return new TeamExecutionEvent(
            Type: TeamExecutionEventType.MemberCompleted,
            TeamId: NormalizeRequired(teamId, nameof(teamId)),
            MemberAgentId: NormalizeRequired(memberAgentId, nameof(memberAgentId)),
            MemberRole: NormalizeRequired(memberRole, nameof(memberRole)),
            Content: NormalizeOptional(content));
    }

    /// <summary>
    /// Takım üyesi yürütmesi hata aldı event'i oluşturur.
    /// </summary>
    public static TeamExecutionEvent MemberFailed(
        string teamId,
        string memberAgentId,
        string memberRole,
        string errorMessage,
        string? errorCode = null)
    {
        return new TeamExecutionEvent(
            Type: TeamExecutionEventType.MemberFailed,
            TeamId: NormalizeRequired(teamId, nameof(teamId)),
            MemberAgentId: NormalizeRequired(memberAgentId, nameof(memberAgentId)),
            MemberRole: NormalizeRequired(memberRole, nameof(memberRole)),
            ErrorCode: NormalizeOptional(errorCode),
            ErrorMessage: NormalizeRequired(errorMessage, nameof(errorMessage)));
    }

    /// <summary>
    /// Takım yürütmesi tamamlandı event'i oluşturur.
    /// </summary>
    public static TeamExecutionEvent TeamCompleted(
        string teamId,
        string? content = null)
    {
        return new TeamExecutionEvent(
            Type: TeamExecutionEventType.TeamCompleted,
            TeamId: NormalizeRequired(teamId, nameof(teamId)),
            Content: NormalizeOptional(content));
    }

    /// <summary>
    /// Takım yürütmesi hata aldı event'i oluşturur.
    /// </summary>
    public static TeamExecutionEvent TeamFailed(
        string teamId,
        string errorMessage,
        string? errorCode = null)
    {
        return new TeamExecutionEvent(
            Type: TeamExecutionEventType.TeamFailed,
            TeamId: NormalizeRequired(teamId, nameof(teamId)),
            ErrorCode: NormalizeOptional(errorCode),
            ErrorMessage: NormalizeRequired(errorMessage, nameof(errorMessage)));
    }

    private static string NormalizeRequired(
        string value,
        string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                $"{parameterName} cannot be empty.",
                parameterName);
        }

        return value.Trim();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}