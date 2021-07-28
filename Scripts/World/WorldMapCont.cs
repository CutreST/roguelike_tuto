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
        private RenderSystem _renderSys;

        private SpawnSystem _spawnSys;

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
        private readonly string PLAYER_NAME;

        [Export]
        private readonly string MAP_VISIBLE_NAME;

        [Export]
        private readonly string MAP_SHADOW_NAME;

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

            //lo metemos en el stack aquí
            manager.AddToStack(sys);

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
            TileMap tilemap = this.TryGetFromChild_Rec<TileMap>(MAP_VISIBLE_NAME);
            manager.TryGetSystem<RenderSystem>(out _renderSys, true);
            _renderSys.VisibleMap = tilemap;
            _renderSys.Cont = this;

            tilemap = this.TryGetFromChild_Rec<TileMap>(MAP_SHADOW_NAME);
            _renderSys.ShadowMap = tilemap;

            //metemos el spawneador
            manager.TryGetSystem<SpawnSystem>(out _spawnSys, true);
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
            Dictionary<MyPoint, EnitityType> enemies;
            Tile?[,] tiles;
            (tiles, pos, enemies) = this._generator.GetWholePack();
            this.MyWorld = new WorldMap(tiles);

            //TODO: change this in it's file
            _player.GlobalPosition = new Vector2(pos.x * 24 + 12, pos.y * 24 + 12);
            List<Sprite> renderable = new List<Sprite>();

            Vector2 globPos;
            Sprite sprite;

            Entities.Entity entity;
            foreach (MyPoint entPos in enemies.Keys)
            {
                globPos = TileToWorldPos((Vector2)entPos, true);
                if (_spawnSys.TrySpawnEntity(enemies[entPos], globPos, this, out entity) == false)
                {
                    Messages.Print(base.Name, "ipossible to spwan enemy " + enemies[entPos].ToString());
                }

                sprite = entity.TryGetFromChild_Rec<Sprite>();
                renderable.Add(sprite);
            }

            _renderSys.StartMap(this.MyWorld.WIDTH, this.MyWorld.HEIGHT, renderable);

            //spawneamos

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
            pos /= _renderSys.ShadowMap.CellSize;
            pos.x = (int)pos.x;
            pos.y = (int)pos.y;

            return pos;
        }

        public Vector2 TileToWorldPos(Vector2 pos, bool addCenterOffset)
        {
            pos = pos * _renderSys.ShadowMap.CellSize;

            if (addCenterOffset)
            {
                pos.x += (int)_renderSys.ShadowMap.CellSize.x / 2;
                pos.y += (int)_renderSys.ShadowMap.CellSize.y / 2;
            }

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

        public Tile.TileType GetTileType(in int posX, in int posY, in bool isGlobalPos)
        {
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
            if (this.GetTileAt(out t, tilePos.X, tilePos.Y, false))
            {
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
    }
}
