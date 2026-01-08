namespace EquipOps.Common.Helper
{
    public class ExceptionResponse
    {
        public int StatusCodeRequest { get; set; }
        public string ExceptionMessage { get; set; }
        public string? ExceptionStackTrace { get; set; }
        public bool IsWarning { get; set; }

        public ExceptionResponse(Exception exception, bool isWarning, string exceptionMessage, ApiStatusCode statusCode = ApiStatusCode.BAD_REQUEST)
        {
            StatusCodeRequest = (int)statusCode;
            ExceptionMessage = $"{exceptionMessage} | Original Message: {exception?.Message ?? "No Message Available"}";
            ExceptionStackTrace = exception?.StackTrace ?? Environment.StackTrace;
            IsWarning = isWarning;
        }
    }
}
