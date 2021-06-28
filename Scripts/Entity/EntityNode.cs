using Godot;

namespace Entities
{
    /// <summary>
    /// The entity node
    /// </summary>
    public class EntityNode : Node
    {        
        /// <summary>
        /// The entity 
        /// </summary>
        public Entity MyEntity { get; set; }        

        #region Godot methods
        /// <summary>
        /// Call on Awake unity. Creates a new <see cref="Entity"/> if <see cref="MyEntity"/> is null
        /// </summary>
        public override void _EnterTree()
        {
            if (this.MyEntity == null)
            {
                this.MyEntity = new Entity(this);
                GD.Print("Created entity at ", base.Name);
            }            
        }
       
        public override void _Ready()
        {            
            MyEntity.OnStart(this);
        }
       
        #endregion


    }
}
