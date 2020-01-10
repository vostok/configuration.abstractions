using System;
using JetBrains.Annotations;

namespace Vostok.Configuration.Abstractions.Attributes
{
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class AliasAttribute : Attribute
    {
        public AliasAttribute(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Alias value cannot be empty or contain only whitespace characters", nameof(value));

            Value = value;
        }

        public string Value { get; }
    }
}