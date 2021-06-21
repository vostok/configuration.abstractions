using System;
using JetBrains.Annotations;

namespace Vostok.Configuration.Abstractions.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// Specifies that an instance of this class may be created without parameterless constructors.
    /// Note that classes created by this way will not have default member values specified in field/property initializers.
    /// </summary>
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Class)]
    public class OmittedConstructorsAttribute : Attribute
    {
    }
}