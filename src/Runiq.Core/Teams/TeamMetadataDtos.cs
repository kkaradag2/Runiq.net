namespace Runiq.Core.Teams;

/// <summary>
/// Dashboard tarafından görüntülenecek agent team metadata bilgisini taşır.
/// </summary>
public sealed record TeamMetadataDto(
    string Id,
    string Name,
    string Instructions,
    string ExecutionMode,
    IReadOnlyList<TeamMemberMetadataDto> Members);

/// <summary>
/// Dashboard tarafından görüntülenecek agent team üyesi metadata bilgisini taşır.
/// </summary>
public sealed record TeamMemberMetadataDto(
    string AgentId,
    string Role,
    string? Instructions);