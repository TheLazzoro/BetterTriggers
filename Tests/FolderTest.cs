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
    public class FolderTest
    {
        static ScriptLanguage language = ScriptLanguage.Jass;
        static string name = "TestProject";
        static string projectPath;
        static War3Project project;
        static string directory = System.IO.Directory.GetCurrentDirectory();

        static ExplorerElementFolder element1;


        [ClassInitialize]
        public static void Init(TestContext context)
        {
            Console.WriteLine("-----------");
            Console.WriteLine("RUNNING FOLDER TESTS");
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

            string fullPath = Folders.Create();
            controllerProject.OnCreateElement(fullPath);
            element1 = Project.lastCreated as ExplorerElementFolder;
            Project.currentSelectedElement = element1.GetPath();

            fullPath = ControllerVariable.Create();
            controllerProject.OnCreateElement(fullPath);

            fullPath = ControllerTrigger.Create();
            controllerProject.OnCreateElement(fullPath);
        }

        [TestCleanup]
        public void AfterEach()
        {
            ControllerProject controller = new();
            Project.Close();
        }


        [TestMethod]
        public void OnCreateFolder()
        {
            ControllerProject controllerProject = new ControllerProject();
            string fullPath = Folders.Create();
            controllerProject.OnCreateElement(fullPath);
            var element = Project.lastCreated as ExplorerElementFolder;

            string expectedName = Path.GetFileNameWithoutExtension(fullPath);
            string actualName = element.GetName();

            Assert.AreEqual(expectedName, actualName);
        }

        [TestMethod]
        public void OnPasteFolder()
        {
            var root = Project.projectFiles[0];
            ControllerProject controllerProject = new ControllerProject();
            controllerProject.CopyExplorerElement(element1);
            var element = controllerProject.PasteExplorerElement(root) as ExplorerElementFolder;

            int expectedElements = element1.explorerElements.Count;
            int actualElements = element.explorerElements.Count;

            Assert.AreEqual(element, Project.lastCreated);
            Assert.AreEqual(expectedElements, actualElements);
            Assert.AreNotEqual(element.GetName(), element1.GetName());

            element.explorerElements.ForEach(el =>
            {
                element1.explorerElements.ForEach(toCompare =>
                {
                    Assert.AreNotEqual(el.GetName(), toCompare.GetName());
                });
            });
        }
    }
}
