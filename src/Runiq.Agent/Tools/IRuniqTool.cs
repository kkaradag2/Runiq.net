namespace Runiq.Agents.Tools;

/// <summary>
/// Host uygulamanın code-first şekilde tanımladığı çalıştırılabilir Runiq tool sözleşmesini temsil eder.
/// </summary>
/// <typeparam name="TInput">Tool çalıştırılırken alınacak güçlü tipli input modelidir.</typeparam>
/// <typeparam name="TOutput">Tool çalıştırıldıktan sonra dönecek güçlü tipli output modelidir.</typeparam>
public interface IRuniqTool<TInput, TOutput>
{
    /// <summary>
    /// Tool'u verilen input ile çalıştırır.
    /// </summary>
    /// <param name="input">Tool input modelidir.</param>
    /// <param name="cancellationToken">İptal isteğini taşır.</param>
    /// <returns>Tool çalıştırma sonucunu döner.</returns>
    Task<TOutput> ExecuteAsync(
        TInput input,
        CancellationToken cancellationToken = default);
}