using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using War3Net.Build.Info;

namespace Tests
{
    [TestClass]
    public class FolderTest : TestBase
    {
        static ScriptLanguage language = ScriptLanguage.Jass;
        static string parentFolder = "FolderTestProjects";
        string name = "TestProject";
        string projectPath;
        static string directory = Directory.GetCurrentDirectory();

        static ExplorerElement element1;

        private Project _project;


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
            name = "Project-" + Guid.NewGuid().ToString();
            var projectFolder = Path.Combine(directory, parentFolder);
            if (Directory.Exists(Path.Combine(projectFolder, name)))
                Directory.Delete(Path.Combine(projectFolder, name), true);
            if (File.Exists(Path.Combine(projectFolder, name + ".json")))
                File.Delete(Path.Combine(projectFolder, name + ".json"));

            projectPath = Project.Create(language, name, projectFolder);
            _project = Project.Load(projectPath);
            _project.EnableFileEvents(false); // TODO: Not ideal for testing, but necessary with current architecture.

            string fullPath = _project.Folders.Create();
            _project.OnCreateElement(fullPath);
            element1 = _project.lastCreated;
            _project.currentSelectedElement = element1.GetPath();

            fullPath = _project.Variables.Create();
            _project.OnCreateElement(fullPath);

            fullPath = _project.Triggers.Create();
            _project.OnCreateElement(fullPath);
        }

        [TestCleanup]
        public void AfterEach()
        {
            _project.Close();
        }


        [TestMethod]
        public void OnCreateFolder()
        {
            string fullPath = _project.Folders.Create();
            _project.OnCreateElement(fullPath);
            var element = _project.lastCreated;

            string expectedName = Path.GetFileNameWithoutExtension(fullPath);
            string actualName = element.GetName();

            Assert.AreEqual(expectedName, actualName);
        }

        [TestMethod]
        public void OnPasteFolder()
        {
            var root = _project.projectFiles[0];
            _project.CopyExplorerElement(element1);
            var pastedElement = _project.PasteExplorerElement(root);

            int expectedElements = element1.ExplorerElements.Count;
            int actualElements = pastedElement.ExplorerElements.Count;

            Assert.AreEqual(pastedElement, _project.lastCreated);
            Assert.AreEqual(expectedElements, actualElements);
            Assert.AreNotEqual(pastedElement.GetName(), element1.GetName());

            foreach (var el in pastedElement.ExplorerElements)
            {
                foreach (var toCompare in element1.ExplorerElements)
                {
                    string expected = el.GetName();
                    string actual = toCompare.GetName();
                    Assert.AreNotEqual(expected, actual);
                }
            }
        }
    }
}
