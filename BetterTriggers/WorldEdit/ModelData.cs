﻿using CASCLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniParser;
using IniParser.Model;
using System.IO;
using IniParser.Parser;
using BetterTriggers.Utility;
using BetterTriggers.Models.War3Data;
using BetterTriggers.WorldEdit.GameDataReader;
using Newtonsoft.Json.Bson;
using War3Net.IO.Slk;
using NuGet.ContentModel;
using War3Net.Build.Widget;

namespace BetterTriggers.WorldEdit
{
    public static class ModelData
    {
        private static List<AssetModel> assetModels = new List<AssetModel>();

        public static List<AssetModel> GetModelsAll()
        {
            return assetModels;
        }


        internal static void Load(bool isTest = false)
        {
            assetModels.Clear();


            // some asset strings occur multiple times
            HashSet<AssetModel> hashset = new HashSet<AssetModel>();
            Stream abilityskin;

            List<Stream> filesWithAbilityArt = new List<Stream>();

            if (isTest)
            {
                abilityskin = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "TestResources/abilityskin.txt"), FileMode.Open);
            }
            else
            {
                if (!isTest && WarcraftStorageReader.GameVersion < new Version(1, 32))
                {
                    filesWithAbilityArt.Add(WarcraftStorageReader.OpenFile(@"units\campaignabilityfunc.txt"));
                    filesWithAbilityArt.Add(WarcraftStorageReader.OpenFile(@"units\campaignunitfunc.txt"));
                    filesWithAbilityArt.Add(WarcraftStorageReader.OpenFile(@"units\commonabilityfunc.txt"));
                    filesWithAbilityArt.Add(WarcraftStorageReader.OpenFile(@"units\humanabilityfunc.txt"));
                    filesWithAbilityArt.Add(WarcraftStorageReader.OpenFile(@"units\humanunitfunc.txt"));
                    filesWithAbilityArt.Add(WarcraftStorageReader.OpenFile(@"units\itemabilityfunc.txt"));
                    filesWithAbilityArt.Add(WarcraftStorageReader.OpenFile(@"units\neutralabilityfunc.txt"));
                    filesWithAbilityArt.Add(WarcraftStorageReader.OpenFile(@"units\neutralunitfunc.txt"));
                    filesWithAbilityArt.Add(WarcraftStorageReader.OpenFile(@"units\nightelfabilityfunc.txt"));
                    filesWithAbilityArt.Add(WarcraftStorageReader.OpenFile(@"units\nightelfunitfunc.txt"));
                    filesWithAbilityArt.Add(WarcraftStorageReader.OpenFile(@"units\orcabilityfunc.txt"));
                    filesWithAbilityArt.Add(WarcraftStorageReader.OpenFile(@"units\orcunitfunc.txt"));
                    filesWithAbilityArt.Add(WarcraftStorageReader.OpenFile(@"units\undeadabilityfunc.txt"));
                    filesWithAbilityArt.Add(WarcraftStorageReader.OpenFile(@"units\undeadunitfunc.txt"));
                }
                else
                {
                    filesWithAbilityArt.Add(WarcraftStorageReader.OpenFile(@"units\abilityskin.txt"));
                }
            }

            foreach (var stream in filesWithAbilityArt)
            {
                StreamReader reader = new StreamReader(stream);
                string text = reader.ReadToEnd();
                stream.Close();

                var data = IniFileConverter.GetIniData(text);

                var enumSections = data.Sections.GetEnumerator();
                while (enumSections.MoveNext())
                {
                    var section = enumSections.Current;
                    var enumKeys = section.Keys.GetEnumerator();
                    var category = string.Empty;
                    while (enumKeys.MoveNext())
                    {
                        var key = enumKeys.Current;
                        if (key.KeyName == "Art" || key.KeyName == "Researchart")
                            new Icon(key.Value, AbilityTypes.GetName(section.SectionName), "Ability");
                        else if (key.KeyName == "Buffart")
                            new Icon(key.Value, BuffTypes.GetName(section.SectionName), "Buff");

                        if (key.KeyName == "skinType")
                            category = key.Value;
                        if (key.KeyName == "Targetart" ||
                            key.KeyName == "Specialart" ||
                            key.KeyName == "Missileart" ||
                            key.KeyName == "Casterart" ||
                            key.KeyName == "Buffart" ||
                            key.KeyName == "Effectart"
                            )
                        {
                            if (key.Value != "")
                            {
                                string displayName = Locale.GetDisplayName(section.SectionName);
                                if (displayName == null)
                                    displayName = "";

                                switch (key.KeyName)
                                {
                                    case "Targetart":
                                        displayName += " <Target>";
                                        break;
                                    case "Specialart":
                                        displayName += " <Special>";
                                        break;
                                    case "Missileart":
                                        displayName += " <Missile>";
                                        break;
                                    case "Casterart":
                                        displayName += " <Caster>";
                                        break;
                                    case "Effectart":
                                        displayName += " <Effect>";
                                        break;
                                    default:
                                        break;
                                }
                                string[] paths = key.Value.Split(',');
                                foreach (var path in paths)
                                {
                                    hashset.Add(new AssetModel()
                                    {
                                        DisplayName = displayName,
                                        Path = path,
                                        Category = category
                                    });
                                }
                            }
                        }
                    }
                }
            }

            AddAssetModels(hashset);
        }

        private static void AddAssetModels(HashSet<AssetModel> hashset)
        {
            var unitData = UnitTypes.GetBase();
            var destData = DestructibleTypes.GetBase();
            var doodData = DoodadTypes.GetBase();
            var itemData = ItemTypes.GetBase();

            // TODO: wtf
            for (int i = 0; i < unitData.Count; i++)
            {
                try
                {
                    if (unitData[i].Model == null)
                    {
                        continue;
                    }

                    hashset.Add(new AssetModel()
                    {
                        DisplayName = unitData[i].Name == null ? "" : UnitTypes.GetName(unitData[i].Id),
                        Path = unitData[i].Model,
                        Category = "Unit"
                    });
                } catch(Exception) { }
            }
            for (int i = 0; i < destData.Count; i++)
            {
                if (destData[i].Model != null)
                    hashset.Add(new AssetModel()
                    {
                        DisplayName = destData[i].DisplayName == null ? "" : destData[i].DisplayName,
                        Path = destData[i].Model,
                        Category = "Destructible"
                    });
            }
            for (int i = 0; i < doodData.Count; i++)
            {
                hashset.Add(new AssetModel()
                {
                    DisplayName = doodData[i].DisplayName == null ? "" : doodData[i].DisplayName,
                    Path = doodData[i].Model,
                    Category = "Doodad"
                });
            }
            for (int i = 0; i < itemData.Count; i++)
            {
                hashset.Add(new AssetModel()
                {
                    DisplayName = itemData[i].DisplayName == null ? "" : itemData[i].DisplayName,
                    Path = itemData[i].Model,
                    Category = "Items"
                });
            }

            assetModels = hashset.OrderBy(asset => asset.DisplayName).ToList();
        }
    }
}
