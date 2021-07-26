using Godot;
using System;

namespace Entities.Components
{

    public class AttackComp : Node, IComponentNode
    {
        public Entity MyEntity { get; set; }

        #region Export properties
        [Export]
        public int Health{ get; set; }
        [Export]
        public int Attack{ get; set; }
        [Export]
        public int Deffense{ get; set; }
        #endregion


        public void ReceiveAttack(in AttackComp other){
            int drop = other.Attack - this.Deffense;

            if(drop > 0){
                Health -= drop;
                Messages.Print(this.MyEntity.Name + " health", this.Health.ToString());
                //event

                if(Health <= 0){
                    this.MyEntity.QueueFree();
                    Messages.Print(this.MyEntity.Name, "Goodbye my friend");
                }
            }
        }


        #region Component methdos
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
        #endregion
    }
}
