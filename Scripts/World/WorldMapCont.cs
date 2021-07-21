using Base;
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

        private RenderSystem _renderSys;

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


            //pains the tilemap
            //this.RunTileMap();
            

            //put the controller to the gamesys
            System_Manager manager = System_Manager.GetInstance(this);
            InGameSys sys;

            manager.TryGetSystem<InGameSys>(out sys, true);
            sys.MyWorldCont = this;

            //ok, so the controller is responsible of calling the movement system 
            //and set itself as a reference.
            //The singleton only works with nodes (i can't get the root node without a node), so, it makes sense
            //i think.
            MovementSystem movementSystem;
            manager.TryGetSystem<MovementSystem>(out movementSystem, true);
            movementSystem.MyWorld = this;

            //metemos generator para testear
            _generator = new DungeonGenerator(this);

            //buscamos al player
            _player = base.GetTree().Root.TryGetFromChild_Rec<Entities.Entity>(PLAYER_NAME);

            //metemos el sistema render, oju, cambiar esto un pcoo
            _tilemap = this.TryGetFromChild_Rec<TileMap>();
            manager.TryGetSystem<RenderSystem>(out _renderSys, true);
            _renderSys.Tilemap = _tilemap;
            _renderSys.Cont = this;

            //rellenamos el diccionario
            this.SetMap();
        }


        #endregion
        const string SPACE_ACTION = "ui_select";
        /// <summary>
        /// input for creating a new map.
        /// </summary>
        /// <param name="delta"></param>
        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);
            if (Input.IsActionJustPressed(SPACE_ACTION))
            {
                //clear
                _tilemap.Clear();
                MyWorld.ClearMap();

                this.SetMap();
            }
        }
        public void NewTurn(in Vector2 pos)
        {
            //print fow and so
            _renderSys.PaintFOV(this.WorldToTilePos(pos));
           
        }

        private void SetMap()
        {
            Vector2 pos;
            Dictionary<MyPoint, byte> enemies;
            Tile?[,] tiles;
            (tiles, pos, enemies) = this._generator.GetWholePack();
            this.MyWorld = new WorldMap(tiles);



            //this.MyWorld = new WorldMap(this._generator.GetTiles(out pos));
            //this.PaintMap();

            //TODO: change this in it's file
            _player.GlobalPosition = new Vector2(pos.x * 24 + 12, pos.y * 24 + 12);
            _renderSys.StartMap(this.MyWorld.WIDTH, this.MyWorld.HEIGHT);
        }     

        /// <summary>
        /// Is the <see cref="Tile"/> blocked at position?
        /// </summary>
        /// <param name="posX">Position X</param>
        /// <param name="posY">Position Y</param>
        /// <returns></returns>
        public bool IsTileBlocked(in int posX, in int posY, in bool globalPosition)
        {
            Tile? temp;

            if (this.GetTileAt(out temp, posX, posY, globalPosition))
            {
                return temp.Value.IsBlocked;
            }

            //if null, the player dosn't move
            return true;
        }

        public bool IsTileNoSee(in int posX, in int posY, in bool globalPosition)
        {
            Tile? temp;

            if (this.GetTileAt(out temp, posX, posY, globalPosition))
            {
                return temp.Value.IsSightBloked;
            }

            //if null, the player dosn't see
            return false;
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

        public bool GetTileAt(out Tile? tile, in int posX, in int posY, in bool isGlobalPos)
        {

            Vector2 tilePos;

            if (isGlobalPos)
            {
                tilePos = WorldToTilePos(new Vector2(posX, posY));
            }
            else
            {
                tilePos = new Vector2(posX, posY);
            }

            //out of bonds
            if (tilePos.x < 0 || tilePos.x >= MyWorld.WIDTH || tilePos.y < 0 || tilePos.y >= MyWorld.HEIGHT)
            {
                //Messages.Print(base.Name, "The tile at pos (" + posX + ", " + posY + ") is out of bonds");
                tile = null;
                return false;
            }

            tile = MyWorld.Tiles[(int)tilePos.x, (int)tilePos.y];

            if (tile != null)
            {
                return true;
            }

            return false;

        }

        public Tile.TileType GetTileType(in int posX, in int posY, in bool isGlobalPos){
            MyPoint tilePos;

            if (isGlobalPos)
            {
                tilePos = (MyPoint)WorldToTilePos(new Vector2(posX, posY));
            }
            else
            {
                tilePos = new MyPoint(posX, posY);
            }

            //out of bonds
            if (tilePos.X < 0 || tilePos.X >= MyWorld.WIDTH || tilePos.Y < 0 || tilePos.Y >= MyWorld.HEIGHT)
            {
                //Messages.Print(base.Name, "The tile at pos (" + posX + ", " + posY + ") is out of bonds");

                return Tile.TileType.NULL;
            }

            Tile? t;
            if(this.GetTileAt(out t, tilePos.X, tilePos.Y, false)){
                return t.Value.MyType;
            }

            return Tile.TileType.NULL;
        }


        public void OpenDoor(in int posX, in int posY, in bool isGlobalPos, in Tile.TileType newType)
        {
            Vector2 pos;

            if (isGlobalPos)
            {
                pos = this.WorldToTilePos(new Vector2(posX, posY));
            }
            else
            {
                pos = new Vector2(posX, posY);
            }

            //de momento lo cambiamos a suelo
            MyWorld.Tiles[(int)pos.x, (int)pos.y] = new Tile(newType);
            //joder hay que cambiar la pintura            
            //this.PaintCell((int)pos.x, (int)pos.y, newType);
            _renderSys.PaintCell((int)pos.x, (int)pos.y);
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
