using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Configuration.Abstractions.Merging;
using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Configuration.Abstractions.Tests
{
    // TODO(krait): Review tests.
    [TestFixture]
    public class ObjectNode_Tests
    {
        [Test]
        public void Constructors_are_equal()
        {
            var sets1 = new ObjectNode("Name", new SortedDictionary<string, ISettingsNode> { ["value"] = new ValueNode("x") });
            var sets2 = new ObjectNode("Name", new SortedDictionary<string, ISettingsNode> { ["value"] = new ValueNode("x") });
            sets1.Should().BeEquivalentTo(sets2);
            Equals(sets1, sets2).Should().BeTrue("checks overrided Equals method");
        }

        [Test]
        public void Equals_returns_false_by_name()
        {
            var sets1 = new ObjectNode("Name1", new SortedDictionary<string, ISettingsNode> { ["value"] = new ValueNode("x") });
            var sets2 = new ObjectNode("Name2", new SortedDictionary<string, ISettingsNode> { ["value"] = new ValueNode("x") });
            Equals(sets1, sets2).Should().BeFalse();
        }

        [Test]
        public void Equals_returns_false_by_children_key()
        {
            var sets1 = new ObjectNode("Name", new SortedDictionary<string, ISettingsNode> { ["value1"] = new ValueNode("x") });
            var sets2 = new ObjectNode("Name", new SortedDictionary<string, ISettingsNode> { ["value2"] = new ValueNode("x") });
            Equals(sets1, sets2).Should().BeFalse();
        }

        [Test]
        public void Equals_returns_false_by_children_value()
        {
            var sets1 = new ObjectNode("Name", new SortedDictionary<string, ISettingsNode> { ["value"] = new ValueNode("x1") });
            var sets2 = new ObjectNode("Name", new SortedDictionary<string, ISettingsNode> { ["value"] = new ValueNode("x2") });
            Equals(sets1, sets2).Should().BeFalse();
        }

        [Test]
        public void Hashes_should_be_equal_for_equal_instances()
        {
            var sets1 = new ObjectNode("Name", new SortedDictionary<string, ISettingsNode> { ["value"] = new ValueNode("x") }).GetHashCode();
            var sets2 = new ObjectNode("Name", new SortedDictionary<string, ISettingsNode> { ["value"] = new ValueNode("x") }).GetHashCode();
            sets1.Should().Be(sets2);
        }

        [Test]
        public void Keys_should_be_case_insensitive()
        {
            var sets = new ObjectNode("Name", new SortedDictionary<string, ISettingsNode>(StringComparer.InvariantCultureIgnoreCase)
            {
                ["value"] = new ValueNode("v0"),
                ["VALUE"] = new ValueNode("v1"),    //rewrites
                ["TeSt"] = new ValueNode("v2"),
            });
            
            sets.Children.Count().Should().Be(2, "v0 was rewrited");

            sets["value"].Value.Should().Be("v1");
            sets["VALUE"].Value.Should().Be("v1");
            sets["TEST"].Value.Should().Be("v2");
            sets["test"].Value.Should().Be("v2");
        }

        [Test]
        public void Should_return_second_tree_on_shallow_merge_and_same_children_names()
        {
            var sets1 = new ObjectNode(new SortedDictionary<string, ISettingsNode>
            {
                ["value1"] = new ValueNode("x1", "value1"),
                ["value2"] = new ValueNode("x1", "value2"),
            });
            var sets2 = new ObjectNode(new SortedDictionary<string, ISettingsNode>
            {
                ["value1"] = new ValueNode("x2", "value1"),
                ["value3"] = new ValueNode("x2", "value3"),
            });

            var merge = sets1.Merge(sets2, new SettingsMergeOptions {ObjectMergeStyle = ObjectMergeStyle.Shallow});
            merge.Should().Be(sets2);
        }

        [Test]
        public void Should_return_other_on_shallow_merge_and_same_children_names()
        {
            var comparer = StringComparer.InvariantCultureIgnoreCase;
            var sets1 = new ObjectNode(new SortedDictionary<string, ISettingsNode>(comparer)
            {
                ["value1"] = new ObjectNode("value1", new SortedDictionary<string, ISettingsNode>(comparer)
                {
                    ["subvalue"] = new ValueNode("subvalue", "sx1"),
                }),
                ["value2"] = new ValueNode("value2", "x1"),
            });
            var sets2 = new ObjectNode(new SortedDictionary<string, ISettingsNode>(comparer)
            {
                ["value2"] = new ValueNode("value2", "x2"),
                ["VALUE1"] = new ObjectNode("VALUE1", new SortedDictionary<string, ISettingsNode>(comparer)
                {
                    ["subvalue"] = new ValueNode("subvalue", "sx2"),
                }),
            });

            var merge = sets1.Merge(sets2, new SettingsMergeOptions {ObjectMergeStyle = ObjectMergeStyle.Shallow});
            merge.Children.Count().Should().Be(2);
            merge["value2"].Value.Should().Be("x2");
            merge["value2"].Name.Should().Be("value2");
            merge["value1"].Name.Should().Be("VALUE1");
            merge["value1"]["subvalue"].Value.Should().Be("sx2");
            merge["value1"]["subvalue"].Name.Should().Be("subvalue");
        }

        [Test]
        public void Should_return_other_on_another_node_type()
        {
            var sets1 = new ObjectNode(new SortedDictionary<string, ISettingsNode>{ ["value"] = new ValueNode("x1") });
            var sets2 = new ValueNode("x2");

            var merge = sets1.Merge(sets2);
            merge.Value.Should().Be("x2");

            merge = sets2.Merge(sets1);
            merge["value"].Value.Should().Be("x1");
        }

        [Test]
        public void Should_make_deep_merge_correctly()
        {
            var comparer = StringComparer.InvariantCultureIgnoreCase;
            var sets1 = new ObjectNode(new SortedDictionary<string, ISettingsNode>(comparer)
            {
                ["value1"] = new ObjectNode("value1", new SortedDictionary<string, ISettingsNode>(comparer)
                {
                    ["sv1"] = new ValueNode("sv1", "sx1"),
                    ["sv2"] = new ValueNode("sv2", "sx1"),
                }),
                ["value2"] = new ValueNode("value2", "x1"),
                ["value3"] = new ValueNode("value3", "x1"),
            });
            var sets2 = new ObjectNode(new SortedDictionary<string, ISettingsNode>(comparer)
            {
                ["VALUE1"] = new ObjectNode("VALUE1", new SortedDictionary<string, ISettingsNode>(comparer)
                {
                    ["SV2"] = new ValueNode("sv2", "sx2"),
                    ["sv3"] = new ValueNode("sv3", "sx2"),
                }),
                ["VALUE2"] = new ValueNode("VALUE2", "x2"),
            });

            var merge = sets1.Merge(sets2, new SettingsMergeOptions { ObjectMergeStyle = ObjectMergeStyle.Deep });
            merge.Children.Count().Should().Be(3);
            merge["value1"]["sv1"].Value.Should().Be("sx1");
            merge["value1"]["sv2"].Value.Should().Be("sx2");
            merge["value1"]["sv3"].Value.Should().Be("sx2");
            merge["value2"].Value.Should().Be("x2");
            merge["value3"].Value.Should().Be("x1");
        }
        
        [Test]
        public void ScopeTo_should_return_this_when_empty_scope()
        {
            var node = new ObjectNode("root");
            node.ScopeTo().Should().BeSameAs(node);
        }

        [Test]
        public void ScopeTo_should_scope_to_child_when_valid_key()
        {
            var value = new ValueNode("value");
            var node = new ObjectNode(new Dictionary<string, ISettingsNode>
            {
                ["key"] = value
            });
            node.ScopeTo("key").Should().BeSameAs(value);
        }

        [TestCase(null, TestName = "null string")]
        [TestCase("", TestName = "empty string")]
        [TestCase("otherKey", TestName = "non-existent key")]
        public void ScopeTo_should_return_null_when_invalid_key(string key)
        {
            var node = new ObjectNode(new Dictionary<string, ISettingsNode>
            {
                ["key"] = new ValueNode("value")
            });
            node.ScopeTo(key).Should().BeNull();
        }

        [Test]
        public void ScopeTo_should_scope_recursively()
        {
            var value = new ValueNode("value");
            
            var child = Substitute.For<ISettingsNode>();
            child.ScopeTo("a", "b").Returns(value);
            
            var node = new ObjectNode(new Dictionary<string, ISettingsNode>
            {
                ["key"] = child
            });
            node.ScopeTo("key", "a", "b").Should().Be(value);
        }
    }
}