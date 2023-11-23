using BetterTriggers;
using BetterTriggers.Containers;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.WorldEdit;
using Newtonsoft.Json;
using System.Diagnostics;
using War3Net.Build;

public static class Program
{
    const string mapCollectionFolder = "D:/w3 maps/warcraft3-map-archive/Warcraft 3 Map Archive";
    const string resultsFolder = mapCollectionFolder + "/results";
    static string currentProjectDestinationDir = string.Empty;

    public static void Main(string[] args)
    {
        Casc.GameVersion = new Version(1, 35, 1); // hack. We need the newest version to load our custom frame definition script.
        BetterTriggers.Init.Initialize(true);

        if (!Directory.Exists(resultsFolder))
        {
            Directory.CreateDirectory(resultsFolder);
        }

        string[] maps = Directory.GetFileSystemEntries(mapCollectionFolder, "*", SearchOption.AllDirectories);
        int completed = 0;
        int failed = 0;
        Stopwatch timer = new Stopwatch();
        timer.Start();
        for (int i = 0; i < maps.Length; i++)
        {
            try
            {
                string mapFilePath = maps[i];
                var map = Map.Open(mapFilePath);
                ConvertMap_GenerateScript(mapFilePath);
                string[] srcFiles = Directory.GetFileSystemEntries(currentProjectDestinationDir + "/src", "*", SearchOption.TopDirectoryOnly);
                if(srcFiles.Length == 0)
                {
                    throw new Exception("Project was not converted correctly");
                }

                completed++;
            }
            catch (Exception ex)
            {
                Project.Close();
                if (Directory.Exists(currentProjectDestinationDir))
                {
                    Directory.Delete(currentProjectDestinationDir, true);
                }
                failed++;
            }

            if (i % 20 == 0)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine($"Completed: {completed}");
                Console.WriteLine($"Fails:     {failed}");

                Console.WriteLine($"Estimated time left: {timer.Elapsed * (maps.Length / (i + 1))}");
                Console.WriteLine($"------------------------------");
            }
        }
    }

    static void ConvertMap_GenerateScript(string mapPath)
    {
        TriggerConverter triggerConverter = new TriggerConverter();
        currentProjectDestinationDir = Path.Combine(resultsFolder, Path.GetFileNameWithoutExtension(mapPath));
        var projectFile = triggerConverter.Convert(mapPath, currentProjectDestinationDir);
        BetterTriggers.TestMap.Builder builder = new();

        string projectFileContent = File.ReadAllText(projectFile);
        var war3project = JsonConvert.DeserializeObject<War3Project>(projectFileContent);
        war3project.War3MapDirectory = mapPath;
        File.WriteAllText(projectFile, JsonConvert.SerializeObject(war3project));

        Project.Load(projectFile);
        CustomMapData.Load(mapPath);
        bool success;
        string script;
        (success, script) = builder.GenerateScript();

        // Just for the sake of it
        CustomMapData.ReloadMapData();
        Project.Close();
    }
}