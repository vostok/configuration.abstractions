using JetBrains.Annotations;
using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Configuration.Abstractions
{
    /// <summary>
    /// Implements binding of <see cref="ISettingsNode"/>s to specific models.
    /// </summary>
    [PublicAPI]
    public interface ISettingsBinder<out TSettings>
    {
        /// <summary>
        /// <para>Binds the provided <see cref="ISettingsNode"/> instance to type <typeparamref name="TSettings"/>.</para>
        /// <para>An exception will be thrown if the binding fails.</para>
        /// </summary>
        [CanBeNull]
        TSettings Bind([CanBeNull] ISettingsNode rawSettings);
    }

    /// <summary>
    /// Implements binding of <see cref="ISettingsNode"/> to specific models.
    /// </summary>
    [PublicAPI]
    public interface ISettingsBinder
    {
        /// <summary>
        /// <para>Binds the provided <see cref="ISettingsNode"/> instance to type <typeparamref name="TSettings"/>.</para>
        /// <para>An exception will be thrown if the binding fails.</para>
        /// </summary>
        [CanBeNull]
        TSettings Bind<TSettings>([CanBeNull] ISettingsNode rawSettings);
    }
}