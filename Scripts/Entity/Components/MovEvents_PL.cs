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

        private CameraSystem _cam;

        public void OnMove(in Vector2 pos){
            _cam.Move(pos);
            _world.NewTurn(pos);
            //Messages.Print("Moveeeed");
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
            System_Manager manager = System_Manager.GetInstance(this);
            InGameSys sys;
            manager.TryGetSystem<InGameSys>(out sys);
            manager.TryGetSystem<CameraSystem>(out _cam, true);
            _world = sys.MyWorldCont;
        }

        public void Reset()
        {
            
        }
    }
}
