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
        /// <para>If there is no source preconfigured for <typeparamref name="TSettings"/> with <see cref="SetupSourceFor{TSettings}"/>, an exception is thrown immediately.</para>
        /// <para>Normally the method just returns the last configuration observed through <see cref="Observe{TSettings}()"/>.</para>
        /// <para>If an error (originating from either <see cref="IConfigurationSource"/>, parser or anything inbetween) happens after observing valid settings at least once, it doesn't affect the behaviour of <see cref="Get{TSettings}()"/>: the method just returns last seen valid settings.</para>
        /// <para>If such an error arises before observing first valid settings instance, it causes <see cref="Get{TSettings}()"/> calls to throw exceptions until first valid settings are observed.</para>
        /// <para>If there is no last value and no errors (<see cref="IConfigurationSource"/> doesn't produce anything), this method blocks waiting for a value.</para>
        /// <para>It's expected for this method to be extremely cheap and be called each time the app needs access to settings.</para>
        /// <para>It's also expected for this method to be thread-safe.</para>
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
        /// <para>Lets the <see cref="IConfigurationProvider"/> know that settings of type <typeparamref name="TSettings"/> should be taken from the provided <paramref name="source"/>.</para>
        /// <para>Until this method is called for <typeparamref name="TSettings"/> any attempt to <see cref="Get{TSettings}()"/> or <see cref="Observe{TSettings}()"/> settings of type <typeparamref name="TSettings"/> without explicitly specifying source will throw an exception.</para>
        /// <para>If called multiple times for the same <typeparamref name="TSettings"/>, the last provided <paramref name="source"/> will be used. To set up multiple sources for a settings type, pass a combined source.</para>
        /// </summary>
        void SetupSourceFor<TSettings>([NotNull] IConfigurationSource source);
    }
}