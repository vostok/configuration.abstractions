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

        /// <inheritdoc />
        public ISettingsNode Merge(ISettingsNode other, SettingsMergeOptions options = null)
        {
            if (other == null)
                return this;

            if (!(other is ArrayNode))
                return other;

            if (options == null)
                options = new SettingsMergeOptions();

            switch (options.ArrayMergeStyle)
            {
                case ArrayMergeStyle.Replace:
                    return other;
                case ArrayMergeStyle.Concat:
                    return new ArrayNode(Name, children.Concat(other.Children).ToArray());
                case ArrayMergeStyle.Union:
                    return new ArrayNode(Name, children.Union(other.Children).ToArray());
                default:
                    return null;
            }
        }

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

            var thisChExists = children != null;
            var otherChExists = other.children != null;

            if (Name != other.Name ||
                thisChExists != otherChExists)
                return false;

            if (thisChExists && !new HashSet<ISettingsNode>(children).SetEquals(other.children))
                return false;

            return true;
        }

        /// <summary>
        /// Returns the hash code of the current <see cref="ArrayNode"/>.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                var nameHashCode = Name?.GetHashCode() ?? 0;
                var hashCode = (nameHashCode * 397) ^ (children != null ? ChildrenHash() : 0);
                return hashCode;
            }

            int ChildrenHash()
            {
                var res = children
                    .Select(k => k.GetHashCode())
                    .Aggregate(0, (a, b) => unchecked(a + b));
                return unchecked(res * 599);
            }
        }

        #endregion
    }
}