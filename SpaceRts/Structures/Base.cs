using SpaceRts.Map;
using System;
using Microsoft.Xna.Framework.Graphics;
namespace SpaceRts.Structures
{
    public class Base : Structure
    {

        public Base(Model model, Cell cell) : base(cell, model, StructureTypes.Base)
        { }

    }
}
