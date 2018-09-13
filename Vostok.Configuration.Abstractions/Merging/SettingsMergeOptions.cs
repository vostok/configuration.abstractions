using JetBrains.Annotations;

namespace Vostok.Configuration.Abstractions.Merging
{
    /// <summary>
    /// Specifies the way to merge settings trees.
    /// </summary>
    [PublicAPI]
    public class SettingsMergeOptions
    {
        /// <summary>
        /// The way to merge object nodes.
        /// </summary>
        public ObjectMergeStyle ObjectMergeStyle { get; set; } = ObjectMergeStyle.Deep;

        /// <summary>
        /// The way to merge array nodes.
        /// </summary>
        public ArrayMergeStyle ArrayMergeStyle { get; set; } = ArrayMergeStyle.Concat;
    }
}