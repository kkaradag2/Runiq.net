using Runiq.ContextSpaces.Models;

namespace SampleApp.Contexts;

/// <summary>
/// Travel Agent için kullanılan örnek seyahat planlama context space tanımını sağlar.
/// </summary>
internal static class TravelPlanningContext
{
    /// <summary>
    /// Travel Agent'ın kullanacağı seyahat planlama context space örneğini oluşturur.
    /// </summary>
    public static ContextSpace Create()
    {
        return new ContextSpace(
                id: "travel-planning",
                name: "Travel Planning Context",
                description: "Shared read-only context for city trip planning agents.")
            .AddSource(new ContextSpaceSource(
                id: "travel-docs",
                name: "Travel Documents",
                kind: ContextSpaceSourceKind.UploadedDocuments,
                description: "Sample travel planning documents and city guide notes."));
    }
}