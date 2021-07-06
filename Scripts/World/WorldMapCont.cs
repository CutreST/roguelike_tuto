using Godot;
using MySystems;
using System;
using System.Collections.Generic;
using World.Dungeon;

namespace World
{
    /// <summary>
    /// Controller for the map. 
    /// </summary>
    /// <remarks>
    /// So, for each level the thing that changes is the <see cref="MyWorld"/>. So, the controller is persistent
    /// trought the gameplay. 
    /// </remarks>

    public class WorldMapCont : Node
    {
        //pillamos el tilemap para poder dibujar, lo hará esto 
        //TODO: OJU! esto podría ser un sistema y ale.
        /// <summary>
        /// The current tilemap
        /// </summary>
        /// <remarks>
        /// OJU! Work In Progress.
        /// Have to thing about the map creation.
        /// </remarks>
        private TileMap _tilemap;

        /// <summary>
        /// The world map.
        /// </summary>
        public WorldMap MyWorld { get; set; }

        private DungeonGenerator _generator;

        #region Map settings

        //OJU
        //This hardcoded settings are only for test. 
        //Have to remove when creating diferents maps
        [Export]
        private readonly int WIDTH;
        [Export]
        private readonly int HEIGHT;

        [Export]
        private readonly string WALL_NAME;

        [Export]
        private readonly string FLOOR_NAME;

        [Export]
        private readonly string CORRIDOR_NAME;

        [Export]
        private readonly string DOOR_NAME;

        [Export]
        private readonly string PLAYER_NAME;

        #endregion

        private Entities.Entity _player;

        #region Godot Methods
        public override void _EnterTree()
        {
            base._EnterTree();
            //create a new world,
            //OJU! change if changes the map
            this.MyWorld = new WorldMap(WIDTH, HEIGHT);

            //pains the tilemap
            //this.RunTileMap();
            _tilemap = this.TryGetFromChild_Rec<TileMap>();

            //put the controller to the gamesys
            InGameSys sys;

            System_Manager.GetInstance(this).TryGetSystem<InGameSys>(out sys, true);
            sys.MyWorldCont = this;

            //ok, so the controller is responsible of calling the movement system 
            //and set itself as a reference.
            //The singleton only works with nodes (i can't get the root node without a node), so, it makes sense
            //i think.
            MovementSystem movementSystem;
            System_Manager.GetInstance(this).TryGetSystem<MovementSystem>(out movementSystem, true);
            movementSystem.MyWorld = this;

            //metemos generator para testear
            _generator = new DungeonGenerator(this);

            //buscamos al player
            _player = base.GetTree().Root.TryGetFromChild_Rec<Entities.Entity>(PLAYER_NAME);


            //rellenamos el diccionario
            this.SetMap();
        }

       

