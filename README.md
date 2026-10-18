# Jellyfin.Plugin.MediaOnly

Plugin para **Jellyfin** que ignora durante el escaneo de bibliotecas cualquier
archivo cuya extensión **no sea de vídeo o imagen**.

## Cómo funciona

Implementa `IResolverIgnoreRule` (`MediaBrowser.Controller.Resolvers`), la
interfaz que Jellyfin usa para decidir si una ruta entra o no en una biblioteca.
Jellyfin descubre e inyecta automáticamente las implementaciones encontradas en
los plugins instalados, así que basta con instalar el plugin:

- **Directorios**: nunca se ignoran (el scanner debe seguir descendiendo).
- **Bibliotecas seleccionadas**: el filtro solo se aplica a las bibliotecas que
  marques en la configuración (se resuelven por su ruta física). Las demás
  bibliotecas se comportan como siempre: subtítulos, `.nfo`, etc. intactos.
- **Archivos con extensión permitida**: se procesan con normalidad.
- **Archivos con otra extensión o sin extensión**: se ignoran y ni siquiera
  aparecen como items en la biblioteca.

La configuración está en **Panel → Plugins → Media Only Filter**. Si no marcas
ninguna biblioteca, el plugin no filtra nada.

### Extensiones por defecto

- **Vídeo**: `mp4 mkv avi mov wmv m4v mpg mpeg ts m2ts mts vob webm flv iso img rm rmvb mxf strm …`
- **Imagen**: `jpg jpeg png gif bmp tif tiff webp heic heif avif jxl svg` + RAW (`cr2 cr3 nef arw dng …`)

> **Ojo**: el filtro solo afecta a las bibliotecas seleccionadas, por lo que tus
> bibliotecas de películas/series con subtítulos (`.srt`, `.ass`) y `.nfo`
> siguen funcionando con normalidad. Si una biblioteca filtrada también necesita
> subtítulos o metadatos locales, añade esas extensiones a la lista.
> El arte de carpetas (`poster.jpg`, `folder.png`, etc.) ya está cubierto por
> las extensiones de imagen.

## Compilación

Requiere .NET SDK 9.0:

```powershell
dotnet publish -c Release
```

El ensamblado queda en
`bin\Release\net9.0\publish\Jellyfin.Plugin.MediaOnly.dll`.

## Instalación manual

1. Detén el servidor Jellyfin.
2. Crea la carpeta `MediaOnly` dentro del directorio de plugins:
   - Windows (instalador): `%ProgramData%\Jellyfin\Server\plugins\MediaOnly\`
   - Windows (portable): `<jellyfin>\data\plugins\MediaOnly\`
   - Linux/Docker: `<config>/plugins/MediaOnly/`
3. Copia dentro **solo** `Jellyfin.Plugin.MediaOnly.dll`
   (las DLLs `Jellyfin.Controller`/`Jellyfin.Model` ya las aporta el servidor).
4. Arranca Jellyfin y comprueba en **Panel → Plugins** que aparece *Activo*.
5. Ejecuta un **escaneo de bibliotecas** para aplicar el filtro.

## Instalación vía repositorio de plugins (opcional)

El `build.yaml` incluido permite empaquetar el plugin con
[jprm](https://github.com/oddstr13/jellyfin-plugin-repository-manager)
y publicarlo en tu propio repositorio de plugins:

```bash
jprm --verbosity=debug plugin build .
jprm --verbosity=debug repo build --url=<url-base> --output=repo .
```

## Compatibilidad

- Compilado contra `Jellyfin.Controller` **10.11.x** y `net9.0`
  (`targetAbi: 10.11.0.0` en `build.yaml`).
- Si tu servidor es otra versión (p. ej. 10.10 → net8.0, o 12.x), ajusta
  `TargetFramework`, la versión de los `PackageReference` y `targetAbi`, y
  recompila.
