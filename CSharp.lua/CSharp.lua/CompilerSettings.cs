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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CSharpLua {
  internal sealed class CompilerSettings {
    internal readonly bool isProject_;
    internal readonly string input_;
    internal readonly string output_;
    internal readonly string[] libs_;
    internal readonly string[] metas_;
    internal readonly Regex[]? packageIncl_;
    internal readonly Regex[]? packageExcl_;
    internal readonly string[] cscArguments_;
    internal readonly bool isClassic_;
    internal readonly string[]? attributes_;
    internal readonly string[]? enums_;

    internal CompilerSettings(string input, string output, string lib, string meta, string? packageIncl, string? packageExcl, string csc, bool isClassic, string? atts, string? enums) {
      isProject_ = new FileInfo(input).Extension.ToLower() == ".csproj";
      input_ = input;
      output_ = output;
      libs_ = Utility.Split(lib);
      metas_ = Utility.Split(meta);
      if (packageIncl != null) {
        packageIncl_ = Utility.Split(packageIncl, false).Select(package => new Regex($"^{Regex.Escape(package).Replace("\\?", ".").Replace("\\*", ".*")}$", RegexOptions.IgnoreCase)).ToArray();
      }
      if (packageExcl != null) {
        packageExcl_ = Utility.Split(packageExcl, false).Select(package => new Regex($"^{Regex.Escape(package).Replace("\\?", ".").Replace("\\*", ".*")}$", RegexOptions.IgnoreCase)).ToArray();
      }
      cscArguments_ = string.IsNullOrEmpty(csc) ? Array.Empty<string>() : csc.Trim().Split(' ', '\t');
      isClassic_ = isClassic;
      if (atts != null) {
        attributes_ = Utility.Split(atts, false);
      }
      if (enums != null) {
        enums_ = Utility.Split(enums, false);
      }
    }
  }
}
