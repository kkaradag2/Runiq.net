namespace Runiq.Agents.Configuration
{
    /// <summary>
    /// Agent'ın model sağlayıcısına bağlanırken kullanacağı opsiyonel endpoint ayarlarını taşır.
    /// </summary>
    public class ProviderOptions
    {
        /// <summary>
        /// Varsayılan sağlayıcı adresini ezmek için kullanılacak URL değeridir.
        /// Örnek: http://localhost:8090 veya https://api.openai.com/v1.
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// İstek zaman aşımı süresidir. Boş bırakılırsa varsayılan süre kullanılır.
        /// </summary>
        public TimeSpan? Timeout { get; set; }
    }
}