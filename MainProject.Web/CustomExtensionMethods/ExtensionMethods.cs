namespace MainProject.Web.ExtensionMethods
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using ElmahCore;
    using ElmahCore.Mvc;
    using MainProject.Common;
    using MainProject.Core.Common.ResponseTypes;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Features;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Configuration.Json;
    using Newtonsoft.Json;

    public static class ConfigurationBuilderExtensionMethods
    {
        #region ENCRYPTED JSON FILE CONFIGURATION BUILDER
        private class EncryptedJsonConfigurationSource : JsonConfigurationSource
        {
            private class EncryptedJsonConfigurationProvider : JsonConfigurationProvider
            {
                public EncryptedJsonConfigurationProvider(EncryptedJsonConfigurationSource source)
                    : base(source)
                {
                }
                public override void Load(Stream stream)
                {
                    using var reader = new StreamReader(stream);
                    dynamic result = JsonConvert.DeserializeObject(reader.ReadToEnd());
                    CryptoStream settings = DecryptString(Convert.ToString(result.AppSettings));
                    base.Load(settings);
                }
                private CryptoStream DecryptString(string text)
                {
                    var buffer = Convert.FromBase64String(text);
                    using var aes = Aes.Create();
                    aes.Mode = CipherMode.ECB;
                    aes.Key = Encoding.UTF8.GetBytes((Source as EncryptedJsonConfigurationSource).PrivateKey);
                    aes.IV = new byte[16];

                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    return new CryptoStream(new MemoryStream(buffer), decryptor, CryptoStreamMode.Read);
                }
            }
            public EncryptedJsonConfigurationSource(string privateKey)
            {
                PrivateKey = privateKey;
            }
            public override IConfigurationProvider Build(IConfigurationBuilder builder)
            {
                EnsureDefaults(builder);
                return new EncryptedJsonConfigurationProvider(this);
            }
            public readonly string PrivateKey;
        }
        public static IConfigurationBuilder AddEncryptedJsonFile(this IConfigurationBuilder builder, string path, string privateKey, bool optional, bool reloadOnChange)
        {
            var encryptedJsonConfigurationSource = new EncryptedJsonConfigurationSource(privateKey)
            {
                Path = path,
                Optional = optional,
                ReloadOnChange = reloadOnChange,
                FileProvider = null
            };
            encryptedJsonConfigurationSource.ResolveFileProvider();
            return builder.Add(encryptedJsonConfigurationSource);
        }
        #endregion
    }

    public static class ApplicationBuilderExtensionMethods
    {
        #region ERROR LOGGING
        public static void UseErrorLogging(this IApplicationBuilder app)
        {
            app.UseWhen(context => context.Request.Path
                .StartsWithSegments("/errorlog", StringComparison.OrdinalIgnoreCase), appBuilder =>
                {
                    appBuilder.Use(next =>
                    {
                        return async context =>
                        {
                            context.Features.Get<IHttpBodyControlFeature>().AllowSynchronousIO = true;
                            await next(context);
                        };
                    });
                })
                .UseElmah();
        }
        #endregion

        #region EXCEPTION HANDLER MIDDLEWARE
        public static void UseExceptionHandlerMiddleware(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(
                options =>
                {
                    options.Run(
                        async context =>
                        {
                            var ex = context.Features.Get<IExceptionHandlerFeature>();
                            var exception = ex.Error.GetBaseException();
                            var errorMessage = (exception is CustomException)
                                ? $"Error. {exception.Message}"
                                : $"Error. No es posible completar la operación.";

                            context.RiseError(exception);
                            if (context.Request.IsJsonMediaTypeRequest())
                            {
                                await context.Response.WriteJsonAsync(new ErrorResponse(errorMessage, exception.GetBaseException()));
                            }
                            else
                            {
                                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                                await context.Response.CompleteAsync();
                            }
                        });
                }
            );
        }
        #endregion
    }
}
