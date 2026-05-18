namespace Runiq.Core.Metadata;

/// <summary>
/// Dashboard tarafından kullanılacak runtime metadata bilgilerini sağlar.
/// </summary>
public interface IRuntimeMetadataService
{
    /// <summary>
    /// Host uygulamada register edilmiş agent listesini döndürür.
    /// </summary>
    IReadOnlyList<AgentMetadataDto> GetAgents();
}