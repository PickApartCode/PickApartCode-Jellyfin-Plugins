using Jellyfin.Plugin.Suggestions.Models;

namespace Jellyfin.Plugin.Suggestions.Services;

public interface ISuggestionStore
{
    Task<IReadOnlyList<SuggestionRequest>> GetAllAsync(CancellationToken cancellationToken);
    Task<SuggestionRequest?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<SuggestionRequest> CreateAsync(SuggestionRequest suggestion, CancellationToken cancellationToken);
    Task<SuggestionRequest> UpdateAsync(SuggestionRequest suggestion, CancellationToken cancellationToken);
}
