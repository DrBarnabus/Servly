﻿using Microsoft.Extensions.DependencyInjection;
using Servly.Core.Services;
using Servly.Core.Services.Implementations;

namespace Servly.Core.Extensions;

public static class ServlyBuilderExtensions
{
    private const string SystemClockModuleName = "Core.SystemClock";

    public static IServlyBuilder AddSystemClock(this IServlyBuilder builder)
    {
        if (builder.TryRegisterModule(SystemClockModuleName))
            return builder;

        builder.Services
            .AddSingleton<IClock, SystemClock>();

        return builder;
    }
}
