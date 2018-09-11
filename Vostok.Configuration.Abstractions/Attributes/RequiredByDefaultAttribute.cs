using System;
using JetBrains.Annotations;

namespace Vostok.Configuration.Abstractions.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// Marks all fields and properties required by default.
    /// </summary>
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class RequiredByDefaultAttribute : Attribute
    {
    }
}