using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
        public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient httpClient, string url, T data)
        {
            var dataAsString = JsonConvert.SerializeObject(data);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return httpClient.PostAsync(url, content);
        }

        /// <summary>
        /// Sends an HTTP PUT request with JSON data as the content.
        /// </summary>
        /// <typeparam name="T">The type of data to be serialized as JSON.</typeparam>
        /// <param name="httpClient">The HttpClient instance.</param>
        /// <param name="url">The URL to send the request to.</param>
        /// <param name="data">The data to be serialized and sent as JSON.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static Task<HttpResponseMessage> PutAsJsonAsync<T>(this HttpClient httpClient, string url, T data)
        {
            var dataAsString = JsonConvert.SerializeObject(data);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return httpClient.PutAsync(url, content);
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

            return JsonConvert.DeserializeObject<T>(dataAsString);
        }

        /// <summary>
        /// Sends an HTTP HEAD request to the specified URI.
        /// </summary>
        /// <param name="client">The HttpClient instance.</param>
        /// <param name="requestUri">The URI to send the request to.</param>
        /// <returns>A task representing the asynchronous operation and containing the response message.</returns>
        public static Task<HttpResponseMessage> HeadAsync(this HttpClient client, string requestUri)
        {
            return client.HeadAsync(new Uri(requestUri));
        }

        /// <summary>
        /// Sends an HTTP HEAD request to the specified URI with the specified completion option.
        /// </summary>
        /// <param name="client">The HttpClient instance.</param>
        /// <param name="requestUri">The URI to send the request to.</param>
        /// <param name="completionOption">The HttpCompletionOption to use for the request.</param>
        /// <returns>A task representing the asynchronous operation and containing the response message.</returns>
        public static Task<HttpResponseMessage> HeadAsync(this HttpClient client, Uri requestUri)
        {
            return client.HeadAsync(requestUri, HttpCompletionOption.ResponseContentRead);
        }

        /// <summary>
        /// Sends an HTTP HEAD request to the specified URI with the specified completion option.
        /// </summary>
        /// <param name="client">The HttpClient instance.</param>
        /// <param name="requestUri">The URI to send the request to.</param>
        /// <param name="completionOption">The HttpCompletionOption to use for the request.</param>
        /// <returns>A task representing the asynchronous operation and containing the response message.</returns>
        public static Task<HttpResponseMessage> HeadAsync(
            this HttpClient client,
            string requestUri,
            HttpCompletionOption completionOption
        )
        {
            return client.HeadAsync(new Uri(requestUri), completionOption);
        }

        /// <summary>
        /// Sends an HTTP HEAD request to the specified URI with the specified completion option.
        /// </summary>
        /// <param name="client">The HttpClient instance.</param>
        /// <param name="requestUri">The URI to send the request to.</param>
        /// <param name="completionOption">The HttpCompletionOption to use for the request.</param>
        /// <returns>A task representing the asynchronous operation and containing the response message.</returns>
        public static Task<HttpResponseMessage> HeadAsync(
            this HttpClient client,
            Uri requestUri,
            HttpCompletionOption completionOption
        )
        {
            return client.HeadAsync(requestUri, completionOption, CancellationToken.None);
        }

        /// <summary>
        /// Sends an HTTP HEAD request to the specified URI with the specified completion option.
        /// </summary>
        /// <param name="client">The HttpClient instance.</param>
        /// <param name="requestUri">The URI to send the request to.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation and containing the response message.</returns>

        public static Task<HttpResponseMessage> HeadAsync(
            this HttpClient client,
            string requestUri,
            CancellationToken cancellationToken
        )
        {
            return client.HeadAsync(new Uri(requestUri), cancellationToken);
        }

        /// <summary>
        /// Sends an HTTP HEAD request to the specified URI with the specified completion option.
        /// </summary>
        /// <param name="client">The HttpClient instance.</param>
        /// <param name="requestUri">The URI to send the request to.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation and containing the response message.</returns>
        public static Task<HttpResponseMessage> HeadAsync(
            this HttpClient client,
            Uri requestUri,
            CancellationToken cancellationToken
        )
        {
            return client.HeadAsync(requestUri, HttpCompletionOption.ResponseContentRead, cancellationToken);
        }

        /// <summary>
        /// Sends an HTTP HEAD request to the specified URI with the specified completion option.
        /// </summary>
        /// <param name="client">The HttpClient instance.</param>
        /// <param name="requestUri">The URI to send the request to.</param>
        /// <param name="completionOption">The HttpCompletionOption to use for the request.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation and containing the response message.</returns>
        public static Task<HttpResponseMessage> HeadAsync(
            this HttpClient client,
            string requestUri,
            HttpCompletionOption completionOption,
            CancellationToken cancellationToken
        )
        {
            return client.HeadAsync(new Uri(requestUri), completionOption, cancellationToken);
        }

        /// <summary>
        /// Sends an HTTP HEAD request to the specified URI with the specified completion option.
        /// </summary>
        /// <param name="client">The HttpClient instance.</param>
        /// <param name="requestUri">The URI to send the request to.</param>
        /// <param name="completionOption">The HttpCompletionOption to use for the request.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation and containing the response message.</returns>
        public static Task<HttpResponseMessage> HeadAsync(
            this HttpClient client,
            Uri requestUri,
            HttpCompletionOption completionOption,
            CancellationToken cancellationToken
        )
        {
            return client.SendAsync(new HttpRequestMessage(HttpMethod.Head, requestUri), completionOption,
                cancellationToken);
        }

        /// <summary>
        /// Sends an HTTP request with the specified method, URI, headers, and content.
        /// </summary>
        /// <param name="client">The HttpClient instance.</param>
        /// <param name="method">The HTTP method to use for the request.</param>
        /// <param name="requestUri">The URI to send the request to.</param>
        /// <param name="headers">Optional headers to include in the request.</param>
        /// <param name="content">Optional HTTP content to include in the request.</param>
        /// <returns>A task representing the asynchronous operation and containing the response message.</returns>
        public static Task<HttpResponseMessage> SendAsync(
            this HttpClient client,
            HttpMethod method,
            Uri requestUri,
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers = null,
            HttpContent content = null
        )
        {
            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = content
            };

            if (!(headers is null))
            {
                foreach (var (key, value) in headers)
                {
                    request.Headers.Add(key, value);
                }
            }

            return client.SendAsync(request);
        }


        /// <summary>
        /// Sends an HTTP request with a Bearer token in the authorization header and optional JSON content.
        /// </summary>
        /// <param name="httpClient">The HttpClient instance.</param>
        /// <param name="method">The HTTP method to use for the request.</param>
        /// <param name="path">The path or URL to send the request to.</param>
        /// <param name="requestBody">The JSON data to include in the request body.</param>
        /// <param name="accessToken">The Bearer token for authorization.</param>
        /// <param name="ct">A cancellation token to cancel the operation.</param>
        /// <returns>A task representing the asynchronous operation and containing the response message.</returns>
        public static async Task<HttpResponseMessage> SendRequestWithBearerTokenAsync(this HttpClient httpClient, HttpMethod method, string path, object requestBody, string accessToken, CancellationToken ct)
        {
            var request = new HttpRequestMessage(method, path);

            if (requestBody != null)
            {
                var json = JsonConvert.SerializeObject(requestBody, Formatting.None);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                request.Content = content;
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.SendAsync(request, ct);
            return response;
        }
    }
}