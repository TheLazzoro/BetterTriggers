using BetterTriggers;
using BetterTriggers.Containers;
using BetterTriggers.TestMap;
using BetterTriggers.WorldEdit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using War3Net.Build;
using War3Net.Build.Script;

namespace Tests
{
    [TestClass]
    public class ImportTest
    {
        string projectDir;
        string projectFilePath;
        string mapPath;

        public ImportTest()
        {
            projectDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Projects/ImportTestMap");
            projectFilePath = Path.Combine(projectDir, "ImportTestMap.json");
            mapPath = Path.Combine(projectDir, "map/Map.w3x");
        }

        [TestCleanup]
        public void AfterEach()
        {
            Project.Close();
            Directory.Delete(projectDir, true);
        }

        [TestMethod]
        public void ImportTriggersTest()
        {
            Casc.GameVersion = new Version(1, 36, 1); // hack.
            BetterTriggers.Init.Initialize(true);
            var project = Project.Load(projectFilePath);
            var map = Map.Open(mapPath);

            TriggerConverter converter = new TriggerConverter(mapPath);
            converter.ImportIntoCurrentProject(map.Triggers.TriggerItems);

            Builder mapBuilder = new Builder();
            mapBuilder.GenerateScript();

            List<int> triggerIds = new List<int>();
            List<int> variableIds = new List<int>();
            var triggers = project.Triggers.GetAll();
            var variables = project.Variables.GetGlobals();

            // Assert no duplicate id's
            for (int i = 0; i < triggers.Count; i++)
            {
                var t = triggers[i];
                Assert.IsFalse(triggerIds.Contains(t.GetId()));
                triggerIds.Add(t.GetId());
            }
            for (int i = 0; i < variables.Count; i++)
            {
                var v = variables[i];
                Assert.IsFalse(variableIds.Contains(v.GetId()));
                variableIds.Add(v.GetId());
            }
        }
    }
}
