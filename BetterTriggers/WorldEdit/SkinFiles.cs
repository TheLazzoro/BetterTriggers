﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using War3Net.Build.Object;
using War3Net.Build.Extensions;
using War3Net.Common.Extensions;
using BetterTriggers.Models.War3Data;
using System.Threading;

namespace BetterTriggers.WorldEdit
{
    /// <summary>
    /// 1.33+ files.
    /// </summary>
    public static class SkinFiles
    {
        public static void Load()
        {
            if (CustomMapData.MPQMap.AbilitySkinObjectData != null)
            {
                var abilitySkinObjectData = CustomMapData.MPQMap.AbilitySkinObjectData;

                List<LevelObjectModification> abilities = new();
                abilities.AddRange(abilitySkinObjectData.BaseAbilities);
                abilities.AddRange(abilitySkinObjectData.NewAbilities);
                abilities.ForEach(a =>
                {
                    string abilcode;
                    if (a.NewId != 0)
                        abilcode = Int32Extensions.ToRawcode(a.NewId);
                    else
                        abilcode = Int32Extensions.ToRawcode(a.OldId);

                    var abilityType = AbilityTypes.GetAbilityType(abilcode);
                    foreach (var modification in a.Modifications)
                    {
                        string rawcode = Int32Extensions.ToRawcode(modification.Id);
                        if (rawcode == "anam")
                        {
                            string newName = MapStrings.GetString(modification.ValueAsString);
                            abilityType.DisplayName = newName;
                        }

                        else if (rawcode == "ansf")
                        {
                            string newSuffix = MapStrings.GetString(modification.ValueAsString);
                            abilityType.EditorSuffix = newSuffix;
                        }
                    }
                });
            }

            if (CustomMapData.MPQMap.BuffSkinObjectData != null)
            {
                var buffSkinObjectData = CustomMapData.MPQMap.BuffSkinObjectData;

                List<SimpleObjectModification> buffs = new();
                buffs.AddRange(buffSkinObjectData.BaseBuffs);
                buffs.AddRange(buffSkinObjectData.NewBuffs);
                buffs.ForEach(b =>
                {
                    string buffcode;
                    if (b.NewId != 0)
                        buffcode = Int32Extensions.ToRawcode(b.NewId);
                    else
                        buffcode = Int32Extensions.ToRawcode(b.OldId);

                    var buffType = BuffTypes.GetBuffType(buffcode);
                    bool hasTip = false;
                    for (int i = 0; i < b.Modifications.Count; i++)
                    {
                        var mod = b.Modifications[i];
                        string rawcode = Int32Extensions.ToRawcode(mod.Id);
                        hasTip = rawcode == "ftip";
                        if (hasTip)
                            break;
                    }
                    foreach (var modification in b.Modifications)
                    {
                        string rawcode = Int32Extensions.ToRawcode(modification.Id);
                        if (rawcode == "fnam" && !hasTip)
                        {
                            string newName = MapStrings.GetString(modification.ValueAsString);
                            buffType.DisplayName = newName;
                        }
                        else if (rawcode == "ftip")
                        {
                            string newName = MapStrings.GetString(modification.ValueAsString);
                            buffType.DisplayName = newName;
                        }
                        else if (rawcode == "fnsf")
                        {
                            string newSuffix = MapStrings.GetString(modification.ValueAsString);
                            buffType.EditorSuffix = newSuffix;
                        }
                    }
                });
            }

            if (CustomMapData.MPQMap.DestructableSkinObjectData != null)
            {
                var destSkinObjectData = CustomMapData.MPQMap.DestructableSkinObjectData;

                List<SimpleObjectModification> dests = new();
                dests.AddRange(destSkinObjectData.BaseDestructables);
                dests.AddRange(destSkinObjectData.NewDestructables);
                dests.ForEach(d =>
                {
                    string destcode;
                    if (d.NewId != 0)
                        destcode = Int32Extensions.ToRawcode(d.NewId);
                    else
                        destcode = Int32Extensions.ToRawcode(d.OldId);

                    var destType = DestructibleTypes.GetDestType(destcode);
                    foreach (var modification in d.Modifications)
                    {
                        string rawcode = Int32Extensions.ToRawcode(modification.Id);
                        if (rawcode == "bnam")
                        {
                            string newName = MapStrings.GetString(modification.ValueAsString);
                            destType.DisplayName = newName;
                        }

                        else if (rawcode == "bsuf")
                        {
                            string newSuffix = MapStrings.GetString(modification.ValueAsString);
                            destType.EditorSuffix = newSuffix;
                        }
                    }
                });
            }

            if (CustomMapData.MPQMap.DoodadSkinObjectData != null)
            {
                var doodSkinObjectData = CustomMapData.MPQMap.DoodadSkinObjectData;

                List<VariationObjectModification> doods = new();
                doods.AddRange(doodSkinObjectData.BaseDoodads);
                doods.AddRange(doodSkinObjectData.NewDoodads);
                doods.ForEach(d =>
                {
                    string doodcode;
                    if (d.NewId != 0)
                        doodcode = Int32Extensions.ToRawcode(d.NewId);
                    else
                        doodcode = Int32Extensions.ToRawcode(d.OldId);

                    var doodType = DoodadTypes.GetDoodadType(doodcode);
                    foreach (var modification in d.Modifications)
                    {
                        string rawcode = Int32Extensions.ToRawcode(modification.Id);
                        if (rawcode == "dnam")
                        {
                            string newName = MapStrings.GetString(modification.ValueAsString);
                            doodType.DisplayName = newName;
                        }
                    }
                });
            }

            if (CustomMapData.MPQMap.ItemSkinObjectData != null)
            {
                var itemSkinObjectData = CustomMapData.MPQMap.ItemSkinObjectData;

                List<SimpleObjectModification> items = new();
                items.AddRange(itemSkinObjectData.BaseItems);
                items.AddRange(itemSkinObjectData.NewItems);
                items.ForEach(i =>
                {
                    string itemcode;
                    if (i.NewId != 0)
                        itemcode = Int32Extensions.ToRawcode(i.NewId);
                    else
                        itemcode = Int32Extensions.ToRawcode(i.OldId);

                    var itemType = ItemTypes.GetItemType(itemcode);
                    foreach (var modification in i.Modifications)
                    {
                        string rawcode = Int32Extensions.ToRawcode(modification.Id);
                        if (rawcode == "unam")
                        {
                            string newName = MapStrings.GetString(modification.ValueAsString);
                            itemType.DisplayName = newName;
                        }
                    }
                });
            }

            if (CustomMapData.MPQMap.UnitSkinObjectData != null)
            {
                var unitSkinObjectData = CustomMapData.MPQMap.UnitSkinObjectData;

                List<SimpleObjectModification> unitTypes = new();
                unitTypes.AddRange(unitSkinObjectData.BaseUnits);
                unitTypes.AddRange(unitSkinObjectData.NewUnits);
                unitTypes.ForEach(u =>
                {
                    string unitcode;
                    if (u.NewId != 0)
                        unitcode = Int32Extensions.ToRawcode(u.NewId);
                    else
                        unitcode = Int32Extensions.ToRawcode(u.OldId);

                    UnitType unitType = UnitTypes.GetUnitType(unitcode);
                    foreach (var modification in u.Modifications)
                    {
                        string rawcode = Int32Extensions.ToRawcode(modification.Id);
                        if (rawcode == "unam")
                        {
                            string newName = MapStrings.GetString(modification.ValueAsString);
                            unitType.Name.Name = newName;
                        }
                        else if (rawcode == "upro")
                        {
                            string newProperName = MapStrings.GetString(modification.ValueAsString);
                            unitType.Name.Propernames = newProperName;
                        }
                        else if (rawcode == "unsf")
                        {
                            string newSuffix = MapStrings.GetString(modification.ValueAsString);
                            unitType.Name.EditorSuffix = newSuffix;
                        }
                    }
                });
            }

            if (CustomMapData.MPQMap.UpgradeSkinObjectData != null)
            {
                var upgradeSkinObjectData = CustomMapData.MPQMap.UpgradeSkinObjectData;

                List<LevelObjectModification> upgrades = new();
                upgrades.AddRange(upgradeSkinObjectData.BaseUpgrades);
                upgrades.AddRange(upgradeSkinObjectData.NewUpgrades);
                upgrades.ForEach(up =>
                {
                    string upgradeCode;
                    if (up.NewId != 0)
                        upgradeCode = Int32Extensions.ToRawcode(up.NewId);
                    else
                        upgradeCode = Int32Extensions.ToRawcode(up.OldId);

                    var upgradeType = UpgradeTypes.GetUpgradeType(upgradeCode);
                    foreach (var modification in up.Modifications)
                    {
                        string rawcode = Int32Extensions.ToRawcode(modification.Id);
                        if (rawcode == "gnam")
                        {
                            string newName = MapStrings.GetString(modification.ValueAsString);
                            upgradeType.DisplayName = newName;
                        }
                        else if (rawcode == "gnsf")
                        {
                            string newSuffix = MapStrings.GetString(modification.ValueAsString);
                            upgradeType.EditorSuffix = newSuffix;
                        }
                    }
                });
            }
        }
    }
}
