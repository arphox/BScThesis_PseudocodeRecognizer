using System;
using System.Collections.Generic;
using System.Linq;

namespace SyntaxAnalysis.Tree
{
    public sealed class SyntaxTree<T>
    {
        public TreeNode<T> Root { get; }
        public TreeNode<T> CurrentNode { get; private set; }

        internal SyntaxTree(T rootValue)
        {
            Root = new TreeNode<T>(null, rootValue);
            CurrentNode = Root;
        }

        /// <summary>
        /// Starts a new node and sets the <see cref="CurrentNode"/> to the newly created node
        /// </summary>
        internal TreeNode<T> StartNode(T value)
        {
            TreeNode<T> child = CurrentNode.AddChild(value);
            CurrentNode = child;
            return child;
        }

        /// <summary>
        /// Ends the current node, setting the <see cref="CurrentNode"/> to the current node's parent node.
        /// </summary>
        internal void EndNode()
        {
            CurrentNode = CurrentNode.Parent ?? throw new InvalidOperationException("This node does not have a parent set (probably the root of the tree).");
        }

        /// <summary>
        /// Removes the last added node.
        /// </summary>
        internal void RemoveLastAddedNode()
        {
            if (CurrentNode.Children.Any())
            {
                CurrentNode.Children.Remove(CurrentNode.Children.Last());
            }
            else
            {
                if (CurrentNode.Parent == null)
                    throw new InvalidOperationException("The root node cannot be removed!");

                CurrentNode.Parent.Children.Remove(CurrentNode);
                CurrentNode = CurrentNode.Parent;
            }
        }

        /// <summary>
        /// Gets the leaves of this <see cref="SyntaxTree{T}"/>.
        /// </summary>
        public IList<T> GetLeaves() => Root.GetLeaves();
    }
}