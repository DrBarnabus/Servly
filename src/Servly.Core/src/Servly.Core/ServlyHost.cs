// Copyright (c) 2021 DrBarnabus

using Servly.Core.Internal;

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
