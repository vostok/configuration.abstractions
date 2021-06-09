using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Vostok.Configuration.Abstractions.Merging;

namespace Vostok.Configuration.Abstractions.SettingsTree
{
    /// <summary>
    /// A settings tree node representing a mapping of <see cref="string"/> keys to <see cref="ISettingsNode"/> values.
    /// </summary>
    [PublicAPI]
    public sealed class ObjectNode : ISettingsNode, IEquatable<ObjectNode>
    {
        private static readonly Dictionary<string, ISettingsNode> EmptyDictionary = new Dictionary<string, ISettingsNode>();

        private readonly Dictionary<string, ISettingsNode> children = EmptyDictionary;

        /// <summary>
        /// <para>Creates a new <see cref="ObjectNode"/> with the given <paramref name="name"/> and <paramref name="children"/>.</para>
        /// <para>All provided child nodes must have non-null names.</para>
        /// </summary>
        public ObjectNode([CanBeNull] string name, [CanBeNull] [ItemNotNull] IEnumerable<ISettingsNode> children)
        {
            Name = name;

            if (children == null)
                return;

            foreach (var child in children)
            {
                // ReSharper disable once ConstantConditionalAccessQualifier
                if (child?.Name == null)
                    throw new ArgumentException($"All nodes from '{nameof(children)}' must have non-null names.");

                if (this.children == EmptyDictionary)
                {
                    var childrenCapacity = children is ICollection<ISettingsNode> collection ? collection.Count : 4;

                    this.children = new Dictionary<string, ISettingsNode>(childrenCapacity, Comparers.NodeName);
                }

                this.children[child.Name] = child;
            }
        }

        /// <summary>
        /// Creates a new <see cref="ObjectNode"/> with the given <paramref name="name"/> and no children.
        /// </summary>
        public ObjectNode([CanBeNull] string name)
            : this(name, null as ICollection<ISettingsNode>)
        {
        }

        /// <summary>
        /// Creates a new <see cref="ObjectNode"/> with the given <paramref name="children"/> and no name.
        /// </summary>
        public ObjectNode([CanBeNull] ICollection<ISettingsNode> children)
            : this(null, children)
        {
        }

        internal ObjectNode(ObjectNodeBuilder builder)
            : this(builder.Name, builder.Children)
        {
        }

        private ObjectNode(string name, Dictionary<string, ISettingsNode> children)
        {
            Name = name;
            this.children = children;
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public IEnumerable<ISettingsNode> Children => children.Values;

        public int ChildrenCount => children.Count;

        /// <inheritdoc />
        public ISettingsNode Merge(ISettingsNode other, SettingsMergeOptions options = null)
        {
            options = options ?? SettingsMergeOptions.Default;

            if (options.CustomMerge != null)
            {
                var (ok, merged) = options.CustomMerge(this, other);
                if (ok)
                    return merged;
            }

            if (other == null)
                return this;

            if (!(other is ObjectNode objectNodeOther))
                return other;

            if (children.Count == 0)
                return other;

            if (objectNodeOther.children.Count == 0)
                return this;

            switch (options.ObjectMergeStyle)
            {
                case ObjectMergeStyle.Shallow:
                    return ShallowMerge(objectNodeOther, options);
                case ObjectMergeStyle.Deep:
                    return DeepMerge(objectNodeOther, options);
                default:
                    return null;
            }
        }

        public override string ToString() => SettingsNodeRenderer.Render(this);

        /// <summary>
        /// Returns a new <see cref="ObjectNodeBuilder"/> based on this <see cref="ObjectNode"/>'s contents.
        /// </summary>
        public ObjectNodeBuilder ToBuilder() => new ObjectNodeBuilder(this);

        /// <inheritdoc />
        public ISettingsNode this[string name] => children.TryGetValue(name, out var value) ? value : null;

        string ISettingsNode.Value { get; }

        private ISettingsNode DeepMerge(ObjectNode other, SettingsMergeOptions options)
        {
            var newChildren = new Dictionary<string, ISettingsNode>(children, Comparers.NodeName);

            foreach (var pair in other.children)
            {
                newChildren[pair.Key] = SettingsNodeMerger.Merge(this[pair.Key], pair.Value, options);
            }

            return new ObjectNode(other.Name, newChildren);
        }

        private ISettingsNode ShallowMerge(ObjectNode other, SettingsMergeOptions options)
        {
            if (other.children.Count != children.Count)
                return other;

            var newChildren = new Dictionary<string, ISettingsNode>(children.Count, Comparers.NodeName);

            foreach (var pair in children)
            {
                if (!other.children.TryGetValue(pair.Key, out var otherValue))
                    return other;

                newChildren[pair.Key] = SettingsNodeMerger.Merge(pair.Value, otherValue, options);
            }

            return new ObjectNode(other.Name, newChildren);
        }

        #region Equality

        /// <inheritdoc />
        public override bool Equals(object obj) => Equals(obj as ObjectNode);

        /// <inheritdoc />
        public bool Equals(ObjectNode other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (!Comparers.NodeName.Equals(Name, other.Name))
                return false;

            if (children.Count != other.children.Count)
                return false;

            foreach (var pair in children)
            {
                if (!other.children.TryGetValue(pair.Key, out var otherValue) || !pair.Value.Equals(otherValue))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the hash code of the current <see cref="ObjectNode"/>.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int HashPair(KeyValuePair<string, ISettingsNode> pair)
                    => ((pair.Key?.GetHashCode() ?? 0) * 397) ^ (pair.Value?.GetHashCode() ?? 0);

                var nameHash = Name != null ? Comparers.NodeName.GetHashCode(Name) : 0;

                return (nameHash * 397) ^ children.Aggregate(children.Count, (current, element) => current ^ HashPair(element));
            }
        }

        #endregion
    }
}