using BetterTriggers.Models.War3Data;
using BetterTriggers.Utility;
using CASCLib;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using War3Net.Build.Extensions;
using War3Net.Build.Object;
using War3Net.Common.Extensions;
using War3Net.IO.Slk;

namespace BetterTriggers.WorldEdit
{
    public class UpgradeTypes
    {
        private static Dictionary<string, UpgradeType> upgrades;
        private static Dictionary<string, UpgradeType> upgradesBaseEdited = new();
        private static Dictionary<string, UpgradeType> upgradesCustom = new();

        internal static List<UpgradeType> GetAll()
        {
            List<UpgradeType> list = new List<UpgradeType>();
            var enumerator = upgrades.GetEnumerator();
            while (enumerator.MoveNext())
            {
                UpgradeType upgradeType;
                var key = enumerator.Current.Key;
                if (upgradesBaseEdited.ContainsKey(key))
                {
                    upgradesBaseEdited.TryGetValue(key, out upgradeType);
                    list.Add(upgradeType);
                }
                else
                {
                    upgrades.TryGetValue(key, out upgradeType);
                    list.Add(upgradeType);
                }
            }

            list.AddRange(upgradesCustom.Select(kvp => kvp.Value).ToList());

            return list;
        }

        public static UpgradeType GetUpgradeType(string upgradecode)
        {
            UpgradeType upgradeType;
            upgradesCustom.TryGetValue(upgradecode, out upgradeType);

            if (upgradeType == null)
                upgradesBaseEdited.TryGetValue(upgradecode, out upgradeType);

            if (upgradeType == null)
                upgrades.TryGetValue(upgradecode, out upgradeType);

            if (upgradeType == null)
                upgradeType = new UpgradeType()
                {
                    UpgradeCode = upgradecode,
                    DisplayName = "<Unknown Upgrade>"
                };

            return upgradeType;
        }

        internal static string GetName(string upgradecode)
        {
            UpgradeType upgradeType = GetUpgradeType(upgradecode);
            if (upgradeType == null)
                return "<Empty Name>";
            else if (upgradeType.DisplayName == null)
                return "<Empty Name>";

            string name = upgradeType.DisplayName;
            if (!string.IsNullOrEmpty(upgradeType.EditorSuffix))
                name += " " + upgradeType.EditorSuffix;

            return name;
        }

        internal static void LoadFromCASC(bool isTest)
        {
            upgrades = new Dictionary<string, UpgradeType>();
            Stream upgradedata;
            IniData upgradeFunc;

            if (isTest)
            {
                upgradedata = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "TestResources/upgradedata.slk"), FileMode.Open);

