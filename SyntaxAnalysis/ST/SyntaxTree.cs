using System;
using System.Collections.Generic;
using System.Linq;

namespace SyntaxAnalysis.ST
{
    public class SyntaxTree<T>
    {
        private static readonly bool PrintprettyEnabled = false; // only not const because I don't want compiler warnings

        public TreeNode<T> Root { get; }
        internal TreeNode<T> CurrentNode { get; private set; }

        internal SyntaxTree(T rootValue)
        {
            Root = new TreeNode<T>(null, rootValue);
            CurrentNode = Root;
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
            return Root.GetLeaves();
        }
        private void PrintPretty()
        {
            if (!PrintprettyEnabled)
            {
                return;
            }

            Console.WriteLine("\n\n\n\n\n");
            Root.PrintNode("", CurrentNode);
        }
    }
}