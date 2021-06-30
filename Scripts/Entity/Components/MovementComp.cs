using Godot;
using MySystems;
using System;

namespace Entities.Components
{

    public class MovementComp : Node, IComponentNode
    {
        public Entity MyEntity { get; set; }

        public Vector2 Direction{ get; protected set; }

        private MovementSystem _movSys;
        //TODO:
        //Esto lo cambiaremos
        [Export]
        public readonly int TILE_HEIGHT;

        [Export]
        public readonly int TILE_WIDTH;

        public void Move(in Vector2 direction){
            Direction = direction;
            _movSys.Move(this);
            //this.GlobalPosition += new Vector2(direction.x * TILE_WIDTH, direction.y * TILE_HEIGHT);
        }

        #region Godot methods
        public override void _EnterTree()
        {
            base._EnterTree();
            this.OnAwake();
        }
        #endregion

        #region Node comp methods
        public void OnAwake()
        {
            System_Manager.GetInstance(this).TryGetSystem<MovementSystem>(out _movSys, true);
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
        #endregion
    }
}
