using Godot;
using MySystems;
using System;
using System.Collections.Generic;

namespace World
{
    public enum EnitityType : byte { TROLL, ORC, EMPTY };

    public class SpawnSystem : System_Base
    {
        Dictionary<EnitityType, string> _typeAndPath;


        public bool TrySpawnEntity(in EnitityType entTipe, in Vector2 pos, in Node parent, out Entities.Entity ent)
        {
            string a;            

            if (_typeAndPath.TryGetValue(entTipe, out a))
            {
                PackedScene p = GD.Load<PackedScene>(a);
                ent = p.Instance<Entities.Entity>();
                //Entities.Entity temp = GD.Load<Entities.Entity>(a);
                ent.GlobalPosition = pos;
                parent.AddChild(ent);
                return true;
            }

            ent = null;
            return false;

        }


        private void PopulateDictionary()
        {
            _typeAndPath = new Dictionary<EnitityType, string>();

            _typeAndPath.Add(EnitityType.ORC, "res://Scenes/Entities/Orc.tscn");
            _typeAndPath.Add(EnitityType.TROLL, "res://Scenes/Entities/Troll.tscn");
        }

        public override void OnEnterSystem(params object[] obj)
        {
            Messages.EnterSystem(this);
            this.PopulateDictionary();

        }

        public override void OnExitSystem(params object[] obj)
        {
            _typeAndPath.Clear();
            _typeAndPath = null;
        }
    }
}
