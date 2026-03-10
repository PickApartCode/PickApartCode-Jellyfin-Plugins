using Jellyfin.Plugin.Suggestions.Models;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Querying;

namespace Jellyfin.Plugin.Suggestions.Services;

public sealed class SuggestionDuplicateService
{
    private readonly ILibraryManager _libraryManager;

    public SuggestionDuplicateService(ILibraryManager libraryManager)
    {
        _libraryManager = libraryManager;
    }

    public void MarkDuplicateIfExists(SuggestionRequest request)
    {
        var query = new InternalItemsQuery
        {
            IncludeItemTypes = ResolveItemTypes(request.MediaType),
            SearchTerm = request.Name ?? request.SearchTerm,
            Recursive = true,
            Limit = 5
        };

        var matches = _libraryManager.GetItemList(query)
            .Where(x => IsLikelySame(request, x))
            .ToList();

        if (matches.Count == 0)
        {
            request.IsDuplicate = false;
            request.DuplicateReason = null;
            return;
        }

        request.IsDuplicate = true;
        request.DuplicateReason = $"Already present in library ({matches[0].Name}).";
    }

    private static bool IsLikelySame(SuggestionRequest request, BaseItem item)
    {
        if (!string.IsNullOrWhiteSpace(request.ProviderId) && item.ProviderIds?.Values.Contains(request.ProviderId) == true)
        {
            return true;
        }

        if (!string.Equals(request.Name, item.Name, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (request.ProductionYear.HasValue && item.ProductionYear.HasValue)
        {
            return request.ProductionYear.Value == item.ProductionYear.Value;
        }

        return true;
    }

    private static string[] ResolveItemTypes(SuggestionMediaType mediaType)
        => mediaType switch
        {
            SuggestionMediaType.Movie => ["Movie"],
            SuggestionMediaType.TvShow => ["Series"],
            SuggestionMediaType.Book => ["Book"],
            SuggestionMediaType.AudioBook => ["AudioBook"],
            SuggestionMediaType.Song => ["Audio"],
            _ => []
        };
}
