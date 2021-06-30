using Godot;
using MySystems.MyInput;
using System;
using World;

namespace MySystems
{
    /// <summary>
    /// The InGame system, resposible of the updates of the objects and the input (at some point)
    /// </summary>
    public class InGameSys : Visual_SystemBase
    {       
        public WorldMapCont MyWorldCont{ get; set; }


        //esto tendriamos que cambiarlo seguramente, pondremos una interfaz para los inputs
        public InputInGame GameInput{ get => (InputInGame)MyInput; set => MyInput = value; }

        public delegate void SimpleDelegate();
        public event SimpleDelegate OnPauseStack_E;
        public event SimpleDelegate OnResumeStack_E;

        public static float GameTime { get; private set; }

        #region Constructor
        public InGameSys() : base(null, null)
        {

        }

        public InGameSys(in Node go, in System_Manager manager) : base(go, manager)
        {

        }     
        #endregion

        #region System
        public override void OnEnterSystem(params object[] obj)
        {
            Messages.EnterSystem(this);
            GameInput = new InputInGame(this);

        }
        public override void OnExitSystem(params object[] obj)
        {
            Messages.ExitSystem(this);
            LateObjs.Clear();
            UpdateObjs.Clear();
            PhysicObjs.Clear();          

            if(GO != null)
                GO.QueueFree();
        }
        #endregion

        #region Stack
        public override void OnEnterStack()
        {
            Messages.EnterStack(this);
        }

        public override void OnExitStack()
        {
            Messages.ExitStack(this);
        }


        public override void OnPauseStack()
        {
            Messages.PauseStack(this);
            if (this.OnPauseStack_E != null)
            {
                this.OnPauseStack_E();
            }
        }

        public override void OnResumeStack()
        {
            Messages.ResumeStack(this);
            if (this.OnResumeStack_E != null)
            {
                this.OnResumeStack_E();
            }
        }
        #endregion

        #region Update

        public override void MyUpdate(in float delta)
        {
            GameTime += delta;
            MyInput.GetInput();
            base.MyUpdate(delta);
        }

        #endregion

      
    }
}
