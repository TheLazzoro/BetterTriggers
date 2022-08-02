using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;

namespace CSharpLua {
  public sealed class PackageException : Exception {
    public PackageException() {
    }
    public PackageException(string message) : base(message) {
    }
  }

  internal sealed class VersionStatus {
    private bool triedSelect;
    public VersionRange Allowed { get; private set; }
    public NuGetVersion Selected { get; private set; }
    [DisallowNull]
    public NuGetFramework Framework { get; set; }
    public VersionStatus(VersionRange versionRange) {
      triedSelect = false;
      Allowed = versionRange;
    }
    public bool SelectBestMatch(string id) {
      if (!triedSelect) {
        triedSelect = true;
        Selected = Allowed.FindBestMatch(PackageHelper.GetAvailableVersions(id));
        return Selected != null;
      } else {
        return false;
      }
    }
    public void UpdateAllowedRange(VersionRange range) {
      Allowed = VersionRange.CommonSubSet(new[] { Allowed, range });
    }
    public override string ToString() {
      return Selected?.ToString() ?? Allowed.ToString();
    }
  }

  public sealed class PackageReferenceModel {
    public string PackageName { get; set; }
    public string PackagePath { get; set; }
    public string FrameworkShortFolderName { get; set; }
    public string VersionNormalizedString { get; set; }
    internal bool? IsDecompile { get; set; }
  }

  public static class PackageHelper {
    private static readonly string _globalPackagesPath = Path.Combine(NuGetEnvironment.GetFolderPath(NuGetFolderPath.NuGetHome), SettingsUtility.DefaultGlobalPackagesFolderPath);

    internal static IEnumerable<NuGetVersion> GetAvailableVersions(string packageName) {
      var packagePath = Path.Combine(_globalPackagesPath, packageName);
      // TODO: run package restore (always or only when directory doesn't exist?)
      if (!Directory.Exists(packagePath)) {
        yield break;
      }
      foreach (var packageVersionPath in Directory.EnumerateDirectories(packagePath)) {
        yield return NuGetVersion.Parse(new DirectoryInfo(packageVersionPath).Name);
      }
    }

    public static IEnumerable<string> EnumerateSourceFiles(PackageReferenceModel package, out string baseFolder) {
      var sourcesPath = Path.Combine(package.PackagePath, "contentFiles", "cs");
      return EnumerateFiles(sourcesPath, package.FrameworkShortFolderName, "*.cs", SearchOption.AllDirectories, out baseFolder);
    }

    public static IEnumerable<string> EnumerateLibs(string packagePath, string frameworkShortFolderName) {
      var libPath = Path.Combine(packagePath, "lib");
      return EnumerateFiles(libPath, frameworkShortFolderName, "*.dll", SearchOption.TopDirectoryOnly, out _);
    }

    public static IEnumerable<string> EnumerateLibs(PackageReferenceModel package) {
      var libPath = Path.Combine(package.PackagePath, "lib");
      return EnumerateFiles(libPath, package.FrameworkShortFolderName, "*.dll", SearchOption.TopDirectoryOnly, out _);
    }

    public static IEnumerable<string> EnumerateLibs(PackageReferenceModel package, out string baseFolder) {
      var libPath = Path.Combine(package.PackagePath, "lib");
      return EnumerateFiles(libPath, package.FrameworkShortFolderName, "*.dll", SearchOption.TopDirectoryOnly, out baseFolder);
    }

    public static IEnumerable<string> EnumerateMetas(PackageReferenceModel package) {
      var libPath = Path.Combine(package.PackagePath, "lib");
      return EnumerateFiles(libPath, package.FrameworkShortFolderName, "*.xml", SearchOption.TopDirectoryOnly, out _);
    }

    private static IEnumerable<string> EnumerateFiles(string path, string frameworkFolderName, string searchPattern, SearchOption searchOption, out string frameworkPath) {
      if (frameworkFolderName is null) {
        var targetFrameworkCount = Directory.EnumerateDirectories(path).Count();
        if (targetFrameworkCount > 1) {
          throw new ArgumentNullException(nameof(frameworkFolderName));
        } else if (targetFrameworkCount == 0) {
          frameworkPath = null;
          return Array.Empty<string>();
        } else {
          frameworkFolderName = Directory.EnumerateDirectories(path).Single();
        }
      }
      if (!Directory.Exists(path)) {
        frameworkPath = null;
        return Array.Empty<string>();
      }
      frameworkPath = Path.Combine(path, frameworkFolderName ?? Directory.EnumerateDirectories(path).Single());
      if (!Directory.Exists(frameworkPath)) {
        var targetFramework = NuGetFramework.Parse(frameworkFolderName);
        var compatibleFrameworks = Directory.EnumerateDirectories(path)
          .Where(folder => NuGetFrameworkUtility.IsCompatibleWithFallbackCheck(targetFramework, NuGetFramework.ParseFolder(new DirectoryInfo(folder).Name)));
        // TODO: how to select best match framework?
        frameworkPath = compatibleFrameworks.FirstOrDefault();
        if (frameworkPath is null) {
          // Ignore folders that contain no .dll (or .cs) files (empty lib folders usually have a file called "_._" in it).
          compatibleFrameworks = Directory.EnumerateDirectories(path)
          .Where(folder => NuGetFrameworkUtility.IsCompatibleWithFallbackCheck(NuGetFramework.ParseFolder(new DirectoryInfo(folder).Name), targetFramework))
          .Where(folder => Directory.EnumerateFiles(folder, searchPattern, SearchOption.AllDirectories).FirstOrDefault() != null);
          // TODO: how to select best match framework?
          frameworkPath = compatibleFrameworks.FirstOrDefault();
          if (frameworkPath is null) {
            return Array.Empty<string>();
          }
        }
      }
      return Directory.EnumerateFiles(frameworkPath, searchPattern, searchOption);
    }

