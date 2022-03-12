using BetterTriggers.Controllers;
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
        static string sourceFolder = System.IO.Directory.GetCurrentDirectory() + @"\" + "source";
        static string targetFolder = System.IO.Directory.GetCurrentDirectory() + @"\" + "target";

        [TestInitialize]
        public void BeforeEach()
        {
            Directory.Delete(sourceFolder, true);
            Directory.Delete(targetFolder, true);
            Directory.CreateDirectory(sourceFolder);
            Directory.CreateDirectory(targetFolder);
        }

        [TestMethod]
        public void FileMove()
        {
            Directory.CreateDirectory(sourceFolder);
            Directory.CreateDirectory(targetFolder);

            var file = sourceFolder + @"\" + "testFile";
            File.WriteAllText(file, "This is a test.");

            ControllerFileSystem controller = new ControllerFileSystem();
            controller.MoveFile(file, targetFolder, 0);


            var newPath = targetFolder + @"\" + "testFile";
            
            var expected = true;
            var actual = File.Exists(newPath);

            Assert.AreEqual(expected, actual);
        }
    }
}
