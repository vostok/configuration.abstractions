using FluentAssertions;
using NUnit.Framework;

namespace Vostok.Configuration.Abstractions.Tests
{
    [TestFixture]
    public class ValueNode_Tests : TreeConstructionSet
    {
        [Test]
        public void Name_should_equal_value_if_null()
        {
            var sets = Value("x");
            sets.Name.Should().Be("x");
        }

        [Test]
        public void Equals_returns_false_by_name()
        {
            var sets1 = Value("name1", "x");
            var sets2 = Value("name2", "x");
            Equals(sets1, sets2).Should().BeFalse();
        }

        [Test]
        public void Equals_returns_false_by_value()
        {
            var sets1 = Value("name", "x1");
            var sets2 = Value("name", "x2");
            Equals(sets1, sets2).Should().BeFalse();
        }

        [Test]
        public void Hashes_should_be_equal_for_equal_instances()
        {
            var sets1 = Value("name", "x").GetHashCode();
            var sets2 = Value("name", "x").GetHashCode();
            sets1.Should().Be(sets2);
        }

        [Test]
        public void Should_always_return_second_node_on_merge()
        {
            var sets1 = Value("name1", "x");
            var sets2 = Value("name2", "x");
            var merge = sets1.Merge(sets2, null);
            merge.Value.Should().Be("x");
            merge.Name.Should().Be("name2");

            sets1 = Value("name", "x1");
            sets2 = Value("name", "x2");
            merge = sets1.Merge(sets2, null);
            merge.Value.Should().Be("x2");
            merge.Name.Should().Be("name");
        }

        [Test]
        public void Merge_with_null_should_keep_non_null_node()
        {
            var node = Value("xx");
            node.Merge(null).Should().BeSameAs(node);
        }

        [Test]
        public void Equals_should_be_case_insensitive_for_names()
        {
            var node1 = Value("xx", "yy");
            var node2 = Value("XX", "yy");

            node1.Equals(node2).Should().BeTrue();
        }

        [Test]
        public void Equals_should_be_case_sensitive_for_values()
        {
            var node1 = Value("xx", "yy");
            var node2 = Value("xx", "YY");

            node1.Equals(node2).Should().BeFalse();
        }

        [Test]
        public void GetHashCode_should_use_node_name()
        {
            var node1 = Value("xx", "yy");
            var node2 = Value("yy", "yy");

            node1.GetHashCode().Should().NotBe(node2.GetHashCode());
        }

        [Test]
        public void GetHashCode_should_use_case_insensitive_hash_for_node_name()
        {
            var node1 = Value("xx", "yy");
            var node2 = Value("XX", "yy");

            node1.GetHashCode().Should().Be(node2.GetHashCode());
        }
    }
}