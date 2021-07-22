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

        public static explicit operator MyPoint(in Vector2 vect) => new MyPoint((int)vect.x, (int)vect.y);

        public static explicit operator Vector2(in MyPoint point) => new Vector2(point.X, point.Y);

        public static MyPoint operator +(in MyPoint a, in MyPoint other) => new MyPoint(a.X + other.X, a.Y + other.Y);

        public static MyPoint operator /(in MyPoint a, in MyPoint other) => new MyPoint(a.X / other.X, a.Y / other.Y);

        public static bool operator ==(in MyPoint a, in MyPoint other) => (a.X == other.X && a.Y == other.Y);

        public static bool operator !=(in MyPoint a, in MyPoint other) => (a.X != other.X || a.Y != other.Y);
    }

}