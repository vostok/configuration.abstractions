using System;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Commons.Testing;
using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Configuration.Abstractions.Tests
{
    [TestFixture]
    public class SettingsNodeExtensions_Tests : TreeConstructionSet
    {
        [Test]
        public void ScopeTo_should_return_null_if_node_is_null()
        {
            var node = null as ISettingsNode;

            node.ScopeTo("key").Should().BeNull();
        }

        [Test]
        public void ScopeTo_should_throw_if_scope_is_null()
        {
            var node = new ObjectNode("root");

            new Action(() => node.ScopeTo(null as string[])).Should().Throw<ArgumentNullException>().Which.ShouldBePrinted();
        }

        [Test]
        public void ScopeTo_should_throw_if_scope_contains_null_items()
        {
            var node = new ObjectNode("root");

            new Action(() => node.ScopeTo(null as string)).Should().Throw<ArgumentException>().Which.ShouldBePrinted();
        }

        [Test]
        public void ScopeTo_should_return_same_node_if_scope_is_empty()
        {
            var objectNode = new ObjectNode("root");
            var arrayNode = new ArrayNode("root");
            var valueNode = new ValueNode("root");

            objectNode.ScopeTo().Should().BeSameAs(objectNode);
            arrayNode.ScopeTo().Should().BeSameAs(arrayNode);
            valueNode.ScopeTo().Should().BeSameAs(valueNode);
        }

        [Test]
        public void ScopeTo_should_scope_multiple_levels_down()
        {
            var node = Object("root", Object("level1a", ("level2", "hey ho")), new ObjectNode("level1b"));

            node.ScopeTo("level1a", "level2").Value.Should().Be("hey ho");
        }

        [Test]
        public void ScopeTo_should_return_null_if_given_path_does_not_exist()
        {
            var node = Object("root", Object("level1a", ("level2", "hey ho")), new ObjectNode("level1b"));

            node.ScopeTo("level1b", "level2").Should().BeNull();
        }

        [Test]
        public void ScopeTo_should_return_null_if_given_path_contains_non_object_nodes()
        {
            var node = Object("root", Array("array", "item1", "item2"), Value("leaf", "xxx"));

            node.ScopeTo("array", "0").Should().BeNull();
            node.ScopeTo("leaf", "_").Should().BeNull();
        }
    }
}