namespace MainProject.Core.Common.ResponseTypes
{
    using System;

    public sealed class SuccessResponse : IResponse
    {
        public bool Error { get; }
        public string ErrorMessage { get; }
        public Exception Exception { get; }
        public object Resources { get; }

        public SuccessResponse(object resources)
        {
            Error = false;
            ErrorMessage = null;
            Exception = null;
            Resources = resources;
        }

        public SuccessResponse()
            : this(resources: null)
        {
        }
    }
}
