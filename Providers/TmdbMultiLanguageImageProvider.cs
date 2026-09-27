using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.TmdbMultiLanguage.Models;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.TmdbMultiLanguage.Providers;

public sealed class TmdbMultiLanguageImageProvider : IRemoteImageProvider, IHasOrder
{
    private const string ApiBase = "https://api.themoviedb.org/3";
    private const string ImageBase = "https://image.tmdb.org/t/p/original";
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TmdbMultiLanguageImageProvider> _logger;

    public TmdbMultiLanguageImageProvider(IHttpClientFactory httpClientFactory, ILogger<TmdbMultiLanguageImageProvider> logger)
    { _httpClientFactory = httpClientFactory; _logger = logger; }

    public string Name => "TMDB Multi-Language";
    public int Order => 0;
    public bool Supports(BaseItem item) => item is Movie or Series or Season;
    public IEnumerable<ImageType> GetSupportedImages(BaseItem item) => new[] { ImageType.Primary, ImageType.Backdrop, ImageType.Logo };

    public async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken)
    {
        var config = Plugin.Instance?.Configuration;
        if (config is null || string.IsNullOrWhiteSpace(config.TmdbApiKey)) return Array.Empty<RemoteImageInfo>();
        var tmdbId = item.GetProviderId(MetadataProvider.Tmdb);
        if (item is Season season && string.IsNullOrWhiteSpace(tmdbId))
        {
            tmdbId = season.Series?.GetProviderId(MetadataProvider.Tmdb);
        }

        if (string.IsNullOrWhiteSpace(tmdbId)) return Array.Empty<RemoteImageInfo>();

        var primary = Parse(config.GetLanguagesFor(ImageType.Primary));
        var backdrop = Parse(config.GetLanguagesFor(ImageType.Backdrop));
        var logo = Parse(config.GetLanguagesFor(ImageType.Logo));
        var langs = BuildLanguageParam(primary, backdrop, logo);
        string url;
        if (item is Season season)
        {
            if (!season.IndexNumber.HasValue) return Array.Empty<RemoteImageInfo>();
            url = $"{ApiBase}/tv/{Uri.EscapeDataString(tmdbId)}/season/{season.IndexNumber.Value}/images?api_key={Uri.EscapeDataString(config.TmdbApiKey)}&include_image_language={Uri.EscapeDataString(langs)}";
        }
        else
        {
            var mediaType = item is Movie ? "movie" : "tv";
            url = $"{ApiBase}/{mediaType}/{Uri.EscapeDataString(tmdbId)}/images?api_key={Uri.EscapeDataString(config.TmdbApiKey)}&include_image_language={Uri.EscapeDataString(langs)}";
        }

        try
        {
            using var response = await _httpClientFactory.CreateClient().GetAsync(url, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("TMDB returned {StatusCode} for {Item}", (int)response.StatusCode, item.Name);
                return Array.Empty<RemoteImageInfo>();
            }
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            var data = await JsonSerializer.DeserializeAsync<TmdbImageResponse>(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
            var result = new List<RemoteImageInfo>();
            Add(result, data?.Posters, ImageType.Primary, primary, config.IgnoreUnratedImages);
            Add(result, data?.Backdrops, ImageType.Backdrop, backdrop, config.IgnoreUnratedImages);
            Add(result, data?.Logos, ImageType.Logo, logo, config.IgnoreUnratedImages);
            return result;
        }
        catch (Exception ex) when (ex is HttpRequestException or JsonException or TaskCanceledException)
        {
            _logger.LogWarning(ex, "Unable to fetch TMDB images for {Item}", item.Name);
            return Array.Empty<RemoteImageInfo>();
        }
    }

    public Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken) =>
        _httpClientFactory.CreateClient().GetAsync(url, cancellationToken);

    private void Add(List<RemoteImageInfo> target, List<TmdbImage>? source, ImageType type, List<string?> priority, bool ignoreUnrated)
    {
        if (source is null) return;
        target.AddRange(source.Where(x => !ignoreUnrated || x.VoteAverage > 0)
            .Select(x => new { Image=x, Priority=Priority(x.Iso6391, priority) }).Where(x => x.Priority >= 0)
            .OrderBy(x => x.Priority).ThenByDescending(x => x.Image.VoteAverage)
            .Select(x => new RemoteImageInfo { Url=ImageBase+x.Image.FilePath, Type=type, ProviderName=Name, Language=x.Image.Iso6391, Width=x.Image.Width, Height=x.Image.Height, CommunityRating=x.Image.VoteAverage }));
    }

    private static List<string?> Parse(string? value) => (value ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Select(x => x.Equals("null", StringComparison.OrdinalIgnoreCase) ? null : x)
        .Distinct(StringComparer.OrdinalIgnoreCase).ToList();

    private static int Priority(string? lang, List<string?> order)
    {
        for (var i=0;i<order.Count;i++)
            if ((order[i] is null && string.IsNullOrEmpty(lang)) || string.Equals(order[i], lang, StringComparison.OrdinalIgnoreCase)) return i;
        return -1;
    }

    private static string BuildLanguageParam(params List<string?>[] lists) =>
        string.Join(",", lists.SelectMany(x => x).Select(x => x ?? "null").Distinct(StringComparer.OrdinalIgnoreCase));
}