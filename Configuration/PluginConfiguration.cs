using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.MediaOnly.Configuration;

/// <summary>
/// Configuración del plugin Media Only Filter.
/// </summary>
public class PluginConfiguration : BasePluginConfiguration
{
    /// <summary>
    /// Extensiones de vídeo permitidas por defecto (sin punto, en minúsculas).
    /// </summary>
    public static readonly string[] DefaultVideoExtensions =
    {
        "3g2", "3gp", "asf", "avi", "divx", "dvr-ms", "f4v", "flv", "img",
        "iso", "m2t", "m2ts", "m4v", "mk3d", "mkv", "mov", "mp2", "mp4",
        "mpeg", "mpg", "mts", "mxf", "ogm", "ogv", "rm", "rmvb", "strm",
        "ts", "vob", "webm", "wmv", "xvid"
    };

    /// <summary>
    /// Extensiones de imagen permitidas por defecto (sin punto, en minúsculas).
    /// </summary>
    public static readonly string[] DefaultImageExtensions =
    {
        "arw", "avif", "bmp", "cr2", "cr3", "dng", "gif", "heic", "heif",
        "jfif", "jpe", "jpeg", "jpg", "jxl", "nef", "orf", "png", "raf",
        "rw2", "svg", "tif", "tiff", "webp"
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
    /// </summary>
    public PluginConfiguration()
    {
        Enabled = true;
        IgnoreFilesWithoutExtension = true;
        AllowedExtensions = DefaultVideoExtensions
            .Concat(DefaultImageExtensions)
            .ToList();
        TargetLibraries = new List<string>();
    }

    /// <summary>
    /// Gets or sets a value indicating whether el filtro está activo.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether se ignoran archivos sin extensión.
    /// </summary>
    public bool IgnoreFilesWithoutExtension { get; set; }

    /// <summary>
    /// Gets or sets the list of allowed extensions (without leading dot).
    /// </summary>
    public List<string> AllowedExtensions { get; set; }

    /// <summary>
    /// Gets or sets the names of the libraries (virtual folders) where the
    /// filter applies. Vacío = el filtro no se aplica a ninguna biblioteca.
    /// </summary>
    public List<string> TargetLibraries { get; set; }
}
