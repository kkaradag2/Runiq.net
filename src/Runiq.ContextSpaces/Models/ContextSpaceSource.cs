namespace Runiq.ContextSpaces.Models;

/// <summary>
/// Context space içinde agent'ın erişebileceği bir bilgi kaynağını temsil eder.
/// </summary>
public sealed class ContextSpaceSource
{
    /// <summary>
    /// Kaynak için benzersiz teknik kimliği ifade eder.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Dashboard ve metadata çıktılarında gösterilecek okunabilir kaynak adını ifade eder.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Kaynağın türünü ifade eder.
    /// </summary>
    public ContextSpaceSourceKind Kind { get; }

    /// <summary>
    /// Kaynağın okunabilir açıklamasını ifade eder.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Yeni bir context space source örneği oluşturur.
    /// </summary>
    /// <param name="id">Kaynak için benzersiz teknik kimlik.</param>
    /// <param name="name">Kaynak için okunabilir ad.</param>
    /// <param name="kind">Kaynağın türü.</param>
    /// <param name="description">Kaynağın isteğe bağlı açıklaması.</param>
    public ContextSpaceSource(
        string id,
        string name,
        ContextSpaceSourceKind kind,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Context space source id cannot be empty.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Context space source name cannot be empty.", nameof(name));
        }

        Id = id.Trim();
        Name = name.Trim();
        Kind = kind;
        Description = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();
    }
}