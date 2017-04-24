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
        private TreeView visualTree;
        private SyntaxTree<Token> dataTree;

        public SyntaxTreeConverter(TreeView treeView, SyntaxTree<Token> syntaxTree)
        {
            this.visualTree = treeView;
            this.dataTree = syntaxTree;
        }


        public void SetTreeView()
        {
            visualTree.Items.Add(TraverseNode(dataTree.Root));
        }

        private TreeViewItem TraverseNode(TreeNode<Token> node)
        {
            TreeViewItem item = new TreeViewItem()
            {
                Header = node.Value
            };
            foreach (TreeNode<Token> child in node.Children)
            {
                item.Items.Add(TraverseNode(child));
            }
            return item;
        }
    }
}