using System;
using System.Collections.Generic;
using System.Linq;

namespace SyntaxAnalysis.ST
{
    public class TreeNode<T>
    {
        public T Value { get; set; }
        public List<TreeNode<T>> Children { get; } = new List<TreeNode<T>>();
        internal TreeNode<T> Parent { get; }

        internal TreeNode(TreeNode<T> parent)
        {
            Parent = parent;
        }
        internal TreeNode(TreeNode<T> parent, T value) : this(parent)
        {
            Value = value;
        }

        public override string ToString() => Value.ToString();


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
        internal List<T> GetLeaves()
        {
            if (Children.Count == 0)
            {
                return new List<T>() { Value };
            }
            else
            {
                IEnumerable<T> leaves = new List<T>();
                foreach (TreeNode<T> child in Children)
                {
                    leaves = leaves.Concat(child.GetLeaves());
                }
                return leaves.ToList();
            }
        }
    }
}