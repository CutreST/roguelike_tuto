using Godot;
using System;
using System.Collections.Generic;
using World.Dungeon.Generators;
using World;

namespace World.Dungeon
{
    //creo que esto lo podemos poner como un sistema, pero bueno.
    //también habría que cambiar el nombre, porque no es un generador o bien sí,
    //cambiar el otro
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
            return new SimpleGenerator().GetTiles(out pos);
        }

        

       



    }
}
