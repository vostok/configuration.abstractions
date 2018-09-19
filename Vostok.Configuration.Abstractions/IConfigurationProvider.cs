using System;
using JetBrains.Annotations;

namespace Vostok.Configuration.Abstractions
{
    /// <summary>
    /// Provides settings for your application, fresh and warm.
    /// </summary>
    [PublicAPI]
    public interface IConfigurationProvider
    {
        /// <summary>
        /// <para>Returns the most recent value of <typeparamref name="TSettings"/> from preconfigured configuration sources.</para>
        /// <para>Implementations should comply to the following rules.</para>
        /// <para>If there is no source preconfigured for <typeparamref name="TSettings"/>, an exception is thrown immediately.</para>
        /// <para>If the source is found, this method uses the corresponding subscription and works exactly the same as <see cref="IConfigurationSource.Get"/> in <see cref="IConfigurationSource"/>.</para>
        /// <para>Note: <see cref="Get{TSettings}()"/> should be implemented over <see cref="Observe{TSettings}()"/> so that exceptions are reported to the provided error callback (see implementation-specific docs for details).</para>
        /// <para>It's expected for this method to be extremely cheap and be called each time the app needs access to settings.</para>
        /// <para>It's also expected for this method to be thread-safe.</para>
        /// </summary>
        [NotNull]
        TSettings Get<TSettings>();

        /// <summary>
        /// <para>Returns the most recent value of <typeparamref name="TSettings"/> from the given <paramref name="source"/>.</para>
        /// <para>Implementations should comply to the following rules.</para>
        /// <para>If there is no source preconfigured for <typeparamref name="TSettings"/>, an exception is thrown immediately.</para>
        /// <para>If the source is found, this method uses the corresponding subscription and works exactly the same as <see cref="IConfigurationSource.Get"/> in <see cref="IConfigurationSource"/>.</para>
        /// <para>Note: <see cref="Get{TSettings}()"/> should be implemented over <see cref="Observe{TSettings}()"/> so that exceptions are reported to the provided error callback (see implementation-specific docs for details).</para>
        /// <para>All required state (cached subscription, cached last value and exception) should be stored in a limited-size cache keyed by <see cref="IConfigurationSource"/> instance.</para>
        /// <para>It's expected for this method to be extremely cheap and be called each time the app needs access to settings.</para>
        /// <para>It's also expected for this method to be thread-safe.</para>
        /// </summary>
        [NotNull]
        TSettings Get<TSettings>([NotNull] IConfigurationSource source);

        /// <summary>
        /// <para>Returns an <see cref="IObservable{T}"/> that receives the new value of <typeparamref name="TSettings"/> each time it is updated from its corresponding preconfigured source.</para>
        /// <para>Implementations should comply to the following rules.</para>
        /// <para>If there is no source preconfigured for <typeparamref name="TSettings"/>, an exception is thrown immediately.</para>
        /// <para>This method should be implemented using <see cref="ObserveWithErrors{TSettings}()"/>. The only difference is that instead of pushing errors to user this method reports them to the provided error callback (see implementation-specific docs for details).</para>
        /// <para>It's also expected for this method to be thread-safe.</para>
        /// </summary>
        [NotNull]
        IObservable<TSettings> Observe<TSettings>();

        /// <summary>
        /// <para>Returns an <see cref="IObservable{T}"/> that receives the new value of <typeparamref name="TSettings"/> each time it is updated from the given <paramref name="source"/>.</para>
        /// <para>Implementations should comply to the following rules.</para>
        /// <para>If there is no source preconfigured for <typeparamref name="TSettings"/>, an exception is thrown immediately.</para>
        /// <para>This method should be implemented using <see cref="ObserveWithErrors{TSettings}()"/>. The only difference is that instead of pushing errors to user this method reports them to the provided error callback (see implementation-specific docs for details).</para>
        /// <para>All required state (cached subscription, cached last value and exception) should be stored in a limited-size cache keyed by <see cref="IConfigurationSource"/> instance.</para>
        /// <para>It's also expected for this method to be thread-safe.</para>
        /// </summary>
        [NotNull]
        IObservable<TSettings> Observe<TSettings>([NotNull] IConfigurationSource source);

