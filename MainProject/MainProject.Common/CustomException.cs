namespace MainProject.Common
{
    using System;

    public class CustomException : Exception
    {
        public CustomException(string message)
            : base(message) { }
    }
}