        #endregion
        const string SPACE_ACTION = "ui_select";
        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);
            if(Input.IsActionJustPressed(SPACE_ACTION)){

                //clear
                _tilemap.Clear();
                MyWorld.ClearMap();

                this.SetMap();
            }
        }

        private void SetMap()
        {
            Vector2 pos;

            this.MyWorld = new WorldMap(this._generator.GetTiles(out pos));
            this.PaintMap();


            _player.GlobalPosition = new Vector2(pos.x * 24 + 12, pos.y * 24 + 12);

            //ahora pintamos
            //this.PaintWalls(walls);
            //this.PaintFloor(floor);

            /*this.PaintTiles(walls, WALL_NAME);
            this.PaintTiles(floor, FLOOR_NAME);
            this.PaintTiles(corridor, CORRIDOR_NAME);*/
        }

        private void PaintMap(){
            Tile? currentTile;
            int id;
            for (int x = 0; x < MyWorld.WIDTH; x++){
                for (int y = 0; y < MyWorld.HEIGHT; y++){
                    currentTile = MyWorld.Tiles[x, y];
                    if(currentTile.HasValue){
                        switch(MyWorld.Tiles[x,y].Value.MyType){
                            case Tile.TileType.WALL:
                                id = 1;
                                break;

                            case Tile.TileType.FLOOR:
                                id = 2;
                                break;

                            case Tile.TileType.DOOR:
                                id = 4;
                                break;

                            default:
                                id = 3;
                                break;
                        }

                        _tilemap.SetCell(x, y, id);
                    }
                    
                }
            }
        }

        private void CreateCollisionTiles(in List<Vector2> walls)
        {
            for (int i = 0; i < walls.Count; i++)
            {
                MyWorld.Tiles[(int)walls[i].x, (int)walls[i].y] = new Tile(true);                
            }
        }

        private void CreateFloorTiles(in List<Vector2> floor)
        {
            for (int i = 0; i < floor.Count; i++)
            {
                MyWorld.Tiles[(int)floor[i].x, (int)floor[i].y] = new Tile(false);
            }
        }

        private void PaintTiles(in List<Vector2> tiles, in string tileName){
            int id = _tilemap.TileSet.FindTileByName(tileName);

            for (int i = 0; i < tiles.Count; i++){
                _tilemap.SetCell((int)tiles[i].x, (int)tiles[i].y, id);
            }
        }
        [Obsolete]
        private void PaintWalls(in List<Vector2> walls)
        {
            int id = _tilemap.TileSet.FindTileByName(WALL_NAME);
            for (int i = 0; i < walls.Count; i++)
            {
                _tilemap.SetCell((int)walls[i].x, (int)walls[i].y, id);
            }

        }

        [Obsolete]
        private void PaintFloor(in List<Vector2> floor)
        {
            int id = _tilemap.TileSet.FindTileByName(FLOOR_NAME);
            for (int i = 0; i < floor.Count; i++)
            {
                _tilemap.SetCell((int)floor[i].x, (int)floor[i].y, id);
            }
        }

        /// <summary>
        /// Is the <see cref="Tile"/> blocked at position?
        /// </summary>
        /// <param name="posX">Position X</param>
        /// <param name="posY">Position Y</param>
        /// <returns></returns>
        public bool IsTileBlocked(in int posX, in int posY)
        {
            Vector2 tilePos = WorldToTilePos(new Vector2(posX, posY));            

            //out of bonds
            if (tilePos.x < 0 || tilePos.x >= WIDTH || tilePos.y < 0 || tilePos.y >= HEIGHT )
            {
                Messages.Print(base.Name, "The tile at pos (" + posX + ", " + posY + ") is out of bonds");
                return true;
            }

            //right now null = INtransitable, now there's a floor
            if (MyWorld.Tiles[(int)tilePos.x, (int)tilePos.y] == null)
            {
                return true;
            }

            //returns current tile is blocked
            return MyWorld.Tiles[(int)tilePos.x, (int)tilePos.y].Value.IsBlocked;

        }

        /// <summary>
        /// Pass a world position to a tile position
        /// </summary>
        /// <param name="pos">World position</param>
        /// <returns>Tile position</returns>
        public Vector2 WorldToTilePos(Vector2 pos)
        {
            pos /= _tilemap.CellSize;
            pos.x = (int)pos.x;
            pos.y = (int)pos.y;

            return pos;
        }

        /// <summary>
        /// Paints the tile map        
        /// </summary>
        /// <remarks>
        /// TODO:
        /// Change name and autotile.
        ///</remarks>
        private void RunTileMap()
        {
            //primero pillamos tilemap


            //si no existe, nos vamos   
            if (_tilemap == null)
            {
                Messages.Print(base.Name, "Tilemap not found on children.");
                return;
            }

            TileSet set = _tilemap.TileSet;

            int a = set.FindTileByName(WALL_NAME);

            for (int x = 0; x < MyWorld.Tiles.GetLength(0); x++)
            {
                for (int y = 0; y < MyWorld.Tiles.GetLength(1); y++)
                {
                    if (MyWorld.Tiles[x, y] != null && MyWorld.Tiles[x, y].Value.IsBlocked)
                    {
                        _tilemap.SetCell(x, y, a);
                    }
                }
            }



        }

    }
}
