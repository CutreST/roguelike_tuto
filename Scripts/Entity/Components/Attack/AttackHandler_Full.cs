using Entities.Components.Actions;
using Godot;
using System;
using System.Collections.Generic;

namespace Entities.Components
{

    public class AttackHandler_Full : AttackHandler_Simple
    {
        [Export]
        List<ActionResource> _onStart;

        [Export]
        List<ActionResource> _modMaxHealth;

        [Export]
        List<ActionResource> _modAttack;

        [Export]
        List<ActionResource> _modDeffense;

        public void OnStart(in Entity ent)
        {
            if (this.IsListValid(_onStart))
            {
                for (int i = 0; i < _onStart.Count; i++)
                {
                    _onStart[i].DoAction(ent);
                }
            }
        }

        public void OnModifyMaxHealth(in Entity ent)
        {
            if (this.IsListValid(_modMaxHealth))
            {
                for (int i = 0; i < _modMaxHealth.Count; i++)
                {
                    _modMaxHealth[i].DoAction(ent);
                }
            }
        }

        public void OnModifyAttack(in Entity ent)
        {
            if (this.IsListValid(_modAttack))
            {
                for (int i = 0; i < _modAttack.Count; i++)
                {
                    _modAttack[i].DoAction(ent);
                }
            }
        }

        public void OnModifyDeffense(in Entity ent)
        {
            if (this.IsListValid(_modDeffense))
            {
                for (int i = 0; i < _modDeffense.Count; i++)
                {
                    _modDeffense[i].DoAction(ent);
                }
            }
        }


    }

}