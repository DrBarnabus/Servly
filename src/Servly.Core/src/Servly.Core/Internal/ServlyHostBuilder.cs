// Copyright (c) 2021 DrBarnabus

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Servly.Core.Exceptions;
using Servly.Core.Extensions;
using System;

namespace Servly.Core.Internal
{
    internal class ServlyHostBuilder : IServlyHostBuilder
    {
        private readonly string[]? _args;

        private Action<WebHostBuilderContext, IApplicationBuilder>? _configureDelegate;
        private Action<WebHostBuilderContext, IServiceCollection>? _configureServicesDelegate;
        private Action<WebHostBuilderContext, IServlyBuilder>? _configureServlyDelegate;

        private bool _hostBuilt;

        public ServlyHostBuilder(string[]? args = null)
        {
            _args = args;
        }

        public IServlyHostBuilder ConfigureServly(Action<WebHostBuilderContext, IServlyBuilder> configureDelegate)
        {
            _ = configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate));

            _configureServlyDelegate = configureDelegate;
            return this;
        }

        public IServlyHostBuilder ConfigureServly(Action<IServlyBuilder> configureDelegate)
        {
            _ = configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate));

            return ConfigureServly((_, servlyBuilder) => configureDelegate(servlyBuilder));
        }

        public IServlyHostBuilder ConfigureServices(Action<WebHostBuilderContext, IServiceCollection> configureDelegate)
        {
            _ = configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate));

            _configureServicesDelegate = configureDelegate;
            return this;
        }

        public IServlyHostBuilder ConfigureServices(Action<IServiceCollection> configureDelegate)
        {
            _ = configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate));

            return ConfigureServices((_, services) => configureDelegate(services));
        }

        public IServlyHostBuilder Configure(Action<WebHostBuilderContext, IApplicationBuilder> configureDelegate)
        {
            _ = configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate));

            _configureDelegate = configureDelegate;
            return this;
        }

        public IServlyHostBuilder Configure(Action<IApplicationBuilder> configureDelegate)
        {
            _ = configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate));
            return Configure((_, app) => configureDelegate(app));
        }

        public IServlyHost Build()
        {
            if (_hostBuilt)
                throw new ServlyHostAlreadyBuiltException();

            _hostBuilt = true;

            var hostBuilder = Host.CreateDefaultBuilder(_args)
                .ConfigureWebHostDefaults(webHostBuilder =>
                {
                    webHostBuilder.ConfigureServices((context, services) =>
                    {
                        if (_configureServlyDelegate is null)
                            services.AddServly();
                        else
                            services.AddServly(servlyBuilder => _configureServlyDelegate(context, servlyBuilder));
                    });

                    if (_configureServicesDelegate is not null)
                        webHostBuilder.ConfigureServices(_configureServicesDelegate);

                    if (_configureDelegate is not null)
                        webHostBuilder.Configure((context, app) =>
                        {
                            app.UseServly();

                            _configureDelegate(context, app);
                        });
                });

            return new ServlyHost(hostBuilder.Build());
        }
    }
}
