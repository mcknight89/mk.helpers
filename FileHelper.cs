using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mk.helpers
{

    public static class FileHelper
    {
        public class DownloadFileResult
        {
            public string FilePath { get; set; }
            public long Size { get; set; }
        }

        public static async Task<DownloadFileResult> DownloadFileAsync(string url, Action<long>? onDownloadProgress, string filePath = null)
        {
            var client = new HttpClient();

            filePath = filePath ?? Path.GetTempFileName();
            if (!(filePath.Contains("/") || filePath.Contains("\\")))
                filePath = $"{Path.GetTempPath()}{filePath}";

            File.Delete(filePath + ".incomplete");
            File.Delete(filePath);

            long totalSize = 0;
            long temp = 0;
            using (var file = new FileStream(filePath + ".incomplete", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await client.DownloadDataAsync(url, file, (progress) =>
                {
                    totalSize = progress;
                    if (progress > 1024 || temp < progress - 1024)
                    {
                        onDownloadProgress?.Invoke(totalSize);
                        temp = progress;
                    }
                });
                file.Close();
            }
            Thread.Sleep(500);// Might be locked;
            File.Move(filePath + ".incomplete", filePath);

            return new DownloadFileResult
            {
                FilePath = filePath,
                Size = totalSize,
            };
        }

        public static void ReadJsonArrayFile<T>(string filePath, Action<T> onData, Action<double> onProgress, bool readSynchronously = false)
        {
            Task[] tasks = new Task[10];

            if (!File.Exists(filePath))
                throw new FileNotFoundException();
            JsonSerializer serializer = new JsonSerializer();

            using FileStream s = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using StreamReader sr = new StreamReader(s);
            using JsonReader reader = new JsonTextReader(sr);
            var lastProgress = DateTime.Now;

            while (reader.Read())
            {
                // deserialize only when there's "{" character in the stream
                if (reader.TokenType == JsonToken.StartObject)
                {
                    var o = serializer.Deserialize<T>(reader);


                    if (readSynchronously)
                    {
                        onData?.Invoke(o);
                    }
                    else
                    {
                        while (!tasks.Any(d => d == null || d.IsCompletedSuccessfully))
                        {
                            Thread.Sleep(1);
                        }

                        var indic = tasks.Select((s, i) => new { s, i })
                            .Where(d => d.s == null || d.s.IsCompletedSuccessfully)
                            .Select(d => d.i).First();

                        tasks[indic] = Task.Factory.StartNew(() =>
                        {
                            onData?.Invoke(o);
                        });
                    }

                    if (lastProgress.AddSeconds(2) < DateTime.Now)
                    {
                        lastProgress = DateTime.Now;
                        var progress = Math.Round((double)sr.BaseStream.Position / sr.BaseStream.Length * 100, 2);
                        onProgress?.Invoke(progress);
                    }

                }
            }
            return;
        }
    }


}
