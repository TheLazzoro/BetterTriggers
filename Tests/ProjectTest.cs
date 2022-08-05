using BetterTriggers.Containers;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
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
        static War3Project project;
        static string directory = System.IO.Directory.GetCurrentDirectory();

        static IExplorerElement element1, element2, element3;


        //[AssemblyInitialize]
        //public static void Init()
        //{

        //}

        [TestInitialize]
        public void BeforeEach()
        {
            if (Directory.Exists(directory + @"/" + name))
                Directory.Delete(directory + @"/" + name, true);
            if (File.Exists(directory + @"/" + name + ".json"))
                File.Delete(directory + @"/" + name + ".json");

            ControllerProject controllerProject = new ControllerProject();
            projectPath = controllerProject.CreateProject(language, name, directory);
            project = controllerProject.LoadProject(projectPath);
            controllerProject.SetEnableFileEvents(false); // TODO: Not ideal for testing, but necessary with current architecture.

        }

        [TestCleanup]
        public void AfterEach()
        {
        }

        [TestMethod]
        public void CallingCreateProject_WhenCreate_CheckIfExists()
        {
            var language = ScriptLanguage.Jass;
            var name = "TestProject2";
            var directory = System.IO.Directory.GetCurrentDirectory();

            ControllerProject controller = new ControllerProject();
            projectPath = controller.CreateProject(language, name, directory);
            project = controller.LoadProject(projectPath);

            Assert.AreEqual("jass", project.Language);
            Assert.AreEqual(name, project.Name);

            Assert.IsTrue(File.Exists(projectPath), "Project file does not exist.");
        }

        [TestMethod]
        public void CallingLoadProject_CheckIfExists()
        {
            ControllerProject controller = new ControllerProject();
            var loadedProject = controller.LoadProject(projectPath);

            Assert.AreEqual(project.Name, loadedProject.Name);
            Assert.AreEqual(project.Language, loadedProject.Language);
        }

        [TestMethod]
        public void OnRenameElement()
        {
            ControllerProject controllerProject = new ControllerProject();
            ControllerTrigger controllerTrigger = new ControllerTrigger();
            string fullPath = controllerTrigger.CreateTrigger();
            controllerProject.OnCreateElement(fullPath);
            var element = Triggers.GetLastCreated();

            string newName = "MyTrigger";
            string newFullPath = Path.Combine(Path.GetDirectoryName(element.GetPath()), newName + ".j");

            controllerProject.RenameElement(element, "newName");
            controllerProject.OnRenameElement(element.GetPath(), newFullPath);

            string expectedPath = newFullPath;
            string actualPath = element.GetPath();

            Assert.AreEqual(expectedPath, actualPath);
        }
    }
}
