using System;
using System.Collections.Generic;

namespace Vostok.Configuration.Abstractions.SettingsTree
{
    internal static class Comparers
    {
        public static readonly IEqualityComparer<string> NodeName = StringComparer.OrdinalIgnoreCase;
        public static readonly IComparer<string> NodeNameComparer = StringComparer.OrdinalIgnoreCase;
    }
}