using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Vostok.Configuration.Abstractions.Merging;
using Vostok.Commons.Collections;

namespace Vostok.Configuration.Abstractions.SettingsTree
{
    /// <summary>
    /// A settings tree node representing a mapping of <see cref="String"/> keys to <see cref="ISettingsNode"/> values.
    /// </summary>
    [PublicAPI]
    public sealed class ObjectNode : ISettingsNode, IEquatable<ObjectNode>
    {
        private readonly ImmutableArrayDictionary<string, ISettingsNode> children = ImmutableArrayDictionary<string, ISettingsNode>.Empty;

        /// <summary>
        /// Creates a new <see cref="ObjectNode"/> with the given <paramref name="name"/> and <paramref name="children"/>.
        /// </summary>
        public ObjectNode([CanBeNull] string name, [CanBeNull] ICollection<ISettingsNode> children)
        {
            Name = name;

            if (children != null)
            {
                this.children = new ImmutableArrayDictionary<string, ISettingsNode>(children.Count, Comparers.NodeName);
                foreach (var child in children)
                    this.children = this.children.Set(child.Name, child);
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

        private ObjectNode(string name, ImmutableArrayDictionary<string, ISettingsNode> children)
        {
            Name = name;
            this.children = children;
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public IEnumerable<ISettingsNode> Children => children.Values;

        /// <inheritdoc />
        public ISettingsNode Merge(ISettingsNode other, SettingsMergeOptions options = null)
        {
            if (other == null)
                return this;

            if (!(other is ObjectNode objectNodeOther))
                return other;

            if (!Comparers.NodeName.Equals(Name, other.Name))
                return other;

            options = options ?? SettingsMergeOptions.Default;

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

        /// <inheritdoc />
        public ISettingsNode this[string name] => children.TryGetValue(name, out var res) ? res : null;

        string ISettingsNode.Value { get; }

        private ISettingsNode DeepMerge(ObjectNode other, SettingsMergeOptions options)
        {
            var matchingKeys = children.Keys.Intersect(other.children.Keys, Comparers.NodeName);
            var uniqueKeys = children.Keys.Unique(other.children.Keys, Comparers.NodeName);

            var newChildren = new ImmutableArrayDictionary<string, ISettingsNode>(children.Count + other.children.Count, Comparers.NodeName);
            foreach (var key in uniqueKeys)
                newChildren = newChildren.Set(key, this[key] ?? other[key]);
            foreach (var key in matchingKeys)
                newChildren = newChildren.Set(key, this[key]?.Merge(other[key], options));

            return new ObjectNode(Name, newChildren);
        }

        private ISettingsNode ShallowMerge(ObjectNode other, SettingsMergeOptions options)
        {
            if (!children.Keys.SequenceEqual(other.children.Keys, Comparers.NodeName))
                return other;

            var newChildren = new ImmutableArrayDictionary<string, ISettingsNode>(children.Count, Comparers.NodeName);
            foreach (var key in children.Keys)
                newChildren = newChildren.Set(key, this[key]?.Merge(other[key], options));

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

            if (!new HashSet<string>(children.Keys).SetEquals(other.children.Keys) ||
                !new HashSet<ISettingsNode>(children.Values).SetEquals(other.children.Values))
                return false;

            return true;
        }

        /// <summary>
        /// Returns the hash code of the current <see cref="ObjectNode"/>.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int HashPair(KeyValuePair<string, ISettingsNode> pair) => (pair.Key?.GetHashCode() ?? 0) * 397 ^ (pair.Value?.GetHashCode() ?? 0);

                var nameHash = Name != null ? Comparers.NodeName.GetHashCode(Name) : 0;

                return nameHash * 397 ^ children.Aggregate(children.Count, (current, element) => current * 397 ^ HashPair(element));
            }
        }

        #endregion
    }
}