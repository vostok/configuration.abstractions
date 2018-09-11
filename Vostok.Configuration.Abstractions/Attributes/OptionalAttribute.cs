using System;
using JetBrains.Annotations;

namespace Vostok.Configuration.Abstractions.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// Marks a field or property optional. If the value cannot be parsed it is set to default.
    /// </summary>
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class OptionalAttribute : Attribute
    {
    }
}