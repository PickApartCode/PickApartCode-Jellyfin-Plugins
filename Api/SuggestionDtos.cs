using Jellyfin.Plugin.Suggestions.Models;
using MediaBrowser.Model.Services;

namespace Jellyfin.Plugin.Suggestions.Api;

[Route("/Suggestions", "GET", Summary = "Gets suggestions", IsHidden = false)]
public sealed class GetSuggestions : IReturn<IReadOnlyList<SuggestionRequest>>
{
    public SuggestionStatus? Status { get; set; }
    public string? UserId { get; set; }
}

[Route("/Suggestions", "POST", Summary = "Creates a suggestion", IsHidden = false)]
public sealed class CreateSuggestion : IReturn<SuggestionRequest>
{
    public SuggestionMediaType MediaType { get; set; }
    public string SearchTerm { get; set; } = string.Empty;
    public string? Comment { get; set; }
}

[Route("/Suggestions/{Id}", "POST", Summary = "Moderates a suggestion", IsHidden = false)]
public sealed class ModerateSuggestion : IReturn<SuggestionRequest>
{
    public Guid Id { get; set; }
    public SuggestionStatus Status { get; set; }
    public string? ModerationNote { get; set; }
}
