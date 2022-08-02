using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSharpLua.Tests {
  [TestClass]
  public class UnitTest1 {
    [TestMethod]
    public void TestCSharpLuaTemplateThroughPackage() {
      var outputDirectory = @"..\..\..\Output\CSProj\CSharpLuaTemplate";

      var csproj = Directory.EnumerateFiles(@"..\..\..\Input\CSProj\CSharpLuaTemplate", "*.csproj", SearchOption.TopDirectoryOnly).FirstOrDefault();
      var input = csproj ?? throw new FileNotFoundException();

      var compiler = new Compiler(input, outputDirectory, string.Empty, null, null, false, null, string.Empty) {
        IsExportMetadata = false,
        IsModule = false,
        IsInlineSimpleProperty = false,
        IsPreventDebugObject = true,
        IsCommentsDisabled = false,
      };

      try {
        compiler.CompileSingleFile("war3map", Array.Empty<string>());
      } catch (CompilationErrorException e) {
        // Console.WriteLine(e.Message);
        Assert.Fail();
      }
    }

    [TestMethod]
    [DynamicData(nameof(GetInputBugExceptionSourceFiles), DynamicDataSourceType.Method)]
    public void TestBugException(string inputFile) {
      TestCompile(inputFile, @"..\..\..\Output\BugException");
    }

    [TestMethod]
    [DynamicData(nameof(GetInputRuntimeBugSourceFiles), DynamicDataSourceType.Method)]
    public void TestRuntimeBug(string inputFile) {
      TestCompile(inputFile, @"..\..\..\Output\RuntimeBug");
    }

    [TestMethod]
    [DynamicData(nameof(GetInputTemplateAttributeSourceFiles), DynamicDataSourceType.Method)]
    public void TestTemplateAttribute(string inputFile) {
      TestCompile(inputFile, @"..\..\..\Output\TemplateAttribute");
    }

    [TestMethod]
    [DynamicData(nameof(GetInputVerbatimStringSourceFiles), DynamicDataSourceType.Method)]
    public void TestVerbatimString(string inputFile) {
      TestCompile(inputFile, @"..\..\..\Output\VerbatimString");
    }

    [TestMethod]
    public void TestCommonApi() {
      TestCompile(@"..\..\..\Input\WarcraftApi\Common.cs", @"..\..\..\Output\WarcraftApi", PackageHelper.GetLibs("War3Net.CodeAnalysis.Common", "*").Single());
    }

    private void TestCompile(string input, string output, string libs = null) {
      const bool Debug = false;
      var csc = Debug ? "-define:DEBUG" : null;
      var compiler = new Compiler(input, output, libs, null, csc, false, null, null)
      {
        IsExportMetadata = false,
        IsModule = false,
        IsInlineSimpleProperty = false,
      };

      // compiler.CompileSingleFile(inputFile, Array.Empty<string>());
      compiler.Compile();
    }

    private static IEnumerable<object[]> GetInputBugExceptionSourceFiles() {
      return Directory.EnumerateFiles(@"..\..\..\Input\BugException", "*.cs", SearchOption.TopDirectoryOnly).Select(file => new object[] { file });
    }

    private static IEnumerable<object[]> GetInputRuntimeBugSourceFiles() {
      return Directory.EnumerateFiles(@"..\..\..\Input\RuntimeBug", "*.cs", SearchOption.TopDirectoryOnly).Select(file => new object[] { file });
    }

    private static IEnumerable<object[]> GetInputTemplateAttributeSourceFiles() {
      return Directory.EnumerateFiles(@"..\..\..\Input\TemplateAttribute", "*.cs", SearchOption.TopDirectoryOnly).Select(file => new object[] { file });
    }

    private static IEnumerable<object[]> GetInputVerbatimStringSourceFiles() {
      return Directory.EnumerateFiles(@"..\..\..\Input\VerbatimString", "*.cs", SearchOption.TopDirectoryOnly).Select(file => new object[] { file });
    }
  }
}
