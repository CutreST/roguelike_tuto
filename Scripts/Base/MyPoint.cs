using Godot;
using System;

namespace Base
{
    public struct MyPoint
    {
        public int X;
        public int Y;

        public MyPoint(in int x, in int y){
            this.X = x;
            this.Y = y;
        }       

        public override string ToString(){
            return String.Format("({0},{1})", X, Y);
        }

    }

}