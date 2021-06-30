using Entities.Components;
using Godot;
using System;
using World;

namespace MySystems
{
    /// <summary>
    /// Movement system.
    /// </summary>
    /// <remarks>
    /// Each <see cref="MovementComp"/> calls this system to be able to perform a movement. Right now the movement system is 
    /// tilebased, so we neeed a reference to the <see cref="WorlMapCont"/>. OJU! It's the world map the one that holds the reference.
    /// TODO:
    /// Change this, cause we need a world to be able to move.
    /// </remarks>
    public class MovementSystem : System_Base
    {

        public WorldMapCont MyWorld { private get; set; }
        private Vector2 tempPos;


        public void Move(in MovementComp mov){
            //en algún momento deberíamos enviar cualquier mierda
            
            tempPos = mov.MyEntity.GlobalPosition + new Vector2(mov.TILE_WIDTH, mov.TILE_HEIGHT) * mov.Direction;

            //check for null only for test prourposes (ex: test the movement outside the world an so on)
            if(MyWorld == null || MyWorld.IsTileBlocked((int)tempPos.x, (int)tempPos.y) == false){
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
