using LogViewer.Model.Abstractions;
using LogViewer.Model.Parsers;
using LogViewer.ViewModel.Abstractions;
using LogViewer.ViewModel.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace LogViewer.Configuration;

public static class ServicesConfiguration
{
    public static void AddCoreServices(this IServiceCollection services)
    {
        services.AddSingleton<ILogParser, JsonLogParser>();
    }

    public static void AddViewModels(this IServiceCollection services)
    {
        services.AddSingleton<IMainViewModel, MainViewModel>();
    }
}