// Copyright (c) 2021 DrBarnabus

using Microsoft.Extensions.DependencyInjection;

namespace Servly.Core
{
    public static class ServlyBuilder
    {
        public static IServlyBuilder Create(IServiceCollection services)
        {
            return new Internal.ServlyBuilder(services);
        }
    }
}
