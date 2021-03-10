// Copyright (c) 2021 DrBarnabus

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Servly.Core
{
    public interface IServlyHostBuilder
    {
        IServlyHostBuilder ConfigureServly(Action<WebHostBuilderContext, IServlyBuilder> configureDelegate);

        IServlyHostBuilder ConfigureServly(Action<IServlyBuilder> configureDelegate);

        IServlyHostBuilder ConfigureServices(Action<WebHostBuilderContext, IServiceCollection> configureDelegate);

        IServlyHostBuilder ConfigureServices(Action<IServiceCollection> configureDelegate);

        IServlyHostBuilder Configure(Action<WebHostBuilderContext, IApplicationBuilder> configureDelegate);

        IServlyHostBuilder Configure(Action<IApplicationBuilder> configureDelegate);

        IServlyHost Build();
    }
}
