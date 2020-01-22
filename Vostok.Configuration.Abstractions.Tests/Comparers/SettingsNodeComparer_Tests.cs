using FluentAssertions;
using NUnit.Framework;
using Vostok.Configuration.Abstractions.Comparers;
using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Configuration.Abstractions.Tests.Comparers
{
    [TestFixture]
    public class SettingsNodeComparer_Tests : TreeConstructionSet
    {
        [Test]
        public void Should_be_true_for_equal_trees()
        {
            var left = Object(
                "root",
                Object("obj", ("prop1", "value1"), ("prop2", "value2")),
                Array(
                    "array",
                    Object("0", Value("x", "100")),
                    Object("1", Value("x", "200"))));

            var right = Object(
                "root",
                Object("obj", ("prop1", "value1"), ("prop2", "value2")),
                Array(
                    "array",
                    Object("0", Value("x", "100")),
                    Object("1", Value("x", "200"))));

            Equals(left, right).Should().BeTrue();
        }

        [Test]
        public void Should_be_false_for_non_equal_values()
        {
            var left = Object(
                "root",
                Object("obj", ("prop1", "value1"), ("prop2", "value2")),
                Array(
                    "array",
                    Object("0", Value("x", "100")),
                    Object("1", Value("x", "200"))));

            var right = Object(
                "root",
                Object("obj", ("prop1", "value1"), ("prop2", "value2")),
                Array(
                    "array",
                    Object("0", Value("x", "100")),
                    Object("1", Value("x", "300"))));

            Equals(left, right).Should().BeFalse();
        }

        [Test]
        public void Should_be_false_for_non_equal_properties()
        {
            var left = Object(
                "root",
                Object("obj", ("prop1", "value1"), ("prop2", "value2")),
                Array(
                    "array",
                    Object("0", Value("x", "100")),
                    Object("1", Value("x", "200"))));

            var right = Object(
                "root",
                Object("obj", ("prop1", "value1"), ("prop2", "value3")),
                Array(
                    "array",
                    Object("0", Value("x", "100")),
                    Object("1", Value("x", "200"))));

            Equals(left, right).Should().BeFalse();
        }

        [Test]
        public void Should_be_false_for_non_equal_properties_keys()
        {
            var left = Object(
                "root",
                Object("obj", ("prop1", "value1"), ("prop2", "value2")),
                Array(
                    "array",
                    Object("0", Value("x", "100")),
                    Object("1", Value("x", "200"))));

            var right = Object(
                "root",
                Object("obj", ("prop1", "value1"), ("prop3", "value2")),
                Array(
                    "array",
                    Object("0", Value("x", "100")),
                    Object("1", Value("x", "200"))));

            Equals(left, right).Should().BeFalse();
        }

        [Test]
        public void Should_be_false_for_non_equal_arrays()
        {
            var left = Object(
                "root",
                Object("obj", ("prop1", "value1"), ("prop2", "value2")),
                Array(
                    "array",
                    Object("0", Value("x", "100")),
                    Object("1", Value("x", "200"))));

            var right = Object(
                "root",
                Object("obj", ("prop1", "value1"), ("prop2", "value3")),
                Array(
                    "array",
                    Object("0", Value("x", "100")),
                    Object("1", Value("x", "200")),
                    Object("2", Value("x", "300"))));

            Equals(left, right).Should().BeFalse();
        }

        [Test]
        public void Should_exclude_values()
        {
            var left = Object(
                "root",
                Object("obj", ("prop1", "value1"), ("prop2", "value2")),
                Array(
                    "array",
                    Object("0", Value("x", "100")),
                    Object("1", Value("x", "200"))));

            var right = Object(
                "root",
                Object("obj", ("prop1", "value1"), ("prop2", "value2")),
                Array(
                    "array",
                    Object("0", Value("x", "100")),
                    Object("1", Value("x", "300"))));

            Equals(
                    left,
                    right,
                    new SettingsCompareOptions
                    {
                        ExcludedPaths = new[] {new[] {"root"}}
                    })
                .Should()
                .BeTrue();

            Equals(
                    left,
                    right,
                    new SettingsCompareOptions
                    {
                        ExcludedPaths = new[] {new[] {"root", "array"}}
                    })
                .Should()
                .BeTrue();

            Equals(
                    left,
                    right,
                    new SettingsCompareOptions
                    {
                        ExcludedPaths = new[] {new[] {"root", "array", "x"}}
                    })
                .Should()
                .BeTrue();

            Equals(
                    left,
                    right,
                    new SettingsCompareOptions
                    {
                        ExcludedPaths = new[] {new[] {"root", "array", "y"}}
                    })
                .Should()
                .BeFalse();
        }

        [Test]
        public void Should_exclude_properties()
        {
            var left = Object(
                "root",
                Object("obj", ("prop1", "value1"), ("prop2", "value2")),
                Array(
                    "array",
                    Object("0", Value("x", "100")),
                    Object("1", Value("x", "200"))));

            var right = Object(
                "root",
                Object("obj", ("prop1", "value1"), ("prop2", "value3")),
                Array(
                    "array",
                    Object("0", Value("x", "100")),
                    Object("1", Value("x", "200"))));

            Equals(
                    left,
                    right,
                    new SettingsCompareOptions
                    {
                        ExcludedPaths = new[] {new[] {"root"}}
                    })
                .Should()
                .BeTrue();

            Equals(
                    left,
                    right,
                    new SettingsCompareOptions
                    {
                        ExcludedPaths = new[] {new[] {"root", "obj"}}
                    })
                .Should()
                .BeTrue();

            Equals(
                    left,
                    right,
                    new SettingsCompareOptions
                    {
                        ExcludedPaths = new[] {new[] {"root", "obj", "prop2"}}
                    })
                .Should()
                .BeTrue();

            Equals(
                    left,
                    right,
                    new SettingsCompareOptions
                    {
                        ExcludedPaths = new[] {new[] {"root", "obj", "prop1"}}
                    })
                .Should()
                .BeFalse();
        }

        [Test]
        public void Should_exclude_extra_properties()
        {
            var left = Object("obj", ("prop1", "value1"), ("prop2", "value2"));

            var right = Object("obj", ("prop1", "value1"));

            Equals(
                    left,
                    right)
                .Should()
                .BeFalse();

            Equals(
                    left,
                    right,
                    new SettingsCompareOptions
                    {
                        ExcludedPaths = new[] {new[] {"obj", "prop2"}}
                    })
                .Should()
                .BeTrue();
        }

        [Test]
        public void Should_exclude_properties_keys()
        {
            var left = Object(
                "root",
                Object("obj", ("prop1", "value1"), ("prop2", "value2")),
                Array(
                    "array",
                    Object("0", Value("x", "100")),
                    Object("1", Value("x", "200"))));

            var right = Object(
                "root",
                Object("obj", ("prop1", "value1"), ("prop3", "value2")),
                Array(
                    "array",
                    Object("0", Value("x", "100")),
                    Object("1", Value("x", "200"))));

            Equals(
                    left,
                    right,
                    new SettingsCompareOptions
                    {
                        ExcludedPaths = new[] {new[] {"root"}}
                    })
                .Should()
                .BeTrue();

            Equals(
                    left,
                    right,
                    new SettingsCompareOptions
                    {
                        ExcludedPaths = new[] {new[] {"root", "obj"}}
                    })
                .Should()
                .BeTrue();

            Equals(
                    left,
                    right,
                    new SettingsCompareOptions
                    {
                        ExcludedPaths = new[] {new[] {"root", "obj", "prop2"}}
                    })
                .Should()
                .BeFalse();

            Equals(
                    left,
                    right,
                    new SettingsCompareOptions
                    {
                        ExcludedPaths = new[]
                        {
                            new[] {"root", "obj", "prop2"},
                            new[] {"root", "obj", "prop3"}
                        }
                    })
                .Should()
                .BeTrue();
        }

        [Test]
        public void Should_exclude_arrays()
        {
            var left = Object(
                "root",
                Object("obj", ("prop1", "value1"), ("prop2", "value2")),
                Array(
                    "array",
                    Object("0", Value("x", "100")),
                    Object("1", Value("x", "200"))));

            var right = Object(
                "root",
                Object("obj", ("prop1", "value1"), ("prop2", "value2")),
                Array(
                    "array",
                    Object("0", Value("x", "100")),
                    Object("1", Value("x", "200")),
                    Object("2", Value("x", "300"))));

            Equals(
                    left,
                    right,
                    new SettingsCompareOptions
                    {
                        ExcludedPaths = new[] {new[] {"root"}}
                    })
                .Should()
                .BeTrue();

            Equals(
                    left,
                    right,
                    new SettingsCompareOptions
                    {
                        ExcludedPaths = new[] {new[] {"root", "array"}}
                    })
                .Should()
                .BeTrue();

            Equals(
                    left,
                    right,
                    new SettingsCompareOptions
                    {
                        ExcludedPaths = new[] {new[] {"root", "array", "x"}}
                    })
                .Should()
                .BeFalse();

            Equals(
                    left,
                    right,
                    new SettingsCompareOptions
                    {
                        ExcludedPaths = new[] {new[] {"root", "array", "y"}}
                    })
                .Should()
                .BeFalse();
        }

        private static bool Equals(ISettingsNode left, ISettingsNode right, SettingsCompareOptions options = null) =>
            new SettingsNodeComparer(options).Equals(left, right);
    }
}