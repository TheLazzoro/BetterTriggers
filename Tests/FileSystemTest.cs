using GUI.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tests
{
    [TestClass]
    public class FileSystemTest
    {
        [TestMethod]
        public void FileMove()
        {
            /*
            var sourceFolder = System.IO.Directory.GetCurrentDirectory() + @"\" + "source";
            var targetFolder = System.IO.Directory.GetCurrentDirectory() + @"\" + "target";
            Directory.CreateDirectory(sourceFolder);
            Directory.CreateDirectory(targetFolder);

            var file = sourceFolder + @"\" + "testFile";
            File.WriteAllText(file, "This is a test.");

            ControllerFileSystem controller = new ControllerFileSystem();
            controller.MoveFile(file, targetFolder);


            var newPath = targetFolder + @"\" + "testFile";
            
            var expected = true;
            var actual = File.Exists(newPath);

            Assert.AreEqual(expected, actual);
            */
        }
    }
}
