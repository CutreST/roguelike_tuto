using Godot;
using System;

namespace Entities.Components.Actions
{

    public class TurnDead_Action : ActionResource
    {
        [Export]
        private Texture _deadSprite;


        public override void DoAction(in Entity ent)
        {
            AttackComp attackComp;
            if(ent.TryGetIComponentNode<AttackComp>(out attackComp) == false){
                return;
            }

            if(attackComp.Health > 0){
                return;
            }

            RenderComp renderComp;
            ent.TryGetIComponentNode<RenderComp>(out renderComp);

            renderComp.Texture = _deadSprite;

            //delet attackComponent node, TODO: modify this, something better
            ent.TryRemoveIComponentNode<AttackComp>();
            attackComp.Free();
            Messages.Print("Yeeeeelllow, is it me who you are looking for?");
            //turn off ai
        }
    }
}
