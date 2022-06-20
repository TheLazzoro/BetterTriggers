using CASCLib;
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
using Model.War3Data;

namespace BetterTriggers.WorldEdit
{
    internal static class ModelData
    {
        private static List<AssetModel> assetModels = new List<AssetModel>();

        internal static List<AssetModel> GetModelsAll()
        {
            return assetModels;
        }

        internal static void Load()
        {
            assetModels.Clear();
            var unitData = UnitTypes.GetBase();
            var destData = DestructibleTypes.GetBase();
            var doodData = DoodadTypes.GetBase();
            var itemData = ItemTypes.GetBase();


            // some asset strings occur multiple times
            HashSet<AssetModel> hashset = new HashSet<AssetModel>();

            var units = (CASCFolder)Casc.GetWar3ModFolder().Entries["units"];
            CASCFile abilitySkin = (CASCFile)units.Entries["abilityskin.txt"];
            var file = Casc.GetCasc().OpenFile(abilitySkin.FullName);
            StreamReader reader = new StreamReader(file);
            string text = reader.ReadToEnd();

            string iniFile = IniFileConverter.Convert(text);
            IniDataParser parser = new IniDataParser();
            parser.Configuration.AllowDuplicateSections = true;
            parser.Configuration.AllowDuplicateKeys = true;
            IniData data = parser.Parse(iniFile);

            var enumSections = data.Sections.GetEnumerator();
            while (enumSections.MoveNext())
            {
                var section = enumSections.Current;
                var enumKeys = section.Keys.GetEnumerator();
                var category = string.Empty;
                while (enumKeys.MoveNext())
                {
                    var key = enumKeys.Current;
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
                            string displayName = Locale.Translate(section.SectionName);
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

            // TODO: wtf
            for (int i = 0; i < unitData.Count; i++)
            {
                hashset.Add(new AssetModel()
                {
                    DisplayName = unitData[i].Name == null ? "" : unitData[i].Name,
                    Path = unitData[i].Model,
                    Category = "Unit"
                });
            }
            for (int i = 0; i < destData.Count; i++)
            {
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
