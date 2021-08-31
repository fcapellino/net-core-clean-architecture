namespace MainProject.Infrastructure.Common.ResponseTypes
{
    using System;

    public interface IResponse
    {
        bool Error { get; }
        string ErrorMessage { get; }
        Exception Exception { get; }
        object Resources { get; }
    }
}
