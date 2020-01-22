using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Vostok.Commons.Helpers.Comparers;
using Path = System.Collections.Generic.List<string>;

namespace Vostok.Configuration.Abstractions.SettingsTree
{
    [PublicAPI]
    public class SettingsNodeComparer
    {
        [NotNull]
        public static IReadOnlyList<IReadOnlyList<string>> FindDifferences(
            [CanBeNull] ISettingsNode left,
            [CanBeNull] ISettingsNode right)
        {
            if (left?.Name != null || right?.Name != null)
            {
                left = new ObjectNode(null, new[] {left});
                right = new ObjectNode(null, new[] {right});
            }

            var different = new HashSet<Path>(new ListComparer<string>());
            Compare(new Path(), different, left, right);

            return different.ToList();
        }

        private static void Compare(
            [NotNull] Path path,
            [NotNull] HashSet<Path> different,
            [CanBeNull] ISettingsNode left,
            [CanBeNull] ISettingsNode right)
        {
            if (ReferenceEquals(left, right))
                return;

            if (left?.Value != right?.Value || left == null != (right == null))
                different.Add(path);

            var leftDict = ChildrenToDictionary(left);
            var rightDict = ChildrenToDictionary(right);

            var allKeys = new HashSet<string>(leftDict.Keys.Concat(rightDict.Keys));

            foreach (var key in allKeys)
            {
                leftDict.TryGetValue(key, out var l);
                rightDict.TryGetValue(key, out var r);

                var newPath = path;
                var array = left is ArrayNode || right is ArrayNode;
                if (!array)
                    newPath = path.Concat(new[] {key}).ToList();

                Compare(newPath, different, l, r);
            }
        }

        private static Dictionary<string, ISettingsNode> ChildrenToDictionary(
            [CanBeNull] ISettingsNode node)
        {
            if (node == null)
                return new Dictionary<string, ISettingsNode>();

            if (node is ArrayNode)
                return node.Children.Select((x, i) => (x, i))
                    .ToDictionary(
                        x => x.i.ToString(),
                        x => x.x);

            return node.Children.ToDictionary(x => x.Name, x => x);
        }
    }
}