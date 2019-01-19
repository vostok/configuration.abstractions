using JetBrains.Annotations;
using Vostok.Configuration.Abstractions.Merging;

namespace Vostok.Configuration.Abstractions.SettingsTree
{
    [PublicAPI]
    public static class SettingsNodeMerger
    {
        [CanBeNull]
        public static ISettingsNode Merge(
            [CanBeNull] ISettingsNode node1, 
            [CanBeNull] ISettingsNode node2, 
            [CanBeNull] SettingsMergeOptions options)
        {
            if (node1 == null)
                return node2;

            if (node2 == null)
                return node1;

            return node1.Merge(node2, options);
        }
    }
}