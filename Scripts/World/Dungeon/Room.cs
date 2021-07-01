using Godot;
using System;
using System.Collections.Generic;

namespace World.Dungeon
{
    public class Room
    {
        public int TopLeftX { get; set; }
        public int TopLeftY { get; set; }

        public int BottomRightX { get; set; }
        public int BottomRightY { get; set; }

        public int Width { get => Mathf.Abs(BottomRightX - TopLeftX); }
        public int Heigth { get => Mathf.Abs(BottomRightY - TopLeftY); }

        public int CenterX { get => (int)Width / 2; }
        public int CenterY { get => (int)Heigth / 2; }

        public Room(in int topX, in int topY, in int bottomX, in int bottomY)
        {
            this.TopLeftX = topX;
            this.TopLeftY = topY;
            this.BottomRightX = bottomX;
            this.BottomRightY = bottomY;
        }

        //esto puede estar en el propio sistema de generación.
        //es más, la room puede ser sólo una struct y ale
        //pero lo dejamos aquí para debuggear ahora
        public List<Vector2> GetFloorPositions()
        {
            List<Vector2> floors = new List<Vector2>();

            for (int x = TopLeftX + 1; x < BottomRightX; x++)
            {
                for (int y = TopLeftY + 1; y < BottomRightY; y++)
                {
                    floors.Add(new Vector2(x, y));
                }
            }

            return floors;
        }

        public List<Vector2> GetWallsPositions(){
            List<Vector2> walls = new List<Vector2>();

            for (int y = TopLeftY; y <= BottomRightY; y++){
                if(y == TopLeftY || y == BottomRightY){
                    for (int x = TopLeftX; x <= BottomRightX; x++){
                        walls.Add(new Vector2(x, y));
                    }                   
                }else{
                    walls.Add(new Vector2(TopLeftX, y));
                    walls.Add(new Vector2(BottomRightX, y));
                }
            }
                return walls;
        }

    }
}
