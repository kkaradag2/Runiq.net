using Runiq.Agents.Providers;

namespace Runiq.Agents.Models
{
    /// <summary>
    /// Agent model bilgisini provider/model formatından ayrıştırılmış şekilde temsil eder.
    /// </summary>
    public sealed class ModelReference
    {
        private ModelReference(string providerName, string modelName)
        {
            ProviderName = providerName;
            ModelName = modelName;
        }

        /// <summary>
        /// Model sağlayıcı adıdır. Örnek: openai, ollama, groq.
        /// </summary>
        public string ProviderName { get; }

        /// <summary>
        /// Sağlayıcı üzerinde çağrılacak model adıdır. Örnek: gpt-5, llama3.2.
        /// </summary>
        public string ModelName { get; }

        /// <summary>
        /// Model referansını provider/model formatından ayrıştırır ve doğrular.
        /// </summary>
        public static ModelReference Parse(string model)
        {
            if (string.IsNullOrWhiteSpace(model))
            {
                throw new ArgumentException("Model cannot be empty.", nameof(model));
            }

            var parts = model.Split('/', 2, StringSplitOptions.TrimEntries);

            if (parts.Length != 2 ||
                string.IsNullOrWhiteSpace(parts[0]) ||
                string.IsNullOrWhiteSpace(parts[1]))
            {
                throw new ArgumentException(
                    "Model must be in 'provider/model' format. Example: openai/gpt-5 or ollama/llama3.2.",
                    nameof(model));
            }

            var providerName = parts[0].Trim().ToLowerInvariant();
            var modelName = parts[1].Trim();

            ProviderDefaults.EnsureSupported(providerName);

            return new ModelReference(providerName, modelName);
        }
    }
}