using Godot;
using System;
using System.Collections.Generic;

namespace World.Dungeon
{
    public struct Room
    {
        //tuples, de momento usamos esto para no tener que implementar una estructura custom
        //que, a la larga, será lo suyo
        public (int X, int Y) TopLeft;
        public (int X, int Y) BottomRight;      

        public int Width { get => Mathf.Abs(BottomRight.X - TopLeft.X); }
        public int Heigth { get => Mathf.Abs(BottomRight.Y - TopLeft.Y); }

        public int CenterX { get => (int)Width / 2 + TopLeft.X; }
        public int CenterY { get => (int)Heigth / 2 + TopLeft.Y; }

        public Room(in int topX, in int topY, in int bottomX, in int bottomY)
        {
            this.TopLeft.X = topX;
            this.TopLeft.Y = topY;
            this.BottomRight.X = bottomX;
            this.BottomRight.Y = bottomY;
        }

        //esto puede estar en el propio sistema de generación.
        //es más, la room puede ser sólo una struct y ale
        //pero lo dejamos aquí para debuggear ahora
        public List<Vector2> GetFloorPositions()
        {
            List<Vector2> floors = new List<Vector2>();

            for (int x = TopLeft.X + 1; x < BottomRight.X; x++)
            {
                for (int y = TopLeft.Y + 1; y < BottomRight.Y; y++)
                {
                    floors.Add(new Vector2(x, y));
                }
            }

            return floors;
        }

        public List<Vector2> GetWallsPositions()
        {
            List<Vector2> walls = new List<Vector2>();

            for (int y = TopLeft.Y; y <= BottomRight.Y; y++)
            {
                if (y == TopLeft.Y || y == BottomRight.Y)
                {
                    for (int x = TopLeft.X; x <= BottomRight.X; x++)
                    {
                        walls.Add(new Vector2(x, y));
                    }
                }
                else
                {
                    walls.Add(new Vector2(TopLeft.X, y));
                    walls.Add(new Vector2(BottomRight.X, y));
                }
            }
            return walls;
        }

    }
}
