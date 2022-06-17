using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Symplify.Conversion.SDK.DemoApp.Services
{
    public class SymplifyService : IHostedService, ISymplifyService
    {
        private readonly ILogger<SymplifyService> _logger;
        private readonly IServiceProvider _serviceProvider;

        private readonly ClientConfig _config;
        private SymplifyClient Client;

        public SymplifyService(IServiceProvider serviceProvider, ILogger<SymplifyService> logger)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            string websiteId = GetWebsiteID();
            string cdnHost = Environment.GetEnvironmentVariable("SSTSDK_CDNHOST") ?? "fake-cdn.localhost.test";
            string cdnPort = Environment.GetEnvironmentVariable("SSTSDK_CDNPORT") ?? "5443";
            string cdnBaseURL = "https://" + cdnHost + ":" + cdnPort;

            _config = new(websiteId, cdnBaseURL);
            _logger.LogInformation("Created service with config: {ClientConfig}", _config);
        }

        public string GetWebsiteID()
        {
            return Environment.GetEnvironmentVariable("SSTSDK_WEBSITEID") ?? "5620187";
        }

        public SymplifyClient GetClient()
        {
            return Client;
        }

        public async Task StartUp()
        {
            _logger.LogInformation("Starting SDK client, loading config");
            // in a real application, the HttpClient would likely be injected
            Client = new(_config, new HttpClient(), new DefaultLogger());
            await Client.LoadConfig();
        }

        // from IHostedService
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var symplifyService = scope.ServiceProvider.GetRequiredService<ISymplifyService>();
            await symplifyService.StartUp();
        }

        // from IHostedService
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    public interface ISymplifyService
    {
        public string GetWebsiteID();
        public SymplifyClient GetClient();
        public Task StartUp();
    }
}
