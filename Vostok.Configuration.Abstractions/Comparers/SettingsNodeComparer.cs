using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Vostok.Commons.Helpers.Comparers;
using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Configuration.Abstractions.Comparers
{
    [PublicAPI]
    public class SettingsNodeComparer
    {
        private readonly SettingsCompareOptions options;

        public SettingsNodeComparer(SettingsCompareOptions options = null) =>
            this.options = options ?? new SettingsCompareOptions();

        public bool Equals(
            [CanBeNull] ISettingsNode left,
            [CanBeNull] ISettingsNode right)
        {
            if (ShouldExcludePath(new List<string>()))
                return true;

            if (ShouldExcludePath(new List<string> {left?.Name}))
                left = null;

            if (ShouldExcludePath(new List<string> {right?.Name}))
                right = null;

            if (left?.Name != right?.Name)
                return false;

            var path = new List<string>();
            if (left?.Name != null)
                path.Add(left?.Name);
            return EquivalentInner(path, left, right);
        }

        private bool EquivalentInner(
            [NotNull] List<string> path,
            [CanBeNull] ISettingsNode left,
            [CanBeNull] ISettingsNode right)
        {
            if (ReferenceEquals(left, right))
                return true;

            if (left == null || right == null)
                return false;

            if (left.Name != right.Name)
                return false;

            if (left is ValueNode leftValue && right is ValueNode rightValue)
                return leftValue.Value == rightValue.Value;

            if (left is ObjectNode leftObject && right is ObjectNode rightObject)
                return EquivalentObject(path, leftObject.Children, rightObject.Children);

            if (left is ArrayNode leftArray && right is ArrayNode rightArray)
                return EquivalentArray(path, leftArray.Children, rightArray.Children);

            return false;
        }

        private bool EquivalentArray(
            List<string> path,
            IEnumerable<ISettingsNode> left,
            IEnumerable<ISettingsNode> right)
        {
            var leftSorted = left.OrderBy(x => x.Name).ToList();
            var rightSorted = right.OrderBy(x => x.Name).ToList();

            if (leftSorted.Count != rightSorted.Count)
                return false;

            for (var i = 0; i < leftSorted.Count; i++)
            {
                if (!EquivalentInner(path, leftSorted[i], rightSorted[i]))
                    return false;
            }

            return true;
        }

        private bool EquivalentObject(
            List<string> path,
            IEnumerable<ISettingsNode> leftList,
            IEnumerable<ISettingsNode> rightList)
        {
            var left = leftList.ToDictionary(x => x.Name);
            var right = rightList.ToDictionary(x => x.Name);

            var allKeys = new HashSet<string>(left.Keys.Concat(right.Keys));

            foreach (var key in allKeys)
            {
                left.TryGetValue(key, out var l);
                right.TryGetValue(key, out var r);

                path.Add(key);

                if (!ShouldExcludePath(path) && !EquivalentInner(path, l, r))
                    return false;

                path.RemoveAt(path.Count - 1);
            }

            return true;
        }

        private bool ShouldExcludePath(
            [NotNull] IReadOnlyList<string> path) =>
            options?.ExcludedPaths != null && options.ExcludedPaths.Any(p => ListComparer<string>.Instance.Equals(p, path));
    }
}