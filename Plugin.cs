using System;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using Jellyfin.Plugin.Suggestions.Configuration;

namespace Jellyfin.Plugin.Suggestions;

public sealed class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
    public static Plugin? Instance { get; private set; }

    public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
        : base(applicationPaths, xmlSerializer)
    {
        Instance = this;
    }

    public override string Name => "Content Suggestions";

    public override Guid Id => Guid.Parse("2a95cbab-9ced-4093-ac5a-cbfbe35e4e95");

    public PluginPageInfo[] GetPages()
        =>
        [
            new PluginPageInfo
            {
                Name = "suggestions",
                EmbeddedResourcePath = $"{GetType().Namespace}.Web.admin.suggestions.html"
            },
            new PluginPageInfo
            {
                Name = "suggestionsjs",
                EmbeddedResourcePath = $"{GetType().Namespace}.Web.admin.suggestions.js"
            }
        ];
}
