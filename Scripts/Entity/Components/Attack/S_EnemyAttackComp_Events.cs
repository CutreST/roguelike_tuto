using Godot;
using System;

namespace Entities.Components
{
    /// <summary>
    /// Class for holding all the <see cref="AttackComp"/> events. Decided to do a separate class so, is
    /// easiest to modify the behaviour of each entity. 
    /// </summary>
    /// <remarks>
    /// Ok, at this point I think that this need a redesign. For example, we can make some <see cref="Resources"/>
    /// that have all the behaviour that we want and then have also a list and call all of them when spawning the event.
    /// <para>
    /// For example, we can have two diferent resources that only gets called if the health is 0 or below, one changes 
    /// the sprite and turns off the ai 'cause, is dead; and the other, changes the ai, changes the monster sprite and
    /// has a buff on the stats, as a new transformation. I like it, it needs some work but, maybe, why not?
    /// CURRENTLY THINKING ABOUT IT!!!!
    /// /<para>
    /// </remarks>

    public class S_EnemyAttackComp_Events : BasicAttackComp_Events, IComponentNode
    {
        [Export]
        private Texture _deadSprite;
        public override void OnModifyHealth(in int health)
        {
            Messages.Print(MyEntity.Name, "Health event!!!!");
            if (health <= 0)
            {
                this.TurnDead();
            }
        }

        public override void OnModifyMaxHealth(in int maxHealt) { }

        public override void OnModifyAttack(in int attack) { }

        public override void OnModifyDeffense(in int deffense) { }


        protected void TurnDead()
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

        public override void OnAwake() { }
        public override void OnSetFree() { }

        public override void Ontart() { }

        public override void Reset() { }


    }
}
