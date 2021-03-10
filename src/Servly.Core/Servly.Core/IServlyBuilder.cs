// Copyright (c) 2021 DrBarnabus

using Microsoft.Extensions.DependencyInjection;
using System;

namespace Servly.Core
{
    public interface IServlyBuilder
    {
        IServiceCollection Services { get; }

        bool TryRegisterModule(string moduleName);

        bool IsModuleRegistered(string moduleName);

        void AddBuildAction(Action<IServiceCollection> buildActionDelegate);

        void AddInitializer<TInitializer>()
            where TInitializer : class, IInitializer;

        void AddOptions<TOptions>(string configurationSection)
            where TOptions : class, new();

        TOptions GetOptions<TOptions>()
            where TOptions : class, new();

        void Build();
    }
}
