using Godot;
using System;

namespace Entities.Components.Items
{

    public struct ItemMods 
    {
        public enum ModType{DEFFENSE, ATTACK, HEALTH}
        public ModType MyType{ get; set; }
        public int Value{ get; set; }

        public ItemMods(in ModType myType, in int value ){
            this.MyType = myType;
            this.Value = value;
        }

    }
}
