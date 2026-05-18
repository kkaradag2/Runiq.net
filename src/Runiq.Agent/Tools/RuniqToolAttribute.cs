namespace Runiq.Agents.Tools;

/// <summary>
/// Bir Runiq tool sınıfının model tarafında hangi ad ve açıklama ile görüneceğini belirtir.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class RuniqToolAttribute : Attribute
{
    /// <summary>
    /// Tool adını açıklama olmadan oluşturur.
    /// </summary>
    /// <param name="name">Model tarafına gönderilecek benzersiz tool adıdır.</param>
    public RuniqToolAttribute(string name)
        : this(name, string.Empty)
    {
    }

    /// <summary>
    /// Tool adını ve açıklamasını oluşturur.
    /// </summary>
    /// <param name="name">Model tarafına gönderilecek benzersiz tool adıdır.</param>
    /// <param name="description">Tool'un ne işe yaradığını açıklayan kısa metindir.</param>
    public RuniqToolAttribute(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Tool name cannot be empty.", nameof(name));
        }

        Name = name.Trim();
        Description = description?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// Model tarafında kullanılacak tool adıdır.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Model tarafına gönderilecek tool açıklamasıdır.
    /// </summary>
    public string Description { get; }
}