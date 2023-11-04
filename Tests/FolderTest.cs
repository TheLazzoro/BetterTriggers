using BetterTriggers.Containers;
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

            Project project = Project.CurrentProject;
            projectPath = Project.Create(language, name, directory);
            project = Project.Load(projectPath);
            project.EnableFileEvents(false); // TODO: Not ideal for testing, but necessary with current architecture.

            string fullPath = project.Folders.Create();
            project.OnCreateElement(fullPath);
            element1 = project.lastCreated as ExplorerElementFolder;
            project.currentSelectedElement = element1.GetPath();

            fullPath = project.Variables.Create();
            project.OnCreateElement(fullPath);

            fullPath = project.Triggers.Create();
            project.OnCreateElement(fullPath);
        }

        [TestCleanup]
        public void AfterEach()
        {
            Project.Close();
        }


        [TestMethod]
        public void OnCreateFolder()
        {
            string fullPath = Project.CurrentProject.Folders.Create();
            Project.CurrentProject.OnCreateElement(fullPath);
            var element = Project.CurrentProject.lastCreated as ExplorerElementFolder;

            string expectedName = Path.GetFileNameWithoutExtension(fullPath);
            string actualName = element.GetName();

            Assert.AreEqual(expectedName, actualName);
        }

        [TestMethod]
        public void OnPasteFolder()
        {
            var root = Project.CurrentProject.projectFiles[0];
            Project.CurrentProject.CopyExplorerElement(element1);
            var element = Project.CurrentProject.PasteExplorerElement(root) as ExplorerElementFolder;

            int expectedElements = element1.explorerElements.Count;
            int actualElements = element.explorerElements.Count;

            Assert.AreEqual(element, Project.CurrentProject.lastCreated);
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
