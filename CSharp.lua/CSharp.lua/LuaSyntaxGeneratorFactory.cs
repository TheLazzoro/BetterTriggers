#nullable enable

using Cake.Incubator.Project;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;

namespace CSharpLua {
  internal static class LuaSyntaxGeneratorFactory {
    private const string configurationDebug = "Debug";
    private const string configurationRelease = "Release";
    private const string kDllSuffix = ".dll";
    private const char kLuaModuleSuffix = '!';

    internal static LuaSyntaxGenerator CreateGenerator(CompilerSettings? settings, LuaSyntaxGenerator.SettingInfo settingInfo) {
      return settings is null
        ? CreateGeneratorForFile(settingInfo)
        : settings.isProject_
          ? CreateGeneratorForProject(settings, settingInfo)
          : CreateGeneratorForFolder(settings, settingInfo);
    }

    internal static LuaSyntaxGenerator CreateGeneratorForFile(LuaSyntaxGenerator.SettingInfo settingInfo) {
      throw new NotImplementedException();
    }

    internal static LuaSyntaxGenerator CreateGeneratorForFolder(CompilerSettings settings, LuaSyntaxGenerator.SettingInfo settingInfo) {
      var folderPath = settings.input_;
      var files = GetFolderSourceFiles(folderPath);
      var packageBaseFolders = new List<string>();
      var codes = files.Select(i => (File.ReadAllText(i), i));
      var libs = GetLibs(settings.libs_, out var luaModuleLibs);
      settingInfo.LuaModuleLibs = luaModuleLibs.ToHashSet();

      if (Directory.Exists(folderPath)) {
        settingInfo.AddBaseFolder(folderPath, false);
      } else if (files.Count() == 1) {
        settingInfo.AddBaseFolder(new FileInfo(files.Single()).DirectoryName, true);
      } else {
        // throw new NotImplementedException("Unable to determine basefolder(s) when the input is a list of source files.");
      }

      return new LuaSyntaxGenerator(codes, libs, settings.cscArguments_, settings.metas_, settingInfo);
    }

    internal static LuaSyntaxGenerator CreateGeneratorForProject(CompilerSettings settings, LuaSyntaxGenerator.SettingInfo settingInfo) {
      var projectPath = settings.input_;
      var mainProject = ProjectHelper.ParseProject(projectPath, IsCompileDebug(settings.cscArguments_) ? configurationDebug : configurationRelease);
      var projects = mainProject?.EnumerateProjects().ToArray();
      var packages = PackageHelper.EnumeratePackages(mainProject.TargetFrameworkVersions.First(), projects.Select(project => project.project));
      var files = GetProjectsSourceFiles(projects);
      var packageBaseFolders = new List<string>();
      if (packages != null) {
        var searchDirectories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var package in packages) {
          if (IsDecompile(package, settings)) {
            PackageHelper.EnumerateLibs(package, out var baseFolder);
            if (string.IsNullOrWhiteSpace(baseFolder)) {
              continue;
            }

            searchDirectories.Add(baseFolder);
          }
        }

        foreach (var package in packages) {
          var packageFiles = PackageHelper.EnumerateSourceFiles(package, out var baseFolder).ToArray();
          if (packageFiles.Length > 0) {
            files = files.Concat(packageFiles);
            packageBaseFolders.Add(baseFolder);
          }
          if (IsDecompile(package, settings)) {
            var packageLibs = PackageHelper.EnumerateLibs(package, out baseFolder).ToArray();
            if (packageLibs.Length > 0) {
              var folderInfo = new DirectoryInfo(baseFolder);
              while (!string.Equals(folderInfo.Name, "packages", StringComparison.OrdinalIgnoreCase)) {
                folderInfo = folderInfo.Parent;
              }

              baseFolder = Path.Combine(
                  Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                  "CSharpLua",
                  "packages",
                  baseFolder[(folderInfo.FullName.Length + 1)..]);

              Directory.CreateDirectory(baseFolder);

              var decompiledLibFiles = packageLibs.Select(lib =>
              {
                var libFileInfo = new FileInfo(lib);
                var packageFileNameWithoutExtension = Path.GetFileNameWithoutExtension(libFileInfo.Name);

                var fileName = Path.Combine(baseFolder, packageFileNameWithoutExtension + ".cs");
                if (!File.Exists(fileName)) {
                  var decompilerSettings = new DecompilerSettings {
                    LoadInMemory = true,
                    ThrowOnAssemblyResolveErrors = true,
                  };

                  var metadataOptions = decompilerSettings.ApplyWindowsRuntimeProjections
                    ? MetadataReaderOptions.ApplyWindowsRuntimeProjections
                    : MetadataReaderOptions.None;

                  var peFile = new PEFile(
                    lib,
                    File.OpenRead(lib),
                    PEStreamOptions.PrefetchEntireImage,
                    metadataOptions);

                  var assemblyResolver = new UniversalAssemblyResolver(
                    lib,
                    decompilerSettings.ThrowOnAssemblyResolveErrors,
                    peFile.DetectTargetFrameworkId(),
                    decompilerSettings.LoadInMemory ? PEStreamOptions.PrefetchMetadata : PEStreamOptions.Default,
                    metadataOptions);

                  foreach (var searchDirectory in searchDirectories) {
                    assemblyResolver.AddSearchDirectory(searchDirectory);
                  }

                  var decompilerTypeSystem = new DecompilerTypeSystem(peFile, assemblyResolver, decompilerSettings);
                  var decompiler = new CSharpDecompiler(decompilerTypeSystem, decompilerSettings);
                  var syntaxTree = decompiler.DecompileWholeModuleAsSingleFile();
                  foreach (var child in syntaxTree.Children) {
                    if (child is AttributeSection attributeSection && attributeSection.AttributeTarget == "assembly") {
                      child.Remove();
                    }
                  }

                  using var fileWriter = File.CreateText(fileName);
                  syntaxTree.AcceptVisitor(new CSharpOutputVisitor(fileWriter, decompilerSettings.CSharpFormattingOptions));
                }

                return fileName;
              });

              files = files.Concat(decompiledLibFiles.ToArray());
              packageBaseFolders.Add(baseFolder);
            }
          }
        }
      }
      var codes = files.Select(i => (File.ReadAllText(i), i));
      var metas = settings.metas_.Concat(packages.Where(package => !IsDecompile(package, settings)).SelectMany(package => PackageHelper.EnumerateMetas(package)));
      var libs = GetLibs(settings.libs_.Concat(packages.Where(package => !IsDecompile(package, settings)).SelectMany(package => PackageHelper.EnumerateLibs(package))), out var luaModuleLibs);
      settingInfo.LuaModuleLibs = luaModuleLibs.ToHashSet();

