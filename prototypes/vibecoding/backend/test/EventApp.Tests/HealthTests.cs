using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;






















}    }        }            Assert.Equal("Healthy", doc.RootElement.GetProperty("status").GetString());            using var doc = JsonDocument.Parse(json);
n            var json = await res.Content.ReadAsStringAsync();            Assert.Equal(HttpStatusCode.OK, res.StatusCode);            var res = await client.GetAsync("/health");            var client = _factory.CreateClient();        {        public async Task HealthEndpoint_ReturnsHealthy()
n        [Fact]        }            _factory = factory;        {
n        public HealthTests(WebApplicationFactory<Program> factory)        private readonly WebApplicationFactory<Program> _factory;    {    public class HealthTests : IClassFixture<WebApplicationFactory<Program>>{nnamespace EventApp.Tests