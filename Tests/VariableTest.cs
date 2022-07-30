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
    public class VariableTest
    {
        static ScriptLanguage language = ScriptLanguage.Jass;
        static string name = "TestProject";
        static string projectPath;
        static War3Project project;
        static string directory = System.IO.Directory.GetCurrentDirectory();

        static ExplorerElementVariable element1, element2, element3;


        //[AssemblyInitialize]
        //public static void Init()
        //{

        //}

        [TestInitialize]
        public void BeforeEach()
        {
            if (Directory.Exists(Path.Combine(directory, name)))
                Directory.Delete(Path.Combine(directory, name), true);
            if(File.Exists(Path.Combine(directory, name + ".json")))
                File.Delete(Path.Combine(directory, name + ".json"));

            ControllerProject controllerProject = new ControllerProject();
            projectPath = controllerProject.CreateProject(language, name, directory);
            project = controllerProject.LoadProject(projectPath);
            controllerProject.SetEnableFileEvents(false); // TODO: Not ideal for testing, but necessary with current architecture.

            ControllerVariable controllerVariable = new ControllerVariable();
            string fullPath = controllerVariable.CreateVariable();
            controllerProject.OnCreateElement(fullPath);
            element1 = Variables.GetLastCreated();

            fullPath = controllerVariable.CreateVariable();
            controllerProject.OnCreateElement(fullPath);
            element2 = Variables.GetLastCreated();

            fullPath = controllerVariable.CreateVariable();
            controllerProject.OnCreateElement(fullPath);
            element3 = Variables.GetLastCreated();
        }

        [TestCleanup]
        public void AfterEach()
        {
        }


        [TestMethod]
        public void OnCreateVariable()
        {
            ControllerProject controllerProject = new ControllerProject();
            ControllerVariable controllerVariable = new ControllerVariable();
            string fullPath = controllerVariable.CreateVariable();
            controllerProject.OnCreateElement(fullPath);
            var element = Variables.GetLastCreated();

            string expectedName = Path.GetFileNameWithoutExtension(fullPath);
            string actualName = element.GetName();

            Assert.AreEqual(expectedName, actualName);
        }

        [TestMethod]
        public void OnPasteVariable()
        {
            ControllerProject controllerProject = new ControllerProject();
            controllerProject.CopyExplorerElement(element1);
            var element = controllerProject.PasteExplorerElement(element3) as ExplorerElementVariable;

            int expectedArray0 = element1.variable.ArraySize[0];
            int expectedArray1 = element1.variable.ArraySize[1];
            int actualArray0 = element.variable.ArraySize[0];
            int actualArray1 = element.variable.ArraySize[0];

            string expectedType = element1.variable.Type;
            string actualType = element.variable.Type;

            Assert.AreEqual(element, Variables.GetLastCreated());
            Assert.AreEqual(expectedArray0, actualArray0);
            Assert.AreEqual(expectedArray1, actualArray1);
            Assert.AreEqual(expectedType, actualType);
        }
    }
}
