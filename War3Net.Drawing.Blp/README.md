## War3Net.Drawing.Blp
### Based on code from [SereniaBLPLib](https://github.com/WoW-Tools/SereniaBLPLib)

[![NuGet downloads](https://img.shields.io/nuget/dt/War3Net.Drawing.Blp.svg)](https://www.nuget.org/packages/War3Net.Drawing.Blp)
[![NuGet version](https://img.shields.io/nuget/v/War3Net.Drawing.Blp.svg)](https://www.nuget.org/packages/War3Net.Drawing.Blp)

## Description

War3Net.Drawing.Blp is a library for reading files with the ".blp" extension.

The BLP file format is used to store images/textures. There exist three formats of the file type: BLP0, BLP1, and BLP2.
- BLP0 is used in Warcraft III beta, and is currently not supported by this library.
- BLP1 is used in Warcraft III Reign of Chaos and The Frozen Throne.
- BLP2 is used in World of Warcraft.

## Examples

```C#
using (var fileStream = File.OpenRead(inputImagePath))
{
    var blpFile = new BlpFile(fileStream);
    var bitmap = blpFile.GetSKBitmap();

    return bitmap;
}
```

## Dependencies

War3Net.Drawing.Blp depends on [SkiaSharp](https://github.com/mono/SkiaSharp) to decode images.
Depending on the target framework, additional methods are available to get the image in a different bitmap type (SKBitmap, Bitmap, BitmapSource).

- [![dotnet framework 4.6](https://img.shields.io/badge/.NET%20framework-v4.6-brightgreen.svg)](https://github.com/microsoft/dotnet/blob/master/Documentation/compatibility/README.md#net-framework-46)
    [![SkiaSharp](https://img.shields.io/badge/SkiaSharp-v1.68.0-blue.svg)](https://www.nuget.org/packages/SkiaSharp)

- [![dotnet standard 1.3](https://img.shields.io/badge/.NET%20standard-v1.3-brightgreen.svg)](https://github.com/dotnet/standard/blob/master/docs/versions/netstandard1.3.md)
    [![SkiaSharp](https://img.shields.io/badge/SkiaSharp-v1.68.0-blue.svg)](https://www.nuget.org/packages/SkiaSharp)
    [![NETStandard.Library](https://img.shields.io/badge/NETStandard.Library-v1.6.1-blue.svg)](https://www.nuget.org/packages/NETStandard.Library/)

- [![dotnet standard 2.0](https://img.shields.io/badge/.NET%20standard-v2.0-brightgreen.svg)](https://github.com/dotnet/standard/blob/master/docs/versions/netstandard2.0.md)
    [![SkiaSharp](https://img.shields.io/badge/SkiaSharp-v1.68.0-blue.svg)](https://www.nuget.org/packages/SkiaSharp)
    [![System.Drawing.Common](https://img.shields.io/badge/System.Drawing.Common-v4.5.1-blue.svg)](https://www.nuget.org/packages/System.Drawing.Common)
