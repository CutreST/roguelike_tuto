using Godot;
using System;

namespace MySystems
{
    /// <summary>
    /// The main node for <see cref="System_Manager"/>
    /// We use the node to call physics, process and to access the tree without 
    /// the need of another node and so on.
    /// <para>
    /// Plus, we have a custom timer runned by the update delta.
    /// </para>
    /// </summary>
    public class AppManager_GO : Node
    {

        /// <summary>
        /// The <see cref="System_Manager"/> for the application
        /// </summary>
        public System_Manager Manager { get; set; }        
       
        /// <summary>
        /// Total time of the application.
        /// </summary>
        public static float Time { get; private set; }

        public static float Delta;
        public override void _EnterTree()
        {
            GD.Print("Instance made to the tree");
        }

        public override void _Process(float delta)
        {
            Manager.MyUpdate(delta);
            Time += delta;
            Delta = delta;
        }

        public override void _PhysicsProcess(float delta)
        {
            Manager.MyPhysic(delta);
        }


    }
}
