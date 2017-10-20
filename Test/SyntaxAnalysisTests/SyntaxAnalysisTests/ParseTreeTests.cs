using System;
using System.Collections.Generic;
using NUnit.Framework;
using SyntaxAnalysis.Tree;

namespace SyntaxAnalysisTests
{
    [TestFixture]
    public class ParseTreeTests
    {
        [Test]
        public void Basic()
        {
            ParseTree<int> st = new ParseTree<int>(2);
            Assert.That(st.Root.Value, Is.EqualTo(2));
            Assert.That(st.CurrentNode.Value, Is.EqualTo(2));
            Assert.That(st.CurrentNode.Children, Has.Count.EqualTo(0));
            Assert.Throws<InvalidOperationException>(st.EndNode);
        }

        [Test]
        public void CanAdd()
        {
            ParseTree<string> st = new ParseTree<string>("a");
            var root = st.CurrentNode;
            var b = st.StartNode("b");
            Assert.That(st.CurrentNode, Is.EqualTo(b));
            Assert.That(st.CurrentNode.Parent, Is.EqualTo(root));

            var c = st.StartNode("c");
            Assert.That(st.CurrentNode, Is.EqualTo(c));
            Assert.That(st.CurrentNode.Parent, Is.EqualTo(b));

            var d = st.CurrentNode.AddChild("d");
            Assert.That(d.Children, Is.Empty);
            Assert.That(d.Parent, Is.EqualTo(c));

            var e = st.CurrentNode.AddChild("e");
            Assert.That(e.Children, Is.Empty);
            Assert.That(e.Parent, Is.EqualTo(c));

            var f = st.CurrentNode.AddChild("f");
            Assert.That(f.Children, Is.Empty);
            Assert.That(f.Parent, Is.EqualTo(c));

            Assert.That(c.Children, Has.Count.EqualTo(3));
        }

        [Test]
        public void CanRemove()
        {
            ParseTree<double> st = new ParseTree<double>(1.0);
            var root = st.CurrentNode;
            var node2 = st.StartNode(2.0);
            var node2Child1 = st.CurrentNode.AddChild(2.1);
            var node3 = st.StartNode(3.0);
            var node3Child1 = st.CurrentNode.AddChild(3.1);
            var node3Child2 = st.CurrentNode.AddChild(3.2);
            st.CurrentNode.AddChild(3.3);

            Assert.That(st.CurrentNode, Is.EqualTo(node3));

            st.RemoveLastAddedNode();
            Assert.That(st.CurrentNode.Children, Has.Count.EqualTo(2));
            Assert.That(st.CurrentNode.Children[0], Is.EqualTo(node3Child1));
            Assert.That(st.CurrentNode.Children[1], Is.EqualTo(node3Child2));

            st.RemoveLastAddedNode();
            Assert.That(st.CurrentNode.Children, Has.Count.EqualTo(1));
            Assert.That(st.CurrentNode.Children[0], Is.EqualTo(node3Child1));

            st.RemoveLastAddedNode();
            Assert.That(st.CurrentNode.Children, Is.Empty);

            st.RemoveLastAddedNode();
            Assert.That(st.CurrentNode.Children, Has.Count.EqualTo(1));
            Assert.That(st.CurrentNode.Children[0], Is.EqualTo(node2Child1));

            st.RemoveLastAddedNode();
            Assert.That(st.CurrentNode, Is.EqualTo(node2));

            st.RemoveLastAddedNode();
            Assert.That(st.CurrentNode, Is.EqualTo(root));

            Assert.Throws<InvalidOperationException>(st.RemoveLastAddedNode);
        }

        [Test]
        public void CanEnd()
        {
            ParseTree<string> st = new ParseTree<string>("a");
            var a = st.CurrentNode;
            var b = st.StartNode("b");
            Assert.That(st.CurrentNode, Is.EqualTo(b));
            Assert.That(st.CurrentNode.Parent, Is.EqualTo(a));

            st.EndNode();
            Assert.That(st.CurrentNode, Is.EqualTo(a));
            Assert.That(st.CurrentNode.Parent, Is.Null);
        }

        [Test]
        public void GetLeaves()
        {
            ParseTree<string> st = new ParseTree<string>("a");
            var a = st.CurrentNode;
            var a1 = a.AddChild("a1");
            var a2 = a.AddChild("a2");
            var a21 = a2.AddChild("a21");
            var a211 = a21.AddChild("a211");
            var a3 = a.AddChild("a3");
            var b = st.StartNode("b");
            var b1 = b.AddChild("b1");
            st.StartNode("c");
            var c1 = st.CurrentNode.AddChild("c1");
            var c2 = st.CurrentNode.AddChild("c2");
            var c3 = st.CurrentNode.AddChild("c3");
            var d = a.AddChild("d");

            // a    a1
            //      a2  a21 a211
            //      a3
            //      b   b1
            //          c   c1
            //              c2
            //              c3
            //      d

            IList<string> leaves = st.GetLeaves();
            Assert.That(leaves, Has.Count.EqualTo(8));
            Assert.That(leaves, Contains.Item(a1.Value));
            Assert.That(leaves, Contains.Item(a211.Value));
            Assert.That(leaves, Contains.Item(a3.Value));
            Assert.That(leaves, Contains.Item(b1.Value));
            Assert.That(leaves, Contains.Item(c1.Value));
            Assert.That(leaves, Contains.Item(c2.Value));
            Assert.That(leaves, Contains.Item(c3.Value));
            Assert.That(leaves, Contains.Item(d.Value));
        }
    }
}