// Copyright (c) 2021 DrBarnabus

using Microsoft.Extensions.DependencyInjection;
using Servly.Core.Services;
using Servly.Core.Services.Implementations;

namespace Servly.Core.Extensions
{
    public static class ServlyBuilderExtensions
    {
        public static IServlyBuilder AddServiceId(this IServlyBuilder servlyBuilder)
        {
            // Check the Module is not already Registered
            if (!servlyBuilder.TryRegisterModule(nameof(IServiceId)))
                return servlyBuilder;

            servlyBuilder.Services.AddSingleton<IServiceId, ServiceId>();
            servlyBuilder.AddInitializer<ServiceIdInitializer>();

            return servlyBuilder;
        }
    }
}
