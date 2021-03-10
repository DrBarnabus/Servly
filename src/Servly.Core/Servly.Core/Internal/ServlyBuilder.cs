// Copyright (c) 2021 DrBarnabus

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace Servly.Core.Internal
{
    internal class ServlyBuilder : IServlyBuilder
    {
        private readonly List<Action<IServiceCollection>> _buildActions;

        private bool _built;

        public ServlyBuilder(IServiceCollection services)
        {
            _buildActions = new List<Action<IServiceCollection>>();

            Services = services;
        }

        public IServiceCollection Services { get; }

        public void AddBuildAction(Action<IServiceCollection> buildActionDelegate)
        {
            _ = buildActionDelegate ?? throw new ArgumentNullException(nameof(buildActionDelegate));
            _buildActions.Add(buildActionDelegate);
        }

        public void AddInitializer<TInitializer>()
            where TInitializer : class, IInitializer
        {
            AddBuildAction(services => services.AddSingleton<IInitializer, TInitializer>());
        }

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

            foreach (var buildAction in _buildActions)
                buildAction(Services);

            Services.AddSingleton<IStartupInitializer, StartupInitializer>();
        }
    }
}
