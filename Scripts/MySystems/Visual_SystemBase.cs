using System.Collections.Generic;
using MySystems.MyInput;
using Base.Interfaces;
using Godot;


namespace MySystems
{
    /// <summary>
    /// An abstract class for every visual system that needs an Update and Physics calls
    /// </summary>
    public abstract class Visual_SystemBase : System_Base, IUpdate, IPhysic
    {
        #region Properties

        /// <summary>
        /// List of objects to be called at <see cref="MyUpdate(in float)"/>
        /// </summary>
        public List<IUpdate> UpdateObjs { get; set; }

        /// <summary>
        /// List of objects to be called at <see cref="MyPhysic(in float)"/>
        /// </summary>
        public List<IPhysic> PhysicObjs { get; set; }

        public List<ILateUpdate> LateObjs { get; set; }

        /// <summary>
        /// The gameobject holding the system
        /// <para>This will act as a parent for every compoent of the system. If we don't need to render the whole system, is better this way</para>
        /// </summary>
        public Node GO { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="go">The gameobject to attach</param>

        public InputBase MyInput { get; set; }

        //public delegate void InputDelegate(InputAction.CallbackContext context);
        public Visual_SystemBase(in Node go, in System_Manager manager)
        {
            this.GO = go;
            this.UpdateObjs = new List<IUpdate>();
            this.PhysicObjs = new List<IPhysic>();
            this.LateObjs = new List<ILateUpdate>();
            this.MyManager = manager;
        }
        #endregion

        #region Updates
        /// <summary>
        /// Called every frame
        /// </summary>
        /// <param name="delta">Time between frames</param>
        public virtual void MyUpdate(in float delta)
        {            
            for (int i = this.UpdateObjs.Count - 1; i >= 0; i--)
            {
                this.UpdateObjs[i].MyUpdate(delta);
            }
        }

        /// <summary>
        /// Called every physic frame
        /// </summary>
        /// <param name="delta">Tiome between calls</param>
        public virtual void MyPhysic(in float delta)
        {            
            //Debug.Log("Update count: " + this.PhysicObjs.Count);
            for (int i = this.PhysicObjs.Count - 1; i >= 0; i--)
            {
                this.PhysicObjs[i].MyPhysic(delta);
                //Debug.LogError("IIII: " + i);
                
            }
        }

        /// <summary>
        /// Called and the end of every famrame
        /// </summary>
        /// <param name="delta">Delta Time</param>
        public void MyLateUpdate(in float delta)
        {
            if(this.LateObjs.Count == 0)
            {
                return;
            }
            for (int i = this.LateObjs.Count - 1; i >= 0; i--){
                this.LateObjs[i].MyLateUpdate(delta);
            }
        }

        #endregion

        #region List methods

        /// <summary>
        /// Adds an <see cref="IUpdate"/> object to <see cref="UpdateObjs"/> if it is not inside
        /// </summary>
        /// <param name="obj">The object to add</param>
        /// <returns></returns>
        public virtual bool AddToUpdate(in IUpdate obj)
        {
            if (this.UpdateObjs.Contains(obj) == false)
            {
                this.UpdateObjs.Add(obj);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds an <see cref="IPhysic"/> object to <see cref="PhysicObjs"/> if it is not inside
        /// </summary>
        /// <param name="obj">The object to add</param>
        /// <returns></returns>
        public virtual bool AddToPhysic(in IPhysic obj)
        {
            if (this.PhysicObjs.Contains(obj) == false)
            {
                this.PhysicObjs.Add(obj);
                return true;
            }

            return false;
        }


        /// <summary>
        /// Adds an <see cref="ILateUpdate"/> object to <see cref="LateObjs/> if it is not inside
        /// </summary>
        /// <param name="obj">The object to add</param>
        /// <returns></returns>
        public virtual bool AddToLateUpdate(in ILateUpdate obj)
        {
            
            if (this.LateObjs.Contains(obj) == false)
            {
                this.LateObjs.Add(obj);
                return true;
            }
            

            return false;
        }

        /// <summary>
        /// Remove an <see cref="ILateUpdate"/> object to <see cref="LateObjss"/> if it is not inside
        /// </summary>
        /// <param name="obj">The object to add</param>
        /// <returns></returns>
        public virtual bool RemoveFromLateUpdate(in ILateUpdate obj)
        {
            if (this.LateObjs.Contains(obj))
            {
                this.LateObjs.Remove(obj);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Remove a <see cref="IUpdate"/> object form <see cref="UpdateObjs"/>
        /// </summary>
        /// <param name="obj">The object to remove</param>
        public virtual void RemoveFromUpdate(in IUpdate obj)
        {
            this.UpdateObjs.Remove(obj);
        }

        /// <summary>
        /// Remove a <see cref="IUPhysic"/> object form <see cref="PhysicObjs"/>
        /// </summary>
        /// <param name="obj">The object to remove</param>
        public virtual void RemoveFromPhysic(in IPhysic obj)
        {
            this.PhysicObjs.Remove(obj);
        }
        #endregion

        #region Stack methods
        /// <summary>
        /// Called when the system enters to the stack
        /// </summary>
        public abstract void OnEnterStack();

        /// <summary>
        /// called when the system exits the stack
        /// </summary>
        public abstract void OnExitStack();

        /// <summary>
        /// Called when the system is at the mniddle of stack
        /// </summary>
        public abstract void OnPauseStack();

        /// <summary>
        /// Called when the system is at the first position of the stack and operational again.
        /// </summary>
        public abstract void OnResumeStack();


        #endregion
        /*
        public void InitializeInput(in MyInputSys.ActionMapsEnum action)
        {
            MyInput.EnableActionMaps(action);
        }*/

    }
}
