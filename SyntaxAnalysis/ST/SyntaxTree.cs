using System;
using System.Collections.Generic;
using System.Linq;

namespace SyntaxAnalysis.ST
{
    public class SyntaxTree<T>
    {
        private static readonly bool PRINTPRETTY_ENABLED = false; // only not const because I don't want compiler warnings

        public TreeNode<T> Root { get; private set; }
        internal TreeNode<T> CurrentNode { get; private set; }

        internal SyntaxTree(T rootValue)
        {
            this.Root = new TreeNode<T>(null, rootValue);
            CurrentNode = this.Root;
        }

        internal void StartNode(T value)
        {
            TreeNode<T> newNode = new TreeNode<T>(CurrentNode, value);
            CurrentNode.Children.Add(newNode);
            CurrentNode = newNode;

            PrintPretty();
        }
        internal void EndNode()
        {
            CurrentNode = CurrentNode.Parent;

            PrintPretty();
        }
        internal void RemoveLastNode()
        {
            if (CurrentNode.Children.Count == 0)
            {
                CurrentNode.Parent.Children.Remove(CurrentNode);
                CurrentNode = CurrentNode.Parent;
            }
            else
            {
                CurrentNode.Children.Remove(CurrentNode.Children.Last());
            }

            PrintPretty();
        }
        public T GetLastToken()
        {
            if (CurrentNode == null)
            {
                return Root.Value;
            }
            if (CurrentNode.Children.Count == 0)
            {
                return CurrentNode.Value;
            }
            else
            {
                return CurrentNode.Children.Last().Value;
            }
        }



        public List<T> GetLeaves()
        {
            // based on http://stackoverflow.com/q/27884356/4215389
            List<T> leaves = new List<T>();
            if (Root == null)
            {
                return leaves;
            }

            Queue<TreeNode<T>> nodeQueue = new Queue<TreeNode<T>>();
            nodeQueue.Enqueue(Root);

            TreeNode<T> currentNode = Root;
            while (nodeQueue.Count != 0)
            {
                currentNode = nodeQueue.Dequeue();
                if (currentNode.Children.Count == 0)
                { 
                    leaves.Add(currentNode.Value);
                }
                currentNode.Children.ForEach(child => nodeQueue.Enqueue(child));
            }
            return leaves;
        }
        private void PrintPretty()
        {
            if (!PRINTPRETTY_ENABLED)
            {
                return;
            }

            Console.WriteLine("\n\n\n\n\n");
            Root.PrintNode("", CurrentNode);
        }
    }
}