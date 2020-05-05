using System;
using JetBrains.Annotations;

namespace Vostok.Configuration.Abstractions
{
    /// <summary>
    /// <para>Provides settings for your application, fresh and warm.</para>
    /// <para>See <see cref="Get{TSettings}()"/> and <see cref="Observe{TSettings}()"/> methods for more details.</para>
    /// </summary>
    [PublicAPI]
    public interface IConfigurationProvider
    {
        /// <summary>
        /// <para>Returns the most recent value of <typeparamref name="TSettings"/> from preconfigured configuration sources.</para>
        /// <para>Implementations should comply to the following rules:</para>
        /// <list type="bullet">
        ///     <item><description>If there is no source preconfigured for <typeparamref name="TSettings"/> with <see cref="SetupSourceFor{TSettings}"/>, an exception is thrown immediately.</description></item>
        ///     <item><description>Normally the method just returns the last configuration observed through <see cref="Observe{TSettings}()"/>.</description></item>
        ///     <item>
        ///         <description>If an error (originating from either <see cref="IConfigurationSource"/>, parser or anything inbetween)
        ///         happens after observing valid settings at least once, it doesn't affect the behaviour of <see cref="Get{TSettings}()"/>:
        ///         the method just returns last seen valid settings. Such error are exposed to user in an implementation-specific way, such as a specialized callback.
        ///     </description>
        ///     </item>
        ///     <item><description>If such an error arises before observing first valid settings instance, it causes <see cref="Get{TSettings}()"/> calls to throw exceptions until first valid settings are observed.</description></item>
        ///     <item><description>If there is no last value and no errors (<see cref="IConfigurationSource"/> doesn't produce anything), this method blocks waiting for a value.</description></item>
        ///     <item><description>It's expected for this method to be extremely cheap and be called each time the app needs access to settings.</description></item>
        ///     <item><description>It's also expected for this method to be thread-safe.</description></item>
        /// </list>
        /// </summary>
        TSettings Get<TSettings>();

        /// <summary>
        /// <para>This method behaves similar to its parameterless counterpart (<see cref="Get{TSettings}()"/>) with following exceptions:</para>
        /// <list type="bullet">
        ///     <item><description>It doesn't require to set up the source for type beforehand with <see cref="SetupSourceFor{TSettings}"/>: source is passed as argument.</description></item>
        ///     <item>
        ///         <description>
        ///         It stores internal state (subscriptions to sources, cached last values) in a cache limited in size.
        ///         This cache might drop mentioned state in the event of overflow, which can potentially violate the guarantee of not producing exceptions after observing first valid value.
        ///         Cache overflows typically result from passing lots of distinct <see cref="IConfigurationSource"/> instances to this method.
        ///         </description>
        ///     </item>
        /// </list>
        /// </summary>
        TSettings Get<TSettings>([NotNull] IConfigurationSource source);

        /// <summary>
        /// <para>Returns an <see cref="IObservable{T}"/> that receives the new value of <typeparamref name="TSettings"/> each time it is updated from its corresponding preconfigured source.</para>
        /// <para>Implementations should comply to the following rules:</para>
        /// <list type="bullet">
        ///     <item>
        ///         <description>
        ///         If there is no source preconfigured for <typeparamref name="TSettings"/> with <see cref="SetupSourceFor{TSettings}"/>, an exception is thrown immediately.
        ///         Else, a subscription to the corresponding source is created once for given <typeparamref name="TSettings"/>.
        ///         </description>
        ///     </item>
        ///     <item><description>Returned sequence never produces <see cref="IObserver{T}.OnCompleted"/> or <see cref="IObserver{T}.OnError"/> notifications: only valid settings instances are exposed via <see cref="IObserver{T}.OnNext"/>.</description></item>
        ///     <item><description>Errors are exposed to user in an implementation-specific way, such as a specialized callback.</description></item>
        ///     <item><description>New subscribers receive current settings value (if it already exists) immediately after subscription.</description></item>
        ///     <item><description>It's also expected for this method to be thread-safe.</description></item>
        /// </list>
        /// <para>Remember to unsubscrube from returned sequence when it's no longer needed: lifetime of internal resources might be tied to user subscriptions.</para>
        /// </summary>
        [NotNull]
        IObservable<TSettings> Observe<TSettings>();

        /// <summary>
        /// <para>This method behaves similar to its parameterless counterpart (<see cref="Get{TSettings}()"/>) with following exceptions:</para>
        /// <list type="bullet">
        ///     <item><description>It doesn't require to set up the source for type beforehand with <see cref="SetupSourceFor{TSettings}"/>: source is passed as argument.</description></item>
        /// </list>
        /// </summary>
        [NotNull]
        IObservable<TSettings> Observe<TSettings>([NotNull] IConfigurationSource source);

        /// <summary>
        /// <para>Lets the <see cref="IConfigurationProvider"/> know that settings of type <typeparamref name="TSettings"/> should be taken from the provided <paramref name="source"/>.</para>
        /// <para>Until this method is called for <typeparamref name="TSettings"/> any attempt to <see cref="Get{TSettings}()"/> or <see cref="Observe{TSettings}()"/> settings of type <typeparamref name="TSettings"/> without explicitly specifying source will throw an exception.</para>
        /// <para>If called multiple times for the same <typeparamref name="TSettings"/>, the last provided <paramref name="source"/> will be used. To set up multiple sources for a settings type, pass a combined source.</para>
        /// <para>Using this method after calling <see cref="Get{TSettings}()"/> or <see cref="Observe{TSettings}()"/> will fail with exception as dynamic reconfiguration of sources is not supported.</para>
        /// </summary>
        void SetupSourceFor<TSettings>([NotNull] IConfigurationSource source);

        /// <summary>
        /// <para>Attempts to set associate given <paramref name="source"/> with type <typeparamref name="TSettings"/>.</para>
        /// <para>Returns <c>true</c> on success or <c>false</c> if a source has already been configured for this type.</para>
        /// <para>See <see cref="SetupSourceFor{T}"/> for mor details.</para>
        /// </summary>
        bool TrySetupSourceFor<TSettings>([NotNull] IConfigurationSource source);

        /// <summary>
        /// Returns whether there is a source configured for <typeparamref name="TSettings"/>.
        /// </summary>
        bool HasSourceFor<TSettings>();
    }
}