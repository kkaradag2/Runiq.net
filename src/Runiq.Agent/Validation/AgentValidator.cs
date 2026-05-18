namespace Runiq.Agents.Validation
{
    /// <summary>
    /// Agent kayıtlarının runtime başlamadan önce geçerli ve tutarlı olup olmadığını doğrular.
    /// </summary>
    public static class AgentValidator
    {
        /// <summary>
        /// Kayıtlı agent listesini doğrular. Hata bulunursa uygulamanın startup sırasında durması için exception fırlatır.
        /// </summary>
        public static void ValidateRegisteredAgents(IEnumerable<Agent> agents)
        {
            ArgumentNullException.ThrowIfNull(agents);

            var agentList = agents.ToList();

            ValidateDuplicateIds(agentList);

            foreach (var agent in agentList)
            {
                ValidateAgent(agent);
            }
        }

        private static void ValidateDuplicateIds(IReadOnlyCollection<Agent> agents)
        {
            var duplicateIds = agents
                .GroupBy(agent => agent.Id, StringComparer.OrdinalIgnoreCase)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToArray();

            if (duplicateIds.Length > 0)
            {
                throw new InvalidOperationException(
                    $"Runiq agent registration failed. Duplicate agent id detected: {string.Join(", ", duplicateIds)}.");
            }
        }

        private static void ValidateAgent(Agent agent)
        {
            ValidateProviderUrl(agent);
            ValidateTimeout(agent);
        }

        private static void ValidateProviderUrl(Agent agent)
        {
            var url = agent.Provider?.Url;

            if (string.IsNullOrWhiteSpace(url))
            {
                return;
            }

            if (!Uri.TryCreate(url, UriKind.Absolute, out _))
            {
                throw new InvalidOperationException(
                    $"Runiq agent registration failed. Agent '{agent.Id}' has invalid provider url: '{url}'.");
            }
        }

        private static void ValidateTimeout(Agent agent)
        {
            var timeout = agent.Provider?.Timeout;

            if (timeout is null)
            {
                return;
            }

            if (timeout <= TimeSpan.Zero)
            {
                throw new InvalidOperationException(
                    $"Runiq agent registration failed. Agent '{agent.Id}' has invalid provider timeout. Timeout must be greater than zero.");
            }
        }
    }
}