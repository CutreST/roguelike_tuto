using Godot;
using System;

namespace MySystems.MyInput
{

    public abstract class InputBase 
    {
        public Visual_SystemBase ParentSystem { get; protected set; }

        public InputBase(in Visual_SystemBase visual){
            this.ParentSystem = visual;
        }

        public abstract void GetInput();

    }
}
