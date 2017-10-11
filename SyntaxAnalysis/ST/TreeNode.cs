using System;
using System.Collections.Generic;

namespace SyntaxAnalysis.ST
{
    public class TreeNode<T>
    {
        public T Value { get; }
        public IList<TreeNode<T>> Children { get; } = new List<TreeNode<T>>();
        public TreeNode<T> Parent { get; }

        internal TreeNode(TreeNode<T> parent, T value)
        {
            Parent = parent;
            Value = value;
        }

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

        internal IList<T> GetLeaves()
        {
            if (Children.Count == 0)
            {
                return new List<T>() { Value };
            }

            List<T> leaves = new List<T>();
            foreach (TreeNode<T> child in Children)
            {
                leaves.AddRange(child.GetLeaves());
            }
            return leaves;
        }

        public override string ToString() => Value.ToString();
    }
}