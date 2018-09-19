using System;
using JetBrains.Annotations;

namespace Vostok.Configuration.Abstractions
{
    /// <summary>
    /// Provides configuration in the form of raw settings trees (<see cref="ISettingsNode"/>s).
    /// </summary>
    [PublicAPI]
    public interface IConfigurationSource
    {
        /// <summary>
        /// <para>Returns the most recent version of settings.</para>
        /// <para>Implementations should comply to the following rules.</para>
        /// <para>The returned <see cref="ISettingsNode"/> instance should be cached, so that this method is cheap and can be called freely.</para>
        /// <para>Normally this method just returns the last observed configuration (which is obtained using a dedicated subscription to <see cref="Observe"/>). Error component of the observed pairs is ignored.</para>
        /// <para>If there is no last value and the subscription is alive, this method waits for it to produce something.</para>
        /// <para>If the subscription is failed, it is created anew.</para>
        /// <para>If after all this the current subscription completes with error, that error is thrown.</para>
        /// <para>It's expected for this method to be thread-safe.</para>
        /// </summary>
        [NotNull]
        ISettingsNode Get();

        /// <summary>
        /// <para>Returns an observable sequence of raw settings along with non-fatal errors.</para>
        /// <para>Implementations should comply to the following rules.</para>
        /// <para>A background worker periodically (and/or by signal) attempts to read and parse a new configuration from the underlying source.</para>
        /// <para>If the update was successful and the new configuration differs from the old one, it is pushed to all observers. The error component in pair is null in this case.</para>
        /// <para>If the update was not successful, due to unavailability of the underlying source or inability to parse the new configuration, there are three cases.</para>
        /// <list type="number">
        /// <item><description><para>
        /// A correct configuration had been obtained in the past, and this is not the first time we are getting an error since the last successful attempt. 
        /// Then we compare the current error with the error from the previous attempt. If they differ, we push the pair (last correct settings, new error). Elsewise, we do nothing.
        /// </para></description></item>
        /// <item><description><para>
        /// A correct configuration had been obtained in the past, and this is the first time we are getting an error since the last success. Then we always push the pair (last correct settings, error).
        /// </para></description></item>
        /// <item><description><para>
        /// Correct configuration was never obtained. Then, as we don't know if the problem will ever be fixed, we have no choice but to call <see cref="IObserver{T}.OnError"/> on all observers.
        /// If the user wants to retry, they have to handle it themselves: incur a cooldown and re-subscribe to <see cref="Observe"/>. The cooldown is needed because while the source is broken all new subscribers receive <see cref="IObserver{T}.OnError"/> immediately.
        /// </para></description></item>
        /// </list>
        /// <para>New subscribers receive the last value (if it exists) immediately after subscription.</para>
        /// <para>Note that a subsсription returned by this method becomes inactive after receiving an <see cref="IObserver{T}.OnError"/> notification and <b>it's required to re-subscribe in order to receive further updates</b>.</para>
        /// <para>It's expected for this method to be thread-safe.</para>
        /// </summary>
        [NotNull]
        IObservable<(ISettingsNode settings, Exception error)> Observe();
    }
}