﻿using Microsoft.Extensions.DependencyInjection;
using Servly.Core;
using Servly.Idempotency.AspNetCore;
using Servly.Idempotency.AspNetCore.Implementations;
using Servly.Idempotency.AspNetCore.Middleware;

// ReSharper disable once CheckNamespace
namespace Servly.Extensions;

public static class ServlyBuilderExtensions
{
    private const string IdempotencyMiddlewareModuleName = "Idempotency";

    public static IServlyBuilder AddIdempotencyMiddleware(this IServlyBuilder builder)
    {
        if (builder.TryRegisterModule(IdempotencyMiddlewareModuleName))
            return builder;

        builder.Services
            .AddSingleton<IIdempotencyPersistenceProvider, RedisIdempotencyPersistenceProvider>()
            .AddScoped<IdempotencyMiddleware>();

        return builder;
    }
}
