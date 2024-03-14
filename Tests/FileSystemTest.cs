using BetterTriggers;
using BetterTriggers.Containers;
using BetterTriggers.Utility;
using BetterTriggers.WorldEdit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using War3Net.Build.Info;

namespace Tests
{
    [TestClass]
    public class FileSystemTest : TestBase
    {
        static string sourceFolder = System.IO.Directory.GetCurrentDirectory() + @"\" + "source";
        static string targetFolder = System.IO.Directory.GetCurrentDirectory() + @"\" + "target";
        static string projectFolder = Path.Combine(Directory.GetCurrentDirectory(), "testProject");

        [ClassInitialize]
        public static void BeforeAll(TestContext context)
        {
            Console.WriteLine("-----------");
            Console.WriteLine("RUNNING FS TESTS");
            Console.WriteLine("-----------");
            Console.WriteLine("");
        }

        [TestInitialize]
        public void BeforeEach()
        {
            if (Directory.Exists(sourceFolder))
                Directory.Delete(sourceFolder, true);
            if (Directory.Exists(targetFolder))
                Directory.Delete(targetFolder, true);
            Directory.CreateDirectory(sourceFolder);
            Directory.CreateDirectory(targetFolder);

            string projectFile = Project.Create(ScriptLanguage.Jass, "test", projectFolder);
            Project.Load(projectFile);
        }

        [TestCleanup]
        public void AfterEach()
        {
            Project.Close();
            Directory.Delete(projectFolder, true);
        }

        [TestMethod]
        public void FileMove()
        {
            Directory.CreateDirectory(sourceFolder);
            Directory.CreateDirectory(targetFolder);
            var file = sourceFolder + @"\" + "testFile";
            
            File.WriteAllText(file, "This is a test.");
            FileSystemUtil.Move(file, targetFolder, 0);

            var newPath = targetFolder + @"\" + "testFile";
            var expected = true;
            var actual = File.Exists(newPath);

            Assert.AreEqual(expected, actual);
        }
    }
}
