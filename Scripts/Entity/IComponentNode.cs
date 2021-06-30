using Godot;
using System;

namespace Entities.Components
{
    /// <summary>
    /// Interface for all the components nodes.
    /// <para>
    /// As Godot uses a true OOP, its easy to use the interface
    /// </para>
    /// </summary>
    public interface IComponentNode
    {
        /// <summary>
        /// The <see cref="Entity"/>, father of the component. Each <see cref="Entity"/> has a list components marked with this interface
        /// </summary>
        Entity MyEntity { get; set; }
        //Entity MyEntity { get; set; }

        /// <summary>
        /// Ready method
        /// </summary>
        void Ontart();

        /// <summary>
        /// Enter tree method
        /// </summary>
        void OnAwake();

        /// <summary>
        /// Called when setted free
        /// </summary>
        void OnSetFree();

        void Reset();

    }
}
