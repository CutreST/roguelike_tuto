using Godot;
using System;

/// <summary>
/// Namespace for all the world related
/// </summary>
namespace World{
    
    /// <summary>
    /// The worldmap. It has a 2d array of <see cref="Tile"/>
    /// </summary>
    public struct WorldMap{

        /// <summary>
        /// The witdh of the world
        /// </summary>
        public readonly int WIDTH;

        /// <summary>
        /// Height of the world
        /// </summary>
        public readonly int HEIGHT;

        /// <summary>
        /// 2d array of <see cref="Tile"/> that compose the map
        /// </summary>
        public Tile?[,] Tiles;

        /// <summary>
        /// Constructor.
        /// <para>
        /// OJU! It populates the map with some default settings
        /// </para>
        /// </summary>
        /// <param name="width">Width of the map</param>
        /// <param name="height">Height of the map</param>
        public WorldMap(in int width, in int height){
            this.WIDTH = width;
            this.HEIGHT = height;

            this.Tiles = new Tile?[WIDTH, HEIGHT];

            //this.PopulateMap();
        }

        public WorldMap(in Tile?[,] tiles){
            this.Tiles = tiles;
            this.WIDTH = tiles.GetLength(0);
            this.HEIGHT = tiles.GetLength(1);
        }

        public void ClearMap(){
            this.Tiles = new Tile?[WIDTH, HEIGHT];
        }        

        /// <summary>
        /// Populates the map with some default options
        /// </summary>
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