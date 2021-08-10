using Entities.Components;
using Godot;
using System;
using System.Collections.Generic;

namespace MySystems
{

    public class CollisionSystem : System_Base
    {
        List<CollisionComp> _colObjects;        

        #region add-remove from list
        public bool TryAddToCollision(in CollisionComp coll){
            if(_colObjects.Contains(coll)){
                return false;
            }

            this._colObjects.Add(coll);
            return true;
        }

        public void RemoveFromCollision(in CollisionComp coll){
            if(_colObjects.Contains(coll)){
                _colObjects.Remove(coll);
            }
        }

        public void ClearColl(){
            for (int i = 0; i < _colObjects.Count; i++){
                _colObjects[i].QueueFree();
            }

            _colObjects.Clear();
        }
        #endregion

        public bool IsColliding(in CollisionComp emiter, out CollisionComp receiver){
            for (int i = 0; i < _colObjects.Count; i++){
                if(emiter != _colObjects[i] && emiter.Position.x == _colObjects[i].Position.x && emiter.Position.y == _colObjects[i].Position.y){
                    receiver = _colObjects[i];
                    return true;
                }
            }

            receiver = null;
            return false;
        }

        #region System methods
        public override void OnEnterSystem(params object[] obj)
        {
            Messages.EnterSystem(this);
            _colObjects = new List<CollisionComp>();
        }

        public override void OnExitSystem(params object[] obj)
        {
            Messages.ExitSystem(this);
        }
        #endregion
    }
}
