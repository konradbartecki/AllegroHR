using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Me.Bartecki.Allegro.Domain.Model;
using Me.Bartecki.Allegro.Infrastructure.Integrations.RepositoryStores;
using Me.Bartecki.Allegro.Infrastructure.Integrations.RepositoryStores.GitHub;
using Me.Bartecki.Allegro.Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;

namespace Me.Bartecki.Allegro.Api.IntegrationTests
{
    [TestClass]
    public class IntegrationTests
    {
        //TODO: On incorrect username
        //TODO: On no internet
        //TODO: On wrong token
        //TODO: On api exception
        private HttpClient _cilent;

        private HttpClient GetMockedHttpClinet()
        {
            var projectDir = Directory.GetCurrentDirectory();
            var responsePath = Path.Combine(projectDir, "example_response.json");
            var responseText = File.ReadAllText(responsePath);

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                // Setup the PROTECTED method to mock
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                // prepare the expected response of the mocked http call
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseText),
                })
                .Verifiable();
            return new HttpClient(handlerMock.Object);
        }

        private IHostBuilder GetHostBuilder()
        {
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, "appsettings.json");

            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    // Add TestServer
                    webHost.UseTestServer();
                    webHost.ConfigureAppConfiguration(c => c.AddJsonFile(configPath));
                    webHost.UseStartup<Startup>();
                });
            return hostBuilder;
        }

        [TestMethod]
        public void CanCalculateStatistics_Real()
        {
            var client = GetHostBuilder().Start().GetTestClient();
            var response = client.SendAsync((new HttpRequestMessage(HttpMethod.Get, "/repositories/konradbartecki"))).Result;
            var json = response.Content.ReadAsStringAsync().Result;
            var stats = JsonConvert.DeserializeObject<UserStatistics>(json);

            stats.Owner.Should().Be("konradbartecki");
            stats.AverageForks.Should().BeGreaterThan(0);
            stats.AverageSize.Should().BeGreaterThan(0);
            stats.AverageWatchers.Should().BeGreaterThan(0);
            stats.AverageStargazers.Should().BeGreaterThan(0);
            stats.Letters.Should().NotBeEmpty();
        }


        [TestMethod]
        public void CanCalculateStatistics_Mocked()
        {
            var mockedHttpClient = GetMockedHttpClinet();
            var embeddedService = new EmbeddedResourceService();
            var githubService = new GitHubIntegrationService(mockedHttpClient, embeddedService);

            var client = GetHostBuilder().ConfigureServices(x =>
                x.AddScoped<IRepositoryStoreService, GitHubIntegrationService>(provider => githubService))
                .Start()
                .GetTestClient();
            var response = client.SendAsync((new HttpRequestMessage(HttpMethod.Get, "/repositories/ignored"))).Result;
            var json = response.Content.ReadAsStringAsync().Result;
            var stats = JsonConvert.DeserializeObject<UserStatistics>(json);

            stats.AverageForks.Should().Be(2);
            stats.AverageStargazers.Should().Be(3);
            stats.AverageWatchers.Should().Be(7);
            stats.AverageSize.Should().Be(500);
            stats.Letters.Should().BeEquivalentTo(new Dictionary<char, int>()
            {
                ['a'] = 3,
                ['e'] = 2,
                ['g'] = 2,
                ['l'] = 6,
                ['o'] = 2,
                ['r'] = 2
            });
            stats.Letters.First().Key.Should().Be('a');
            stats.Letters.Last().Key.Should().Be('r');
        }
    }
}
