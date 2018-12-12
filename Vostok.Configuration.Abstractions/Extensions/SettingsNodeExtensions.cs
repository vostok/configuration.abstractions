using System;
using JetBrains.Annotations;

namespace Vostok.Configuration.Abstractions.SettingsTree
{
    /// <summary>
    /// A set of extensions for <see cref="ISettingsNode"/>.
    /// </summary>
    public static class SettingsNodeExtensions
    {
        /// <summary>
        /// <para>Returns a subtree of the given settings tree. The subtree is located through descending into the tree by keys specified in <paramref name="scope"/>. Only <see cref="ObjectNode"/>s can be present along the path.</para>
        /// <para>If the given path is not present in the tree, <c>null</c> is returned.</para>
        /// </summary>
        [CanBeNull]
        public static ISettingsNode ScopeTo([CanBeNull] this ISettingsNode node, [NotNull] [ItemNotNull] params string[] scope)
        {
            if (scope == null)
                throw new ArgumentNullException(nameof(scope));

            foreach (var segment in scope)
            {
                if (segment == null)
                    throw new ArgumentException($"'{nameof(scope)}' should not contain null items.");

                if (node == null)
                    return null;

                node = node[segment];
            }

            return node;
        }
    }
}