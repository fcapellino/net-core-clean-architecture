namespace MainProject.Infrastructure.Common.ResponseTypes
{
    using System;

    public sealed class ErrorResponse : IResponse
    {
        public bool Error { get; }
        public string ErrorMessage { get; }
        public Exception Exception { get; }
        public object Resources { get; }

        public ErrorResponse(string errorMessage, Exception exception)
        {
            Error = true;
            ErrorMessage = errorMessage;
            Exception = exception;
            Resources = null;
        }

        public ErrorResponse(string errorMessage)
            : this(errorMessage: errorMessage, exception: null)
        {
        }
    }
}
