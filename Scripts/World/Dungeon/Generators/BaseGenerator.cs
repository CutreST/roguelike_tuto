using Godot;
using System;

namespace World.Dungeon.Generators
{

    public abstract class BaseGenerator 
    {
        protected Tile?[,] Tiles;

        public abstract ref Tile?[,] GetTiles(out Vector2 pos);
    }
}
