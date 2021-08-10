using Entities.Components;
using Godot;
using System;
using System.Collections.Generic;

namespace MySystems
{

    public class AttackSystem : Visual_SystemBase
    {
        //Dictionary<AttackComp, AttackComp> atackRec;

        public AttackSystem(in Node go, in System_Manager manager) : base(go, manager)
        {
        }

        public AttackSystem() : base(null, null)
        {

        }


        const float TIME_LAPSE = 0.15f;
        float _time;

        AttackComp _hitter;
        AttackComp _receiver;

        int damage;

        MovementSystem _mov;

        public bool Attack(in CollisionComp hitter, in CollisionComp receiver)
        {
            if ((hitter.MyEntity.TryGetIComponentNode<AttackComp>(out _hitter) &&
               receiver.MyEntity.TryGetIComponentNode<AttackComp>(out _receiver)) == false)
            {
                return false;
            }

            ConsoleSystem console;
            MyManager.TryGetSystem<ConsoleSystem>(out console, true);

            //entramos en el stack
            MyManager.AddToStack(this);

            //cálculo daño y escribimos mensaje
            damage = this.DoDamage(ref _hitter, ref _receiver);

            if (damage > 0)
            {
                _receiver.Health -= damage; //-> event on AttackComp
                //mensaje
                //Messages.Print(String.Format("{0} deals {1} of damage to {2}", _hitter.MyEntity.Name, damage.ToString(), _receiver.MyEntity.Name));
                string nameHitter = _hitter.MyEntity.Name;
                string nameReceiver = _receiver.MyEntity.Name;

                if (_receiver.Health > 0)
                {
                    console.WriteAttack(nameHitter, nameReceiver, damage);
                }else{
                    console.WriteDead(nameHitter, nameReceiver);
                }
            }
            else
            {
                //no hay daño, luego no se hace ná
                //mensaje
                Messages.Print(String.Format("{0} deals NO damage to {2}", _hitter.MyEntity.Name, damage.ToString(), _receiver.MyEntity.Name));
                console.WriteZeroAttack(_hitter.MyEntity.Name, _receiver.MyEntity.Name);
            }

            //al devolver true, el movementsystem sabe que ha habido ataque y, por lo tanto, debe devolver al atacante a la pos inicial*/
            return true;
        }

        private int DoDamage(ref AttackComp hitter, ref AttackComp receiver)
        {
            return hitter.Attack - receiver.Deffense;
        }



        public override void MyPhysic(in float delta)
        {
            _time += delta;
            if (_time > TIME_LAPSE)
            {
                MovementComp comp;
                _hitter.MyEntity.TryGetIComponentNode<MovementComp>(out comp);
                comp.MoveToLastPos();
                MyManager.RemoveFromStack(this);
            }
        }

        private void EndQueue()
        {
            MyManager.RemoveFromStack(this);
            _hitter = null;
            _receiver = null;

        }

        #region StackMethods
        public override void OnEnterStack()
        {
            Messages.EnterStack(this);
            _time = 0;
        }
        public override void OnExitStack()
        {
            Messages.ExitStack(this);
            _hitter = null;
            _receiver = null;
        }

        public override void OnPauseStack()
        {
            Messages.PauseStack(this);
        }

        public override void OnResumeStack()
        {
            Messages.ResumeStack(this);
        }

        #endregion

        #region System methods
        public override void OnEnterSystem(params object[] obj)
        {
            Messages.EnterSystem(this);
            MyManager.TryGetSystem<MovementSystem>(out _mov, true);
        }


        public override void OnExitSystem(params object[] obj)
        {
            Messages.ExitSystem(this);
            _mov = null;
            MyManager = null;
        }

        #endregion
    }

}