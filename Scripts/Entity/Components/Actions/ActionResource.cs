using Godot;
using System;

namespace Entities.Components.Actions
{

    public abstract class ActionResource : Resource
    {
        public abstract void DoAction(in Entity ent);
    }
}
