using LogViewer.Core.Interfaces;
using LogViewer.Core.Parsers;
using Microsoft.Extensions.DependencyInjection;

namespace LogViewer.Configuration;

public static class ServicesConfiguration
{
    public static void AddCoreServices(this IServiceCollection services)
    {
        services.AddSingleton<ILogParser, JsonLogParser>();
    }
}