                StringBuilder text = new StringBuilder();
                text.Append(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "TestResources/humanupgradefunc.txt")));
                text.Append(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "TestResources/nightelfupgradefunc.txt")));
                text.Append(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "TestResources/orcupgradefunc.txt")));
                text.Append(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "TestResources/undeadupgradefunc.txt")));
                text.Append(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "TestResources/campaignupgradefunc.txt")));
                upgradeFunc = IniFileConverter.GetIniData(text.ToString());
            }
            else
            {
                var units = (CASCFolder)Casc.GetWar3ModFolder().Entries["units"];
                /* TODO:
                 * We are loading too many upgrades from this.
                 * There are 'upgrades' for 'Chaos Conversions' and other stuff
                 * which are not actual upgrades that show up in the object editor.
                */
                CASCFile abilityData = (CASCFile)units.Entries["upgradedata.slk"];
                upgradedata = Casc.GetCasc().OpenFile(abilityData.FullName);

                CASCFile humanUp = (CASCFile)units.Entries["humanupgradefunc.txt"];
                CASCFile orcUp = (CASCFile)units.Entries["orcupgradefunc.txt"];
                CASCFile undeadUp = (CASCFile)units.Entries["undeadupgradefunc.txt"];
                CASCFile nightelfUp = (CASCFile)units.Entries["nightelfupgradefunc.txt"];
                CASCFile campaignUp = (CASCFile)units.Entries["campaignupgradefunc.txt"];
                StreamReader sr;
                StringBuilder text = new StringBuilder();
                sr = new StreamReader(Casc.GetCasc().OpenFile(humanUp.FullName));
                text.Append(sr.ReadToEnd());
                sr = new StreamReader(Casc.GetCasc().OpenFile(orcUp.FullName));
                text.Append(sr.ReadToEnd());
                sr = new StreamReader(Casc.GetCasc().OpenFile(undeadUp.FullName));
                text.Append(sr.ReadToEnd());
                sr = new StreamReader(Casc.GetCasc().OpenFile(nightelfUp.FullName));
                text.Append(sr.ReadToEnd());
                sr = new StreamReader(Casc.GetCasc().OpenFile(campaignUp.FullName));
                text.Append(sr.ReadToEnd());
                upgradeFunc = IniFileConverter.GetIniData(text.ToString());
            }

            SylkParser sylkParser = new SylkParser();
            SylkTable table = sylkParser.Parse(upgradedata);
            for (int i = 1; i < table.Count(); i++)
            {
                var row = table.ElementAt(i);
                string techcode = (string)row.GetValue(0);
                UpgradeType upgrade = new UpgradeType()
                {
                    UpgradeCode = techcode,
                    DisplayName = Locale.GetDisplayName(techcode),
                    EditorSuffix = Locale.GetEditorSuffix(techcode),
                };

                if (upgrade.UpgradeCode == null)
                    continue;

                upgrades.TryAdd(upgrade.UpgradeCode, upgrade);
            }

            // --- upgrade icon art --- //
            var upgradeSections = upgradeFunc.Sections;
            var enumerator = upgradeSections.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var section = enumerator.Current;
                string sectionName = section.SectionName;
                var keys = section.Keys.GetEnumerator();
                while (keys.MoveNext())
                {
                    var key = keys.Current;
                    if (key.KeyName == "Art")
                    {
                        string[] split = key.Value.Split(",");
                        for (int i = 0; i < split.Length; i++)
                        {
                            new Icon(split[i], UpgradeTypes.GetName(sectionName), "Upgrade");
                        }
                    }
                }
            }
        }

        internal static void Load()
        {
            upgradesBaseEdited = new Dictionary<string, UpgradeType>();
            upgradesCustom = new Dictionary<string, UpgradeType>();

            UpgradeObjectData customUpgrades = CustomMapData.MPQMap.UpgradeObjectData;
            if (customUpgrades == null)
                return;

            // Custom upgrades
            for (int i = 0; i < customUpgrades.BaseUpgrades.Count; i++)
            {
                var upgrade = customUpgrades.BaseUpgrades[i];
                UpgradeType baseUpgrade = GetUpgradeType(Int32Extensions.ToRawcode(upgrade.OldId));
                var u = new UpgradeType()
                {
                    UpgradeCode = upgrade.ToString().Substring(0, 4),
                    DisplayName = baseUpgrade.DisplayName,
                };
                upgradesBaseEdited.TryAdd(baseUpgrade.UpgradeCode, u);
                SetCustomFields(upgrade, u.UpgradeCode);
            }

            for (int i = 0; i < customUpgrades.NewUpgrades.Count; i++)
            {
                var customUpgrade = customUpgrades.NewUpgrades[i];

                UpgradeType baseUpgrade = GetUpgradeType(Int32Extensions.ToRawcode(customUpgrade.OldId));
                string name = baseUpgrade.DisplayName;
                foreach (var modified in customUpgrade.Modifications)
                {
                    if (Int32Extensions.ToRawcode(modified.Id) == "gnam")
                        name = MapStrings.GetString(modified.ValueAsString);
                }

                var upgrade = new UpgradeType()
                {
                    UpgradeCode = customUpgrade.ToString().Substring(0, 4),
                    DisplayName = name,
                };
                upgradesCustom.TryAdd(upgrade.UpgradeCode, upgrade);
                SetCustomFields(customUpgrade, upgrade.UpgradeCode);
            }
        }

        private static void SetCustomFields(LevelObjectModification modified, string techcode)
        {
            UpgradeType upgradeType = GetUpgradeType(techcode);
            string displayName = upgradeType.DisplayName;
            string editorSuffix = upgradeType.EditorSuffix;
            foreach (var modification in modified.Modifications)
            {
                if (Int32Extensions.ToRawcode(modification.Id) == "gnam")
                    displayName = MapStrings.GetString(modification.ValueAsString);
                else if (Int32Extensions.ToRawcode(modification.Id) == "gnsf")
                    editorSuffix = MapStrings.GetString(modification.ValueAsString);
            }
            upgradeType.DisplayName = displayName;
            upgradeType.EditorSuffix = editorSuffix;
        }
    }
}
