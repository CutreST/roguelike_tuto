using Entities.Components.Actions;
using Godot;
using System;
using System.Collections.Generic;

namespace Entities.Components
{

    public class AttackHandler_Simple : Node, IComponentNode
    {
        [Export]
        List<ActionResource> _modHealthActions;

        public Entity MyEntity { get; set; }

        public void OnAwake() {}
    

    public void OnModifyHealth(in Entity ent)
        {
            if (this.IsListValid(_modHealthActions))
            {
                for (int i = 0; i < _modHealthActions.Count; i++)
                {
                    _modHealthActions[i].DoAction(ent);
                }
            }

        }

         protected bool IsListValid(in List<ActionResource> list)
        {
            return !(list == null || _modHealthActions.Count == 0);
        }

        public virtual void OnSetFree(){}

        public virtual void Ontart(){}

        public virtual void Reset(){}

       
    }
}
