using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Vostok.Configuration.Abstractions.Merging;
using Vostok.Commons.Collections;
using Vostok.Configuration.Abstractions.Extensions;

namespace Vostok.Configuration.Abstractions.SettingsTree
{
    /// <summary>
    /// A settings tree node representing a mapping of <see cref="String"/> keys to <see cref="ISettingsNode"/> values.
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
        public ObjectNode([CanBeNull] string name, [CanBeNull] [ItemNotNull] ICollection<ISettingsNode> children)
        {
            Name = name;

            if (children != null)
            {
                this.children = new Dictionary<string, ISettingsNode>(children.Count, Comparers.NodeName);
                foreach (var child in children)
                {
                    // ReSharper disable once ConstantConditionalAccessQualifier
                    if (child?.Name == null)
                        throw new ArgumentException($"All nodes from '{nameof(children)}' must have non-null names.");

                    this.children[child.Name] = child;
                }
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

        private ObjectNode(string name, Dictionary<string, ISettingsNode> children)
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
        public ISettingsNode this[string name] => children.TryGetValue(name, out var value) ? value : null;

        string ISettingsNode.Value { get; }

        private ISettingsNode DeepMerge(ObjectNode other, SettingsMergeOptions options)
        {
            var newKeys = new HashSet<string>(children.Keys, Comparers.NodeName);
            newKeys.UnionWith(other.children.Keys);

            var newChildren = new Dictionary<string, ISettingsNode>(newKeys.Count, Comparers.NodeName);
            foreach (var key in newKeys)
            {
                var thisHasValue = children.TryGetValue(key, out var thisValue);
                var otherHasValue = other.children.TryGetValue(key, out var otherValue);

                if (thisHasValue && otherHasValue)
                    newChildren[key] = thisValue.Merge(otherValue, options);
                else
                    newChildren[key] = thisHasValue ? thisValue : otherValue;
            }
            
            return new ObjectNode(Name, newChildren);
        }

        private ISettingsNode ShallowMerge(ObjectNode other, SettingsMergeOptions options)
        {
            if (!children.Keys.SetEquals(other.children.Keys, Comparers.NodeName))
                return other;

            var newChildren = new Dictionary<string, ISettingsNode>(children.Count, Comparers.NodeName);
            foreach (var pair in children)
                newChildren[pair.Key] = pair.Value?.Merge(other[pair.Key], options);

            return new ObjectNode(Name, newChildren);
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

            return children.Values.SetEquals(other.children.Values);
        }

        /// <summary>
        /// Returns the hash code of the current <see cref="ObjectNode"/>.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int HashPair(KeyValuePair<string, ISettingsNode> pair) => ((pair.Key?.GetHashCode() ?? 0) * 397) ^ (pair.Value?.GetHashCode() ?? 0);

                var nameHash = Name != null ? Comparers.NodeName.GetHashCode(Name) : 0;

                return (nameHash * 397) ^ children.Aggregate(children.Count, (current, element) => (current * 397) ^ HashPair(element));
            }
        }

        #endregion
    }
}