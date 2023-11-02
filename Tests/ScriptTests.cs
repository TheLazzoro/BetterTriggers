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
    public class ScriptTests
    {
        static ScriptLanguage language = ScriptLanguage.Jass;
        static string name = "TestProject";
        static string projectPath;
        static War3Project project;
        static string directory = System.IO.Directory.GetCurrentDirectory();

        static ExplorerElementScript element1, element2, element3;


        [ClassInitialize]
        public static void Init(TestContext context)
        {
            Console.WriteLine("-----------");
            Console.WriteLine("RUNNING SCRIPT TESTS");
            Console.WriteLine("-----------");
            Console.WriteLine("");
        }

        [TestInitialize]
        public void BeforeEach()
        {
            if (Directory.Exists(Path.Combine(directory, name)))
                Directory.Delete(Path.Combine(directory, name), true);
            if (File.Exists(Path.Combine(directory, name + ".json")))
                File.Delete(Path.Combine(directory, name + ".json"));

            ControllerProject controllerProject = new ControllerProject();
            projectPath = controllerProject.CreateProject(language, name, directory);
            project = controllerProject.LoadProject(projectPath);
            Project.EnableFileEvents(false); // TODO: Not ideal for testing, but necessary with current architecture.

            string fullPath = ControllerScript.Create();
            controllerProject.OnCreateElement(fullPath);
            element1 = Project.lastCreated as ExplorerElementScript;

            fullPath = ControllerScript.Create();
            controllerProject.OnCreateElement(fullPath);
            element2 = Project.lastCreated as ExplorerElementScript;

            fullPath = ControllerScript.Create();
            controllerProject.OnCreateElement(fullPath);
            element3 = Project.lastCreated as ExplorerElementScript;
        }

        [TestCleanup]
        public void AfterEach()
        {
            Project.Close();
        }


        [TestMethod]
        public void OnCreateScript()
        {
            ControllerProject controllerProject = new ControllerProject();
            string fullPath = ControllerScript.Create();
            controllerProject.OnCreateElement(fullPath);
            var element = Project.lastCreated as ExplorerElementScript;

            string expectedName = Path.GetFileNameWithoutExtension(fullPath);
            string actualName = element.GetName();

            Assert.AreEqual(expectedName, actualName);
        }

        [TestMethod]
        public void OnPasteScript()
        {
            ControllerProject controllerProject = new ControllerProject();
            controllerProject.CopyExplorerElement(element1);
            var element = controllerProject.PasteExplorerElement(element3) as ExplorerElementScript;

            int suffix = 0;
            string expectedName = element1.GetName() + suffix;
            string actualName = element.GetName();

            string dir = Path.GetDirectoryName(element1.GetPath());
            string name = Path.GetFileNameWithoutExtension(element1.GetPath()) + suffix;
            string extension = Path.GetExtension(element1.GetPath());
            string expectedPath = Path.Combine(dir, name + extension);
            string actualPath = element.GetPath();

            Assert.AreEqual(element, Project.lastCreated);
            Assert.AreEqual(expectedName, actualName);
            Assert.AreEqual(expectedPath, actualPath);
        }
    }
}
