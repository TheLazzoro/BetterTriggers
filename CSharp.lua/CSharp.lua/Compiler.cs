/*
Copyright 2017 YANG Huan (sy.yanghuan@gmail.com).

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

  http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Reflection;

namespace CSharpLua {
  public sealed class Compiler {
    public bool IsExportMetadata { get; set; }
    public bool IsModule { get; set; }
    public bool IsInlineSimpleProperty { get; set; }
    public bool IsPreventDebugObject { get; set; }
    public bool IsCommentsDisabled { get; set; }
    public bool IsNotConstantForEnum { get; set; }
    public bool IsNoConcurrent { get; set; }
    public string Include { get; set; }
    private CompilerSettings Settings { get; }

    public Compiler(string input, string output, string lib, string meta, string csc, bool isClassic, string? atts, string? enums)
      : this(input, output, lib, meta, null, "System.*;*.Sources;Microsoft.NETCore.Platforms;NETStandard.Library", csc, isClassic, atts, enums) {
    }

    public Compiler(string input, string output, string lib, string meta, string? packageIncl, string? packageExcl, string csc, bool isClassic, string? atts, string? enums) {
      Settings = new CompilerSettings(input, output, lib, meta, packageIncl, packageExcl, csc, isClassic, atts, enums);
    }

    public static string CompileSingleCode(string code) {
      var codes = new (string, string)[] { (code, "") };
      var generator = new LuaSyntaxGenerator(codes, GetSystemLibs(), null, Array.Empty<string>(), new LuaSyntaxGenerator.SettingInfo());
      return generator.GenerateSingle();
    }

    /// <summary>
    /// for Blazor to use
    /// </summary>
    public static string CompileSingleCode(string code, IEnumerable<Stream> libs, IEnumerable<Stream> metas) {
      var codes = new (string, string)[] { (code, "") };
      var generator = new LuaSyntaxGenerator(codes, libs, null, metas, new LuaSyntaxGenerator.SettingInfo {
        IsNoConcurrent = true,
      });
      return generator.GenerateSingle();
    }

    public void Compile() {
      if (Include == null) {
        GetGenerator().Generate(Settings.output_);
      } else {
        var luaSystemLibs = GetIncludeCoreSystemPaths(Include);
        GetGenerator().GenerateSingleFile("out.lua", Settings.output_, luaSystemLibs);
      }
    }

    public void CompileSingleFile(string fileName, IEnumerable<string> luaSystemLibs) {
      GetGenerator().GenerateSingleFile(fileName, Settings.output_, luaSystemLibs);
    }

    public void CompileSingleFile(Stream target, IEnumerable<string> luaSystemLibs) {
      GetGenerator().GenerateSingleFile(target, luaSystemLibs);
    }

    internal static List<string> GetSystemLibs() {
      string privateCorePath = typeof(object).Assembly.Location;
      List<string> libs = new List<string> { privateCorePath };

      string systemDir = Path.GetDirectoryName(privateCorePath);
      foreach (string path in Directory.EnumerateFiles(systemDir, "*.dll")) {
        try {
          Assembly.LoadFile(path);
          libs.Add(path);
        } catch {
        }
      }

      return libs;
    }

    private static IEnumerable<string> GetIncludeCoreSystemPaths(string dir) {
      const string kBeginMark = "load(\"";

      string allFilePath = Path.Combine(dir, "All.lua");
      if (!File.Exists(allFilePath)) {
        throw new ArgumentException($"-include: {dir} is not root directory of the CoreSystem library");
      }

      List<string> luaSystemLibs = new();
      var lines = File.ReadAllLines(allFilePath);
      foreach (string line in lines) {
        int i = line.IndexOf(kBeginMark, StringComparison.Ordinal);
        if (i != -1) {
          int begin = i + kBeginMark.Length;
          int end = line.IndexOf('"', begin);
          Contract.Assert(end != -1);
          string name = line[begin..end].Replace('.', '/');
          string path = Path.Combine(dir, "CoreSystem", $"{name}.lua");
          luaSystemLibs.Add(path);
        }
      }
      return luaSystemLibs;
    }

    private LuaSyntaxGenerator GetGenerator() {
      return LuaSyntaxGeneratorFactory.CreateGenerator(Settings, GetGeneratorSettingInfo());
    }

    private LuaSyntaxGenerator.SettingInfo GetGeneratorSettingInfo() {
      return new LuaSyntaxGenerator.SettingInfo {
        IsClassic = Settings.isClassic_,
        IsExportMetadata = IsExportMetadata,
        Attributes = Settings.attributes_,
        Enums = Settings.enums_,
        IsModule = IsModule,
        IsInlineSimpleProperty = IsInlineSimpleProperty,
        IsPreventDebugObject = IsPreventDebugObject,
        IsCommentsDisabled = IsCommentsDisabled,
        IsNotConstantForEnum = IsNotConstantForEnum,
      };
    }
  }
}
