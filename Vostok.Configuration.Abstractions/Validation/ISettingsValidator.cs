using JetBrains.Annotations;

namespace Vostok.Configuration.Abstractions.Validation
{
    /// <summary>
    /// A class responsible for validation of settings of type <typeparamref name="T"/>.
    /// </summary>
    [PublicAPI]
    public interface ISettingsValidator<in T>
    {
        /// <summary>
        /// Validates the provided <paramref name="settings"/>. Should not stop upon encountering the first error. Instead, all errors should be reported to the provided <paramref name="errors"/> collection.
        /// </summary>
        void Validate(T settings, ISettingsValidationErrors errors);
    }
}