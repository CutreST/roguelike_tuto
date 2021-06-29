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

        }
    }


}