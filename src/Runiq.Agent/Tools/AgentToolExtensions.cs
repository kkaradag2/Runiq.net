namespace Runiq.Agents.Tools;

/// <summary>
/// Agent üzerine code-first tool eklemek için kullanılan extension metotlarını içerir.
/// </summary>
public static class AgentToolExtensions
{
    /// <summary>
    /// Agent'a tek bir typed Runiq tool ekler.
    /// </summary>
    /// <typeparam name="TTool">IRuniqTool&lt;TInput,TOutput&gt; uygulayan tool tipidir.</typeparam>
    /// <param name="agent">Tool eklenecek agent örneğidir.</param>
    /// <returns>Tool eklenmiş agent örneğini döner.</returns>
    public static Agent AddTool<TTool>(this Agent agent)
        where TTool : class
    {
        ArgumentNullException.ThrowIfNull(agent);

        agent.AddToolRegistration(
            AgentToolRegistration.FromToolType(typeof(TTool)));

        return agent;
    }
}