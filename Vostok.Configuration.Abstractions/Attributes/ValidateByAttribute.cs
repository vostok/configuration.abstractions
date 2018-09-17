using System;
using JetBrains.Annotations;

namespace Vostok.Configuration.Abstractions.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// Specifies that settings of this type should be validated by the provided validator.
    /// </summary>
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ValidateByAttribute : Attribute
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of <see cref="T:Vostok.Configuration.Abstractions.Attributes.ValidateByAttribute" /> class.
        /// </summary>
        public ValidateByAttribute([NotNull] Type validatorType) => 
            ValidatorType = validatorType ?? throw new ArgumentNullException(nameof(validatorType));

        /// <summary>
        /// The specified validator type.
        /// </summary>
        [NotNull]
        public Type ValidatorType { get; }
    }
}