using Godot;
using MySystems;
using System;

namespace Entities.Components
{
    /// <summary>
    /// Component responsible for the <see cref="Entity"/>'s movement
    /// </summary>

    public class MovementComp : Node, IComponentNode
    {
        public Entity MyEntity { get; set; }

        /// <summary>
        /// The direction of the movement
        /// </summary>
        public Vector2 Direction{ get; protected set; }

        /// <summary>
        /// The <see cref="MovementSystem"/>
        /// </summary>
        private MovementSystem _movSys;

        private MovEvents_PL _movEv;

        //TODO:
        //Create a class, struct or wathever to put the dimensions and so.
        [Export]
        public readonly int TILE_HEIGHT;

        [Export]
        public readonly int TILE_WIDTH;

        /// <summary>
        /// Moves the <see cref="Entity"/> calling <see cref="MovementSystem.Move(in MovementComp)"/>
        /// </summary>
        /// <param name="direction">Direction of the movement</param>
        public void Move(in Vector2 direction){
            Direction = direction;
            _movSys.Move(this);

            if(_movEv != null){
                _movEv.OnMove(MyEntity.GlobalPosition);
            }
        }

        #region Godot methods
        public override void _EnterTree()
        {
            base._EnterTree();
            this.OnAwake();
        }

        public override void _Ready()
        {
            base._Ready();
            this.Ontart();
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
            bool a = MyEntity.TryGetIComponentNode<MovEvents_PL>(out _movEv);
            Messages.Print(base.Name + " got the events?", a.ToString());
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
