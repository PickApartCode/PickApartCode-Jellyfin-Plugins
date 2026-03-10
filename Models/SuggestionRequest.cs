namespace Jellyfin.Plugin.Suggestions.Models;

public enum SuggestionMediaType
{
    Movie,
    TvShow,
    Book,
    AudioBook,
    Song
}

public enum SuggestionStatus
{
    Pending,
    Approved,
    Rejected,
    Added
}

public sealed class SuggestionRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public SuggestionMediaType MediaType { get; set; }
    public string SearchTerm { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public DateTimeOffset RequestedAt { get; set; } = DateTimeOffset.UtcNow;
    public SuggestionStatus Status { get; set; } = SuggestionStatus.Pending;
    public string? ModeratorUserId { get; set; }
    public string? ModerationNote { get; set; }
    public DateTimeOffset? ModeratedAt { get; set; }
    public string? AddedByUserId { get; set; }
    public DateTimeOffset? AddedAt { get; set; }

    // Populated by metadata matching.
    public string? Name { get; set; }
    public int? ProductionYear { get; set; }
    public string? Overview { get; set; }
    public string? ProviderId { get; set; }
    public string? ProviderName { get; set; }

    // Duplicate signals.
    public bool IsDuplicate { get; set; }
    public string? DuplicateReason { get; set; }
}
