using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.WorldEdit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading;
using System.Windows;
using War3Net.Build.Info;

namespace Tests
{
    [TestClass]
    public class TriggerTest : TestBase
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
            Console.WriteLine("RUNNING TRIGGER TESTS");
            Console.WriteLine("-----------");
            Console.WriteLine("");
        }

        [ClassCleanup]
        public static void TestClassCleanup()
        {
        }

        [TestInitialize]
        public void BeforeEach()
        {
            if (Directory.Exists(directory + @"/" + name))
                Directory.Delete(directory + @"/" + name, true);
            if(File.Exists(directory + @"/" + name + ".json"))
                File.Delete(directory + @"/" + name + ".json");

            projectPath = Project.Create(language, name, directory);
            project = Project.Load(projectPath);
            project.EnableFileEvents(false); // TODO: Not ideal for testing, but necessary with current architecture.

            string fullPath = project.Triggers.Create();
            project.OnCreateElement(fullPath); // Force OnCreate 'event'.
            element1 = project.Triggers.GetLastCreated();

            fullPath = project.Triggers.Create();
            project.OnCreateElement(fullPath);
            element2 = project.Triggers.GetLastCreated();

            fullPath = project.Triggers.Create();
            project.OnCreateElement(fullPath);
            element3 = project.Triggers.GetLastCreated();
        }

        [TestCleanup]
        public void AfterEach()
        {
            Project.Close();
        }


        [TestMethod]
        public void OnCreateTrigger()
        {
            string fullPath = project.Triggers.Create();
            project.OnCreateElement(fullPath);

            var element = project.Triggers.GetLastCreated();
            string expectedName = Path.GetFileNameWithoutExtension(fullPath);
            string actualName = element.GetName();

            Assert.AreEqual(expectedName, actualName);
        }

        [TestMethod]
        public void OnPasteTrigger()
        {
            project.CopyExplorerElement(element1);
            var element = project.PasteExplorerElement(element3);

            int expectedTriggerCount = 4;
            int actualTriggerCount = project.Triggers.Count();

            var expectedParameters = Parameter.GetParametersFromTrigger(element1);
            var actualParameters = Parameter.GetParametersFromTrigger(element);
            int expectedParamCount = expectedParameters.Count;
            int actualParamCount = actualParameters.Count;

            Assert.AreEqual(element, project.Triggers.GetLastCreated());
            Assert.AreEqual(expectedTriggerCount, actualTriggerCount);
            Assert.AreEqual(expectedParamCount, actualParamCount);
        }

        [TestMethod]
        public void OnPrepareExplorerTrigger()
        {
            project.Variables.CreateLocalVariable(element1, 0);
            project.Variables.CreateLocalVariable(element1, 1);
            project.Variables.CreateLocalVariable(element1, 2);
            project.CopyExplorerElement(element1);
            var pasted = project.PasteExplorerElement(element1);

            for (int i = 0; i < pasted.trigger.LocalVariables.Count(); i++)
            {
                var copiedLv = (LocalVariable) element1.trigger.LocalVariables.Elements[i];
                var pastedLv = (LocalVariable)pasted.trigger.LocalVariables.Elements[i];
                int notEqualId = copiedLv.variable.Id;
                int actualId = pastedLv.variable.Id;

                Assert.AreNotEqual(notEqualId, actualId);
            }
        }
    }
}
