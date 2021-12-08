using System;
using JetBrains.Annotations;

namespace Vostok.Configuration.Abstractions.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// Marks a field or property optional. If the value is not present it is set to default. A value with incorrect format still produces an error, though.
    /// </summary>
    [PublicAPI]
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class OptionalAttribute : Attribute
    {
    }
}