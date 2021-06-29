using Godot;
using System;

namespace World
{

    public class Tile
    {

        public bool IsBlocked{ get; protected set; }
        public bool IsSightBloked{ get; protected set; }

        public Tile(in bool isBlocked, in bool IsSightBloked){
            this.IsBlocked = isBlocked;
            this.IsSightBloked = IsSightBloked;
        }

        public Tile(in bool isBlocked){
            this.IsBlocked = isBlocked;
            //por defecto, 
            this.IsSightBloked = this.IsBlocked;
        }

    }

}