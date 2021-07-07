using Base;
using Godot;
using System;

namespace World.Dungeon.Generators
{
    public struct RoomGrid 
    {
        public MyPoint GridSize;

        public MyPoint CellSize;

        public bool[,] HasRoom;

        public RoomGrid(in MyPoint gridSize, in MyPoint cellSize){
            this.GridSize = gridSize;
            this.CellSize = cellSize;
            this.HasRoom = new bool[gridSize.X, gridSize.Y];
        }

        public bool HasRoomAt(in int x, in int y){
            if(x < 0 || x >= this.GridSize.X || y < 0 || y >= this.GridSize.Y){
                return false;
            }
            return HasRoom[x, y];
        }

        public void SetRoomAt(in int x, in int y){
            if(x < 0 || x >= this.GridSize.X || y < 0 || y >= this.GridSize.Y){
                return;
            }
            this.HasRoom[x, y] = true;
        }

        public void RemoveRoomAt(in int x, in int y){
            if(x < 0 || x >= this.GridSize.X || y < 0 || y >= this.GridSize.Y){
                return;
            }
            this.HasRoom[x, y] = false;
        }
    }
}
