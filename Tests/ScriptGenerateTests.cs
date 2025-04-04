using BetterTriggers;
using BetterTriggers.Containers;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.TestMap;
using BetterTriggers.WorldEdit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Tests
{
    [TestClass]
    public class ScriptGenerateTests : TestBase
    {
        static War3Project war3project;
        static string mapDir;
        static string projectFile;
        static string tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "Temp");
        static string failedMsg = "Script generate failed. Project folder kept for inspection.";
        bool success;

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            Console.WriteLine("-----------");
            Console.WriteLine("RUNNING SCRIPT GENERATE TESTS");
            Console.WriteLine("-----------");
            Console.WriteLine("");

            if (Directory.Exists(tempFolder))
            {
                string[] tempData = Directory.GetDirectories(tempFolder);
                foreach (var folder in tempData)
                {
                    Directory.Delete(folder, true);
                }
            }
        }

        [TestCleanup]
        public void AfterEach()
        {
            Project.Close();
            string projectDir = Path.GetDirectoryName(projectFile);
            if (success && Directory.Exists(projectDir))
                Directory.Delete(projectDir, true);
        }


        [TestMethod]
        public void ConvertMap_GenerateScript_DirectStrikeReforged_New()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/Direct_Strike_Reforged.w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_DirectStrikeReforged()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/Direct_Strike_Reforged_Open_Source.w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_DirectStrikeReforged_133()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/Direct_Strike_Reforged_Open_Source_1.33.w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_DirectStrikeReforged_134()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/Direct_Strike_Reforged_Open_Source_1.34.w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_Enfo_FFB()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/Enfo FFB - v2.42d.w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_MZA()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/MZA 2.41.w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_AAE_REBORN()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/AAE REBORN V0.5b (beta) UNPROTECTED.w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_Skeletal_Annihilation()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/Skeletal Annihilation 3.075.w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_WoW_Dungeons_Classic()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/WoW Dungeons Classic 3.9.w3x");
            ConvertMap_GenerateScript(mapDir, true); // this one creates variables for all preplaced map objects.

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_Holy_War_Anniversary()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/HolyWarAnniversary1_32a.w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_The_Legend_of_Ergl_131()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/The Legend of Ergl_1.31.w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_Sheol_131()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/Sheol_131.w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_empty_131()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/empty_131.w3m");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_empty_132()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/empty_132.w3m");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_Metastasis_Reclassic_135()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/Metastasis Reclassic 1.0H.120.w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_MpqMap_135()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/MpqMap_135.w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_warquest()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/warquest.w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_Chimney_Isles()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/Chimney Isles.w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_MechaDefenders()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/MechaDefenders 1.1.0.w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_DeadlockNE25()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/DeadlockNE25.w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_Myth01()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/Myth01.CrowsBridge.w3m");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_Myth02()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/Myth02.WillowCreek.w3m");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_WarCard()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/WarCard_B_1.1.24.w3m");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_ZombieHunt()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/ZombieHunt1.10R.w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_KingsAndCursesTD()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/KingsAndCursesTD-V1.198.w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_Dark_Planet()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/Dark Planet 1.06.w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_Universe_Of_Warcraft()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/Universe Of Warcraft 1.7.w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_Risk_Forever()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/Risk Forever (2.5.7).w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_ItemTablesMap()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/ItemTablesMap.w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void ConvertMap_GenerateScript_STAY_ALIVE()
        {
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Maps/STAY ALIVE 6.w3x");
            ConvertMap_GenerateScript(mapDir);

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void GenerateScript_CustomProject_LocalVarMap()
        {
            string projectDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Projects/LocalVarMap/LocalVarMap.json");
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Projects/LocalVarMap/map/Map.w3x");
            CustomMapData.Load(mapDir);
            Project.Load(projectDir);
            Builder builder = new();
            bool success;
            string script;
            (success, script) = builder.GenerateScript();

            Assert.IsTrue(success, failedMsg);
        }

        [TestMethod]
        public void GenerateScript_CustomProject_FramesMap()
        {
            string projectDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Projects/Frames_Map/Frames_Map.json");
            mapDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Projects/Frames_Map/map/Map.w3x");
            CustomMapData.Load(mapDir);
            Project.Load(projectDir);
            Builder builder = new();
            bool success;
            string script;
            (success, script) = builder.GenerateScript();

            Assert.IsTrue(success, failedMsg);
        }



        bool ConvertMap_GenerateScript(string mapDir, bool GenerateAllMapObjectVariables = false)
        {
            TriggerConverter triggerConverter = new TriggerConverter(mapDir);
            string destination = Path.Combine(tempFolder, Path.GetFileNameWithoutExtension(mapDir));
            projectFile = triggerConverter.Convert(destination);

            string projectFileContent = File.ReadAllText(projectFile);
            war3project = JsonConvert.DeserializeObject<War3Project>(projectFileContent);
            war3project.War3MapDirectory = mapDir;
            war3project.GenerateAllObjectVariables = GenerateAllMapObjectVariables;
            File.WriteAllText(projectFile, JsonConvert.SerializeObject(war3project));

            Project.Load(projectFile);
            CustomMapData.Load(mapDir);
            //ControllerMapData.ReloadMapData(); // Crashes on GitHub Actions?
            string script;
            Builder builder = new();
            (success, script) = builder.GenerateScript();

            // Just for the sake of it
            CustomMapData.ReloadMapData();

            return success;
        }
    }
}
