using BetterTriggers.Containers;
using BetterTriggers.Controllers;
using GUI.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.SaveableData;
using System.IO;
using System.Threading;
using System.Windows;

namespace Tests
{
    [TestClass]
    public class ProjectTest
    {
        static string language = "jass";
        static string name = "TestProject";
        static War3Project project;
        static string directory = System.IO.Directory.GetCurrentDirectory();


        //[AssemblyInitialize]
        //public static void Init()
        //{
            
        //}

        [TestInitialize]
        public void BeforeEach()
        {
            ControllerProject controller = new ControllerProject();
            controller.CreateProject(language, name, directory);
            project = controller.LoadProject(directory + @"/" + name + ".json");
        }

        [TestCleanup]
        public void AfterEach()
        {
            Directory.Delete(directory + @"/" + name, true);
            File.Delete(directory + @"/" + name + ".json");
        }

        [TestMethod]
        public void CallingCreateProject_WhenCreate_CheckIfExists()
        {
            var language = "jass";
            var name = "TestProject2";
            var directory = System.IO.Directory.GetCurrentDirectory();

            ControllerProject controller = new ControllerProject();
            controller.CreateProject(language, name, directory);
            project = controller.LoadProject(directory + @"\" + name + ".json");

            Assert.AreEqual(language, project.Language);
            Assert.AreEqual(name, project.Name);
            Assert.AreEqual(project.Root, directory + @"\" + name);

            Assert.IsTrue(Directory.Exists(project.Root), "Directory does not exist.");
            Assert.IsTrue(File.Exists(directory + @"\" + name + ".json"), "Project file does not exist.");
        }

        [TestMethod]
        public void CallingLoadProject_CheckIfExists()
        {
            ControllerProject controller = new ControllerProject();
            var loadedProject = controller.LoadProject(directory + @"\" + name + ".json");

            Assert.AreEqual(project.Root, loadedProject.Root);
            Assert.AreEqual(project.Name, loadedProject.Name);
            Assert.AreEqual(project.Language, loadedProject.Language);
        }
    }
}
