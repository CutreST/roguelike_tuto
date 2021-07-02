using Godot;
using System;
using System.Collections.Generic;
using World.Dungeon.Generators;

namespace World.Dungeon
{

    public class DungeonGenerator
    {
       
        public WorldMapCont Controller{ get; protected set; }

        public DungeonGenerator(in WorldMapCont cont){
            this.Controller = cont;
        }

        /*
        public Tuple<List<Vector2>, List<Vector2>> FloorAndWalls(){
            SimpleGenerator s = new SimpleGenerator();
            List<Vector2> walls = s.GetWalls();
            List<Vector2> floor = s.GetFloors();

            return new Tuple<List<Vector2>, List<Vector2>>(floor, walls);

        }*/

        //OJU!!!
        //this is a hack.
        //At some point we're going to diferentiate the tiles_logic and the tile_visual with types and so on,
        //so, bear with it just a little.
        public (List<Vector2> floor, List<Vector2> walls, List<Vector2> corridors) FloorAndWalls(){
            SimpleGenerator s = new SimpleGenerator();
            List<Vector2> walls = s.GetWalls();
            List<Vector2> floor = s.GetFloors();
            List<Vector2> corridors = s.GetCorridors();
            return (floor, walls, corridors);
        }

     



    }
}
