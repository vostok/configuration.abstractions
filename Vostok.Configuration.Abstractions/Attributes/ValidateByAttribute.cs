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
        private readonly Type validatorType;
        
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of <see cref="T:Vostok.Configuration.Abstractions.Attributes.ValidateByAttribute" /> class.
        /// </summary>
        public ValidateByAttribute(Type validatorType) => this.validatorType = validatorType;

        // TODO(krait): Should we expose it to public like this?
        /// <summary>
        /// Returns an instance of the specified validator class.
        /// </summary>
        [NotNull]
        public object Validator => Activator.CreateInstance(validatorType);
    }
}