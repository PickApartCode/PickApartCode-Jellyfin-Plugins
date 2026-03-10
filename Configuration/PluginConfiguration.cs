using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.Suggestions.Configuration;

public sealed class PluginConfiguration : BasePluginConfiguration
{
    public bool EnableDuplicateCheck { get; set; } = true;
    public bool RequireAdminApproval { get; set; } = true;
    public int MaxSuggestionsPerUserPerDay { get; set; } = 20;
}
