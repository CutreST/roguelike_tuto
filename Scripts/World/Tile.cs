using Godot;
using System;

namespace World
{
    //Creo que esto es candidato a struct
    public struct Tile
    {
        public enum TileType : byte { WALL, FLOOR, DOOR }
        public TileType MyType;
        public bool IsBlocked;
        public bool IsSightBloked;

        public Tile(in TileType tileType, in bool defaultSet){
            MyType = TileType.FLOOR;
            IsBlocked = false;
            IsSightBloked = false;
            this.SetTileType(tileType, defaultSet);
        }

        public Tile(in bool isBlocked, in bool IsSightBloked){
            this.IsBlocked = isBlocked;
            this.IsSightBloked = IsSightBloked;
            MyType = TileType.FLOOR;
        }

        public Tile(in bool isBlocked){
            this.IsBlocked = isBlocked;
            //por defecto, 
            this.IsSightBloked = this.IsBlocked;
            MyType = TileType.FLOOR;
        }

        public void SetTileType(in TileType tileType, in bool defaultSet = true){
            this.MyType = tileType;

            if(defaultSet){
                switch(MyType){
                    case TileType.FLOOR:
                        IsBlocked = false;
                        IsSightBloked = false;
                        break;

                    case TileType.WALL:
                        IsBlocked = true;
                        IsSightBloked = true;
                        break;

                    case TileType.DOOR:
                        IsBlocked = true;
                        IsSightBloked = true;
                        break;
                }
            }
        }

    }

}