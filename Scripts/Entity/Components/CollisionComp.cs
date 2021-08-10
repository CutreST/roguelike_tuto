using Godot;
using MySystems;
using System;
using World;

namespace Entities.Components
{
    /// <summary>
    /// Compontent responsible for the collisions
    /// </summary>
    /// <remarks>
    /// This component can be a <see cref="Node2D"/> and, the <see cref="Position"/> instead of <see cref="Entity.GlobalPosition"/>
    /// could be <see cref="Node2D.GlobalPosition"/>.
    /// I prefer this approach.
    /// </remakrs>
    public class CollisionComp : Node, IComponentNode
    {
        public Entity MyEntity { get; set; }
        public Vector2 Position { get => MyEntity.GlobalPosition; }

        /// <summary>
        /// temp method for handling the collision.
        /// </summary>
        /// <param name="other"></param>
        public void CollisionResponse(in CollisionComp other)
        {
            Messages.Print(other.MyEntity.Name + " has collided with " + this.MyEntity.Name);
        }

        public void CollisionEmiter(in CollisionComp other)
        {
            //ok, cuando emtimos una collisión atacaremos, será más sencillo así.
            //TODO: hacerlo variable
            //pillamos nuestro attack

           /* AttackComp myAttack;
            if (this.MyEntity.TryGetIComponentNode<AttackComp>(out myAttack) == false)
            {
                return;
            }

            AttackComp otherAttack;
            if (other.MyEntity.TryGetIComponentNode<AttackComp>(out otherAttack) == false)
            {
                return;
            }
            
            otherAttack.ReceiveAttack(myAttack);*/
        }


        #region Godot methods
        public override void _EnterTree()
        {
            this.OnAwake();
        }

        public override void _ExitTree()
        {
            this.OnSetFree();
        }
        #endregion

        #region icomponent ethods.
        public void OnAwake()
        {
            //put the coll comp on the list
            CollisionSystem collisionSystem;
            if (System_Manager.GetInstance(this).TryGetSystem<CollisionSystem>(out collisionSystem, true) == false)
            {
                Messages.AddSystemFailed(collisionSystem, MyEntity.Name);
            }

            if (collisionSystem.TryAddToCollision(this) == false)
            {
                Messages.Print(MyEntity.Name, "The collision component couldn't enter the list", Messages.MessageType.ERROR);
            }
        }

        public void OnSetFree()
        {
            //sacar de collision
            CollisionSystem system;

            if (System_Manager.GetInstance(this).TryGetSystem<CollisionSystem>(out system))
            {
                system.RemoveFromCollision(this);
            }
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
