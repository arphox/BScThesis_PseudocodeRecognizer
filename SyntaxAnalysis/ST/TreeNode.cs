using System;
using System.Collections.Generic;
using System.Linq;

namespace SyntaxAnalysis.ST
{
    public class TreeNode<T>
    {
        public TreeNode<T> Parent { get; private set; }
        public T Value { get; set; }
        public List<TreeNode<T>> Children { get; private set; } = new List<TreeNode<T>>();

        public TreeNode(TreeNode<T> parent)
        {
            this.Parent = parent;
        }
        public TreeNode(TreeNode<T> parent, T value) : this(parent)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            return $"Node: {Value.ToString()} ({Children.Count} children)";
        }


        public void PrintNode(string prefix, TreeNode<T> currentNode)
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