using Microsoft.Extensions.DependencyInjection;
using Servly.AspNetCore.Idempotency;
using Servly.AspNetCore.Idempotency.Implementations;
using Servly.Core;
using Servly.AspNetCore.Idempotency.Middleware;
using Servly.Core.Exceptions;

// ReSharper disable once CheckNamespace
namespace Servly.Extensions;

public static class ServlyBuilderExtensions
{
    private const string ModuleName = "AspNetCore.Idempotency";

    public static IIdempotencyBuilder AddIdempotencyMiddleware(this IServlyBuilder builder)
    {
        if (builder.TryRegisterModule(ModuleName))
            throw new ModuleAlreadyRegisteredException(ModuleName);

        builder.Services
            .AddScoped<IdempotencyMiddleware>();

        return new IdempotencyBuilder(builder);
    }
}
