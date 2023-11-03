using BetterTriggers.Containers;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using War3Net.Build.Info;

namespace Tests
{
    [TestClass]
    public class ProjectTest
    {
        static ScriptLanguage language = ScriptLanguage.Jass;
        static string name = "TestProject";
        static string projectPath;
        static Project project;
        static string directory = System.IO.Directory.GetCurrentDirectory();

        static IExplorerElement element1, element2, element3;


        [ClassInitialize]
        public static void Init(TestContext context)
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

            ControllerProject controller = new ControllerProject();
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
            ControllerProject controller = new ControllerProject();
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

            project.RenameElement(element, "newName");
            project.OnRenameElement(element.GetPath(), newFullPath);

            string expectedPath = newFullPath;
            string actualPath = element.GetPath();

            Assert.AreEqual(expectedPath, actualPath);
        }
    }
}
