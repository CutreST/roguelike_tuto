namespace Base.Interfaces
{
    /// <summary>
    /// Interface for every object that needs to be updated every frame
    /// </summary>
    public interface IUpdate
    {
        /// <summary>
        /// Called at every frame
        /// </summary>
        /// <param name="delta">Delta between frames</param>
        void MyUpdate(in float delta);
    }
}

