using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace mk.helpers.tests
{
    [TestClass]
    public class LocalHttpTests
    {
        private class Ping
        {
            public string Name { get; set; } = "";
        }

        [TestMethod]
        public async Task Listen_ReadsAndWritesJson()
        {
            var port = FreePort();
            using var listener = LocalHttp.Listen(port, async context =>
            {
                var ping = await context.ReadAsJsonAsync<Ping>();
                await context.Response.WriteJsonAsync(new { Reply = "hi " + ping.Name });
            });
            using var client = new HttpClient();

            var response = await client.PostAsync($"http://localhost:{port}/", new StringContent("{\"name\":\"mike\"}", Encoding.UTF8, "application/json"));

            Assert.AreEqual("{\"reply\":\"hi mike\"}", await response.Content.ReadAsStringAsync());
            Assert.AreEqual("application/json", response.Content.Headers.ContentType!.MediaType);
            Assert.IsTrue(response.Headers.CacheControl!.NoStore);
        }

        [TestMethod]
        public async Task Listen_TurnsExceptionInto500()
        {
            var port = FreePort();
            using var listener = LocalHttp.Listen(port, context => throw new InvalidOperationException("broken"));
            using var client = new HttpClient();

            var response = await client.GetAsync($"http://localhost:{port}/");

            Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.AreEqual("broken", await response.Content.ReadAsStringAsync());
        }

        private static int FreePort()
        {
            var socket = new TcpListener(IPAddress.Loopback, 0);
            socket.Start();
            var port = ((IPEndPoint)socket.LocalEndpoint).Port;
            socket.Stop();
            return port;
        }
    }
}
