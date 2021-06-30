using Godot;
using MySystems;
using System;

namespace Entities.Components
{
    /// <summary>
    /// Complement for the players input.
    /// <remarks>
    /// Rigth now the only input is the movement. At some point this complement only cares about the player's input (so, things like pause and so on
    /// is for another complement). Rigth now the component deletes itself if doesn't find a <see cref="MovementComp"/>.
    /// <para>IDEA:</para>
    /// <para>
    /// To test the flexibility of the entity-component framework, we can pass instances of this component to diferent entitities and control 
    /// them as we control the player, as in a posession. For example, if the other entity can only move, well, we move or if the entity can only
    /// attack (turret or similar), well, we only attack.    /// 
    /// </para>
    /// </remarks>
    /// </summary>
    public class InputComp : Node, IComponentNode
    {

        public Entity MyEntity { get; set; }

        /// <summary>
        /// As the input ins responsible of "moving", we need the <see cref="MovementComp"/> to move.
        /// TODO:
        /// At some point only change the direction vector, but the game is turn based and each turn is an input of the player, so actually works
        /// </summary>
        private MovementComp _mov;

        #region Godot Methods

        public override void _EnterTree()
        {
            base._EnterTree();
            this.OnAwake();
        }

        public override void _Ready()
        {
            base._Ready();
            this.Ontart();
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            this.OnSetFree();

        }

        #endregion

        #region Component methods
        public void OnAwake()
        {
            //look for the gamesys and subscribe event
            InGameSys gameSys;

            if (System_Manager.GetInstance(this).TryGetSystem<InGameSys>(out gameSys, true) == false)
            {
                //if failed, exit and print error 
                Messages.GetSystemFailed(gameSys, this.MyEntity);
                return;
            }

            System_Manager.GetInstance(this).AddToStack(gameSys);

            //subscribe
            gameSys.GameInput.OnChangeInputVector += OnChangeInputVector;

        }


        public void OnSetFree()
        {
            MyEntity = null;

            //unsubscribe
            InGameSys gameSys;

            if (System_Manager.GetInstance(this).TryGetSystem<InGameSys>(out gameSys, true) == false)
            {
                //same, show error just in case
                Messages.GetSystemFailed(gameSys, this.MyEntity);
                return;
            }

            gameSys.GameInput.OnChangeInputVector -= OnChangeInputVector;

        }

        public void Ontart()
        {
            //try to get the movement component of the entity, if not found, set free the input
            //OJU: to change based on gameplay
            if (MyEntity.TryGetIComponentNode<MovementComp>(out _mov) == false)
            {
                Messages.GetComponentFailed("Movement Component", "Input comp");
                base.QueueFree();
                return;
            }

        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// Raised with <see cref="InGameSys.GameInput.OnChangeInputVector"/>
        /// </summary>
        /// <param name="Vector">The new input vector</param>
        private void OnChangeInputVector(in Vector2 Vector)
        {
            Messages.Print("Input Vector: " + Vector);
            _mov.Move(Vector);
        }
    }

}