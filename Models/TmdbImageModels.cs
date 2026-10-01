using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.TmdbMultiLanguage.Models;

public sealed class TmdbImageResponse
{
    [JsonPropertyName("posters")] public List<TmdbImage>? Posters { get; set; }
    [JsonPropertyName("backdrops")] public List<TmdbImage>? Backdrops { get; set; }
    [JsonPropertyName("logos")] public List<TmdbImage>? Logos { get; set; }
    [JsonPropertyName("stills")] public List<TmdbImage>? Stills { get; set; }
}

public sealed class TmdbImage
{
    [JsonPropertyName("file_path")] public string FilePath { get; set; } = string.Empty;
    [JsonPropertyName("iso_639_1")] public string? Iso6391 { get; set; }
    [JsonPropertyName("width")] public int Width { get; set; }
    [JsonPropertyName("height")] public int Height { get; set; }
    [JsonPropertyName("vote_average")] public double VoteAverage { get; set; }
    [JsonPropertyName("vote_count")] public int VoteCount { get; set; }
}