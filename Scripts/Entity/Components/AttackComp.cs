using Godot;
using System;

namespace Entities.Components
{

    public class AttackComp : Node, IComponentNode
    {
        public Entity MyEntity { get; set; }

        private int _health;
        private BasicHealthEvents _healthEvents;

        #region Export properties
        [Export]
        public int Health
        {
            get => _health;
            set
            {
                _health = value;
                if(_healthEvents != null){
                    _healthEvents.OnModifyHealth(_health);
                }
            }
        }
        [Export]
        public int Attack { get; set; }
        [Export]
        public int Deffense { get; set; }
        #endregion



        public override void _Ready()
        {
            this.Ontart();
        }

        #region Component methdos
        public void OnAwake()
        {
            
        }

        public void OnSetFree()
        {
            throw new NotImplementedException();
        }

        public void Ontart()
        {
            MyEntity.TryGetIComponentNode<BasicHealthEvents>(out _healthEvents);
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
