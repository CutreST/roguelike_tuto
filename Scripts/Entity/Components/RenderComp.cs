using Godot;
using MySystems;
using System;
using World;

namespace Entities.Components
{

    public class RenderComp : Sprite, IComponentNode
    {
        public Entity MyEntity { get; set; }

        #region Godot Methods
        public override void _ExitTree()
        {
            this.OnSetFree();
        }
        #endregion
        public void OnAwake()
        {
            throw new NotImplementedException();
        }

        public void OnSetFree()
        {
            //esto ira en render component
            //además cambiaremos el sprite cuando sea, blah
            RenderSystem render;

            if (System_Manager.GetInstance(this).TryGetSystem<RenderSystem>(out render))
            {
                render.RemoveEntityFromRender(this.MyEntity);
            }
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
