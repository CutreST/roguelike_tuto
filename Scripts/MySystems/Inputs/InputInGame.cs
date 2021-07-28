using Godot;
using System;

namespace MySystems.MyInput
{

    public class InputInGame : InputBase
    {
        private const string INPUT_LEFT = "ui_left";
        private const string INPUT_RIGHT = "ui_right";
        private const string INPUT_UP = "ui_up";
        private const string INPUT_DOWN = "ui_down";

        //oju!!! en godot tirar para arriba es -1
        private const int VALUE_UP = -1;
        private const int VALUE_DOWN = 1;
        private const int VALUE_LEFT = -1;
        private const int VALUE_RIGHT = 1;

        public Vector2 InputVector{ get; private set; }
        private Vector2 _currentVector;

        public delegate void Vector2Delegate(in Vector2 Vector);
        public event Vector2Delegate OnChangeInputVector;

        private void RaiseOnChangeInputVector(in Vector2 inputVector)
        {
            if (OnChangeInputVector != null)
            {
                this.OnChangeInputVector(inputVector);
            }
        }
        
        public InputInGame(in Visual_SystemBase visual) : base(visual)
        {
        }


        public override void GetInput()
        {
            _currentVector = new Vector2();

            if (Input.IsActionPressed(INPUT_LEFT))
            {
                _currentVector.x = VALUE_LEFT;
            }
            else if (Input.IsActionPressed(INPUT_RIGHT))
            {
                _currentVector.x = VALUE_RIGHT;
            }

            if (Input.IsActionPressed(INPUT_UP))
            {
                _currentVector.y = VALUE_UP;
            }
            else if (Input.IsActionPressed(INPUT_DOWN))
            {
                _currentVector.y = VALUE_DOWN;
            }
            InputVector = _currentVector;

            /* //evento;
             if(_inputVector.x != _currentVector.x || _inputVector.y != _currentVector.y){
                 _inputVector = _currentVector;
                 this.RaiseOnChangeInputVector(_inputVector);
             }*/


        }
    }
}
