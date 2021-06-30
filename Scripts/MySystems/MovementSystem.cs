using Entities.Components;
using Godot;
using System;
using World;

namespace MySystems
{

    public class MovementSystem : System_Base
    {

        public WorldMapCont MyWorld { private get; set; }
        private Vector2 tempPos;


        public void Move(in MovementComp mov){
            //en algún momento deberíamos enviar cualquier mierda
            
            tempPos = mov.MyEntity.GlobalPosition + new Vector2(mov.TILE_WIDTH, mov.TILE_HEIGHT) * mov.Direction;

            if(MyWorld.IsTileBlocked((int)tempPos.x, (int)tempPos.y) == false){
                mov.MyEntity.GlobalPosition = tempPos;
            }
        }


        #region Enter-exit system methods
        public override void OnEnterSystem(params object[] obj)
        {
            Messages.EnterSystem(this);
            
        }

        public override void OnExitSystem(params object[] obj)
        {
            Messages.ExitSystem(this);
        }
        #endregion
    }
}
