using Godot;

namespace MySystems
{
    /// <summary>
    /// In order to avoid an empty stack exception at <see cref="System_Manager.VisualSys"/> we create this dummy system
    /// and put it at the begining when the <see cref="System_Manager"/> is created. Doing so we can add and substract systems
    /// without caring about errors and so.
    /// <remarks>
    /// Vale, lo pongo en castellano para enterarnos bien.
    /// Sabemos que muchas veces probamos sistemas así al vuelo y demás, entonces, para
    /// que el stack NUNCA esté vacío y de errores en los updates metemos este dummy system 
    /// cuyop obejetivo es spamear el mensaje que está allí. Nada más crear el system managert el 
    /// systema está creado y se mete en el stack.    /// 
    /// </remarks>
    /// </summary>
    public class DummyVisualSystem : Visual_SystemBase
    {

        #region constructor
        /// <summary>
        /// An empty constructor for this system. The <see cref="Visual_SystemBase.GO"/> is set to null
        /// because this system doesn't need it
        /// </summary>
        public DummyVisualSystem(in System_Manager manager) : base(null, manager)
        {

        }
        #endregion

        //there's messages at enter estack, exit stack and resume stack
        #region Statck
        public override void OnEnterStack()
        {
            Messages.EnterStack(this);
            this.SetTimer();
        }

        public override void OnEnterSystem(params object[] obj)
        {

        }

        public override void OnExitStack()
        {
            Messages.ExitStack(this);
            Messages.Print("The Dummy system is suposed to be allways into the stack!!!", Messages.MessageType.ERROR);            
        }

        public override void OnExitSystem(params object[] obj)
        {

        }

        public override void OnPauseStack()
        {

        }

        public override void OnResumeStack()
        {
            GD.Print("Oju! Dummy System is resumed.");
            this.SetTimer();

        }
        #endregion

        #region Spam mesage and update
        /// <summary>
        /// Time beetwen messages
        /// </summary>
        private const float TIME = 10f;

        /// <summary>
        /// The interval between messages
        /// </summary>
        private float _timeToWait;
        /// <summary>
        /// Spam Message? False = no, we don't need to spam
        /// </summary>
        private bool _spamMessage = true;

        private const string MESSAGE = "Oju! Dummy System is the current System at the stack!";



        /// <summary>
        /// Overrides from <see cref="Visual_SystemBase.MyUpdate(in float)"/>
        /// <para>Spams a message every 10 seconds aprox saying that this system is the current system</para>
        /// </summary>
        /// <param name="delta"></param>
        public override void MyUpdate(in float delta)
        {
            if (_spamMessage == false)
            {
                return;
            }


            if (_timeToWait < AppManager_GO.Time)
            {
                Messages.Print(MESSAGE, Messages.MessageType.ERROR);
                this.SetTimer();
            }
            base.MyUpdate(delta);

        }

        private void SetTimer()
        {
            _timeToWait = (float)AppManager_GO.Time + TIME;
        }
        #endregion
    }
}
