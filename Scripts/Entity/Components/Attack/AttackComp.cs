using Godot;
using System;

namespace Entities.Components
{

    public class AttackComp : Node, IComponentNode
    {
        public Entity MyEntity { get; set; }

        private int _health;

        private int _maxHealth;

        private int _attack;
        private int _deffense;

        private AttackHandler_Full _attackEvents;

        #region Export properties

        [Export]
        public int MaxHealth
        {
            get => _maxHealth;
            set
            {
                _maxHealth = value;
                if (_attackEvents != null)
                    _attackEvents.OnModifyMaxHealth(this.MyEntity);
            }
        }

        [Export]
        public int Health
        {
            get => _health;
            set
            {
                _health = value;
                if (_attackEvents != null)
                    _attackEvents.OnModifyHealth(this.MyEntity);
            }
        }
        [Export]
        public int Attack
        {
            get => _attack;
            set
            {
                _attack = value;
                if (_attackEvents != null)
                    _attackEvents.OnModifyAttack(this.MyEntity);
            }
        }

        [Export]
        public int Deffense
        {
            get => _deffense;
            set
            {
                _deffense = value;
                if (_attackEvents != null)
                    _attackEvents.OnModifyDeffense(this.MyEntity);
            }
        }
        #endregion



        public override void _EnterTree()
        {
            this.OnAwake();
        }

        #region Component methdos
        public void OnAwake(){
            if(MyEntity.TryGetIComponentNode<AttackHandler_Full>(out _attackEvents))
                _attackEvents.OnStart(MyEntity);
        }

        public void OnSetFree(){}

        public void Ontart()
        {
            
        }

        public void Reset(){}
        #endregion
    }
}
