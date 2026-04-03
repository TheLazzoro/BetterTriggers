using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using War3Net.Build.Info;

namespace Tests
{
    [TestClass]
    public class ScriptTests : TestBase
    {
        static ScriptLanguage language = ScriptLanguage.Jass;
        static string parentFolder = "TestProjectScripts";
        static string directory = System.IO.Directory.GetCurrentDirectory();
        
        string name;
        string projectPath;
        ExplorerElement element1, element2, element3;
        Project project;


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
            name = "Project-" + Guid.NewGuid().ToString();
            var projectFolder = Path.Combine(directory, parentFolder);
            if (Directory.Exists(Path.Combine(parentFolder, name)))
                Directory.Delete(Path.Combine(parentFolder, name), true);
            if (File.Exists(Path.Combine(parentFolder, name + ".json")))
                File.Delete(Path.Combine(parentFolder, name + ".json"));

            projectPath = Project.Create(language, name, parentFolder);
            project = Project.Load(projectPath);
            project.EnableFileEvents(false); // TODO: Not ideal for testing, but necessary with current architecture.

            string fullPath = project.Scripts.Create();
            project.OnCreateElement(fullPath);
            element1 = project.lastCreated;

            fullPath = project.Scripts.Create();
            project.OnCreateElement(fullPath);
            element2 = project.lastCreated;

            fullPath = project.Scripts.Create();
            project.OnCreateElement(fullPath);
            element3 = project.lastCreated;
        }

        [TestCleanup]
        public void AfterEach()
        {
            project.Close();
        }


        [TestMethod]
        public void OnCreateScript()
        {
            string fullPath = project.Scripts.Create();
            project.OnCreateElement(fullPath);
            var element = project.lastCreated;

            string expectedName = Path.GetFileNameWithoutExtension(fullPath);
            string actualName = element.GetName();

            Assert.AreEqual(expectedName, actualName);
        }

        [TestMethod]
        public void OnPasteScript()
        {
            project.CopyExplorerElement(element1);
            var element = project.PasteExplorerElement(element3);

            int suffix = 0;
            string expectedName = element1.GetName() + suffix;
            string actualName = element.GetName();

            string dir = Path.GetDirectoryName(element1.GetPath());
            string name = Path.GetFileNameWithoutExtension(element1.GetPath()) + suffix;
            string extension = Path.GetExtension(element1.GetPath());
            string expectedPath = Path.Combine(dir, name + extension);
            string actualPath = element.GetPath();

            Assert.AreEqual(element, project.lastCreated);
            Assert.AreEqual(expectedName, actualName);
            Assert.AreEqual(expectedPath, actualPath);
        }
    }
}
