// Copyright (c) 2021 DrBarnabus

using System.Threading.Tasks;

namespace Servly.Core
{
    public interface IInitializer
    {
        Task InitializeAsync();
    }
}
