using Godot;
using MySystems;
using System;

namespace Entities.Components.TurnData
{

    public class TurnInput : TurnData_Base
    {
        private InGameSys _gameSys;
        private Vector2 dir;
        public override bool DoAction(in TurnComp comp)
        {
            //Messages.Print("\n\n\n", "hola caracola");
            dir = _gameSys.GameInput.InputVector;
            //no movement, so no end turn
            if(dir.x == 0 && dir.y == 0){
                return false;
            }

            //mov.
            MovementComp mov;

            if(comp.MyEntity.TryGetIComponentNode<MovementComp>(out mov)){
                mov.Move(dir);
                return true;
            }

            return false;
        }

        public override void Start(in Node node)
        {

            System_Manager.GetInstance(node).TryGetSystem<InGameSys>(out _gameSys, true);

        }
    }
}
