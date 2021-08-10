using System;
using System.Collections.Generic;

using Base.Interfaces;
using Godot;

namespace MySystems
{
    /// <summary>
    /// The system manager used for the application
    /// </summary>
    public class System_Manager : IUpdate, IPhysic
    {
        /// <summary>
        /// A list of all the instancied systems.
        /// <para>Only admited one instance of a system</para>
        /// </summary>
        public Dictionary<Type, System_Base> MySystems { get; set; }

        /// <summary>
        /// Stack of the current visual systems
        /// </summary>
        protected Stack<Visual_SystemBase> VisualSys { get; set; }

        /// <summary>
        /// The current rendered and updatable system
        /// </summary>
        public Visual_SystemBase currentSys;

        public AppManager_GO NodeManager;

        private static System_Manager _instance;

        /// <summary>
        /// Gets the instance for this <see cref="System_Manager"/> running a singleton.
        /// OJU!!! We have to avoid creating a systemmangaer on it's own
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static System_Manager GetInstance(in Node node)
        {
            if (_instance == null)
            {
                //neww system
                _instance = new System_Manager();
                //new node
                _instance.NodeManager = new AppManager_GO();
                _instance.NodeManager.Name = "AppManager_Node";
                //we call in deferred because first the other node has to be initialized
                node.GetTree().Root.CallDeferred("add_child", _instance.NodeManager);
                
                _instance.NodeManager.Manager = _instance;
                _instance.Init();
            }

            return _instance;
        }


        #region Constructor
        /// <summary>
        /// Constructor. Initialies dicctionaries
        /// </summary>
        private System_Manager()
        {
            this.MySystems = new Dictionary<Type, System_Base>();
            this.VisualSys = new Stack<Visual_SystemBase>();
            
        }

        public System_Manager(in AppManager_GO go){
            this.MySystems = new Dictionary<Type, System_Base>();
            this.VisualSys = new Stack<Visual_SystemBase>();
            _instance = this;
            this.Init();
            this.NodeManager = go;
        }

        public void Init()
        {
            //metemos un dummy system para evitar errores null y demás
            Visual_SystemBase b = new DummyVisualSystem(this);
            this.TryAddSystem(b);
            this.AddToStack(b);
        }
        #endregion

