using Godot;
using System;

namespace Entities.Components
{
    /// <summary>
    /// Class that calls basic health events
    /// </summary>
    /// <remarks>
    /// Ok, at this point I think that this need a redesign. For example, we can make some <see cref="Resources"/>
    /// that have all the behaviour that we want and then have also a list and call all of them when spawning the event.
    /// <para>
    /// For example, we can have two diferent resources that only gets called if the health is 0 or below, one changes 
    /// the sprite and turns off the ai 'cause, is dead; and the other, changes the ai, changes the monster sprite and
    /// has a buff on the stats, as a new transformation. I like it, but maybe later
    /// /<para>
    /// </remarks>

    public class BasicHealthEvents : Node, IComponentNode
    {
        [Export]
        private Texture _deadSprite;

        public Entity MyEntity { get; set; }



        public void OnModifyHealth(in int health)
        {
            Messages.Print(MyEntity.Name, "Health event!!!!");
            if (health <= 0)
            {
                this.TurnDead();
            }
        }

        private void TurnDead()
        {
            RenderComp renderComp;
            MyEntity.TryGetIComponentNode<RenderComp>(out renderComp);

            renderComp.Texture = _deadSprite;
            //delete attac component?
            AttackComp attack;
            MyEntity.TryGetIComponentNode<AttackComp>(out attack);
            MyEntity.TryRemoveIComponentNode<AttackComp>();
            attack.Free();
            //turn off ai


        }

        public void OnAwake()
        {
            throw new NotImplementedException();
        }

        public void OnSetFree()
        {
            throw new NotImplementedException();
        }

        public void Ontart()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }




    }
}
