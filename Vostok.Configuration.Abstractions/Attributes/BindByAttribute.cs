using System;
using JetBrains.Annotations;

namespace Vostok.Configuration.Abstractions.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// Specifies that settings of this type should be bound by a settings binder of provided type.
    /// </summary>
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class BindByAttribute : Attribute
    {
        public BindByAttribute([NotNull] Type binderType) => 
            BinderType = binderType ?? throw new ArgumentNullException(nameof(binderType));

        /// <summary>
        /// The specified binder type.
        /// </summary>
        [NotNull]
        public Type BinderType { get; }
    }
}