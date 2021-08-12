using Godot;
using MySystems;
using System;

namespace Entities.Components.Actions
{

    public class ModStats_Panel : ActionResource
    {
        private PlayerScreenSystem _playerScreen;

        private enum Stat_Type { HEALTH, MAX_HEALTH, DEFFENSE, ATTACK }

        [Export]
        private Stat_Type _myStat;


        public override void DoAction(in Entity ent)
        {
            if (_playerScreen == null)
            {
                if (System_Manager.GetInstance(ent).TryGetSystem<PlayerScreenSystem>(out _playerScreen, true) == false)
                {
                    return;
                }
            }

            AttackComp attack;
            if(ent.TryGetIComponentNode<AttackComp>(out attack) == false)
                return;

            this.RefreshStat(attack);
        }

        private void RefreshStat(in AttackComp attack)
        {
            switch (_myStat)
            {
                case Stat_Type.HEALTH:
                    _playerScreen.UpdateHealth(attack.Health);
                    break;
                case Stat_Type.MAX_HEALTH:
                    _playerScreen.UpdateMaxHealth(attack.MaxHealth);
                    break;
                case Stat_Type.DEFFENSE:
                    _playerScreen.UpdateDeffense(attack.Deffense);
                    break;
                case Stat_Type.ATTACK:
                    _playerScreen.UpdateAttack(attack.Attack);
                    break;
            }
        }
    }
}
