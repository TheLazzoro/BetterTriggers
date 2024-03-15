using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
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

namespace Tests
{
    [TestClass]
    public class TriggerSerializerTests : TestBase
    {
        private static string MAP_FOLDER;
        static string TEMP_FOLDER;

        [ClassInitialize]
        public static void BeforeAll(TestContext context)
        {
            MAP_FOLDER = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Projects");
            TEMP_FOLDER = Path.Combine(Directory.GetCurrentDirectory(), "Temp");
        }

        [TestMethod]
        public void Deserialize_Serialize_Compare_Test()
        {
            // Saved map project using an old version of BT (v1.2.4)
            string projectDir = Path.Combine(MAP_FOLDER, "Direct_Strike_Reforged_Open_Source");
            string src = Path.Combine(projectDir, "src");
            string projectFile = Path.Combine(projectDir, "Direct_Strike_Reforged_Open_Source.json");
            string[] files = Directory.GetFileSystemEntries(src, "*", SearchOption.AllDirectories);
            string[] filesBefore = new string[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                if (File.Exists(file))
                    filesBefore[i] = File.ReadAllText(file);
            }

            var project = Project.Load(projectFile);
            project.AllElements.ForEach(el => el.Value.AddToUnsaved());
            project.Save();

            for (int i = 0; i < filesBefore.Length; i++)
            {
                if (!File.Exists(files[i])) continue;

                string beforeSerialize = filesBefore[i];
                string afterSerialize = File.ReadAllText(files[i]);

                Assert.AreEqual(beforeSerialize, afterSerialize);
            }
        }
    }
}
