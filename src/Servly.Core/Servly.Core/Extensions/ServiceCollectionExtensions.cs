// Copyright (c) 2021 DrBarnabus

using Microsoft.Extensions.DependencyInjection;
using System;

namespace Servly.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServly(this IServiceCollection services, Action<IServlyBuilder> configureDelegate)
        {
            var servlyBuilder = ServlyBuilder.Create(services);

            configureDelegate(servlyBuilder);

            servlyBuilder.Build();

            return services;
        }
    }
}
