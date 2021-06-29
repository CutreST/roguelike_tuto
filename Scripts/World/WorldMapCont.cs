using Godot;
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
            this.RunTileMap();
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
