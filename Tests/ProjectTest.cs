using BetterTriggers;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.WorldEdit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Windows;
using War3Net.Build.Info;

namespace Tests
{
    [TestClass]
    public class ProjectTest : TestBase
    {
        static ScriptLanguage language = ScriptLanguage.Jass;
        static string name = "TestProject";
        static string projectPath;
        static Project project;
        static string directory = System.IO.Directory.GetCurrentDirectory();

        static ExplorerElement element1, element2, element3;


        [ClassInitialize]
        public static void BeforeAll(TestContext context)
        {
            Console.WriteLine("-----------");
            Console.WriteLine("RUNNING PROJECT TESTS");
            Console.WriteLine("-----------");
            Console.WriteLine("");
        }

        [TestInitialize]
        public void BeforeEach()
        {
            if (Directory.Exists(directory + @"/" + name))
                Directory.Delete(directory + @"/" + name, true);
            if (File.Exists(directory + @"/" + name + ".json"))
                File.Delete(directory + @"/" + name + ".json");

            projectPath = Project.Create(language, name, directory);
            project = Project.Load(projectPath);
            project.EnableFileEvents(false); // TODO: Not ideal for testing, but necessary with current architecture.

        }

        [TestCleanup]
        public void AfterEach()
        {
            Project.Close();
        }

        [TestMethod]
        public void CallingCreateProject_WhenCreate_CheckIfExists()
        {
            var language = ScriptLanguage.Jass;
            var name = "TestProject2";
            var directory = System.IO.Directory.GetCurrentDirectory();

            projectPath = Project.Create(language, name, directory);
            project = Project.Load(projectPath);
            project.EnableFileEvents(false); // TODO: Not ideal for testing, but necessary with current architecture.

            Assert.AreEqual("jass", project.war3project.Language);
            Assert.AreEqual(name, project.war3project.Name);

            Assert.IsTrue(File.Exists(projectPath), "Project file does not exist.");
        }

        [TestMethod]
        public void CallingLoadProject_CheckIfExists()
        {
            var loadedProject = Project.Load(projectPath);

            Assert.AreEqual(project.war3project.Name, loadedProject.war3project.Name);
            Assert.AreEqual(project.war3project.Language, loadedProject.war3project.Language);
        }

        [TestMethod]
        public void OnRenameElement()
        {
            string fullPath = project.Triggers.Create();
            project.OnCreateElement(fullPath);
            var element = Project.CurrentProject.Triggers.GetLastCreated();

            string newName = "MyTrigger";
            string newFullPath = Path.Combine(Path.GetDirectoryName(element.GetPath()), newName + ".j");

            element.RenameText = "newName";
            element.Rename();
            project.OnRenameElement(element.GetPath(), newFullPath);

            string expectedPath = newFullPath;
            string actualPath = element.GetPath();

            Assert.AreEqual(expectedPath, actualPath);
        }

        [TestMethod]
        public void VerifyMapPath()
        {
            string dir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/");
            string[] maps = Directory.GetFileSystemEntries(dir, "*", SearchOption.TopDirectoryOnly);

            for (int i = 0; i < maps.Length; i++)
            {
                string fullPath = maps[i];
                bool doesMapExist = Project.VerifyMapPath(fullPath);
                Assert.IsTrue(doesMapExist);
            }
        }
    }
}
