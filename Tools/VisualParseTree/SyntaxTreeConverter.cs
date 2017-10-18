using LexicalAnalysis.Tokens;
using System.Windows.Controls;
using SyntaxAnalysis.Tree;

namespace VisualParseTree
{
    internal static class SyntaxTreeConverter
    {
        /// <summary>
        /// Clears a TreeView and fills it with a <see cref="SyntaxTree{T}"/>'s content.
        /// </summary>
        /// <param name="treeView"></param>
        /// <param name="syntaxTree"></param>
        public static void FillTreeView(TreeView treeView, SyntaxTree<Token> syntaxTree)
        {
            treeView.Items.Clear();
            treeView.Items.Add(TraverseNode(syntaxTree.Root));
        }

        private static TreeViewItem TraverseNode(TreeNode<Token> node)
        {
            TreeViewItem item = new TreeViewItem()
            {
                Header = node.ToString()
            };
            foreach (TreeNode<Token> child in node.Children)
            {
                item.Items.Add(TraverseNode(child));
            }
            return item;
        }
    }
}