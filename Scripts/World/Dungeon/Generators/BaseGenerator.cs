using Base;
using Godot;
using System;
using System.Collections.Generic;

namespace World.Dungeon.Generators
{

    public abstract class BaseGenerator 
    {
        protected Tile?[,] Tiles;

        public abstract ref Tile?[,] GetTiles(out Vector2 pos);

        public abstract Dictionary<MyPoint, byte> SpawnEnemies(in RandomNumberGenerator r);

        public abstract (Tile?[,] tiles, Vector2 playerPos, Dictionary<MyPoint, byte> enemies) GetWholePack();
    }
}
