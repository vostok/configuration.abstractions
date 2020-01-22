using FluentAssertions;
using NUnit.Framework;
using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Configuration.Abstractions.Tests.SettingsTree
{
    [TestFixture]
    public class SettingsNodeComparer_Tests : TreeConstructionSet
    {
        [Test]
        public void FindDifferences_should_be_empty_for_equal_nodes()
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

            SettingsNodeComparer.FindDifferences(left, right)
                .Should()
                .BeEmpty();
        }

        [Test]
        public void FindDifferences_should_find_array_element()
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

            SettingsNodeComparer.FindDifferences(left, right)
                .Should()
                .BeEquivalentTo(
                    new[]
                    {
                        new[] {"root", "array", "x"}
                    });
        }

        [Test]
        public void FindDifferences_should_find_property_value()
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

            SettingsNodeComparer.FindDifferences(left, right)
                .Should()
                .BeEquivalentTo(
                    new[]
                    {
                        new[] {"root", "obj", "prop2"}
                    });
        }

        [Test]
        public void FindDifferences_should_find_missing_or_extra_properties()
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
                Object("obj", ("prop1", "value1"), ("prop3", "value3")),
                Array(
                    "array",
                    Object("0", Value("x", "100")),
                    Object("1", Value("x", "200"))));

            SettingsNodeComparer.FindDifferences(left, right)
                .Should()
                .BeEquivalentTo(
                    new[] {"root", "obj", "prop2"},
                    new[] {"root", "obj", "prop3"});
        }

        [Test]
        public void FindDifferences_should_find_missing_or_extra_array_elements()
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

            SettingsNodeComparer.FindDifferences(left, right)
                .Should()
                .BeEquivalentTo(
                    new[] {"root", "array"},
                    new[] {"root", "array", "x"});
        }

        [Test]
        public void FindDifferences_should_find_missing_array_of_objects()
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
                Object("obj", ("prop1", "value1"), ("prop2", "value2")));

            SettingsNodeComparer.FindDifferences(left, right)
                .Should()
                .BeEquivalentTo(
                    new[] {"root", "array"},
                    new[] {"root", "array", "x"});
        }

        [Test]
        public void FindDifferences_should_find_missing_object()
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
                Array(
                    "array",
                    Object("0", Value("x", "100")),
                    Object("1", Value("x", "200"))));

            SettingsNodeComparer.FindDifferences(left, right)
                .Should()
                .BeEquivalentTo(
                    new[] {"root", "obj"},
                    new[] {"root", "obj", "prop1"},
                    new[] {"root", "obj", "prop2"});
        }
    }
}