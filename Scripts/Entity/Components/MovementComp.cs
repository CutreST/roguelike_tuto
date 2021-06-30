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
