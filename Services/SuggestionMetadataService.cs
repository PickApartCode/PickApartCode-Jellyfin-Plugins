using Jellyfin.Plugin.Suggestions.Models;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace Jellyfin.Plugin.Suggestions.Services;

public sealed class SuggestionMetadataService
{
    private readonly IProviderManager _providerManager;

    public SuggestionMetadataService(IProviderManager providerManager)
    {
        _providerManager = providerManager;
    }

    public async Task<SuggestionRequest> HydrateAsync(SuggestionRequest request, CancellationToken cancellationToken)
    {
        RemoteSearchResult? best = request.MediaType switch
        {
            SuggestionMediaType.Movie => await SearchMovieAsync(request, cancellationToken).ConfigureAwait(false),
            SuggestionMediaType.TvShow => await SearchSeriesAsync(request, cancellationToken).ConfigureAwait(false),
            SuggestionMediaType.Book => await SearchBookAsync(request, cancellationToken).ConfigureAwait(false),
            SuggestionMediaType.AudioBook => await SearchBookAsync(request, cancellationToken).ConfigureAwait(false),
            SuggestionMediaType.Song => await SearchSongAsync(request, cancellationToken).ConfigureAwait(false),
            _ => null
        };

        if (best is null)
        {
            return request;
        }

        request.Name = best.Name;
        request.ProductionYear = best.ProductionYear;
        request.Overview = best.Overview;
        request.ProviderName = best.ProviderName;
        request.ProviderId = best.ProviderIds?.FirstOrDefault().Value;
        return request;
    }

    private async Task<RemoteSearchResult?> SearchMovieAsync(SuggestionRequest request, CancellationToken cancellationToken)
    {
        var query = new RemoteSearchQuery<MovieInfo>
        {
            SearchInfo = new MovieInfo
            {
                Name = request.SearchTerm,
                Year = request.ProductionYear
            }
        };

        var results = await _providerManager.GetRemoteSearchResults(query, cancellationToken).ConfigureAwait(false);
        return results.FirstOrDefault();
    }

    private async Task<RemoteSearchResult?> SearchSeriesAsync(SuggestionRequest request, CancellationToken cancellationToken)
    {
        var query = new RemoteSearchQuery<SeriesInfo>
        {
            SearchInfo = new SeriesInfo
            {
                Name = request.SearchTerm,
                Year = request.ProductionYear
            }
        };

        var results = await _providerManager.GetRemoteSearchResults(query, cancellationToken).ConfigureAwait(false);
        return results.FirstOrDefault();
    }

    private async Task<RemoteSearchResult?> SearchBookAsync(SuggestionRequest request, CancellationToken cancellationToken)
    {
        var query = new RemoteSearchQuery<BookInfo>
        {
            SearchInfo = new BookInfo
            {
                Name = request.SearchTerm,
                Year = request.ProductionYear
            }
        };

        var results = await _providerManager.GetRemoteSearchResults(query, cancellationToken).ConfigureAwait(false);
        return results.FirstOrDefault();
    }

    private async Task<RemoteSearchResult?> SearchSongAsync(SuggestionRequest request, CancellationToken cancellationToken)
    {
        var query = new RemoteSearchQuery<AudioInfo>
        {
            SearchInfo = new AudioInfo
            {
                Name = request.SearchTerm,
                Year = request.ProductionYear
            }
        };

        var results = await _providerManager.GetRemoteSearchResults(query, cancellationToken).ConfigureAwait(false);
        return results.FirstOrDefault();
    }
}
