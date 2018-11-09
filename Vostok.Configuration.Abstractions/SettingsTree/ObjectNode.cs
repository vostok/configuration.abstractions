using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Vostok.Configuration.Abstractions.Merging;

namespace Vostok.Configuration.Abstractions.SettingsTree
{
    /// <summary>
    /// A settings tree node representing a mapping of <see cref="String"/> keys to <see cref="ISettingsNode"/> values.
    /// </summary>
    [PublicAPI]
    public sealed class ObjectNode : ISettingsNode, IEquatable<ObjectNode>
    {
        private readonly IReadOnlyDictionary<string, ISettingsNode> children;

        /// <summary>
        /// Creates a new <see cref="ObjectNode"/> with the given <paramref name="name"/> and <paramref name="children"/>.
        /// </summary>
        public ObjectNode([CanBeNull] string name, [CanBeNull] IReadOnlyDictionary<string, ISettingsNode> children)
        {
            Name = name;
            this.children = children == null
                ? null
                : new SortedDictionary<string, ISettingsNode>(children.ToDictionary(kv => kv.Key, kv => kv.Value),
                    StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Creates a new <see cref="ObjectNode"/> with the given <paramref name="name"/> and no children.
        /// </summary>
        public ObjectNode([CanBeNull] string name)
            : this(name, null)
        {
        }

        /// <summary>
        /// Creates a new <see cref="ObjectNode"/> with the given <paramref name="children"/> and no name.
        /// </summary>
        public ObjectNode([CanBeNull] IReadOnlyDictionary<string, ISettingsNode> children)
            : this(null, children)
        {
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public IEnumerable<ISettingsNode> Children => children?.Values ?? Enumerable.Empty<ObjectNode>();

        /// <inheritdoc />
        public ISettingsNode Merge(ISettingsNode other, SettingsMergeOptions options = null)
        {
            if (!(other is ObjectNode))
                return other;

            if (options == null)
                options = new SettingsMergeOptions();

            switch (options.ObjectMergeStyle)
            {
                case ObjectMergeStyle.Shallow:
                    return ShallowMerge(other, options);
                case ObjectMergeStyle.Deep:
                    return DeepMerge(other, options);
                default:
                    return null;
            }
        }

        /// <inheritdoc />
        public ISettingsNode this[string name] => children.TryGetValue(name, out var res) ? res : null;

        string ISettingsNode.Value { get; }

        private ISettingsNode DeepMerge(ISettingsNode other, SettingsMergeOptions options)
        {
            var comparer = StringComparer.InvariantCultureIgnoreCase;
            var thisNames = children.Keys.ToArray();
            var otherNames = other.Children.Select(c => c.Name).ToArray();
            var duplicates = otherNames.Intersect(thisNames, comparer).ToArray();
            var unique = thisNames.Unique(otherNames, comparer).ToArray();

            var dict = new SortedDictionary<string, ISettingsNode>(comparer);
            foreach (var name in unique)
                dict.Add(name, this[name] ?? other[name]);
            foreach (var name in duplicates)
                dict.Add(name, this[name]?.Merge(other[name], options));
            return new ObjectNode(other.Name, dict);
        }

        private ISettingsNode ShallowMerge(ISettingsNode other, SettingsMergeOptions options)
        {
            var thisNames = children.Keys.OrderBy(k => k).ToArray();
            var otherNames = other.Children.Select(c => c.Name).OrderBy(k => k).ToArray();
            if (!thisNames.SequenceEqual(otherNames, StringComparer.InvariantCultureIgnoreCase))
                return other;
            var dict = new SortedDictionary<string, ISettingsNode>();
            foreach (var name in thisNames)
                dict.Add(name, this[name]?.Merge(other[name], options));
            return new ObjectNode(other.Name, dict);
        }

        #region Equality

        /// <inheritdoc />
        public override bool Equals(object obj) => Equals(obj as ObjectNode);

        /// <inheritdoc />
        public bool Equals(ObjectNode other)
        {
            if (other == null)
                return false;

            var thisChExists = children != null;
            var otherChExists = other.children != null;

            if (Name != other.Name ||
                thisChExists != otherChExists)
                return false;

            if (thisChExists &&
                (!new HashSet<string>(children.Keys).SetEquals(other.children.Keys) ||
                 !new HashSet<ISettingsNode>(children.Values).SetEquals(other.children.Values)))
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
                var nameHashCode = Name?.GetHashCode() ?? 0;
                var hashCode = (nameHashCode * 397) ^ (children != null ? ChildrenHash() : 0);
                return hashCode;
            }

            int ChildrenHash()
            {
                var keysRes = children.Keys
                    .Select(k => k.GetHashCode())
                    .Aggregate(0, (a, b) => unchecked(a + b));
                var valsRes = children.Values
                    .Select(v => v.GetHashCode())
                    .Aggregate(0, (a, b) => unchecked(a + b));
                return unchecked(keysRes * 599) ^ valsRes;
            }
        }

        #endregion
    }
}