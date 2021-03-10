// Copyright (c) 2021 DrBarnabus

using System.Threading.Tasks;

namespace Servly.Core.Internal
{
    internal interface IStartupInitializer
    {
        Task InitializeAsync();
    }
}
