using Microsoft.Extensions.Hosting;

namespace Servly.Hosting.Internal;

internal interface IConfigureContainerAdapter
{
    void ConfigureContainer(HostBuilderContext hostContext, object containerBuilder);
}
