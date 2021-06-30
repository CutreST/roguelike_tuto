using Godot;
using MySystems;
using System;

namespace World
{

    public class WorldMapCont : Node
    {
        //pillamos el tilemap para poder dibujar, lo hará esto 
        //TODO: OJU! esto podría ser un sistema y ale.
        private TileMap _tilemap;

        public WorldMap MyWorld { get; set; }

        #region Map settings
        [Export]
        private readonly int WIDTH;
        [Export]
        private readonly int HEIGHT;

        [Export]
        private readonly string WALL_NAME;


        #endregion


        public override void _EnterTree()
        {
            base._EnterTree();
            this.MyWorld = new WorldMap(WIDTH, HEIGHT);

            //pinta el gamemap
            this.RunTileMap();

            //metemos esto en el gamesys
            InGameSys sys;

            System_Manager.GetInstance(this).TryGetSystem<InGameSys>(out sys, true);
            sys.MyWorldCont = this;

            //pillamos movimiento y metemos esto. 
            //OJU! tenemos un problemilla con la mierda esta, pero bueno
            MovementSystem movementSystem;
            System_Manager.GetInstance(this).TryGetSystem<MovementSystem>(out movementSystem, true);
            movementSystem.MyWorld = this;
        }


        public bool IsTileBlocked(in int posX, in int posY){
            Vector2 tilePos = WorldToTilePos(new Vector2(posX, posY));

            if(tilePos.x < 0 || tilePos.x >= WIDTH || tilePos.y < 0 || tilePos.y >= HEIGHT){
                Messages.Print(base.Name, "The tile at pos (" + posX + ", " + posY + ") is out of bonds");
                return true;
            }

            if(MyWorld.Tiles[(int)tilePos.x, (int)tilePos.y] == null){
                return false;
            }

            
            return MyWorld.Tiles[(int)tilePos.x, (int)tilePos.y].IsBlocked;

        }

        public Vector2 WorldToTilePos(Vector2 pos){
            pos /= _tilemap.CellSize;
            pos.x = (int)pos.x;
            pos.y = (int)pos.y;

            return pos;
        }
        private void RunTileMap()
        {
            //primero pillamos tilemap
            _tilemap = this.TryGetFromChild_Rec<TileMap>();

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
                    if (MyWorld.Tiles[x, y] != null && MyWorld.Tiles[x, y].IsBlocked)
                    {
                        _tilemap.SetCell(x, y, a);
                    }
                }
            }



        }

    }
}
