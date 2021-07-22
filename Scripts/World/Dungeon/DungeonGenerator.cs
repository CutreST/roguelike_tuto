using Godot;
using System;
using System.Collections.Generic;
using World.Dungeon.Generators;
using World;
using Base;

namespace World.Dungeon
{
    /// <summary>
    /// This class is the glue between the <see cref="WorldMapController"/> and all the generators.
    /// </summary>
    /// <remarks>
    /// To THINK: flaw in design, is really needed this class?
    /// </remarks>
    public class DungeonGenerator
    {
       
        public WorldMapCont Controller{ get; protected set; }
        Dictionary<Tile.TileType, List<Vector2>> _tiles;
        Vector2 startPos;

        public DungeonGenerator(in WorldMapCont cont){
            this.Controller = cont;
        }

        public Tile?[,] GetTiles(out Vector2 pos){
            //pillar el tipo de generator
            //devolvemos
            //return new RoomGridGenerator().GetTiles(out pos);
            return new SimpleGenerator().GetTiles(out pos);
            //return new FixedMaps().GetTiles(out pos);
        }

        public (Tile?[,] tiles, Vector2 playerPos, Dictionary<MyPoint, EnitityType> enemies) GetWholePack(){
            return new SimpleGenerator().GetWholePack();
        }

        

       



    }
}