      foreach (var folder in projects.Select(p => p.folder)) {
        settingInfo.AddBaseFolder(folder, false);
      }
      foreach (var folder in packageBaseFolders) {
        settingInfo.AddBaseFolder(folder, false);
      }

      var fileBannerLines = new List<string>();
      if (packages != null && packages.Any()) {
        fileBannerLines.Add("Compiled with the following packages:");
        fileBannerLines.AddRange(packages
          .Select(package => $"  {package.PackageName}: v{package.VersionNormalizedString}{(IsDecompile(package, settings) ? " (decompiled)" : string.Empty)}")
          .OrderBy(s => s, StringComparer.Ordinal));
      }

      return new LuaSyntaxGenerator(codes, libs, settings.cscArguments_, metas, settingInfo, fileBannerLines);
    }

    private static IEnumerable<string> GetFolderSourceFiles(string folder) {
      if (Directory.Exists(folder)) {
        return Directory.EnumerateFiles(folder, "*.cs", SearchOption.AllDirectories);
      }
      return Utility.Split(folder);
    }

    private static IEnumerable<string> GetProjectsSourceFiles(IEnumerable<(string folder, CustomProjectParserResult project)> projects) {
      return projects.SelectMany(project => project.project.EnumerateSourceFiles(project.folder));
    }

    private static IEnumerable<string> GetProjectSourceFiles(string folder, CustomProjectParserResult project) {
      return project.EnumerateSourceFiles(folder);
    }

    private static List<string> GetLibs(IEnumerable<string> additionalLibs, out List<string> luaModuleLibs) {
      luaModuleLibs = new List<string>();
      var libs = Compiler.GetSystemLibs();
      var dlls = new HashSet<string>(libs.Select(lib => new FileInfo(lib).Name));
      if (additionalLibs != null) {
        foreach (string additionalLib in additionalLibs) {
          string lib = additionalLib;
          bool isLuaModule = false;
          if (lib.Last() == kLuaModuleSuffix) {
            lib = lib.TrimEnd(kLuaModuleSuffix);
            isLuaModule = true;
          } else {
            var dllName = new FileInfo(lib).Name;
            if (dlls.Contains(dllName)) {
              // Avoid duplicate dlls.
              continue;
            } else {
              dlls.Add(dllName);
            }
          }

          string path = lib.EndsWith(kDllSuffix) ? lib : lib + kDllSuffix;
          if (File.Exists(path)) {
            if (isLuaModule) {
              luaModuleLibs.Add(Path.GetFileNameWithoutExtension(path));
            }

            libs.Add(path);
          } else {
            throw new CmdArgumentException($"-l {path} is not found");
          }
        }
      }
      return libs;
    }

    private static bool IsCompileDebug(string[] cscArguments) {
      foreach (var arg in cscArguments) {
        if (arg.StartsWith("-debug")) {
          return true;
        }
      }
      return false;
    }

    private static bool IsDecompile(PackageReferenceModel package, CompilerSettings settings) {
      if (!package.IsDecompile.HasValue) {
        package.IsDecompile = CalculateIsDecompile(package, settings);
      }

      return package.IsDecompile.Value;
    }

    private static bool CalculateIsDecompile(PackageReferenceModel package, CompilerSettings settings) {
      if (settings.packageIncl_ is null && settings.packageExcl_ is null) {
        return false;
      }

      if (settings.packageIncl_ is null) {
        return !settings.packageExcl_.Any(regex => regex.IsMatch(package.PackageName));
      }

      if (settings.packageExcl_ is null) {
        return settings.packageIncl_.Any(regex => regex.IsMatch(package.PackageName));
      }

      var bestlen = -1;
      foreach (var regex in settings.packageIncl_) {
        var match = regex.Match(package.PackageName);
        if (match.Success) {
          var len = Regex.Unescape(regex.ToString()).Length;
          if (bestlen <= len) {
            bestlen = len;
          }
        }
      }

      if (bestlen == -1) {
        return false;
      }

      foreach (var regex in settings.packageExcl_) {
        var match = regex.Match(package.PackageName);
        if (match.Success) {
          var len = Regex.Unescape(regex.ToString()).Length;
          if (bestlen <= len) {
            return false;
          }
        }
      }

      return true;
    }
  }
}
