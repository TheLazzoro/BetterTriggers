using BetterTriggers;
using BetterTriggers.Containers;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.TestMap;
using BetterTriggers.WorldEdit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using War3Net.Build;

namespace Tests
{
    [TestClass]
    public class BT2WETests : TestBase
    {
        static string tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "Temp");
        private string projectFile;
        private bool success;

        [ClassInitialize]
        public static void Init(TestContext context)
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

        public static IEnumerable<object[]> GetMapsFromTestFolder
        {
            get
            {
                string[] maps = Directory.GetFileSystemEntries("TestResources\\Maps\\");
                var objects = new List<object[]>();
                for (int i = 0; i < maps.Length; i++)
                {
                    yield return new[] { Path.Combine(Directory.GetCurrentDirectory(), maps[i]) };
                }
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetMapsFromTestFolder), typeof(BT2WETests), DynamicDataSourceType.Property)]
        public void ConvertBT2WE_Test(string mapPath)
        {
            TriggerConverter triggerConverter = new TriggerConverter(mapPath);
            string destination = Path.Combine(tempFolder, Path.GetFileNameWithoutExtension(mapPath));
            projectFile = triggerConverter.Convert(destination);

            string projectFileContent = File.ReadAllText(projectFile);
            var war3project = JsonConvert.DeserializeObject<War3Project>(projectFileContent);
            war3project.War3MapDirectory = mapPath;
            File.WriteAllText(projectFile, JsonConvert.SerializeObject(war3project));

            var project = Project.Load(projectFile);
            CustomMapData.Load(mapPath);
            CustomMapData.ReloadMapData();

            //ControllerMapData.ReloadMapData(); // Crashes on GitHub Actions?
            Builder builder = new();
            builder.BuildMap();

            // yes, we loop. There's one file, but I'm lazy and don't want to think about the file name right now.
            string[] btMaps = Directory.GetFileSystemEntries(project.dist);
            foreach (var btMap in btMaps)
            {
                using (var stream = new FileStream(btMap, FileMode.Open))
                {
                    var map = Map.Open(stream);
                    Assert.IsNotNull(map);
                    success = true;
                    stream.Dispose();
                }
            }

            Project.Close();
        }
    }
}
