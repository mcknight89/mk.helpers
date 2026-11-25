using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace mk.helpers
{
    /// <summary>
    /// Reads and writes requests and responses for small HttpListener servers.
    /// </summary>
    public static class HttpListenerExtensions
    {
        private static readonly JsonSerializerOptions _readOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        private static readonly JsonSerializerOptions _writeOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        /// <summary>
        /// Reads the request body as JSON, with case-insensitive property names.
        /// </summary>
        public static async Task<T> ReadAsJsonAsync<T>(this HttpListenerContext context)
        {
            return await JsonSerializer.DeserializeAsync<T>(context.Request.InputStream, _readOptions);
        }

        /// <summary>
        /// Writes text as UTF-8 with the given content type.
        /// </summary>
        public static async Task WriteTextAsync(this HttpListenerResponse response, string text, string contentType = "text/plain")
        {
            response.ContentType = contentType + "; charset=utf-8";
            await response.WriteBytesAsync(Encoding.UTF8.GetBytes(text ?? ""), response.ContentType);
        }

        /// <summary>
        /// Writes a value as camelCase JSON, with Cache-Control: no-store.
        /// </summary>
        public static async Task WriteJsonAsync(this HttpListenerResponse response, object value)
        {
            response.Headers["Cache-Control"] = "no-store";
            await response.WriteTextAsync(JsonSerializer.Serialize(value, _writeOptions), "application/json");
        }

        /// <summary>
        /// Writes raw bytes, such as a PNG, with the given content type.
        /// </summary>
        public static async Task WriteBytesAsync(this HttpListenerResponse response, byte[] data, string contentType)
        {
            response.ContentType = contentType;
            response.ContentLength64 = data.Length;
            await response.OutputStream.WriteAsync(data, 0, data.Length);
        }
    }
}
