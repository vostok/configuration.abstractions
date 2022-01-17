using System;
using JetBrains.Annotations;
using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Configuration.Abstractions.Attributes
{
    /// <summary>
    /// <para>When applied to a field or property on the model, provides an alternative key to look up in <see cref="ISettingsNode"/>s structure when binding from a settings tree.</para>
    /// <para>By default, field and property names in the model are used as lookup keys.</para>
    /// </summary>
    [PublicAPI]
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class AliasAttribute : Attribute
    {
        public AliasAttribute([NotNull] string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Alias value cannot be empty or contain only whitespace characters", nameof(value));

            Value = value;
        }

        [NotNull]
        public string Value { get; }
    }
}