        /// <summary>
        /// <para>Returns an <see cref="IObservable{T}"/> that receives the new value of <typeparamref name="TSettings"/> each time it is updated from its corresponding preconfigured source along with errors arising in process of doing so.</para>
        /// <para>Implementations should comply to the following rules.</para>
        /// <para>If there is no source preconfigured for <typeparamref name="TSettings"/>, an exception is thrown immediately.</para>
        /// <para>Else, a subscription to the corresponding source is created once for given <typeparamref name="TSettings"/> (see <see cref="IConfigurationSource.Observe"/> for sources). Events that occur on that subscription are handled as follows:</para>
        /// <list type="number">
        /// <item><description><para>
        /// If the received new pair has no exception and can be successfully bound and validated, we push the pair (new settings, null).
        /// </para></description></item>
        /// <item><description><para>
        /// If the received new pair has no exception but the new config fails during binding or validation, we look for a saved successful settings instance. If it exists, and the current error differs from error corresponding to the saved successful instance, we push pair (last successful settings, new error). If there is no previous successful instance, we call <see cref="IObserver{T}.OnError"/> on subscribers.
        /// </para></description></item>
        /// <item><description><para>
        /// If the received new pair has an exception, we push (settings, error) even if the settings didn't change.
        /// </para></description></item>
        /// <item><description><para>
        /// If we receive <see cref="IObserver{T}.OnError"/>: if there is no saved successful instance, we call <see cref="IObserver{T}.OnError"/> ourselves. If there is a successful instance, we push (last successful settings, error from source) and re-subscribe to source after a cooldown. Nothing is pushed if after resubscription we still get an error and the error is the same. This re-subscription loop may continue forever if the source doesn't heal.
        /// </para></description></item>
        /// </list>
        /// <para>If there is an exception in the pair received from the source and there is an exception during binding/validation, they are packed into an <see cref="AggregateException"/>.</para>
        /// <para>If a source provides an exception with the first version of settings, the exception is passed through.</para>
        /// <para>New subscribers receive the last value (if it exists) immediately after subscription.</para>
        /// <para>Note that a subsсription returned by this method becomes inactive after receiving an <see cref="IObserver{T}.OnError"/> notification and <b>it's required to re-subscribe in order to receive further updates</b>.</para>
        /// <para>It's also expected for this method to be thread-safe.</para>
        /// </summary>
        [NotNull]
        IObservable<(TSettings settings, Exception error)> ObserveWithErrors<TSettings>();

        /// <summary>
        /// <para>Returns an <see cref="IObservable{T}"/> that receives the new value of <typeparamref name="TSettings"/> each time it is updated from the given <paramref name="source"/> along with errors arising in process of doing so.</para>
        /// <para>Implementations should comply to the following rules.</para>
        /// <para>If there is no source preconfigured for <typeparamref name="TSettings"/>, an exception is thrown immediately.</para>
        /// <para>Else, a subscription to the corresponding source is created (once for given <typeparamref name="TSettings"/>). Events that occur on that subscription are handled as follows:</para>
        /// <list type="number">
        /// <item><description><para>
        /// If the received new pair has no exception and can be successfully bound and validated, we push the pair (new settings, null).
        /// </para></description></item>
        /// <item><description><para>
        /// If the received new pair has no exception but the new config fails during binding or validation, we look for a saved successful settings instance. If it exists, and the current error differs from error corresponding to the saved successful instance, we push pair (last successful settings, new error). If there is no previous successful instance, we call <see cref="IObserver{T}.OnError"/> on subscribers.
        /// </para></description></item>
        /// <item><description><para>
        /// If the received new pair has an exception, we push (settings, error) even if the settings didn't change.
        /// </para></description></item>
        /// <item><description><para>
        /// If we receive <see cref="IObserver{T}.OnError"/>: if there is no saved successful instance, we call <see cref="IObserver{T}.OnError"/>. If there is a successful instance, we push (last successful settings, error from source) and re-subscribe to source after a cooldown. Nothing is pushed if after resubscription we still get an error and the error is the same. This re-subscription loop may continue forever if the source doesn't heal.
        /// </para></description></item>
        /// </list>
        /// <para>If there is an exception in the pair received from the source and there is an exception during binding/validation, they are packed into an <see cref="AggregateException"/>.</para>
        /// <para>If a source provides an exception with the first version of settings, the exception is passed through.</para>
        /// <para>New subscribers receive the last value (if it exists) immediately after subscription.</para>
        /// <para>Note that a subsсription returned by this method becomes inactive after receiving an <see cref="IObserver{T}.OnError"/> notification and <b>it's required to re-subscribe in order to receive further updates</b>.</para>
        /// <para>All required state (cached source subscription, cached last value and exception) should be stored in a limited-size cache keyed by <see cref="IConfigurationSource"/> instance.</para>
        /// <para>It's also expected for this method to be thread-safe.</para>
        /// </summary>
        [NotNull]
        IObservable<(TSettings settings, Exception error)> ObserveWithErrors<TSettings>([NotNull] IConfigurationSource source);
    }
}