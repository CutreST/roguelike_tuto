/// <summary>
/// Namespace that holds all of the game systems
/// </summary>
namespace Base.Interfaces
{
    /// <summary>
    /// Interface for objects that need to be called at every physic call
    /// </summary>
    public interface IPhysic
    {
        /// <summary>
        /// Called every physic update. 
        /// </summary>
        /// <param name="delta">Time beetwen calls</param>
        void MyPhysic(in float delta);
    }
}
