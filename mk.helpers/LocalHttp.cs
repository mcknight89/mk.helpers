using System;
using System.Net;
using System.Threading.Tasks;

namespace mk.helpers
{
    /// <summary>
    /// Starts a small HTTP server on localhost.
    /// </summary>
    public static class LocalHttp
    {
        /// <summary>
        /// Listens on http://localhost:port/ and runs each request through the handler. An exception becomes a 500 with its message.
        /// </summary>
        public static HttpListener Listen(int port, Func<HttpListenerContext, Task> handle)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add($"http://localhost:{port}/");
            listener.Start();

            _ = AcceptAsync(listener, handle);
            return listener;
        }

        private static async Task AcceptAsync(HttpListener listener, Func<HttpListenerContext, Task> handle)
        {
            while (listener.IsListening)
            {
                HttpListenerContext context;
                try
                {
                    context = await listener.GetContextAsync();
                }
                catch
                {
                    // the listener was stopped
                    return;
                }

                _ = HandleAsync(context, handle);
            }
        }

        private static async Task HandleAsync(HttpListenerContext context, Func<HttpListenerContext, Task> handle)
        {
            try
            {
                await handle(context);
            }
            catch (Exception ex)
            {
                try
                {
                    context.Response.StatusCode = 500;
                    await context.Response.WriteTextAsync(ex.Message);
                }
                catch
                {
                    // the response had already started
                }
            }
            finally
            {
                try
                {
                    context.Response.Close();
                }
                catch
                {
                    // the client went away
                }
            }
        }
    }
}
