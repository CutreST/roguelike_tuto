using Godot;
using MySystems;
using System;

namespace Entities.Components
{

    public class InputComp : Node, IComponentNode
    {
        public Entity MyEntity { get; set; }

        private MovementComp _mov;

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

        public void OnAwake()
        {
            //buscamos el gamesys y nos subscribimos al evento
            InGameSys gameSys;

            if (System_Manager.GetInstance(this).TryGetSystem<InGameSys>(out gameSys, true) == false)
            {
                Messages.GetSystemFailed(gameSys, this.MyEntity);
                return;
            }

            System_Manager.GetInstance(this).AddToStack(gameSys);

            //subscribimos
            gameSys.GameInput.OnChangeInputVector += OnChangeInputVector;

        }

        private void OnChangeInputVector(in Vector2 Vector)
        {
            Messages.Print("Input Vector: " + Vector);
            _mov.Move(Vector);
        }



        public void OnSetFree()
        {
            MyEntity = null;

            //buscamos el gamesys y nos subscribimos al evento
            InGameSys gameSys;

            if (System_Manager.GetInstance(this).TryGetSystem<InGameSys>(out gameSys, true) == false)
            {
                Messages.GetSystemFailed(gameSys, this.MyEntity);
                return;
            }

            gameSys.GameInput.OnChangeInputVector -= OnChangeInputVector;

        }

        public void Ontart()
        {
            //intentamos pillar el comp movimiento
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


    }

}