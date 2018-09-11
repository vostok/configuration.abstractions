using JetBrains.Annotations;

namespace Vostok.Configuration.Abstractions.Validation
{
    /// <summary>
    /// A collection of errors found in settings.
    /// </summary>
    [PublicAPI]
    public interface ISettingsValidationErrors
    {
        /// <summary>
        /// Adds another error message to the collection.
        /// </summary>
        void ReportError(string error);
    }
}