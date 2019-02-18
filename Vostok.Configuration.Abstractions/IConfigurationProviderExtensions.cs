using JetBrains.Annotations;

namespace Vostok.Configuration.Abstractions
{
    /// <summary>
    /// A set of extensions for <see cref="IConfigurationProvider"/>.
    /// </summary>
    [PublicAPI]
    public static class IConfigurationProviderExtensions
    {
        /// <summary>
        /// Calls <see cref="IConfigurationProvider.SetupSourceFor{TSettings}"/> on the given <paramref name="provider"/> and returns the same <paramref name="provider"/>.
        /// </summary>
        public static IConfigurationProvider WithSourceFor<TSettings>(
            [NotNull] this IConfigurationProvider provider,
            [NotNull] IConfigurationSource source)
        {
            provider.SetupSourceFor<TSettings>(source);

            return provider;
        }
    }
}