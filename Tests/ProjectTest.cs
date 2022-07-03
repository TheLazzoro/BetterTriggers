using BetterTriggers.Containers;
using BetterTriggers.Controllers;
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
    public class ProjectTest
    {
        static ScriptLanguage language = ScriptLanguage.Jass;
        static string name = "TestProject";
        static string projectPath;
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
            projectPath = controller.CreateProject(language, name, directory);
            project = controller.LoadProject(projectPath);
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
            var language = ScriptLanguage.Jass;
            var name = "TestProject2";
            var directory = System.IO.Directory.GetCurrentDirectory();

            ControllerProject controller = new ControllerProject();
            projectPath = controller.CreateProject(language, name, directory);
            project = controller.LoadProject(projectPath);

            Assert.AreEqual("jass", project.Language);
            Assert.AreEqual(name, project.Name);
            Assert.AreEqual(project.Root, directory + @"\" + name + @"\" + "src");

            Assert.IsTrue(Directory.Exists(project.Root), "Directory does not exist.");
            Assert.IsTrue(File.Exists(projectPath), "Project file does not exist.");
        }

        [TestMethod]
        public void CallingLoadProject_CheckIfExists()
        {
            ControllerProject controller = new ControllerProject();
            var loadedProject = controller.LoadProject(projectPath);

            Console.WriteLine(project.Root);
            Console.WriteLine(loadedProject.Root);
            Assert.AreEqual(project.Root, loadedProject.Root);
            Assert.AreEqual(project.Name, loadedProject.Name);
            Assert.AreEqual(project.Language, loadedProject.Language);
        }
    }
}
