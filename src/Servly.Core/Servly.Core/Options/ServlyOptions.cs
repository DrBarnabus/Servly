// Copyright (c) 2021 DrBarnabus

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Servly.Core.Options
{
    public class ServlyOptions
    {
        public string? ServiceName { get; set; }

        public bool DisplayStartupBanner { get; set; } = false;

        public bool DisplayPlatformInformation { get; set; } = true;
    }
}
