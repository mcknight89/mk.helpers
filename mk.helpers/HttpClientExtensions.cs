using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace mk.helpers
{
    /// <summary>
    /// Provides extension methods for the HttpClient class.
    /// </summary>
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Asynchronously downloads data from the specified URL and writes it to the provided destination stream.
        /// </summary>
        /// <param name="client">The HttpClient instance.</param>
        /// <param name="requestUrl">The URL to download from.</param>
        /// <param name="destination">The stream to write the downloaded data to.</param>
        /// <param name="downloadProgress">Optional action to report download progress.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        public static async Task DownloadDataAsync(this HttpClient client, string requestUrl, Stream destination, Action<long> downloadProgress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var response = await client.GetAsync(requestUrl, HttpCompletionOption.ResponseHeadersRead))
            {
                var contentLength = response.Content.Headers.ContentLength;
                using (var download = await response.Content.ReadAsStreamAsync())
                {
                    if (download is null)
                        throw new ArgumentNullException(nameof(download));
                    if (!download.CanRead)
                        throw new InvalidOperationException($"'{nameof(download)}' is not readable.");
                    if (destination == null)
                        throw new ArgumentNullException(nameof(destination));
                    if (!destination.CanWrite)
                        throw new InvalidOperationException($"'{nameof(destination)}' is not writable.");

                    var buffer = new byte[10000];
                    long totalBytesRead = 0;
                    int bytesRead;
                    DateTime lastUpdate = DateTime.Now;
                    while ((bytesRead = await download.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) != 0)
                    {
                        await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
                        totalBytesRead += bytesRead;
                        if (lastUpdate.AddSeconds(1) < DateTime.Now)
                        {
                            downloadProgress?.Invoke(totalBytesRead);
                            lastUpdate = DateTime.Now;
                        }
                    }

                    downloadProgress?.Invoke(totalBytesRead);
                }
            }
        }

        /// <summary>
        /// Sends an HTTP POST request with JSON data as the content.
        /// </summary>
        /// <typeparam name="T">The type of data to be serialized as JSON.</typeparam>
        /// <param name="httpClient">The HttpClient instance.</param>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="data">The data to be serialized and sent as JSON.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient httpClient, string url, T data)
        {
            var dataAsString = JsonSerializer.Serialize(data);
            var content = new StringContent(dataAsString, Encoding.UTF8, "application/json");

            return await httpClient.PostAsync(url, content);
        }

        /// <summary>
        /// Sends an HTTP PUT request with JSON data as the content.
        /// </summary>
        /// <typeparam name="T">The type of data to be serialized as JSON.</typeparam>
        /// <param name="httpClient">The HttpClient instance.</param>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="data">The data to be serialized and sent as JSON.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task<HttpResponseMessage> PutAsJsonAsync<T>(this HttpClient httpClient, string url, T data)
        {
            var dataAsString = JsonSerializer.Serialize(data);
            var content = new StringContent(dataAsString, Encoding.UTF8, "application/json");

            return await httpClient.PutAsync(url, content);
        }

        /// <summary>
        /// Asynchronously reads the HTTP content as JSON and deserializes it to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the JSON content to.</typeparam>
        /// <param name="content">The HttpContent to read from.</param>
        /// <returns>A task representing the asynchronous operation and containing the deserialized object.</returns>
        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
        {
            var dataAsString = await content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<T>(dataAsString);
        }
    }
}
