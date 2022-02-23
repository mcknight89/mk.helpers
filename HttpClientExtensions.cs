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
    public static class HttpClientExtensions
    {

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


        public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient httpClient, string url, T data)
        {
            var dataAsString = JsonConvert.SerializeObject(data);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return httpClient.PostAsync(url, content);
        }
        public static Task<HttpResponseMessage> PutAsJsonAsync<T>(this HttpClient httpClient, string url, T data)
        {
            var dataAsString = JsonConvert.SerializeObject(data);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return httpClient.PutAsync(url, content);
        }
        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
        {
            var dataAsString = await content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonConvert.DeserializeObject<T>(dataAsString);
        }
        public static Task<HttpResponseMessage> HeadAsync(this HttpClient client, string requestUri)
        {
            return client.HeadAsync(new Uri(requestUri));
        }

        public static Task<HttpResponseMessage> HeadAsync(this HttpClient client, Uri requestUri)
        {
            return client.HeadAsync(requestUri, HttpCompletionOption.ResponseContentRead);
        }

        public static Task<HttpResponseMessage> HeadAsync(
            this HttpClient client,
            string requestUri,
            HttpCompletionOption completionOption
        )
        {
            return client.HeadAsync(new Uri(requestUri), completionOption);
        }

        public static Task<HttpResponseMessage> HeadAsync(
            this HttpClient client,
            Uri requestUri,
            HttpCompletionOption completionOption
        )
        {
            return client.HeadAsync(requestUri, completionOption, CancellationToken.None);
        }

        public static Task<HttpResponseMessage> HeadAsync(
            this HttpClient client,
            string requestUri,
            CancellationToken cancellationToken
        )
        {
            return client.HeadAsync(new Uri(requestUri), cancellationToken);
        }

        public static Task<HttpResponseMessage> HeadAsync(
            this HttpClient client,
            Uri requestUri,
            CancellationToken cancellationToken
        )
        {
            return client.HeadAsync(requestUri, HttpCompletionOption.ResponseContentRead, cancellationToken);
        }

        public static Task<HttpResponseMessage> HeadAsync(
            this HttpClient client,
            string requestUri,
            HttpCompletionOption completionOption,
            CancellationToken cancellationToken
        )
        {
            return client.HeadAsync(new Uri(requestUri), completionOption, cancellationToken);
        }

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