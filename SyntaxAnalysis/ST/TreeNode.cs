using System;
using System.Collections.Generic;

namespace SyntaxAnalysis.ST
{
    public class TreeNode<T>
    {
        public T Value { get; set; }
        public List<TreeNode<T>> Children { get; private set; } = new List<TreeNode<T>>();
        internal TreeNode<T> Parent { get; private set; }

        internal TreeNode(TreeNode<T> parent)
        {
            this.Parent = parent;
        }
        internal TreeNode(TreeNode<T> parent, T value) : this(parent)
        {
            this.Value = value;
        }

        public override string ToString() => $"Node: {Value.ToString()} ({Children.Count} children(s))";


        internal void PrintNode(string prefix, TreeNode<T> currentNode)
        {
            string op = currentNode == this ? "~" : "+";
            Console.WriteLine($"{prefix} {op} {Value}");

            foreach (TreeNode<T> n in Children)
            {
                if (Children.IndexOf(n) == Children.Count - 1)
                {
                    n.PrintNode(prefix + "    ", currentNode);
                }
                else
                {
                    n.PrintNode(prefix + "   |", currentNode);
                }
            }
        }
    }
}