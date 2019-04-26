using JetBrains.Annotations;

namespace Vostok.Configuration.Abstractions.Merging
{
    /// <summary>
    /// Specifies the way array nodes are merged.
    /// </summary>
    [PublicAPI]
    public enum ArrayMergeStyle
    {
        /// <summary>
        /// Replace one array with another.
        /// </summary>
        Replace,

        /// <summary>
        /// Produce an array containing elements from both arrays. All elements from the first array, then all elements from the second, preserving order inside arrays.
        /// </summary>
        Concat,

        /// <summary>
        /// Produce an array containing unique items from both arrays. The order is the same as with <see cref="Concat"/>. For duplicate items, the first one is kept.
        /// </summary>
        Union,

        /// <summary>
        /// <para>Produce an array containing items obtained by merging corresponding items (by index) from both arrays.</para>
        /// <para>If merged arrays have different items count, the "tail" of the longer array is preserved as-is.</para>
        /// </summary>
        PerElement
    }
}