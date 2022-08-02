## War3Net.Build

[![NuGet downloads](https://img.shields.io/nuget/dt/War3Net.Build.svg)](https://www.nuget.org/packages/War3Net.Build)
[![NuGet version](https://img.shields.io/nuget/v/War3Net.Build.svg)](https://www.nuget.org/packages/War3Net.Build)
[![NuGet prerelease](https://img.shields.io/nuget/vpre/War3Net.Build.svg)](https://www.nuget.org/packages/War3Net.Build/absoluteLatest)

## Description

War3Net.Build is a library for generating the Wacraft III map script and MPQ archive, by reading from C#/vJass source code and war3map files.

## Examples

Please take a look at [War3Map.Template](https://github.com/Drake53/War3Map.Template) to see how to use this library.
This example also works when building a JASS map, with the following adjustments:
```csharp
// ScriptCompilerOptions options = ...
options.MapInfo.ScriptLanguage = ScriptLanguage.Jass;
options.JasshelperCliPath = "path\\to\\jasshelper.exe";
options.CommonJPath = "path\\to\\common.j";
options.BlizzardJPath = "path\\to\\Blizzard.j";
```

## Dependencies

https://github.com/Drake53/War3Net/network/dependencies

NuGet packages:
- [War3Api.Blizzard](https://www.nuget.org/packages/War3Api.Blizzard)
- [War3Net.Build.Core](https://www.nuget.org/packages/War3Net.Build.Core)
- [War3Net.CodeAnalysis.Jass](https://www.nuget.org/packages/War3Net.CodeAnalysis.Jass)
- [War3Net.CSharpLua](https://www.nuget.org/packages/War3Net.CSharpLua)
