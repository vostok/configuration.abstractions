using System;
using JetBrains.Annotations;

namespace Vostok.Configuration.Abstractions.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// Marks all fields and properties required by default. See <see cref="RequiredAttribute"/> for more details.
    /// </summary>
    [PublicAPI]
    [MeansImplicitUse(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.Members)]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
    public class RequiredByDefaultAttribute : Attribute
    {
    }
}