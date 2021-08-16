using Godot;
using System;
using System.Collections.Generic;

namespace Entities.Components.Items
{

    public class Item 
    {
        public enum ItemType : byte {ARMOR, WEAPON, AMULET, END}
        public ItemType MyType{ get; set; }
        public string Name{ get; set; }
        
        public List<ItemMods> Mods{ get; set; }

        public Item(in ItemType myType, in string name, in List<ItemMods> mods){
            this.Name = name;
            this.MyType = myType;
            this.Mods = mods;
        }

        public Item(in ItemType myType, in string name){
            this.Name = name;
            this.MyType = myType;
        }

    }

}