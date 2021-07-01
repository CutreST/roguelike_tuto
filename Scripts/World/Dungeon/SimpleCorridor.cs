using Godot;
using System;

namespace World.Dungeon
{

    public struct SimpleCorridor
    {
        public Vector2 Start;
        public Vector2 End;
        public Vector2 Corner;

        public SimpleCorridor(in Vector2 start, in Vector2 end, in Vector2 corner){
            this.Start = start;
            this.End = end;
            this.Corner = corner;
        }

    }

}