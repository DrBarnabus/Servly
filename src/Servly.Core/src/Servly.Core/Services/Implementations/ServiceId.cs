// Copyright (c) 2021 DrBarnabus

using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Servly.Core.Services.Implementations
{
    internal class ServiceId : IServiceId
    {
        public Guid Id { get; } = Guid.NewGuid();

        public override string ToString() => Id.ToString("N").ToUpperInvariant();
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class ServiceIdInitializer : IInitializer
    {
        private readonly ILogger<ServiceIdInitializer> _logger;
        private readonly IServiceId _serviceId;

        public ServiceIdInitializer(ILogger<ServiceIdInitializer> logger, IServiceId serviceId)
        {
            _logger = logger;
            _serviceId = serviceId;
        }

        public Task InitializeAsync()
        {
            _logger.LogInformation("ServiceId: {ServiceId}", _serviceId.ToString());
            return Task.CompletedTask;
        }
    }
}
