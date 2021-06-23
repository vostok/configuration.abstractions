using System;
using JetBrains.Annotations;

namespace Vostok.Configuration.Abstractions.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// Specifies that an instance of this class should be created without constructors.
    /// Note that classes created by this way will not have default member values specified in field/property initializers.
    /// Used mainly for omitting empty parameterless constructors.
    /// BindBy attribute has higher priority, so constructorless behaviour can be overriden on fields or properties.
    /// </summary>
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class OmitConstructorsAttribute : Attribute
    {
    }
}