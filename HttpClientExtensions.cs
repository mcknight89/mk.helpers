using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mk.helpers
{
	public static class HttpClientExtensions
	{
		public static async Task DownloadFileWithProgressAsync(this HttpClient client, string requestUrl, Stream destination, Action<long> downloadProgress = null, CancellationToken cancellationToken = default(CancellationToken))
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

	}
}
