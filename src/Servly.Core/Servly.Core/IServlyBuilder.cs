// Copyright (c) 2021 DrBarnabus

using Microsoft.Extensions.DependencyInjection;

namespace Servly.Core
{
    public interface IServlyBuilder
    {
        IServiceCollection Services { get; }

        void AddOptions<TOptions>(string configurationSection)
            where TOptions : class, new();

        TOptions GetOptions<TOptions>()
            where TOptions : class, new();

        void Build();
    }
}
