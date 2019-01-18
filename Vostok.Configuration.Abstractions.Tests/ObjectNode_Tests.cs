using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Configuration.Abstractions.Merging;
using Vostok.Configuration.Abstractions.SettingsTree;

namespace Vostok.Configuration.Abstractions.Tests
{
    [TestFixture]
    public class ObjectNode_Tests : TreeConstructionSet
    {
        [Test]
        public void Equals_returns_false_by_name()
        {
            var sets1 = Object("Name1", ("value", "x"));
            var sets2 = Object("Name2", ("value", "x"));
            Equals(sets1, sets2).Should().BeFalse();
        }

        [Test]
        public void Equals_returns_false_by_children_key()
        {
            var sets1 = Object("Name", ("value1", "x"));
            var sets2 = Object("Name", ("value2", "x"));
            Equals(sets1, sets2).Should().BeFalse();
        }

        [Test]
        public void Equals_returns_false_by_children_value()
        {
            var sets1 = Object("Name", ("value", "x1"));
            var sets2 = Object("Name", ("value", "x2"));
            Equals(sets1, sets2).Should().BeFalse();
        }
        
        [Test]
        public void Equals_returns_true_for_instances_with_null_and_empty_children()
        {
            var sets1 = new ObjectNode("Name", null);
            var sets2 = new ObjectNode("Name", new ISettingsNode[0]);
            Equals(sets1, sets2).Should().BeTrue();
        }

        [Test]
        public void Hashes_should_be_equal_for_equal_instances()
        {
            var sets1 = Object("Name", ("value", "x")).GetHashCode();
            var sets2 = Object("Name", ("value", "x")).GetHashCode();
            sets1.Should().Be(sets2);
        }

        [Test]
        public void Keys_should_be_case_insensitive()
        {
            var sets = Object("Name", ("value", "v0"), ("VALUE", "v1"), ("TeSt", "v2"));
            
            sets.Children.Count().Should().Be(2, "v0 was rewrited");

            sets["value"].Value.Should().Be("v1");
            sets["VALUE"].Value.Should().Be("v1");
            sets["TEST"].Value.Should().Be("v2");
            sets["test"].Value.Should().Be("v2");
        }

        [Test]
        public void Should_return_second_tree_on_shallow_merge_and_same_children_names()
        {
            var sets1 = Object(("value1", "value1"), ("value2", "value2"));
            var sets2 = Object(("value3", "value3"), ("value2", "value2"));

            var merge = sets1.Merge(sets2, new SettingsMergeOptions {ObjectMergeStyle = ObjectMergeStyle.Shallow});
            merge.Should().Be(sets2);
        }

        [Test]
        public void Should_return_other_on_another_node_type()
        {
            var sets1 = Object(("value", "x1"));
            var sets2 = Value("x2");

            var merge = sets1.Merge(sets2);
            merge.Value.Should().Be("x2");

            merge = sets2.Merge(sets1);
            merge["value"].Value.Should().Be("x1");
        }

        [Test]
        public void Should_make_deep_merge_correctly()
        {
            var sets1 = Object(Object("value1", ("sv1", "sx1"), ("sv2", "sx1")), Value("value2", "x1"), Value("value3", "x1"));
            var sets2 = Object(Object("VALUE1", ("SV2", "sx2"), ("sv3", "sx2")), Value("VALUE2", "x2"));

            var merge = sets1.Merge(sets2, new SettingsMergeOptions { ObjectMergeStyle = ObjectMergeStyle.Deep });
            merge.Children.Count().Should().Be(3);
            merge["value1"]["sv1"].Value.Should().Be("sx1");
            merge["value1"]["sv2"].Value.Should().Be("sx2");
            merge["value1"]["sv3"].Value.Should().Be("sx2");
            merge["value2"].Value.Should().Be("x2");
            merge["value3"].Value.Should().Be("x1");
        }

        [Test]
        public void Indexer_should_return_correct_child_by_name()
        {
            var node = Object(("key", "value"));

            node["key"].Value.Should().Be("value");
        }

        [Test]
        public void Indexer_should_return_null_if_key_is_not_found()
        {
            var node = new ObjectNode(new ISettingsNode[] {});

            node["key"].Should().BeNull();
        }

        [Test]
        public void Indexer_should_return_null_if_children_is_null()
        {
            var node = new ObjectNode(null as ISettingsNode[]);

            node["key"].Should().BeNull();
        }

        [Test]
        public void Children_should_preserve_original_order()
        {
            var node = Object(("key1", "value1"), ("key3", "value3"), ("key2", "value2"));

            node.Children.Select(n => n.Name).Should().Equal("key1", "key3", "key2");
        }

        [Test]
        public void Merge_with_null_should_keep_non_null_node([Values] ObjectMergeStyle mergeStyle)
        {
            var node = Object("xx", ("a", "b"));

            node.Merge(null, new SettingsMergeOptions { ObjectMergeStyle = mergeStyle }).Should().BeSameAs(node);
        }

        [Test]
        public void Children_must_never_be_null()
        {
            new ObjectNode("xx", null).Children.Should().NotBeNull();
        }

        [Test]
        public void Equals_should_be_case_insensitive_for_names()
        {
            var node1 = Object("xx", ("a", "b"));
            var node2 = Object("XX", ("a", "b"));

            node1.Equals(node2).Should().BeTrue();
        }

        [Test]
        public void GetHashCode_should_use_node_name()
        {
            var node1 = Object("xx", ("a", "b"));
            var node2 = Object("yy", ("a", "b"));

            node1.GetHashCode().Should().NotBe(node2.GetHashCode());
        }

        [Test]
        public void GetHashCode_should_use_case_insensitive_hash_for_node_name()
        {
            var node1 = Object("xx", ("a", "b"));
            var node2 = Object("XX", ("a", "b"));

            node1.GetHashCode().Should().Be(node2.GetHashCode());
        }
    }
}