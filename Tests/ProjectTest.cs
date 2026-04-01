using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using War3Net.Build.Info;

namespace Tests
{
    [TestClass]
    public class ProjectTest : TestBase
    {
        static ScriptLanguage language = ScriptLanguage.Jass;
        static string parentFolder = "TestProjects";
        string name;
        string projectPath;
        static string directory = Directory.GetCurrentDirectory();

        private Project _project;


        [ClassInitialize]
        public static void BeforeAll(TestContext context)
        {
            Console.WriteLine("-----------");
            Console.WriteLine("RUNNING PROJECT TESTS");
            Console.WriteLine("-----------");
            Console.WriteLine("");

            if (Directory.Exists(directory + @"/" + parentFolder))
                Directory.Delete(directory + @"/" + parentFolder, true);
        }

        [TestInitialize]
        public void BeforeEach()
        {
            name = "Project-" + Guid.NewGuid().ToString();
            var projectFolder = Path.Combine(directory, parentFolder);
            if (Directory.Exists(projectFolder + @"/" + name))
                Directory.Delete(projectFolder + @"/" + name, true);
            if (File.Exists(projectFolder + @"/" + name + ".json"))
                File.Delete(projectFolder + @"/" + name + ".json");

            projectPath = Project.Create(language, name, projectFolder);
            _project = Project.Load(projectPath);
            _project.EnableFileEvents(false); // TODO: Not ideal for testing, but necessary with current architecture.

        }

        [TestCleanup]
        public void AfterEach()
        {
            _project.Close();
        }

        [TestMethod]
        public void CallingCreateProject_WhenCreate_CheckIfExists()
        {
            var language = ScriptLanguage.Jass;
            var name = "TestProject2";
            var directory = System.IO.Directory.GetCurrentDirectory();

            projectPath = Project.Create(language, name, directory);
            _project = Project.Load(projectPath);
            _project.EnableFileEvents(false); // TODO: Not ideal for testing, but necessary with current architecture.

            Assert.AreEqual("jass", _project.war3project.Language);
            Assert.AreEqual(name, _project.war3project.Name);

            Assert.IsTrue(File.Exists(projectPath), "Project file does not exist.");
        }

        [TestMethod]
        public void CallingLoadProject_CheckIfExists()
        {
            var loadedProject = Project.Load(projectPath);

            Assert.AreEqual(_project.war3project.Name, loadedProject.war3project.Name);
            Assert.AreEqual(_project.war3project.Language, loadedProject.war3project.Language);
        }

        [TestMethod]
        public void OnRenameElement()
        {
            string fullPath = _project.Triggers.Create();
            _project.OnCreateElement(fullPath);
            var element = _project.Triggers.GetLastCreated();

            string newName = "MyTrigger";
            string newFullPath = Path.Combine(Path.GetDirectoryName(element.GetPath()), newName + ".j");

            element.RenameText = "newName";
            element.Rename();
            _project.OnRenameElement(element.GetPath(), newFullPath);

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
