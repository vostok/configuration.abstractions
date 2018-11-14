using FluentAssertions;
using NUnit.Framework;
using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Configuration.Abstractions.Tests
{
    // TODO(krait): Review tests.
    [TestFixture]
    public class ValueNode_Tests
    {
        [Test]
        public void Name_should_equal_value_if_null()
        {
            var sets = new ValueNode("x");
            sets.Name.Should().Be("x");
        }

        [Test]
        public void Equals_returns_false_by_name()
        {
            var sets1 = new ValueNode("name1", "x");
            var sets2 = new ValueNode("name2", "x");
            Equals(sets1, sets2).Should().BeFalse();
        }

        [Test]
        public void Equals_returns_false_by_value()
        {
            var sets1 = new ValueNode("name", "x1");
            var sets2 = new ValueNode("name", "x2");
            Equals(sets1, sets2).Should().BeFalse();
        }

        [Test]
        public void Hashes_should_be_equal_for_equal_instances()
        {
            var sets1 = new ValueNode("name", "x").GetHashCode();
            var sets2 = new ValueNode("name", "x").GetHashCode();
            sets1.Should().Be(sets2);
        }

        [Test]
        public void Should_always_return_second_node_on_merge()
        {
            var sets1 = new ValueNode("name1", "x");
            var sets2 = new ValueNode("name2", "x");
            var merge = sets1.Merge(sets2, null);
            merge.Value.Should().Be("x");
            merge.Name.Should().Be("name2");

            sets1 = new ValueNode("name", "x1");
            sets2 = new ValueNode("name", "x2");
            merge = sets1.Merge(sets2, null);
            merge.Value.Should().Be("x2");
            merge.Name.Should().Be("name");
        }
        
        [Test]
        public void ScopeTo_should_return_this_when_empty_scope()
        {
            var node = new ValueNode("root");
            node.ScopeTo().Should().BeSameAs(node);
        }

        [Test]
        public void ScopeTo_should_return_null_for_nonEmpty_scope()
        {
            new ValueNode("value").ScopeTo("key").Should().BeNull();
        }
    }
}