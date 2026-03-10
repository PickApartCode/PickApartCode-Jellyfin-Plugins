using Jellyfin.Plugin.Suggestions.Models;
using Jellyfin.Plugin.Suggestions.Services;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Net;
using MediaBrowser.Model.Services;

namespace Jellyfin.Plugin.Suggestions.Api;

public sealed class SuggestionService : IService
{
    private readonly IUserManager _userManager;
    private readonly ISuggestionStore _suggestionStore;
    private readonly SuggestionMetadataService _metadataService;
    private readonly SuggestionDuplicateService _duplicateService;

    public SuggestionService(
        IUserManager userManager,
        ISuggestionStore suggestionStore,
        SuggestionMetadataService metadataService,
        SuggestionDuplicateService duplicateService)
    {
        _userManager = userManager;
        _suggestionStore = suggestionStore;
        _metadataService = metadataService;
        _duplicateService = duplicateService;
    }

    public async Task<object> Get(GetSuggestions request)
    {
        var all = await _suggestionStore.GetAllAsync(CancellationToken.None).ConfigureAwait(false);
        var filtered = all.AsEnumerable();

        if (request.Status.HasValue)
        {
            filtered = filtered.Where(x => x.Status == request.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.UserId))
        {
            filtered = filtered.Where(x => x.UserId == request.UserId);
        }

        return filtered.OrderByDescending(x => x.RequestedAt).ToArray();
    }

    public async Task<object> Post(CreateSuggestion request)
    {
        var user = _userManager.GetUserById(Session.UserId) ?? throw new UnauthorizedAccessException();

        var suggestion = new SuggestionRequest
        {
            UserId = user.Id.ToString("N"),
            UserName = user.Username,
            MediaType = request.MediaType,
            SearchTerm = request.SearchTerm,
            Comment = request.Comment,
            Status = SuggestionStatus.Pending
        };

        await _metadataService.HydrateAsync(suggestion, CancellationToken.None).ConfigureAwait(false);
        _duplicateService.MarkDuplicateIfExists(suggestion);

        return await _suggestionStore.CreateAsync(suggestion, CancellationToken.None).ConfigureAwait(false);
    }

    public async Task<object> Post(ModerateSuggestion request)
    {
        var moderator = _userManager.GetUserById(Session.UserId) ?? throw new UnauthorizedAccessException();
        if (!moderator.Policy.IsAdministrator)
        {
            throw new UnauthorizedAccessException("Only administrators can moderate suggestions.");
        }

        var suggestion = await _suggestionStore.GetAsync(request.Id, CancellationToken.None).ConfigureAwait(false)
            ?? throw new ResourceNotFoundException("Suggestion was not found");

        suggestion.Status = request.Status;
        suggestion.ModeratorUserId = moderator.Id.ToString("N");
        suggestion.ModerationNote = request.ModerationNote;
        suggestion.ModeratedAt = DateTimeOffset.UtcNow;

        if (request.Status == SuggestionStatus.Added)
        {
            suggestion.AddedByUserId = moderator.Id.ToString("N");
            suggestion.AddedAt = DateTimeOffset.UtcNow;
        }

        return await _suggestionStore.UpdateAsync(suggestion, CancellationToken.None).ConfigureAwait(false);
    }
}
