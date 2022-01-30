using GUI.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading;
using System.Windows;

namespace Tests
{
    [TestClass]
    public class ProjectTest
    {
        [TestMethod]
        public void CallingCreateProject()
        {
            var language = "jass";
            var name = "TestProject";
            var directory = System.IO.Directory.GetCurrentDirectory();

            ControllerProject controller = new ControllerProject();
            var project = controller.CreateProject(language, name, directory);

            Assert.AreEqual(language, project.Language);
            Assert.AreEqual(name, project.Name);
            Assert.AreEqual(project.Root, directory + @"\" + name);

            Assert.IsTrue(Directory.Exists(project.Root), "Directory does not exist.");
            Assert.IsTrue(File.Exists(directory + @"\" + name + ".json"), "Project file does not exist.");
        }

        [TestMethod]
        public void CallingLoadProject()
        {
            var language = "jass";
            var name = "TestProject";
            var directory = System.IO.Directory.GetCurrentDirectory();

            ControllerProject controller = new ControllerProject();
            var project = controller.CreateProject(language, name, directory);
            var loadedProject = controller.LoadProject(new System.Windows.Controls.Grid(), directory + @"\" + name + ".json");

            Assert.AreEqual(project.Root, loadedProject.Root);
            Assert.AreEqual(project.Name, loadedProject.Name);
            Assert.AreEqual(project.Language, loadedProject.Language);
        }
    }
}
