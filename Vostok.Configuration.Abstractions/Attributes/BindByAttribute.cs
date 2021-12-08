using System;
using JetBrains.Annotations;

namespace Vostok.Configuration.Abstractions.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// Specifies that settings in annotated type/field/property should be bound by an <see cref="ISettingsBinder{TSettings}"/> of provided type.
    /// </summary>
    [PublicAPI]
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Field | AttributeTargets.Property)]
    public class BindByAttribute : Attribute
    {
        public BindByAttribute([NotNull] Type binderType) =>
            BinderType = binderType ?? throw new ArgumentNullException(nameof(binderType));

        /// <summary>
        /// The specified binder type.
        /// </summary>
        [NotNull]
        public Type BinderType { get; }
    }
}