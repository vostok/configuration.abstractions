using System.Collections.Generic;
using JetBrains.Annotations;

namespace Vostok.Configuration.Abstractions.Comparers
{
    [PublicAPI]
    public class SettingsCompareOptions
    {
        public IReadOnlyCollection<IReadOnlyList<string>> ExcludedPaths;
    }
}