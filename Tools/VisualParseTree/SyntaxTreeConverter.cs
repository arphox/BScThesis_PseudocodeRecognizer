using LexicalAnalysis.Tokens;
using SyntaxAnalysis.ST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace VisualParseTree
{
    class SyntaxTreeConverter
    {
        private TreeView _visualTree;
        private SyntaxTree<Token> _dataTree;

        public SyntaxTreeConverter(TreeView treeView, SyntaxTree<Token> syntaxTree)
        {
            _visualTree = treeView;
            _dataTree = syntaxTree;
        }


        public void SetTreeView()
        {
            _visualTree.Items.Add(TraverseNode(_dataTree.Root));
        }

        private TreeViewItem TraverseNode(TreeNode<Token> node)
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