using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Vostok.Configuration.Abstractions.Merging;

namespace Vostok.Configuration.Abstractions.SettingsTree
{
    /// <summary>
    /// A settings tree node representing an array of items.
    /// </summary>
    [PublicAPI]
    public sealed class ArrayNode : ISettingsNode, IEquatable<ArrayNode>
    {
        private readonly IReadOnlyList<ISettingsNode> children;

        /// <summary>
        /// Creates a new <see cref="ArrayNode"/> with the given <paramref name="name"/> and <paramref name="children"/>.
        /// </summary>
        public ArrayNode([CanBeNull] string name, [CanBeNull] IReadOnlyList<ISettingsNode> children)
        {
            Name = name;
            this.children = children ?? Array.Empty<ISettingsNode>();
        }

        /// <summary>
        /// Creates a new <see cref="ArrayNode"/> with the given <paramref name="name"/> and no children.
        /// </summary>
        public ArrayNode([CanBeNull] string name)
            : this(name, null)
        {
        }

        /// <summary>
        /// Creates a new <see cref="ArrayNode"/> with the given <paramref name="children"/> and no name.
        /// </summary>
        public ArrayNode([CanBeNull] IReadOnlyList<ISettingsNode> children)
            : this(null, children)
        {
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public IEnumerable<ISettingsNode> Children => children;

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

            if (!(other is ArrayNode otherArrayNode))
                return other;

            switch (options.ArrayMergeStyle)
            {
                case ArrayMergeStyle.Replace:
                    return other;

                case ArrayMergeStyle.Concat:
                    return new ArrayNode(other.Name, children.Concat(other.Children).ToArray());

                case ArrayMergeStyle.Union:
                    return new ArrayNode(other.Name, children.Union(other.Children).ToArray());

                case ArrayMergeStyle.PerElement:
                    var newChildren = children.Zip(other.Children, (c1, c2) => SettingsNodeMerger.Merge(c1, c2, options));

                    if (ChildrenCount > otherArrayNode.ChildrenCount)
                        newChildren = newChildren.Concat(Children.Skip(otherArrayNode.ChildrenCount));

                    if (ChildrenCount < otherArrayNode.ChildrenCount)
                        newChildren = newChildren.Concat(other.Children.Skip(ChildrenCount));

                    return new ArrayNode(other.Name, newChildren.ToArray());

                default:
                    return null;
            }
        }

        public override string ToString() => SettingsNodeRenderer.Render(this);

        string ISettingsNode.Value { get; }

        ISettingsNode ISettingsNode.this[string name] => null;

        #region Equality

        /// <inheritdoc />
        public override bool Equals(object obj) => Equals(obj as ArrayNode);

        /// <inheritdoc />
        public bool Equals(ArrayNode other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Comparers.NodeName.Equals(Name, other.Name) && children.SequenceEqual(other.children);
        }

        /// <summary>
        /// Returns the hash code of the current <see cref="ArrayNode"/>.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                var nameHash = Name != null ? Comparers.NodeName.GetHashCode(Name) : 0;

                return (nameHash * 397) ^ children.Aggregate(children.Count, (current, element) => (current * 397) ^ (element?.GetHashCode() ?? 0));
            }
        }

        #endregion
    }
}