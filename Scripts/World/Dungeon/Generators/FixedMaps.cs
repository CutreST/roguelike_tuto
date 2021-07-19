using Godot;
using System;


namespace World.Dungeon.Generators
{

    public class FixedMaps : BaseGenerator
    {
        Tile?[,] _tiles;
        public override ref Tile?[,] GetTiles(out Vector2 pos)
        {
            pos = new Vector2(2, 2);
            string[,] map = {{"##############"},
                             {"#............#"},
                             {"#.......######"},
                             {"#########xxxxx"}
                            };
            _tiles = StringMapToTile(map);
            return ref _tiles;
        }

        private Tile?[,] StringMapToTile(in string[,] map)
        {
            Messages.Print("size ", map[0, 0].Length.ToString());
            Tile?[,] t = new Tile?[map[0,0].Length, map.GetLength(1)];
            Tile? temp;
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    for (int i = 0; i < map[x, y].Length; i++)
                    {
                        switch (map[x,y][i])
                        {
                            case '#':
                                temp = new Tile(Tile.TileType.WALL);
                                break;
                            case '.':
                                temp = new Tile(Tile.TileType.FLOOR);
                                break;
                            default:
                                temp = null;
                                break;
                        }
                        t[i, y] = temp;
                    }
                }
            }

            return t;
        }
    }
}
