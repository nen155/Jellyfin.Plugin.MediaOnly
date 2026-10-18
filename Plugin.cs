using Jellyfin.Plugin.MediaOnly.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace Jellyfin.Plugin.MediaOnly;

/// <summary>
/// Plugin principal: filtra archivos que no sean vídeo o imagen durante el
/// escaneo de bibliotecas.
/// </summary>
public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Plugin"/> class.
    /// </summary>
    /// <param name="applicationPaths">Rutas de la aplicación.</param>
    /// <param name="xmlSerializer">Serializador XML de configuración.</param>
    public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
        : base(applicationPaths, xmlSerializer)
    {
        Instance = this;
    }

    /// <summary>
    /// Gets the current plugin instance.
    /// </summary>
    public static Plugin? Instance { get; private set; }

    /// <inheritdoc />
    public override string Name => "Media Only Filter";

    /// <inheritdoc />
    public override string Description =>
        "Ignora durante el escaneo cualquier archivo cuya extensión no sea de vídeo o imagen.";

    /// <inheritdoc />
    public override Guid Id => Guid.Parse("7f3a2b1e-9c4d-4e8a-b6f2-1d5c8a9e3b07");

    /// <inheritdoc />
    public IEnumerable<PluginPageInfo> GetPages()
    {
        return new[]
        {
            new PluginPageInfo
            {
                Name = "MediaOnlyFilter",
                EmbeddedResourcePath = $"{GetType().Namespace}.Configuration.configPage.html",
                EnableInMainMenu = true,
                MenuSection = "server",
                MenuIcon = "filter_list"
            }
        };
    }
}
