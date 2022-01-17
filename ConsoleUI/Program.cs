using Model.WorldEditData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            Stream s = new FileStream(@"C:\Users\Lasse Dam\Desktop\test2.w3x\war3mapUnits.doo", FileMode.Open);

            BinaryReader reader = new BinaryReader(s);
            string format = new string(reader.ReadChars(4));
            UInt32 version = reader.ReadUInt32();
            int subversion = reader.ReadInt32();
            int unit_count = reader.ReadInt32();

            for (int k = 0; k < unit_count; k++)
            {
                Unit i = new Unit();
                i.Id = new string(reader.ReadChars(4));
                i.Variation = reader.ReadUInt32();
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                float z = reader.ReadSingle();
                i.Position = new Vector3(x, y, z);
                i.Angle = reader.ReadSingle();
                float scaleX = reader.ReadSingle();
                float scaleY = reader.ReadSingle();
                float scaleZ = reader.ReadSingle();
                i.Scale = new Vector3(scaleX, scaleY, scaleZ);

                i.SkinId = new string(reader.ReadChars(4));
                i.Flags = reader.ReadByte();
                i.Owner = reader.ReadInt32();

                reader.ReadByte(); // unknown
                reader.ReadByte(); // unknown

                i.HitPointsPercent = reader.ReadInt32();
                i.ManaPoints = reader.ReadInt32();

                if (subversion >= 11)
                {
                    i.ItemTablePointer = reader.ReadInt32();
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

                i.Gold = reader.ReadInt32();
                i.TargetAcquisition = reader.ReadSingle();
                i.Level = reader.ReadInt32();

                if (subversion >= 11)
                {
                    i.Strength = reader.ReadInt32();
                    i.Agility = reader.ReadInt32();
                    i.Intelligence = reader.ReadInt32();
                }

                int equippedItemCount = reader.ReadInt32();
                i.Items = new List<Tuple<int, string>>();
                for (int j = 0; j < equippedItemCount; j++)
                {
                    int slot = reader.ReadInt32();
                    string id = new string(reader.ReadChars(4));
                    i.Items.Add(new Tuple<int, string>(slot, id));
                }

                int totalAbilites = reader.ReadInt32();
                i.Abilites = new List<Tuple<string, int, int>>();
                for (int j = 0; j < totalAbilites; j++)
                {
                    string id = new string(reader.ReadChars(4));
                    int autocast = reader.ReadInt32();
                    int level = reader.ReadInt32();
                    i.Abilites.Add(new Tuple<string, int, int>(id, autocast, level));
                }

                i.random_Type = reader.ReadInt32();
                // TODO
                switch (i.random_Type)
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
                        reader.BaseStream.Position += reader.ReadByte() * (reader.ReadInt32()* 8);
                        //i.random = read_vector_byte(reader, (byte) (reader.ReadInt32() * 8));
                        break;
                }
                

                i.CustomColor = reader.ReadInt32();
                i.Waygate = reader.ReadInt32();
                i.CreationNumber = reader.ReadInt32();

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
                */
            }
           
        }
        
        /*
        private static List<byte> read_vector_byte(BinaryReader reader, byte size)
        {
            if(reader.BaseStream.Position + sizeof(byte) * size > reader.BaseStream.Length)
            {
                Console.WriteLine("Trying to read out of range of buffer");
            }

            List<byte> result = ReinterpretCast(BaseStream.Position)
            reader.BaseStream.Position += sizeof(byte) * size;
            return result;
        }

        static unsafe TDest ReinterpretCast<TSource, TDest>(TSource source)
        {
            var sourceRef = __makeref(source);
            var dest = default(TDest);
            var destRef = __makeref(dest);
            *(IntPtr*)&destRef = *(IntPtr*)&sourceRef;
            return __refvalue(destRef, TDest);
        }
        */
    }
}
