using BetterTriggers.Containers;
using BetterTriggers.Models.SaveableData;
using BetterTriggers;
using BetterTriggers.WorldEdit;
using Cake.Common.Solution.Project;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using War3Net.Build;
using BetterTriggers.TestMap;

namespace Tests
{
    [TestClass]
    public class BT2WETests : TestBase
    {
        static string tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "Temp");

        public BT2WETests()
        {
            if (Directory.Exists(tempFolder))
            {
                string[] tempData = Directory.GetDirectories(tempFolder);
                foreach (var folder in tempData)
                {
                    Directory.Delete(folder, true);
                }
            }
            else
            {
                Directory.CreateDirectory(tempFolder);
            }
        }

        [TestMethod]
        public void ConvertBT2WE_Test()
        {
            string[] testMaps = Directory.GetFileSystemEntries("TestResources\\Maps\\");
            foreach (var mapPath in testMaps)
            {
                TriggerConverter triggerConverter = new TriggerConverter(mapPath);
                string destination = Path.Combine(tempFolder, Path.GetFileNameWithoutExtension(mapPath));
                var projectFile = triggerConverter.Convert(destination);

                string projectFileContent = File.ReadAllText(projectFile);
                var war3project = JsonConvert.DeserializeObject<War3Project>(projectFileContent);
                war3project.War3MapDirectory = mapPath;
                File.WriteAllText(projectFile, JsonConvert.SerializeObject(war3project));

                Project.Load(projectFile);
                CustomMapData.Load(mapPath);
                //ControllerMapData.ReloadMapData(); // Crashes on GitHub Actions?
                Builder builder = new();
                builder.BuildMap(tempFolder);
            }

            string[] btMaps = Directory.GetFileSystemEntries(tempFolder);
            foreach (var btMap in btMaps)
            {
                bool success = Map.TryOpen(btMap, out var map);
                Assert.IsTrue(success);
            }
        }
    }
}
