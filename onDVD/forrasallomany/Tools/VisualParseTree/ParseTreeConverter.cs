using LexicalAnalysis.Tokens;
using System.Windows.Controls;
using SyntaxAnalysis.Tree;

namespace VisualParseTree
{
    internal static class ParseTreeConverter
    {
        /// <summary>
        /// Clears a TreeView and fills it with a <see cref="ParseTree{T}"/>'s content.
        /// </summary>
        /// <param name="treeView"></param>
        /// <param name="parseTree"></param>
        public static void FillTreeView(TreeView treeView, ParseTree<Token> parseTree)
        {
            treeView.Items.Clear();
            treeView.Items.Add(TraverseNode(parseTree.Root));
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