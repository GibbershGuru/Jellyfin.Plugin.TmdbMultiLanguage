using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.TmdbMultiLanguage;

public sealed class PluginConfiguration : BasePluginConfiguration
{
    public string TmdbApiKey { get; set; } = string.Empty;
    public string PreferredLanguages { get; set; } = "de,en,null";
    public string PrimaryLanguages { get; set; } = string.Empty;
    public string BackdropLanguages { get; set; } = string.Empty;
    public string LogoLanguages { get; set; } = string.Empty;
    public bool IgnoreUnratedImages { get; set; }
    public bool EnableDebugMode { get; set; }

    public string GetLanguagesFor(ImageType imageType)
    {
        var value = imageType switch
        {
            ImageType.Primary => PrimaryLanguages,
            ImageType.Backdrop => BackdropLanguages,
            ImageType.Logo => LogoLanguages,
            _ => string.Empty
        };
        return string.IsNullOrWhiteSpace(value) ? PreferredLanguages : value;
    }
}