namespace MainProject.Common
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Dynamic.Core;
    using System.Net.Mime;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public static class ExtensionMethods
    {
        public static IQueryable<T> ApplyOrdering<T>(this IQueryable<T> query, string propertyName)
        {
            if (!Regex.IsMatch(propertyName ?? string.Empty, "^[a-zA-Z]+(.[a-zA-Z]+)? (asc|desc)$"))
            {
                return query;
            }

            return query.OrderBy(propertyName);
        }

        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int page, int pageSize)
        {
            if (page <= 0)
            {
                page = 1;
            }

            if (pageSize <= 0)
            {
                pageSize = 5;
            }

            return query.Skip((page - 1) * pageSize).Take(pageSize);
        }

        public static bool IsJsonMediaTypeRequest(this HttpRequest request)
        {
            return (request.Headers != null) && request.GetTypedHeaders().Accept
                .Any(h => h.MediaType.Value.Equals(MediaTypeNames.Application.Json));
        }

        public static string NullIfEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        public static async Task<string> ComputeMd5HashAsync(this IFormFile file)
        {
            using var stream = new MemoryStream();
            using var md5 = MD5.Create();

            await file.CopyToAsync(stream);
            return Convert.ToBase64String(md5.ComputeHash(stream.ToArray()));
        }

        public static async Task ForEachAsync<T>(this IEnumerable<T> enumerable, Func<T, Task> action)
        {
            await Task.WhenAll(enumerable.Select(item => action(item)));
        }

        public static async Task<IEnumerable<TResult>> SelectAsync<TSource, TResult>(this IEnumerable<TSource> enumerable, Func<TSource, Task<TResult>> selector)
        {
            return await Task.WhenAll(enumerable.Select(selector));
        }

        public static async Task WriteJsonAsync(this HttpResponse httpResponse, object value, CancellationToken cancellationToken = default)
        {
            var text = JsonConvert.SerializeObject(value, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            httpResponse.StatusCode = StatusCodes.Status200OK;
            httpResponse.ContentType = MediaTypeNames.Application.Json;
            await httpResponse.WriteAsync(text, Encoding.UTF8, cancellationToken);
        }
    }
}
