using System.Collections.Generic;

namespace SyntaxAnalysis.Tree
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

        /// <summary>
        /// Adds a child to the node.
        /// </summary>
        /// <param name="value"></param>
        internal TreeNode<T> AddChild(T value)
        {
            var newNode = new TreeNode<T>(this, value);
            Children.Add(newNode);
            return newNode;
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