        #region List methods
        /// <summary>
        /// Adds a system to the list of systems if it's not
        /// </summary>
        /// <param name="sys">system to enter</param>
        /// <returns></returns>       
        public bool TryAddSystem(in System_Base newSys)
        {
            if (MySystems == null)
            {
                MySystems = new Dictionary<Type, System_Base>();
            }

            if (newSys.MyManager == null || newSys.MyManager != this)
            {
                newSys.MyManager = this;
            }

            if (MySystems.ContainsKey(newSys.GetType()) == false)
            {
                MySystems.Add(newSys.GetType(), newSys);
                newSys.OnEnterSystem();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds a system to <see cref="MySystems"/> if it's not. 
        /// <para> OBSOLETE.</para>
        /// </summary>
        /// <param name="sys"></param>
        /// <returns>The system entered the <see cref="MySystems"/> list?</returns>
        [Obsolete("Use TryAddSystem instead")]
        public bool AddSystemToList(in System_Base sys)
        {
            if (this.MySystems.ContainsKey(sys.GetType()) == false)
            {
                this.MySystems.Add(sys.GetType(), sys);
                sys.OnEnterSystem();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove a system from the list
        /// </summary>
        /// <param name="sys">the system to remove</param>
        /// <returns></returns>
        public bool RemoveSystemFromList(in System_Base sys)
        {
            if (this.MySystems.ContainsKey(sys.GetType()) == false)
            {
                return false;
            }

            this.MySystems.Remove(sys.GetType());
            return true;
        }

        /// <summary>
        /// Gets the system from the list, if not there's a posibility to create the system
        /// </summary>
        /// <typeparam name="T">The type of the system, </typeparam>
        /// <param name="instanceNew">Instance a new system?</param>
        /// <returns></returns>
        [Obsolete("Use TryGetSystem<T>() instead")]
        public T GetSystem<T>(bool instanceNew = false, params object[] obj) where T : System_Base
        {
            System_Base s;
            if (MySystems.TryGetValue(typeof(T), out s) == false && instanceNew == true)
            {
                //Crea una instancia de un generico :S
                s = (T)Activator.CreateInstance(typeof(T), obj);
                s.MyManager = this;

                this.TryAddSystem(s);
            }

            return s as T;
        }

        /// <summary>
        /// Tries to get a system on <see cref="MySystems"/>. If the system is not on the dictionary and the instanceNew is set
        /// to true, it creates a new system of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type of the wanted system</typeparam>
        /// <param name="system">The getted system</param>
        /// <param name="instanceNew">If true, creates a system of type <typeparamref name="T"/> if none exist</param>
        /// <param name="obj">Params for the cosntructos</param>
        /// <returns></returns>
        public bool TryGetSystem<T>(out T system, bool instanceNew = false, params object[] obj) where T : System_Base
        {
            System_Base s;
            bool a = MySystems.TryGetValue(typeof(T), out s);

            if (a == false)
            {
                if (instanceNew == true)
                {
                    s = (T)Activator.CreateInstance(typeof(T), obj);
                    s.MyManager = this;                    
                    a = this.TryAddSystem(s);                     
                }
            }
            system = s as T;
            return a;
        }

        /// <summary>
        /// Clears the system list
        /// </summary>
        public void ClearSystemList()
        {
            DummyVisualSystem d = null;
            foreach(System_Base b in MySystems.Values){
                if(b.GetType() == typeof(DummyVisualSystem)){
                    d = (DummyVisualSystem)b;
                    continue;
                }

                b.OnExitSystem();
            }

            if(d == null)
                d = new DummyVisualSystem(this);

            MySystems.Clear();
            MySystems.Add(d.GetType(), d);
        }
        #endregion

        #region StackMethods
        /// <summary>
        /// Adds a <see cref="Visual_SystemBase"/> to <see cref="VisualSys"/>
        /// </summary>
        /// <param name="sys">The new system</param>
        /// <returns></returns>
        public bool AddToStack(in Visual_SystemBase sys)
        {
            if (this.VisualSys.Contains(sys))
            {
                return false;
            }

            if (currentSys != null)
            {
                this.currentSys.OnPauseStack();
            }

            this.VisualSys.Push(sys);
            this.currentSys = sys;
            this.currentSys.OnEnterStack();
            return true;
        }

        /// <summary>
        /// Remove the system from the stack if it's the current operating system
        /// </summary>
        /// <param name="sys"></param>
        /// <returns></returns>
        public bool RemoveFromStack(in Visual_SystemBase sys)
        {
            if (this.VisualSys.Peek() == sys)
            {
                this.currentSys.OnExitStack();
                this.VisualSys.Pop();
                this.currentSys = this.VisualSys.Peek();
                this.currentSys.OnResumeStack();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Clears the stack <see cref="System_Manager.VisualSys">
        /// </summary>
        public void ClearStack()
        {
            currentSys.OnExitStack();
            Visual_SystemBase systemBase;

            while(VisualSys.Count != 1 && VisualSys.Count > 0){
                systemBase = VisualSys.Pop();
                systemBase.OnExitStack();                 
            }

            VisualSys.Clear();
            currentSys = this.GetSystem<DummyVisualSystem>();
        }
        #endregion

        #region Update/Physics bucles

        public void MyUpdate(in float delta)
        {
            this.currentSys.MyUpdate(delta);
        }

        public void MyPhysic(in float fixedDelta)
        {
            this.currentSys.MyPhysic(fixedDelta);
        }

        public void MyLateUpdate(in float delta)
        {
            this.currentSys.MyLateUpdate(delta);
        }



        #endregion
    }
}
