using Godot;
using System;


namespace World{

    public class WorldMap{
        public readonly int WIDTH;
        public readonly int HEIGHT;

        public Tile[,] Tiles{ get; protected set; }


        public WorldMap(in int width, in int height){
            this.WIDTH = width;
            this.HEIGHT = height;

            this.Tiles = new Tile[WIDTH, HEIGHT];

            this.PopulateMap();
        }

        private void PopulateMap(){
            Tiles[30, 22] = new Tile(true, true);
            Tiles[31, 22] = new Tile(true, true);
            Tiles[32, 22] = new Tile(true, true);

            //right wall
            Tiles[10, 05] = new Tile(true, true);
            Tiles[10, 06] = new Tile(true, true);
            Tiles[10, 07] = new Tile(true, true);
            Tiles[10, 08] = new Tile(true, true);
            Tiles[10, 09] = new Tile(true, true);
            Tiles[10, 10] = new Tile(true, true);

            //left wall
            Tiles[04, 05] = new Tile(true, true);
            Tiles[04, 06] = new Tile(true, true);
            Tiles[04, 07] = new Tile(true, true);
            Tiles[04, 08] = new Tile(true, true);
            Tiles[04, 09] = new Tile(true, true);
            Tiles[04, 10] = new Tile(true, true);

            //top
            Tiles[05, 05] = new Tile(true, true);
            Tiles[06, 05] = new Tile(true, true);
            Tiles[07, 05] = new Tile(true, true);
            Tiles[08, 05] = new Tile(true, true);
            Tiles[09, 05] = new Tile(true, true);

            //down
            Tiles[05, 10] = new Tile(true, true);
            Tiles[06, 10] = new Tile(true, true);
            Tiles[08, 10] = new Tile(true, true);
            Tiles[09, 10] = new Tile(true, true);


        }        
    }


}