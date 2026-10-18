using Jellyfin.Plugin.MediaOnly.Configuration;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Resolvers;
using MediaBrowser.Model.IO;

namespace Jellyfin.Plugin.MediaOnly.Rules;

/// <summary>
/// Regla de resolución que ignora durante el escaneo cualquier archivo cuya
/// extensión no esté en la lista de extensiones permitidas (vídeo/imagen),
/// pero <b>solo dentro de las bibliotecas seleccionadas</b> en la
/// configuración del plugin.
///
/// Jellyfin descubre e inyecta automáticamente las implementaciones de
/// <see cref="IResolverIgnoreRule"/> que encuentra en los plugins.
/// </summary>
public class MediaOnlyIgnoreRule : IResolverIgnoreRule
{
    private static readonly StringComparison PathComparison =
        OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

    private readonly ILibraryManager _libraryManager;
    private readonly object _cacheLock = new();
    private List<string> _cachedSelection = new();
    private string[] _cachedRoots = Array.Empty<string>();

    /// <summary>
    /// Initializes a new instance of the <see cref="MediaOnlyIgnoreRule"/> class.
    /// </summary>
    /// <param name="libraryManager">Gestor de bibliotecas.</param>
    public MediaOnlyIgnoreRule(ILibraryManager libraryManager)
    {
        _libraryManager = libraryManager;
    }

    /// <inheritdoc />
    public bool ShouldIgnore(FileSystemMetadata fileInfo, BaseItem? parent)
    {
        var config = Plugin.Instance?.Configuration;
        if (config is null
            || !config.Enabled
            || fileInfo.IsDirectory
            || config.TargetLibraries.Count == 0)
        {
            return false;
        }

        if (!IsInTargetLibrary(fileInfo.FullName, config))
        {
            return false;
        }

        var extension = Path.GetExtension(fileInfo.FullName);
        if (extension.Length == 0)
        {
            return config.IgnoreFilesWithoutExtension;
        }

        return !config.AllowedExtensions.Contains(
            extension.TrimStart('.'),
            StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Comprueba si la ruta pertenece a alguna de las bibliotecas objetivo,
    /// comparándola con las ubicaciones físicas de dichas bibliotecas.
    /// </summary>
    private bool IsInTargetLibrary(string path, PluginConfiguration config)
    {
        foreach (var root in GetTargetRoots(config))
        {
            if (IsPathInside(path, root))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Devuelve las rutas físicas de las bibliotecas seleccionadas, con caché
    /// invalidada cuando cambia la selección.
    /// </summary>
    private string[] GetTargetRoots(PluginConfiguration config)
    {
        lock (_cacheLock)
        {
            if (_cachedSelection.SequenceEqual(config.TargetLibraries))
            {
                return _cachedRoots;
            }

            var roots = _libraryManager.GetVirtualFolders()
                .Where(vf => config.TargetLibraries.Contains(vf.Name, StringComparer.OrdinalIgnoreCase))
                .SelectMany(vf => vf.Locations)
                .Where(loc => !string.IsNullOrWhiteSpace(loc))
                .ToArray();

            _cachedSelection = config.TargetLibraries.ToList();
            _cachedRoots = roots;
            return roots;
        }
    }

    /// <summary>
    /// Comprueba que <paramref name="path"/> está dentro de
    /// <paramref name="root"/>, exigiendo límite de directorio para evitar
    /// falsos positivos (p. ej. "D:\Fotos" vs "D:\FotosExtra").
    /// </summary>
    private static bool IsPathInside(string path, string root)
    {
        if (!path.StartsWith(root, PathComparison))
        {
            return false;
        }

        if (path.Length == root.Length)
        {
            return true;
        }

        var separator = path[root.Length];
        return separator is '/' or '\\';
    }
}
