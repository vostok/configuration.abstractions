using System;
using JetBrains.Annotations;

namespace Vostok.Configuration.Abstractions.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// <para>Marks a field/property/class secret. External loggers should not expose values of settings marked with this attribute.</para>
    /// <para>Useful for sensitive settings, such as API-keys, passwords and tokens.</para>
    /// </summary>
    [PublicAPI]
    [MeansImplicitUse(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Interface)]
    public class SecretAttribute : Attribute
    {
    }
}