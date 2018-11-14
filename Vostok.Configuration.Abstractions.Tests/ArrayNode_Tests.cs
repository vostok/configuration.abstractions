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
    public class ArrayNode_Tests
    {
        [Test]
        public void Constructors_are_equal()
        {
            var sets1 = new ArrayNode("Name", new List<ISettingsNode> { new ValueNode("x") });
            var sets2 = new ArrayNode("Name", new List<ISettingsNode> { new ValueNode("x") });
            sets1.Should().BeEquivalentTo(sets2);
            Equals(sets1, sets2).Should().BeTrue("checks overrided Equals method");
        }

        [Test]
        public void Equals_returns_false_by_name()
        {
            var sets1 = new ArrayNode("Name1", new List<ISettingsNode> { new ValueNode("x") });
            var sets2 = new ArrayNode("Name2", new List<ISettingsNode> { new ValueNode("x") });
            Equals(sets1, sets2).Should().BeFalse();
        }

        [Test]
        public void Equals_returns_false_by_children_value()
        {
            var sets1 = new ArrayNode("Name", new List<ISettingsNode> { new ValueNode("x1") });
            var sets2 = new ArrayNode("Name", new List<ISettingsNode> { new ValueNode("x2") });
            Equals(sets1, sets2).Should().BeFalse();
        }

        [Test]
        public void Hashes_should_be_equal_for_equal_instances()
        {
            var sets1 = new ArrayNode("Name", new List<ISettingsNode> { new ValueNode("x") }).GetHashCode();
            var sets2 = new ArrayNode("Name", new List<ISettingsNode> { new ValueNode("x") }).GetHashCode();
            sets1.Should().Be(sets2);
        }

        [Test]
        public void Should_return_other_on_merging_with_another_node_type()
        {
            var sets1 = new ArrayNode(new List<ISettingsNode> { new ValueNode("x1") });
            var sets2 = new ValueNode("x2");

            var merge = sets1.Merge(sets2);
            merge.Value.Should().Be("x2");

            merge = sets2.Merge(sets1);
            merge.Children.First().Value.Should().Be("x1");
        }

        [TestCase(ArrayMergeStyle.Replace, TestName = "Replace option")]
        [TestCase(ArrayMergeStyle.Concat, TestName = "Concat option")]
        [TestCase(ArrayMergeStyle.Union, TestName = "Union option")]
        public void Should_merge_with_different_options(ArrayMergeStyle style)
        {
            var sets1 = new ArrayNode(new List<ISettingsNode>
            {
                new ValueNode("x1"),
                new ValueNode("x2"),
                new ValueNode("x3"),
            });
            var sets2 = new ArrayNode(new List<ISettingsNode>
            {
                new ValueNode("x1"),
                new ValueNode("x4"),
                new ValueNode("x5"),
            });

            var merge = sets1.Merge(sets2, new SettingsMergeOptions { ArrayMergeStyle = style });
            switch (style)
            {
                case ArrayMergeStyle.Replace:
                    merge.Children.Select(c => c.Value).Should().ContainInOrder("x1", "x4", "x5");
                    break;
                case ArrayMergeStyle.Concat:
                    merge.Children.Select(c => c.Value).Should().ContainInOrder("x1", "x2", "x3", "x1", "x4", "x5");
                    break;
                case ArrayMergeStyle.Union:
                    merge.Children.Select(c => c.Value).Should().ContainInOrder("x1", "x2", "x3", "x4", "x5");
                    break;
            }
        }

        [Test]
        public void Should_merge_wirh_different_options_right_way()
        {
            var sets1 = new ArrayNode(new List<ISettingsNode>
            {
                new ValueNode("x1"),
                new ObjectNode(new SortedDictionary<string, ISettingsNode>
                {
                    ["value"] = new ValueNode("x11"),
                }),
                new ObjectNode(new SortedDictionary<string, ISettingsNode>
                {
                    ["value"] = new ValueNode("x12"),
                }),
            });
            var sets2 = new ArrayNode(new List<ISettingsNode>
            {
                new ValueNode("x1"),
                new ValueNode("x2"),
                new ObjectNode(new SortedDictionary<string, ISettingsNode>
                {
                    ["value"] = new ValueNode("x11"),
                }),
                new ObjectNode(new SortedDictionary<string, ISettingsNode>
                {
                    ["value"] = new ValueNode("x21"),
                }),
            });

            var merge = sets1.Merge(sets2, new SettingsMergeOptions { ObjectMergeStyle = ObjectMergeStyle.Shallow, ArrayMergeStyle = ArrayMergeStyle.Union });
            var children = merge.Children.ToArray();
            children.Length.Should().Be(5);
            children[0].Value.Should().Be("x1");
            children[1]["value"].Value.Should().Be("x11");
            children[2]["value"].Value.Should().Be("x12");
            children[3].Value.Should().Be("x2");
            children[4]["value"].Value.Should().Be("x21");
        }

        [Test]
        public void ScopeTo_should_return_this_when_empty_scope()
        {
            var node = new ArrayNode("root");
            node.ScopeTo().Should().BeSameAs(node);
        }

        [Test]
        public void ScopeTo_should_scope_to_child_when_correct_index()
        {
            var array = new[] {new ValueNode("value0"), new ValueNode("value1")};
            var node = new ArrayNode(array);
            node.ScopeTo("[1]").Should().BeSameAs(array[1]);
        }

        [TestCase(null, TestName = "null string")]
        [TestCase("", TestName = "empty string")]
        [TestCase("0", TestName = "no braces")]
        [TestCase("key", TestName = "object key")]
        [TestCase("[0", TestName = "no closing brace")]
        [TestCase("0]", TestName = "no opening brace")]
        [TestCase("[100]", TestName = "index is out of range")]
        [TestCase("[-1]", TestName = "index is negative")]
        [TestCase("[a]", TestName = "index is not a number")]
        public void ScopeTo_should_return_null_when_incorrect_index(string index)
        {
            var array = new[] {new ValueNode("value0"), new ValueNode("value1")};
            var node = new ArrayNode(array);
            node.ScopeTo(index).Should().BeNull();
        }

        [Test]
        public void ScopeTo_should_scope_recursively()
        {
            var value = new ValueNode("value");
            
            var child = Substitute.For<ISettingsNode>();
            child.ScopeTo("a", "b").Returns(value);
            
            var node = new ArrayNode(new []{child});
            node.ScopeTo("[0]", "a", "b").Should().Be(value);
        }
    }
}