using System.Collections.Generic;
using JetBrains.Annotations;

namespace Vostok.Configuration.Abstractions
{
    /// <summary>
    /// A class responsible for validation of settings of type <typeparamref name="T"/>.
    /// </summary>
    [PublicAPI]
    public interface ISettingsValidator<in T>
    {
        /// <summary>
        /// Validates the provided <paramref name="settings"/>. Should not stop upon encountering the first error. Instead, all errors should be returned as an enumerable of human-readable messages.
        /// </summary>
        IEnumerable<string> Validate(T settings);
    }
}