using System;
using JetBrains.Annotations;

namespace Vostok.Configuration.Abstractions.Validation
{
    /// <summary>
    /// An error that indicates settings validation failure and encapsulates all encountered validation errors.
    /// </summary>
    [PublicAPI]
    public class SettingsValidationException : Exception
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of <see cref="SettingsValidationException"/> class.
        /// </summary>
        public SettingsValidationException(string message)
            : base(message)
        {
        }
    }
}