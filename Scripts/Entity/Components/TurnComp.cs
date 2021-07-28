using Godot;
using MySystems;
using System;
using Entities.Components.TurnData;

namespace Entities.Components
{

    public class TurnComp : Node, IComponentNode
    {
        public Entity MyEntity { get; set; }

        //resource de behavous
        [Export]
        TurnData_Base _action;

        public bool TurnAction(){
            //Messages.Print(MyEntity.Name, "Doing this turn, baby");
            return this._action.DoAction(this);
        }

        #region Godot Methods
        public override void _EnterTree()
        {
            base._EnterTree();
            this.OnAwake();
            this._action.Start(this);
        }
        #endregion

        public void OnAwake()
        {
            TurnSystem turn;
            System_Manager.GetInstance(this).TryGetSystem<TurnSystem>(out turn, true);

            turn.TryAddTurnComp(this);
        }

        public void OnSetFree()
        {
            throw new NotImplementedException();
        }

        public void Ontart()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
