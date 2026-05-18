namespace Runiq.Core.Metadata;

/// <summary>
/// Studio tarafına dönen agent metadata bilgisini temsil eder.
/// </summary>
public sealed record AgentMetadataDto(
    string Id,
    string Name,
    string Instructions,
    string Model,
    string ReasoningEffort,
    string Verbosity,
    IReadOnlyList<AgentToolMetadataDto> Tools);

/// <summary>
/// Studio tarafında gösterilecek agent tool metadata bilgisini temsil eder.
/// </summary>
public sealed record AgentToolMetadataDto(
    string Name,
    string Description,
    string InputType,
    string OutputType);