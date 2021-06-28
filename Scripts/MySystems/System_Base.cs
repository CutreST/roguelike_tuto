using System;


namespace MySystems
{
    /// <summary>
    /// An abstract class for every posible system
    /// </summary>
    public abstract class System_Base 
    {
        public System_Manager MyManager { get; set; }        

        /// <summary>
        /// Raised when enter the system
        /// </summary>
        /// <param name="obj">Posible objects</param>
        public abstract void OnEnterSystem(params object[] obj);

        /// <summary>
        /// Raised when exiting the system
        /// </summary>
        /// <param name="obj">Posible objects</param>
        public abstract void OnExitSystem(params object[] obj);
    }
}
