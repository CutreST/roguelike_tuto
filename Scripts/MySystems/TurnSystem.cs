using Godot;
using System;
using Base.Interfaces;
using System.Collections.Generic;
using Entities.Components;

namespace MySystems
{

    public class TurnSystem : System_Base, IPhysic
    {


        //Lista de objectos para el turno
        List<TurnComp> _turnObj;

        private int _index;

        private const float TIME_LAPSE = 0.2f;
        private float _time;

        private uint _currentTurn;

        private bool _onTurn;

        public void MyPhysic(in float delta)
        {
            if (_onTurn == false)
            {
                _time += delta;
                if (_time > TIME_LAPSE)
                {
                    _onTurn = true;
                    _time = 0;
                }
            }else{
                if(_index >= this._turnObj.Count){
                    this.StartTurn();
                }else{
                    if(this._turnObj[_index].TurnAction())
                        _index++;
                        _time = 0;
                }
            }


            /*
            //si index mayor, onturn == false y, si onTurn == false, hasta que el tiempo no haya pasado no tiramos al siguiente     
            if (_index >= this._turnObj.Count)
            {

                if (_time > TIME_LAPSE)
                {
                    Messages.Print("Turn number", _currentTurn.ToString());
                    this.StartTurn();
                }

                return;
            }

            if (this._turnObj[_index].TurnAction())
            {
                _index++;
            }*/
        }

        private void StartTurn()
        {
            _currentTurn++;
            _index = 0;
            _onTurn = false;
            _time = 0;
            Messages.Print("Turn number:", _currentTurn.ToString());
        }

        public bool TryAddTurnComp(in TurnComp comp)
        {
            if (this._turnObj.Contains(comp))
            {
                return false;
            }

            this._turnObj.Add(comp);
            return true;
        }

        public bool TryRemoveTurnComp(in TurnComp comp)
        {
            if (this._turnObj.Contains(comp))
            {
                return false;
            }

            this._turnObj.Remove(comp);
            return true;
        }



        #region Sstem methods
        public override void OnEnterSystem(params object[] obj)
        {
            Messages.EnterSystem(this);
            _turnObj = new List<TurnComp>();

            InGameSys sys;
            MyManager.TryGetSystem<InGameSys>(out sys, true);
            sys.AddToPhysic(this);
        }

        public override void OnExitSystem(params object[] obj)
        {
            Messages.ExitSystem(this);
            _turnObj.Clear();
            _turnObj = null;
        }
        #endregion
    }

}