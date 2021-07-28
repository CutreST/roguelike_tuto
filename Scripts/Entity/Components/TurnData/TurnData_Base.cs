using Godot;
using System;

namespace Entities.Components.TurnData
{

    public abstract class TurnData_Base : Resource
    {
        public abstract bool DoAction(in TurnComp comp);

        public abstract void Start(in Node node);

    }
}
