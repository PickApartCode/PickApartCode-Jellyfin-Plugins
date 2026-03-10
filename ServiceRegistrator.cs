using Jellyfin.Plugin.Suggestions.Services;
using MediaBrowser.Common.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace Jellyfin.Plugin.Suggestions;

public sealed class ServiceRegistrator : IPluginServiceRegistrator
{
    public void RegisterServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<ISuggestionStore, JsonSuggestionStore>();
        serviceCollection.AddSingleton<SuggestionMetadataService>();
        serviceCollection.AddSingleton<SuggestionDuplicateService>();
    }
}
