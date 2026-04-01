using BetterTriggers.Containers;
using BetterTriggers.TestMap;
using BetterTriggers.WorldEdit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using War3Net.Build;

namespace Tests
{
    [TestClass]
    public class ImportTest : TestBase
    {
        string projectDir;
        string projectFilePath;
        string mapPath;
        Project project;

        public ImportTest()
        {
            projectDir = Path.Combine(Directory.GetCurrentDirectory(), "TestResources/Projects/ImportTestMap");
            projectFilePath = Path.Combine(projectDir, "ImportTestMap.json");
            mapPath = Path.Combine(projectDir, "map/Map.w3x");
        }

        [TestCleanup]
        public void AfterEach()
        {
            project.Close();
            Directory.Delete(projectDir, true);
        }

        [TestMethod]
        public void ImportTriggersTest()
        {
            project = Project.Load(projectFilePath);
            var map = Map.Open(mapPath);

            TriggerConverter converter = new TriggerConverter(project, mapPath, null);
            converter.ImportIntoCurrentProject(map.Triggers.TriggerItems);

            Builder mapBuilder = new Builder(project);
            mapBuilder.GenerateScript();

            List<int> triggerIds = new List<int>();
            List<int> variableIds = new List<int>();
            var triggers = project.Triggers.GetAll();
            var variables = project.Variables.GetGlobals();

            // Assert no duplicate id's
            for (int i = 0; i < triggers.Count; i++)
            {
                var t = triggers[i];
                int id = t.GetId();
                Assert.IsFalse(triggerIds.Contains(id));
                triggerIds.Add(id);
            }
            for (int i = 0; i < variables.Count; i++)
            {
                var v = variables[i];
                int id = v.GetId();
                Assert.IsFalse(variableIds.Contains(id));
                variableIds.Add(id);
            }
        }
    }
}
