// Copyright (c) 2021 DrBarnabus

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servly.Core.Internal
{
    internal class StartupInitializer : IStartupInitializer
    {
        private readonly List<IInitializer> _initializers;

        public StartupInitializer(IEnumerable<IInitializer> initializers)
        {
            _initializers = initializers.ToList();
        }

        public async Task InitializeAsync()
        {
            if (_initializers.Count == 0)
                return;

            await Task.WhenAll(_initializers.Select(i => i.InitializeAsync()));
        }
    }
}
