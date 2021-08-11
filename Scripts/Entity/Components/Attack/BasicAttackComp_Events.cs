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

    public abstract class BasicAttackComp_Events : Node, IComponentNode
    {
        public Entity MyEntity { get; set; }



        public abstract void OnModifyHealth(in int health);

        public abstract void OnModifyMaxHealth(in int maxHealt);

        public abstract void OnModifyAttack(in int attack);

        public abstract void OnModifyDeffense(in int deffense);

        #region Godot Methods
        public override void _EnterTree()
        {
            this.OnAwake();
        }

        public override void _Ready()
        {
            this.Ontart();
        }

        public override void _ExitTree()
        {
            this.OnSetFree();
        }

        #endregion

        public abstract void OnAwake();

        public abstract void OnSetFree();

        public abstract void Ontart();

        public abstract void Reset();




    }
}
