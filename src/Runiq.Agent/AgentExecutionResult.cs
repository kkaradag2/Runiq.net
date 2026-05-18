namespace Runiq.Agents
{
    /// <summary>
    /// Agent çalıştırma sonucunu başarı veya hata bilgisiyle temsil eder.
    /// </summary>
    public sealed class AgentExecutionResult
    {
        private AgentExecutionResult(
            bool isSuccess,
            string? message,
            string? errorCode,
            string? errorMessage)
        {
            IsSuccess = isSuccess;
            Message = message;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Agent çalıştırma işleminin başarılı olup olmadığını belirtir.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Başarılı çalıştırma sonucunda modelden dönen cevaptır.
        /// </summary>
        public string? Message { get; }

        /// <summary>
        /// Başarısız çalıştırma durumunda hata kodudur.
        /// </summary>
        public string? ErrorCode { get; }

        /// <summary>
        /// Başarısız çalıştırma durumunda kullanıcıya veya geliştiriciye gösterilebilecek hata açıklamasıdır.
        /// </summary>
        public string? ErrorMessage { get; }

        /// <summary>
        /// Başarılı agent çalıştırma sonucu oluşturur.
        /// </summary>
        public static AgentExecutionResult Success(string message)
        {
            return new AgentExecutionResult(
                isSuccess: true,
                message: message,
                errorCode: null,
                errorMessage: null);
        }

        /// <summary>
        /// Başarısız agent çalıştırma sonucu oluşturur.
        /// </summary>
        public static AgentExecutionResult Failure(string errorCode, string errorMessage)
        {
            return new AgentExecutionResult(
                isSuccess: false,
                message: null,
                errorCode: errorCode,
                errorMessage: errorMessage);
        }
    }
}