using Runiq.Agents;

namespace Runiq.Core.Configuration;

/// <summary>
/// Runiq server tarafı runtime kayıt seçeneklerini taşır.
/// </summary>
public sealed class RuniqServerOptions
{
    private readonly List<Agent> _agents = [];

    /// <summary>
    /// Host uygulamada tanımlanan agent kayıtlarını döndürür.
    /// </summary>
    public IReadOnlyList<Agent> Agents => _agents;

    /// <summary>
    /// Runtime'a yeni bir agent kaydı ekler.
    /// </summary>
    public RuniqServerOptions AddAgent(Agent agent)
    {
        ArgumentNullException.ThrowIfNull(agent);

        _agents.Add(agent);

        return this;
    }
}