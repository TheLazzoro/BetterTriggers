using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Model.War3Data
{
    [Obsolete("Not used any more", true)]
    public class Unit
    {
        public string Id;
        public UInt32 Variation;
        public Vector3 Position;
        public float Angle;
        public Vector3 Scale;
        public string SkinId;
        public byte Flags = 2;
        public int Owner = 0;
        public int HitPointsPercent = -1;
        public int ManaPoints = -1;
        public int ItemTablePointer;
        public List<ItemSet> Item_Sets;
        public int Gold;
        public float TargetAcquisition;
        public int Level = 1;
        public int Strength = 0;
        public int Agility = 0;
        public int Intelligence = 0;
        public List<Tuple<int, string>> Items; // Slot, Id
        public List<Tuple<string, int, int>> Abilites; // Id, autocast, level
        public int random_Type = 0;
        public List<byte> random;
        public int CustomColor = -1;
        public int Waygate = -1;
        public int CreationNumber;

    }
}
