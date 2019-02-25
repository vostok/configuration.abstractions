using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;

namespace Vostok.Configuration.Abstractions.SettingsTree
{
    /// <summary>
    /// <para>A builder that allows to efficiently transform an immutable <see cref="ObjectNode"/>.</para>
    /// <para><see cref="ObjectNodeBuilder"/> is designed for one-time usage: use <see cref="SetChild"/> method and <see cref="Name"/> property to change node content and then call <see cref="Build"/>.</para>
    /// </summary>
    [PublicAPI]
    public class ObjectNodeBuilder
    {
        private const int BuildingState = 0;
        private const int BuiltState = 1;

        private int state = BuildingState;

        /// <summary>
        /// Creates a new <see cref="ObjectNodeBuilder"/> with <c>null</c> <see cref="Name"/> and empty children.
        /// </summary>
        public ObjectNodeBuilder()
            : this(null as string)
        {
        }

        /// <summary>
        /// Creates a new <see cref="ObjectNodeBuilder"/> with given <paramref name="name"/> and empty children.
        /// </summary>
        public ObjectNodeBuilder([CanBeNull] string name)
        {
            Name = name;

            Children = new Dictionary<string, ISettingsNode>(Comparers.NodeName);
        }

        /// <summary>
        /// Creates a new <see cref="ObjectNodeBuilder"/> starting with contents of given <paramref name="node"/>.
        /// </summary>
        public ObjectNodeBuilder([NotNull] ObjectNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            Name = node.Name;

            Children = node.Children.ToDictionary(child => child.Name, child => child, Comparers.NodeName);
        }

        /// <summary>
        /// Gets or sets node name.
        /// </summary>
        [CanBeNull]
        public string Name { get; set; }

        /// <summary>
        /// <para>Builds a new <see cref="ObjectNode"/> based on current builder's contents.</para>
        /// <para>Note that it's not safe to continue using an instance of <see cref="ObjectNodeBuilder"/> after calling <see cref="Build"/>.</para>
        /// </summary>
        [NotNull]
        public ObjectNode Build()
        {
            Interlocked.Exchange(ref state, BuiltState);

            return new ObjectNode(this);
        }

        /// <summary>
        /// Saves given child node. Any existing child with the same name gets rewritten.
        /// </summary>
        /// <exception cref="ArgumentNullException">Provided child node is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Provided child node has <c>null</c> name.</exception>
        /// <exception cref="InvalidOperationException"><see cref="Build"/> has been called earlier.</exception>
        public void SetChild([NotNull] ISettingsNode child)
        {
            if (child == null)
                throw new ArgumentNullException(nameof(child));

            if (child.Name == null)
                throw new ArgumentException("All child nodes must have non-null names.");

            EnsureNotBuiltYet();

            Children[child.Name] = child;
        }

        /// <summary>
        /// Removes a child node with given <paramref name="name"/> if it exists.
        /// </summary>
        /// <exception cref="ArgumentNullException">Provided child name is <c>null</c>.</exception>
        public void RemoveChild(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            EnsureNotBuiltYet();

            Children.Remove(name);
        }

        [NotNull]
        internal Dictionary<string, ISettingsNode> Children { get; }

        private void EnsureNotBuiltYet()
        {
            if (state == BuiltState)
                throw new InvalidOperationException($"This {nameof(ObjectNodeBuilder)} has already been used to build a node.");
        }
    }
}
