using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Models.War3Data;
using BetterTriggers.WorldEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using War3Net.Build.Audio;
using War3Net.Build.Environment;
using War3Net.Build.Widget;

namespace BetterTriggers.Controllers
{
    public class ControllerMapData
    {
        public static List<AbilityType> GetAbilitiesAll()
        {
            return AbilityTypes.GetAll();
        }

        public static List<BuffType> GetBuffsAll()
        {
            return BuffTypes.GetAll();
        }

        public static List<UnitType> GetUnitTypesAll()
        {
            return UnitTypes.GetAll();
        }

        public static List<UpgradeType> GetUpgradeTypesAll()
        {
            return UpgradeTypes.GetAll();
        }

        public static List<DestructibleType> GetDestTypesAll()
        {
            return DestructibleTypes.GetAll();
        }

        public static List<DoodadType> GetDoodadTypesAll()
        {
            return DoodadTypes.GetAll();
        }

        public static List<ItemType> GetItemTypesAll()
        {
            return ItemTypes.GetAll();
        }

        public static List<UnitData> GetMapUnits()
        {
            return Units.GetAll();
        }

        public static List<UnitData> GetMapItems()
        {
            return Units.GetMapItemsAll();
        }

        public static List<DoodadData> GetMapDests()
        {
            return Destructibles.GetAll();
        }

        public static List<Region> GetMapRegions()
        {
            return Regions.GetAll();
        }

        public static List<Camera> GetMapCameras()
        {
            return Cameras.GetAll();
        }

        public static List<Sound> GetMapSounds()
        {
            return Sounds.GetSoundsAll();
        }

        public static List<Sound> GetMapMusic()
        {
            return Sounds.GetMusicAll();
        }

        public static List<AssetModel> GetModelData()
        {
            return ModelData.GetModelsAll();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">Reference to map data.</param>
        /// <returns></returns>
        internal static bool ReferencedDataExists(Value value, string returnType)
        {
            if (returnType == "unitcode")
            {
                List<UnitType> unitTypes = UnitTypes.GetAll();
                for (int i = 0; i < unitTypes.Count; i++)
                {
                    if (value.value == unitTypes[i].Id)
                    {
                        return true;
                    }
                }
            }
            else if (returnType == "unit")
            {
                var units = Units.GetAll();
                for (int i = 0; i < units.Count; i++)
                {
                    if (value.value == $"{units[i].ToString()}_{units[i].CreationNumber.ToString("D4")}")
                    {
                        return true;
                    }
                }
            }
            else if (returnType == "destructablecode")
            {
                List<DestructibleType> destTypes = DestructibleTypes.GetAll();
                for (int i = 0; i < destTypes.Count; i++)
                {
                    if (value.value == destTypes[i].DestCode)
                    {
                        return true;
                    }
                }
            }
            else if (returnType == "destructable")
            {
                var dests = Destructibles.GetAll();
                for (int i = 0; i < dests.Count; i++)
                {
                    if (value.value == $"{dests[i].ToString()}_{dests[i].CreationNumber.ToString("D4")}")
                    {
                        return true;
                    }
                }
            }
            else if (returnType == "itemcode")
            {
                List<ItemType> itemTypes = ItemTypes.GetAll();
                for (int i = 0; i < itemTypes.Count; i++)
                {
                    if (value.value == itemTypes[i].ItemCode)
                    {
                        return true;
                    }
                }
            }
            else if (returnType == "item")
            {
                List<UnitData> itemTypes = Units.GetMapItemsAll();
                for (int i = 0; i < itemTypes.Count; i++)
                {
                    if (value.value == $"{itemTypes[i].ToString()}_{itemTypes[i].CreationNumber.ToString("D4")}")
                    {
                        return true;
                    }
                }
            }
            else if (returnType == "doodadcode")
            {
                List<DoodadType> doodadTypes = DoodadTypes.GetAll();
                for (int i = 0; i < doodadTypes.Count; i++)
                {
                    if (value.value == doodadTypes[i].DoodCode)
                    {
                        return true;
                    }
                }
            }
            else if (returnType == "abilcode")
            {
                var abilities = AbilityTypes.GetAll();
                for (int i = 0; i < abilities.Count; i++)
                {
                    if (value.value == abilities[i].AbilCode)
                    {
                        return true;
                    }
                }
            }
            else if (returnType == "buffcode")
            {
                var buffs = BuffTypes.GetAll();
                for (int i = 0; i < buffs.Count; i++)
                {
                    if (value.value == buffs[i].BuffCode)
                    {
                        return true;
                    }
                }
            }
            else if (returnType == "techcode")
            {
                var tech = UpgradeTypes.GetAll();
                for (int i = 0; i < tech.Count; i++)
                {
                    if (value.value == tech[i].UpgradeCode)
                    {
                        return true;
                    }
                }
            }
            else if (returnType == "rect")
            {
                var regions = Regions.GetAll();
                for (int i = 0; i < regions.Count; i++)
                {
                    /* The string Replace exists because values converted with 'TriggerConverter' from a map
                     * have '_' in variable references, but War3Net values have spaces ' ' in them.
                     * Same goes for 'camerasetup' below.
                     */
                    if (value.value.Replace(" ", "_") == regions[i].ToString().Replace(" ", "_"))
                    {
                        return true;
                    }
                }
            }
            else if (returnType == "camerasetup")
            {
                var cameras = Cameras.GetAll();
                for (int i = 0; i < cameras.Count; i++)
                {
                    if (value.value.Replace(" ", "_") == cameras[i].ToString().Replace(" ", "_"))
                    {
                        return true;
                    }
                }
            }
            else
                return true;

            return false;
        }

        public static List<ExplorerElementTrigger> ReloadMapData()
        {
            Commands.CommandManager.Reset();
            CustomMapData.Load();
            var changedTriggers = CustomMapData.RemoveInvalidReferences();
            changedTriggers.ForEach(trig => UnsavedFiles.AddToUnsaved(trig));

            return changedTriggers;
        }
    }
}
