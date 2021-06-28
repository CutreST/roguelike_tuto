using System.Collections.Generic;
using System;
using Entities.Components;
using Godot;

namespace Entities
{

    /// <summary>
    /// Class Entity. It's only a container for all the <see cref="IComponentNode"/>. It has a <see cref="EntityNode"/> that acts as parent, GameObject.
    /// </summary>
    public class Entity : Node2D
    {
        #region Properties
        /// <summary>
        /// The list of all <see cref="MyComponents"/>
        /// </summary>
        //[field:SerializeField]
        public Dictionary<Type, IComponentNode> MyComponents { get; set; }


        /// <summary>
        /// <see cref="EntityNode"/> that contains this entity.
        /// </summary>
        [Obsolete("The entity has no parent now, it's a proper Godot Node")]
        public EntityNode MyParent { get; set; }       

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="entutyMono">The <see cref="EntityNode"/> parent</param>
        public Entity(in EntityNode entutyMono)
        {
            //MyParent = entutyMono;
            MyComponents = new Dictionary<Type, IComponentNode>();
        }

        public Entity(){
            MyComponents = new Dictionary<Type, IComponentNode>();
        }
        #endregion


        #region Godot Methods
        public override void _EnterTree()
        {
            base._EnterTree();
            this.MyComponents = new Dictionary<Type, IComponentNode>();
            this.AddIComponentChildren(this);
            //ok vamos a hacer algo superduper, ahora, cada vez que entre la entity en el árbol buscará a sus 
            //hijos y los seteerá ella misma.


        }

        /// <summary>
        /// Looks throug all the children and if the childrens is a .<see cref="IComponentNode"/> then adds it to
        /// .<see cref="Entity.MyComponents"/> and set the .<see cref="IComponentNode.MyEntity"/> as this entity
        /// </summary>
        private void AddIComponentChildren(in Node root){            

            IComponentNode componentNode;
            Node child;
            for (int i = 0; i < root.GetChildCount(); i++){
                child = root.GetChild(i);
                componentNode = child as IComponentNode;
                if(componentNode  != null){
                    this.TryAddIComponentNode(componentNode);
                    componentNode.MyEntity = this;
                    //de cada objeto miraremos si sus hijos son también mierda de estas
                    this.AddIComponentChildren(child);
                    Messages.Print("yeeloooow");
                }
            }
        }
        #endregion

        #region List methods
        /// <summary>
        /// Addes a <see cref="IComponentNode"/> to <see cref="MyComponents"/> if the list doesn't contain it with a provided generic 
        /// </summary>
        /// <param name="component">The component to add</param>
        /// <returns>Is the component succefully added?</returns>
        public bool TryAddIComponentNode<T>(in IComponentNode component) where T : IComponentNode
        {
            if (this.MyComponents == null)
            {
                this.MyComponents = new Dictionary<Type, IComponentNode>();
                this.MyComponents.Add(typeof(T), component);
                return true;
            }

            if (MyComponents.ContainsKey(typeof(T)) == false)
            {
                MyComponents.Add(typeof(T), component);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Addes a <see cref="IComponentNode"/> to <see cref="MyComponents"/> if the list doesn't contain it
        /// </summary>
        /// <param name="component">The component to add</param>
        /// <returns>Is the component succefully added?</returns>
        public bool TryAddIComponentNode(in IComponentNode component)
        {
            if (this.MyComponents == null)
            {
                this.MyComponents = new Dictionary<Type, IComponentNode>();
                this.MyComponents.Add(component.GetType(), component);
                return true;
            }

            if (MyComponents.ContainsKey(component.GetType()) == false)
            {
                MyComponents.Add(component.GetType(), component);
                GD.Print("Added ", component.GetType(), " to ", base.Name, " entity.");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a <see cref="IComponentNode"/> of the entity
        /// </summary>
        /// <typeparam name="T">The type of the component to get</typeparam>
        /// <param name="component">The out component</param>
        /// <returns>Got The component?</returns>
        public bool TryGetIComponentNode<T>(out T component) where T : class
        {
            component = null;

            if (MyComponents == null)
            {
                return false;
            }

            IComponentNode c;

            MyComponents.TryGetValue(typeof(T), out c);

            if (c != null)
            {
                component = c as T;
                return true;
            }

            return false;

        }
        /// <summary>
        /// Remove a <see cref="IComponentNode"/> by its type if exist inside <see cref="MyComponents"/>
        /// </summary>
        /// <typeparam name="T">Type of the <see cref="IComponentNode"/></typeparam>
        /// <returns>Is the component succefully removed?</returns>
        public bool TryRemoveIComponentNode<T>() where T : IComponentNode
        {
            if (MyComponents == null)
            {
                return false;
            }

            if (MyComponents.ContainsKey(typeof(T)))
            {
                MyComponents.Remove(typeof(T));
                return true;
            }


            return false;
        }

        /// <summary>
        /// Removes a <see ref="IComponentNode"/> if exist, else, returns false
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public bool TryRemoveIComponentNode(in IComponentNode component)
        {
            if (MyComponents == null)
            {
                return false;
            }

            if (MyComponents.ContainsKey(component.GetType()))
            {
                MyComponents.Remove(component.GetType());
                return true;
            }

            return false;
        }
        #endregion

        #region Unity Init Methods

        /// <summary>
        /// Callesd on unity start.
        /// <para>Does nothing</para>
        /// </summary>
        /// <param name="ent">the paren <see cref="EntityNode"/></param>
        public void OnStart(in EntityNode ent) { }

        /// <summary>
        /// Callesd on unity OnValidate.
        /// <para>Does nothing</para>
        /// </summary>
        /// <param name="ent">the paren <see cref="EntityNode"/></param>
        public void OnValidate(in EntityNode ent) { }


        #endregion

        /// <summary>
        /// To call when destroy the entity
        /// </summary>
        public void FreeComponents()
        {
            if (MyComponents == null)
            {
                return;
            }
            foreach (IComponentNode c in MyComponents.Values)
            {
                c.OnSetFree();
            }

            MyComponents.Clear();
        }

        /// <summary>
        /// Resets the compoents
        /// </summary>
        public void ResetComponents()
        {
            foreach (Type c in MyComponents.Keys)
            {
                MyComponents[c].Reset();
            }
        }

    }



}