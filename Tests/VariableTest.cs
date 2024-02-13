using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading;
using War3Net.Build.Info;

namespace Tests
{
    [TestClass]
    public class VariableTest
    {
        static ScriptLanguage language = ScriptLanguage.Jass;
        static string name = "TestProject";
        static string projectPath;
        static Project project;
        static string directory = System.IO.Directory.GetCurrentDirectory();

        static ExplorerElement element1, element2, element3;


        [ClassInitialize]
        public static void Init(TestContext context)
        {
            Console.WriteLine("-----------");
            Console.WriteLine("RUNNING VARIABLE TESTS");
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

            projectPath = Project.Create(language, name, directory);
            project = Project.Load(projectPath);
            project.EnableFileEvents(false); // TODO: Not ideal for testing, but necessary with current architecture.

            string fullPath = project.Variables.Create();
            project.OnCreateElement(fullPath);
            element1 = project.lastCreated;

            fullPath = project.Variables.Create();
            project.OnCreateElement(fullPath);
            element2 = project.lastCreated;

            fullPath = project.Variables.Create();
            project.OnCreateElement(fullPath);
            element3 = project.lastCreated;
        }

        [TestCleanup]
        public void AfterEach()
        {
            Project.Close();
        }


        [TestMethod]
        public void OnCreateVariable()
        {
            string fullPath = project.Variables.Create();
            project.OnCreateElement(fullPath);
            var element = project.lastCreated;

            string expectedName = Path.GetFileNameWithoutExtension(fullPath);
            string actualName = element.GetName();

            Assert.AreEqual(expectedName, actualName);
        }

        [TestMethod]
        public void OnPasteVariable()
        {
            project.CopyExplorerElement(element1);
            var element = project.PasteExplorerElement(element3);

            string expectedName = element1.variable.Name + "2";
            string actualName = element.variable.Name;

            int expectedArray0 = element1.variable.ArraySize[0];
            int expectedArray1 = element1.variable.ArraySize[1];
            int actualArray0 = element.variable.ArraySize[0];
            int actualArray1 = element.variable.ArraySize[0];

            string expectedType = element1.variable.Type;
            string actualType = element.variable.Type;

            Assert.AreEqual(element, project.lastCreated);
            Assert.AreEqual(expectedArray0, actualArray0);
            Assert.AreEqual(expectedArray1, actualArray1);
            Assert.AreEqual(expectedType, actualType);
            Assert.AreEqual(expectedName, actualName);
        }

        [TestMethod]
        public void CloneLocalVariable()
        {
            var trig = new Trigger();
            LocalVariable variable = new LocalVariable();
            project.Variables.CreateLocalVariable(trig, variable, trig.LocalVariables, 0);

            Assert.AreEqual("UntitledVariable", variable.variable.Name);
            Assert.AreEqual(true, variable.variable._isLocal);
        }
    }
}
