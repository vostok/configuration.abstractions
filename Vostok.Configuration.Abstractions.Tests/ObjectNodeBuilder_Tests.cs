using System;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Commons.Testing;
using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Configuration.Abstractions.Tests
{
    [TestFixture]
    internal class ObjectNodeBuilder_Tests
    {
        private ObjectNodeBuilder builder;

        [SetUp]
        public void TestSetup()
        {
            builder = new ObjectNodeBuilder();
        }

        [Test]
        public void Should_start_with_null_name_and_empty_children_by_default()
        {
            builder.Name.Should().BeNull();
            builder.Children.Should().BeEmpty();
        }

        [Test]
        public void Should_start_with_given_name_and_empty_children_when_using_a_ctor_with_name()
        {
            builder = new ObjectNodeBuilder("x");

            builder.Name.Should().Be("x");
            builder.Children.Should().BeEmpty();
        }

        [Test]
        public void Should_start_with_given_object_node_contents()
        {
            var initialNode = new ObjectNode("name", new ISettingsNode[]
            {
                new ValueNode("key1", "value1"), 
                new ValueNode("key2", "value2") 
            });

            builder = new ObjectNodeBuilder(initialNode);

            builder.Name.Should().Be(initialNode.Name);

            builder.Children.Values.Should().Equal(initialNode.Children);
        }

        [Test]
        public void Should_build_equal_to_initial_node_if_nothing_gets_changed()
        {
            var initialNode = new ObjectNode("name", new ISettingsNode[]
            {
                new ValueNode("key1", "value1"),
                new ValueNode("key2", "value2")
            });

            var builtNode = new ObjectNodeBuilder(initialNode).Build();

            builtNode.Should().NotBeSameAs(initialNode);
            builtNode.Should().Be(initialNode);
        }

        [Test]
        public void Should_allow_to_build_multiple_times()
        {
            builder.Build();
            builder.Build();
            builder.Build();
        }

        [Test]
        public void Should_not_allow_to_change_children_after_build()
        {
            builder.Build();

            Action action = () => builder.SetChild(new ValueNode("1", "2"));

            action.Should().Throw<InvalidOperationException>().Which.ShouldBePrinted();
        }

        [Test]
        public void Should_not_allow_to_set_null_child_node()
        {
            Action action = () => builder.SetChild(null);

            action.Should().Throw<ArgumentNullException>().Which.ShouldBePrinted();
        }

        [Test]
        public void Should_not_allow_to_set_child_node_with_null_name()
        {
            Action action = () => builder.SetChild(new ArrayNode(null, new ISettingsNode[]{}));

            action.Should().Throw<ArgumentException>().Which.ShouldBePrinted();
        }

        [Test]
        public void Should_return_mutated_node()
        {
            var initialNode = new ObjectNode("name", new ISettingsNode[]
            {
                new ValueNode("key1", "value1"),
                new ValueNode("key2", "value2")
            });

            builder = new ObjectNodeBuilder(initialNode)
            {
                Name = "name2"
            };
            
            builder.SetChild(new ValueNode("key1", "value0"));
            builder.SetChild(new ValueNode("key3", "value3"));

            var builtNode = builder.Build();

            builtNode.Name.Should().Be("name2");
            builtNode["key1"]?.Value.Should().Be("value0");
            builtNode["key2"]?.Value.Should().Be("value2");
            builtNode["key3"]?.Value.Should().Be("value3");
        }

        [Test]
        public void Should_not_mutate_original_node()
        {
            var initialNode = new ObjectNode("name", new ISettingsNode[]
            {
                new ValueNode("key1", "value1"),
                new ValueNode("key2", "value2")
            });

            builder = new ObjectNodeBuilder(initialNode)
            {
                Name = "name2"
            };

            builder.SetChild(new ValueNode("key1", "value0"));
            builder.SetChild(new ValueNode("key3", "value3"));

            builder.Build();

            initialNode.Name.Should().Be("name");
            initialNode.ChildrenCount.Should().Be(2);
            initialNode["key1"]?.Value.Should().Be("value1");
            initialNode["key2"]?.Value.Should().Be("value2");
        }
    }
}