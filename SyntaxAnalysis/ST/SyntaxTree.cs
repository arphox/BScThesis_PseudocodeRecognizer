using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxAnalysis.ST
{
    public class SyntaxTree<T>
    {
        public TreeNode<T> Root { get; private set; }
        public TreeNode<T> CurrentNode { get; private set; }


        public SyntaxTree(T rootValue)
        {
            this.Root = new TreeNode<T>(null, rootValue);
            CurrentNode = this.Root;
        }

        public void StartNode(T value)
        {
            TreeNode<T> newNode = new TreeNode<T>(CurrentNode, value);
            CurrentNode.Children.Add(newNode);
            CurrentNode = newNode;
        }
        public void EndNode()
        {
            CurrentNode = CurrentNode.Parent;
        }
        public void RemoveLatestNode()
        {
            if (CurrentNode.Children.Count == 0)
            {
                CurrentNode.Parent.Children.Remove(CurrentNode);
            }
            else
            {
                CurrentNode.Children.Remove(CurrentNode.Children.Last());
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
                currentNode = nodeQueue.Peek();
                nodeQueue.Dequeue();

                if (currentNode.Children.Count == 0)
                {
                    leaves.Add(currentNode.Value);
                }

                for (int i = 0; i < currentNode.Children.Count(); i++)
                {
                    nodeQueue.Enqueue(currentNode.Children[i]);
                }
            }
            return leaves;
        }
    }
}