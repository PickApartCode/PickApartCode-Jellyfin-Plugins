using System.Text.Json;
using Jellyfin.Plugin.Suggestions.Models;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Extensions;

namespace Jellyfin.Plugin.Suggestions.Services;

public sealed class JsonSuggestionStore : ISuggestionStore
{
    private readonly string _dataFile;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    public JsonSuggestionStore(IApplicationPaths appPaths)
    {
        var dataPath = Path.Combine(appPaths.DataPath, "suggestions");
        Directory.CreateDirectory(dataPath);
        _dataFile = Path.Combine(dataPath, "requests.json");
    }

    public async Task<IReadOnlyList<SuggestionRequest>> GetAllAsync(CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            return await ReadUnsafeAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<SuggestionRequest?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var all = await GetAllAsync(cancellationToken).ConfigureAwait(false);
        return all.FirstOrDefault(x => x.Id == id);
    }

    public async Task<SuggestionRequest> CreateAsync(SuggestionRequest suggestion, CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var all = await ReadUnsafeAsync(cancellationToken).ConfigureAwait(false);
            all.Add(suggestion);
            await WriteUnsafeAsync(all, cancellationToken).ConfigureAwait(false);
            return suggestion;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<SuggestionRequest> UpdateAsync(SuggestionRequest suggestion, CancellationToken cancellationToken)
    {
        await _lock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var all = await ReadUnsafeAsync(cancellationToken).ConfigureAwait(false);
            var index = all.FindIndex(x => x.Id == suggestion.Id);
            if (index < 0)
            {
                throw new KeyNotFoundException($"Suggestion {suggestion.Id} was not found.");
            }

            all[index] = suggestion;
            await WriteUnsafeAsync(all, cancellationToken).ConfigureAwait(false);
            return suggestion;
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<List<SuggestionRequest>> ReadUnsafeAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_dataFile))
        {
            return [];
        }

        await using var stream = File.OpenRead(_dataFile);
        var data = await JsonSerializer.DeserializeAsync<List<SuggestionRequest>>(stream, _jsonOptions, cancellationToken)
            .ConfigureAwait(false);
        return data ?? [];
    }

    private async Task WriteUnsafeAsync(List<SuggestionRequest> suggestions, CancellationToken cancellationToken)
    {
        var tmp = _dataFile + ".tmp";
        await using (var stream = File.Create(tmp))
        {
            await JsonSerializer.SerializeAsync(stream, suggestions, _jsonOptions, cancellationToken).ConfigureAwait(false);
        }

        FileSystemExtensions.CopyFile(tmp, _dataFile, true);
        File.Delete(tmp);
    }
}
