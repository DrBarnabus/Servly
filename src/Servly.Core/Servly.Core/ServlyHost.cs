// Copyright (c) 2021 DrBarnabus

using Servly.Core.Internal;
using System;

namespace Servly.Core
{
    public static class ServlyHost
    {
        public static IServlyHostBuilder CreateDefaultBuilder(string[]? args = null)
        {
            return new ServlyHostBuilder(args);
        }
    }
}
