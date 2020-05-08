using System;
using SpaceRts.Map;
namespace SpaceRts.Structures
{
    public class Ore : Structure
    {
        public enum OreTypes
        {
            Green,
            Blue,
            Red,
        }

        public static string[] OreNames = { "Green", "Blue", "Red" };

        public OreTypes OreType;

        public Ore(Cell cell, OreTypes oreType) : base(cell, Models.Ores[(int)oreType], StructureTypes.Ore)
        {
            OreType = oreType;
        }
    }
}