    public static IEnumerable<PackageReferenceModel> EnumeratePackages(string targetFrameworkVersion, IEnumerable<Cake.Incubator.Project.CustomProjectParserResult> projects) {
      var targetFramework = NuGetFramework.Parse(targetFrameworkVersion);
      var packages = new Dictionary<string, VersionStatus>();
      void AddPackageReference(string id, VersionRange versionRange) {
        if (packages.TryGetValue(id, out var versionStatus)) {
          if (versionStatus.Selected is null) {
            versionStatus.UpdateAllowedRange(versionRange);
          } else if (!versionRange.Satisfies(versionStatus.Selected)) {
            throw new PackageException($"Incompatible package dependency for package {id} {versionStatus.Selected}: {versionRange}");
          }
        } else {
          packages.Add(id, new VersionStatus(versionRange));
        }
      }
      foreach (var package in projects.SelectMany(project => project.PackageReferences)) {
        AddPackageReference(package.Name, VersionRange.Parse(package.Version));
      }
      var newDependencies = new HashSet<PackageIdentity>();
      while (true) {
        foreach (var package in packages) {
          if (package.Value.SelectBestMatch(package.Key)) {
            newDependencies.Add(new PackageIdentity(package.Key, package.Value.Selected));
          }
        }
        if (newDependencies.Count == 0) {
          break;
        }
        foreach (var newDependency in newDependencies) {
          var dependencyGroup = GetDependencyGroup(newDependency, targetFramework);
          if (dependencyGroup != null) {
            packages[newDependency.Id].Framework = dependencyGroup.TargetFramework;
            foreach (var package in dependencyGroup.Packages) {
              AddPackageReference(package.Id, package.VersionRange);
            }
          } else {
            packages[newDependency.Id].Framework = targetFramework;
          }
        }
        newDependencies.Clear();
      }
      foreach (var package in packages) {
        if (package.Value.Selected != null) {
          yield return new PackageReferenceModel {
            PackageName = package.Key,
            PackagePath = Path.Combine(_globalPackagesPath, package.Key, package.Value.Selected.ToNormalizedString()),
            FrameworkShortFolderName = package.Value.Framework.GetShortFolderName(),
            VersionNormalizedString = package.Value.Selected.ToNormalizedString(),
          };
        }
      }
    }

    public static IEnumerable<string> GetLibs(string packageName, string versionRange) {
      var range = VersionRange.Parse(versionRange);
      var status = new VersionStatus(range);
      if (status.SelectBestMatch(packageName)) {
        var packagePath = Path.Combine(_globalPackagesPath, packageName, status.Selected.ToNormalizedString());
        var frameworkFolderName = status.Framework?.GetShortFolderName();
        foreach (var lib in EnumerateLibs(packagePath, frameworkFolderName)) {
          yield return lib;
        }
      }
    }

    private static PackageDependencyGroup GetDependencyGroup(PackageIdentity package, NuGetFramework targetFramework) {
      var nuspecFile = Path.Combine(_globalPackagesPath, package.Id, package.Version.ToNormalizedString(), $"{package.Id}.nuspec");
      if (!NuGet.Packaging.PackageHelper.IsNuspec(nuspecFile)) {
        throw new PackageException($"Could not locate the .nuspec file for package: {package}");
      }
      var reader = new NuspecReader(nuspecFile);
      var dependencyGroups = reader.GetDependencyGroups();
      if (dependencyGroups.FirstOrDefault() != null) {
        var compatibleGroups = dependencyGroups.Where(dependencyGroup => NuGetFrameworkUtility.IsCompatibleWithFallbackCheck(targetFramework, dependencyGroup.TargetFramework));
        // TODO: how to select best match framework?
        return compatibleGroups.First();
      }
      return null;
    }
  }
}
