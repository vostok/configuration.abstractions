using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Vostok.Configuration.Abstractions.Merging;

namespace Vostok.Configuration.Abstractions.SettingsTree
{
    /// <summary>
    /// A settings tree node representing a simple value. It must be a leaf node.
    /// </summary>
    public sealed class ValueNode : ISettingsNode, IEquatable<ValueNode>
    {
        /// <summary>
        /// Creates a new <see cref="ValueNode"/> with the given <paramref name="name"/> and <paramref name="value"/>.
        /// </summary>
        public ValueNode([CanBeNull] string name, [CanBeNull] string value)
        {
            Value = value;
            Name = name ?? value;
        }

        /// <summary>
        /// Creates a new <see cref="ValueNode"/> with the given <paramref name="value"/> and no name.
        /// </summary>
        public ValueNode([CanBeNull] string value)
            : this(null, value)
        {
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string Value { get; }

        /// <inheritdoc />
        public ISettingsNode Merge(ISettingsNode other, SettingsMergeOptions options = null) => other ?? this;

        IEnumerable<ISettingsNode> ISettingsNode.Children => Enumerable.Empty<ValueNode>();

        ISettingsNode ISettingsNode.this[string name] => null;

        #region Equality

        /// <inheritdoc />
        public override bool Equals(object obj) => Equals(obj as ValueNode);

        /// <inheritdoc />
        public bool Equals(ValueNode other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Comparers.NodeName.Equals(Name, other.Name) && Value == other.Value;
        }

        /// <summary>
        /// Returns the hash code of the current <see cref="ObjectNode"/>.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                var valueHashCode = Value?.GetHashCode() ?? 0;
                var nameHashCode = Name != null ? Comparers.NodeName.GetHashCode(Name) : 0;
                return valueHashCode * 397 + nameHashCode;
            }
        }

        #endregion
    }
}