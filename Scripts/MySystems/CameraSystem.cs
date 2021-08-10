using Godot;
using System;

namespace MySystems
{

    public class CameraSystem : System_Base
    {
        private Camera2D _mainCamera;
        public Camera2D MainCamera{
            get => _mainCamera;
            set{
                _mainCamera = value;
                this.InitCamera();
            }
        }
        
        private void InitCamera(){
            _mainCamera.Current = true;
        }

        public void Move(in Vector2 newPos){
            _mainCamera.GlobalPosition = newPos;
        }

        public void Move(in Vector2 direction, in Vector2 speed){
            _mainCamera.GlobalPosition = new Vector2(direction * speed);
        }

        #region System Methods
        public override void OnEnterSystem(params object[] obj)
        {
            Messages.EnterSystem(this);
        }

        public override void OnExitSystem(params object[] obj)
        {
            Messages.EnterSystem(this);
        }
        #endregion
    }
}
