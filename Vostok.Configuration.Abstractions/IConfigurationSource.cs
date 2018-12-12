using System;
using JetBrains.Annotations;
using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Configuration.Abstractions
{
    /// <summary>
    /// Provides configuration in the form of raw settings trees (<see cref="ISettingsNode"/>s).
    /// </summary>
    [PublicAPI]
    public interface IConfigurationSource
    {
        /// <summary>
        /// <para>Returns an observable sequence of raw settings along with errors encountered while updating settings.</para>
        /// <para>Implementations should comply to the following rules.</para>
        /// <para>A background worker periodically (and/or by signal) attempts to read and parse a new configuration from the underlying source.</para>
        /// <para>If the update was successful and the new configuration differs from the old one, it should be pushed to observers. The error component in pair should be null in this case.</para>
        /// <para>If the update was not successful, due to unavailability of the underlying source or inability to parse the new configuration, the pair (null, error) or (last correct value, error) should be pushed.</para>
        /// <para>It's expected for this method to be thread-safe.</para>
        /// </summary>
        [NotNull]
        IObservable<(ISettingsNode settings, Exception error)> Observe();
    }
}