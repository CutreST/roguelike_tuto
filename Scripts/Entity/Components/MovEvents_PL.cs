using Godot;
using MySystems;
using System;
using World;

namespace Entities.Components
{

    public class MovEvents_PL : Node, IComponentNode
    {
        public Entity MyEntity { get; set; }

        private WorldMapCont _world;

        public void OnMove(in Vector2 pos){
            _world.PaintFOV(pos);
            Messages.Print("Moveeeed");
        }

        public override void _Ready(){
            this.Ontart();
        }

        public void OnAwake()
        {
           
        }

        public void OnSetFree()
        {
            
        }

        public void Ontart()
        {
             InGameSys sys;
            System_Manager.GetInstance(this).TryGetSystem<InGameSys>(out sys);

            _world = sys.MyWorldCont;
        }

        public void Reset()
        {
            
        }
    }
}
