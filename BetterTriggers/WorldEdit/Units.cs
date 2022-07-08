using BetterTriggers.Models.War3Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Threading;
using War3Net.Build.Extensions;
using War3Net.Build.Widget;

namespace BetterTriggers.WorldEdit
{
    public class Units
    {
        internal static List<UnitData> units = new List<UnitData>();
        internal static List<UnitData> items = new List<UnitData>();
        internal static List<UnitData> startLocations = new List<UnitData>();

        internal static List<UnitData> GetAll()
        {
            return units;
        }

        internal static List<UnitData> GetMapItemsAll()
        {
            return items;
        }

        internal static List<UnitData> GetMapStartLocations()
        {
            return startLocations;
        }

        internal static void Load()
        {
            units.Clear();
            items.Clear();

            string filePath = "war3mapUnits.doo";
            if (!File.Exists(Path.Combine(CustomMapData.mapPath, filePath)))
                return;

            while (CustomMapData.IsMapSaving())
            {
                Thread.Sleep(1000);
            }

            using (Stream s = new FileStream(Path.Combine(CustomMapData.mapPath, filePath), FileMode.Open, FileAccess.Read))
            {
                BinaryReader reader = new BinaryReader(s);
                var mapUnits = BinaryReaderExtensions.ReadMapUnits(reader);

                for (int i = 0; i < mapUnits.Units.Count; i++)
                {
                    if (mapUnits.Units[i].ToString() == "sloc")
                    {
                        startLocations.Add(mapUnits.Units[i]);
                        continue;
                    }

                    if (mapUnits.Units[i].IsItem())
                        items.Add(mapUnits.Units[i]);
                    else
                        units.Add(mapUnits.Units[i]);
                }
            }
        }

        [Obsolete("ParseUnits is deprecated, please use static Load instead.", true)]
        public void ParseUnits()
        {
            /*
            Stream s = new FileStream(@"...\war3mapUnits.doo", FileMode.Open);
            BinaryReader reader = new BinaryReader(s);

            var mapUnits = BinaryReaderExtensions.ReadMapUnits(reader);

            string format = new string(reader.ReadChars(4));
            UInt32 version = reader.ReadUInt32();
            int subversion = reader.ReadInt32();
            int unit_count = reader.ReadInt32();

            for (int k = 0; k < unit_count; k++)
            {
                Unit u = new Unit();
                u.Id = new string(reader.ReadChars(4));
                u.Variation = reader.ReadUInt32();
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                float z = reader.ReadSingle();
                u.Position = new Vector3(x, y, z);
                u.Angle = reader.ReadSingle();
                float scaleX = reader.ReadSingle();
                float scaleY = reader.ReadSingle();
                float scaleZ = reader.ReadSingle();
                u.Scale = new Vector3(scaleX, scaleY, scaleZ);

                u.SkinId = new string(reader.ReadChars(4));
                u.Flags = reader.ReadByte();
                u.Owner = reader.ReadInt32();

                reader.ReadByte(); // unknown
                reader.ReadByte(); // unknown

                u.HitPointsPercent = reader.ReadInt32();
                u.ManaPoints = reader.ReadInt32();

                if (subversion >= 11)
                {
                    u.ItemTablePointer = reader.ReadInt32();
                }

                int itemCount = reader.ReadInt32(); // total items count?
                for (int j = 0; j < itemCount; j++)
                {
                    reader.ReadInt32(); // idk yet
                    ItemSet itemSet = new ItemSet();
                    string itemId = new string(reader.ReadChars(4));
                    int chance = reader.ReadInt32();
                    itemSet.items.Add(new Tuple<string, int>(itemId, chance));
                }

                u.Gold = reader.ReadInt32();
                u.TargetAcquisition = reader.ReadSingle();
                u.Level = reader.ReadInt32();

                if (subversion >= 11)
                {
                    u.Strength = reader.ReadInt32();
                    u.Agility = reader.ReadInt32();
                    u.Intelligence = reader.ReadInt32();
                }

                int equippedItemCount = reader.ReadInt32();
                u.Items = new List<Tuple<int, string>>();
                for (int j = 0; j < equippedItemCount; j++)
                {
                    int slot = reader.ReadInt32();
                    string id = new string(reader.ReadChars(4));
                    u.Items.Add(new Tuple<int, string>(slot, id));
                }

                int totalAbilites = reader.ReadInt32();
                u.Abilites = new List<Tuple<string, int, int>>();
                for (int j = 0; j < totalAbilites; j++)
                {
                    string id = new string(reader.ReadChars(4));
                    int autocast = reader.ReadInt32();
                    int level = reader.ReadInt32();
                    u.Abilites.Add(new Tuple<string, int, int>(id, autocast, level));
                }

                u.random_Type = reader.ReadInt32();
                // TODO
                switch (u.random_Type)
                {
                    case 0:
                        reader.BaseStream.Position += reader.ReadByte() * 4;
                        //i.random = read_vector_byte(reader, 4);
                        break;
                    case 1:
                        reader.BaseStream.Position += reader.ReadByte() * 8;
                        //i.random = read_vector_byte(reader, 8);
                        break;
                    case 2:
                        reader.BaseStream.Position += reader.ReadByte() * (reader.ReadInt32() * 8);
                        //i.random = read_vector_byte(reader, (byte) (reader.ReadInt32() * 8));
                        break;
                }


                u.CustomColor = reader.ReadInt32();
                u.Waygate = reader.ReadInt32();
                u.CreationNumber = reader.ReadInt32();

                units.Add(u);

                // Either a unit or an item
                /*
                if (units_slk.row_headers.contains(i.id) || i.id == "sloc" || i.id == "uDNR" || i.id == "bDNR")
                {
                    units.push_back(i);
                }
                else
                {
                    items.push_back(i);
                }
            }

            s.Close();
            */
        }
    }
}
