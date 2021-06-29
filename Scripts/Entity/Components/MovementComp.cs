using Godot;
using System;

namespace Entities.Components
{

    public class MovementComp : Node2D, IComponentNode
    {
        public Entity MyEntity { get; set; }

        //TODO:
        //Esto lo cambiaremos
        [Export]
        public readonly int TILE_HEIGHT;

        [Export]
        public readonly int TILE_WIDTH;

        public void Move(in Vector2 direction){
            this.GlobalPosition += new Vector2(direction.x * TILE_WIDTH, direction.y * TILE_HEIGHT);
        }


        public void OnAwake()
        {
            throw new NotImplementedException();
        }

        public void OnSetFree()
        {
            throw new NotImplementedException();
        }

        public void Ontart()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
