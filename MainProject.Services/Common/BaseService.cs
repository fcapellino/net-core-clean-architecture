namespace MainProject.Services.Common
{
    using System;
    using Microsoft.Extensions.Configuration;

    public abstract class BaseService : IDisposable
    {
        protected readonly IConfiguration _configuration;

        protected BaseService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public abstract void Dispose();
    }
}
