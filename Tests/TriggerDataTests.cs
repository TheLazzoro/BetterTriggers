using BetterTriggers;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.WorldEdit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using War3Net.Build.Info;

namespace Tests
{
    [TestClass]
    public class TriggerDataTest : TestBase
    {
        static ScriptLanguage language = ScriptLanguage.Jass;
        static string name = "TestProject";
        static string projectPath;
        static Project project;
        static string directory = System.IO.Directory.GetCurrentDirectory();

        static ExplorerElement variable;
        static string variablePath;


        [ClassInitialize]
        public static void Init(TestContext context)
        {
            Console.WriteLine("-----------");
            Console.WriteLine("RUNNING TRIGGER DATA TESTS");
            Console.WriteLine("-----------");
            Console.WriteLine("");
        }

        [ClassCleanup]
        public static void Shutdown()
        {
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

            string fullPath = project.Variables.Create();
            variablePath = fullPath;
            project.OnCreateElement(fullPath); // Force OnCreate 'event'.
            variable = project.lastCreated;
        }

        [TestCleanup]
        public void AfterEach()
        {
            Project.Close();
        }


        [TestMethod]
        public void GetValueName()
        {
            string valueName = project.Triggers.GetValueName(null, "placeholder");

            Assert.IsNotNull(valueName);
        }

        [TestMethod]
        public void GetReturnType()
        {
            string expected = "player";
            string actual = TriggerData.GetReturnType("Player00");
            Assert.AreEqual(expected, actual);

            expected = "unit";
            actual = TriggerData.GetReturnType("GetTriggerUnit");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetParameterReturnTypes()
        {
            Function function = new Function()
            {
                value = "CreateNUnitsAtLoc"
            };

            List<string> expected = new List<string>();
            expected.Add("integer");
            expected.Add("unitcode");
            expected.Add("player");
            expected.Add("location");
            expected.Add("real");

            List<string> actual = TriggerData.GetParameterReturnTypes(function, null);

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }


            // --- 'SetVariable' special case --- //

            List<Parameter> parameters = new List<Parameter>();
            parameters.Add(new VariableRef()
            {
                VariableId = variable.GetId()
            });
            parameters.Add(new Value()
            {
                value = "42"
            });
            function = new Function()
            {
                value = "SetVariable",
                parameters = parameters
            };

            expected.Clear();
            expected.Add("integer");
            expected.Add("integer");
            actual = TriggerData.GetParameterReturnTypes(function, null);

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }


            // --- 'SetVariable' edge case --- //
            // When deleting a variable we should expect the reference to be deleted also,
            // which is what happens in @GetParameterReturnTypes.

            project.OnDeleteElement(variablePath);

            expected.Clear();
            expected.Add("null");
            expected.Add("null");
            actual = TriggerData.GetParameterReturnTypes(function, null);

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }
    }
}
