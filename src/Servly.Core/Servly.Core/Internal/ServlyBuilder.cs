// Copyright (c) 2021 DrBarnabus

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Servly.Core.Internal
{
    internal class ServlyBuilder : IServlyBuilder
    {
        private bool _built;

        public ServlyBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }

        public void AddOptions<TOptions>(string configurationSection)
            where TOptions : class, new()
        {
            using var serviceProvider = Services.BuildServiceProvider();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            Services.Configure<TOptions>(configuration.GetSection(configurationSection));
        }

        public TOptions GetOptions<TOptions>()
            where TOptions : class, new()
        {
            using var serviceProvider = Services.BuildServiceProvider();
            var options = serviceProvider.GetRequiredService<IOptions<TOptions>>();
            return options.Value;
        }

        public void Build()
        {
            // TODO: Replace with ServlyAlreadyBuiltException (or a generic ServlyException)
            if (_built)
                throw new InvalidOperationException("Build can only be called once.");

            _built = true;
        }
    }
}
