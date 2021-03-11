// Copyright (c) 2021 DrBarnabus

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Servly.Core.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Servly.Core.Internal
{
    internal class ServlyBuilder : IServlyBuilder
    {
        private readonly List<Action<IServiceCollection>> _buildActions;
        private readonly ConcurrentDictionary<string, bool> _moduleRegistry;

        private int _built;

        public ServlyBuilder(IServiceCollection services)
        {
            _buildActions = new List<Action<IServiceCollection>>();
            _moduleRegistry = new ConcurrentDictionary<string, bool>();

            Services = services;
        }

        public IServiceCollection Services { get; }

        public bool TryRegisterModule(string moduleName)
        {
            return _moduleRegistry.TryAdd(moduleName, true);
        }

        public bool IsModuleRegistered(string moduleName)
        {
            return _moduleRegistry.TryGetValue(moduleName, out bool value) && value;
        }

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
            if (Interlocked.Exchange(ref _built, 1) == 1)
                throw new ServlyBuilderAlreadyBuiltException();

            foreach (var buildAction in _buildActions)
                buildAction(Services);

            Services.AddSingleton<IStartupInitializer, StartupInitializer>();
        }
    }
}
