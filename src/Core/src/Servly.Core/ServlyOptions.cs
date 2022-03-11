namespace Servly.Core;

public class ServlyOptions
{
    public string? ServiceName { get; set; }

    public bool DisplayStartupBanner { get; set; } = false;

    public bool DisplayStartupInformation { get; set; } = false;
}
