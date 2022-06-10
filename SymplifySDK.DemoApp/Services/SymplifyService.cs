﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SymplifySDK;
using SymplifySDK.Allocation.Config;

namespace SymplifySDK.DemoApp.Services
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
            string websiteId = Environment.GetEnvironmentVariable("SSTSDK_WEBSITEID") ?? "4711";
            string cdnHost = Environment.GetEnvironmentVariable("SSTSDK_CDNHOST") ?? "fake-cdn.localhost.test";
            string cdnPort = Environment.GetEnvironmentVariable("SSTSDK_CDNPORT") ?? "5443";
            string cdnBaseURL = "https://" + cdnHost + ":" + cdnPort;

            _config = new(websiteId, cdnBaseURL);
            _logger.LogInformation("Created service with config: {ClientConfig}", _config);
        }

        public SymplifyClient GetClient()
        {
            return Client;
        }

        public async Task StartUp()
        {
            _logger.LogInformation("Starting SDK client, loading config");
            Client = new(_config);
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
        public SymplifyClient GetClient();
        public Task StartUp();
    }
}