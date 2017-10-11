using LexicalAnalysis.LexicalAnalyzer;
using LexicalAnalysis.Tokens;
using SyntaxAnalysis;
using SyntaxAnalysis.ST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VisualParseTree
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string Code = "program_kezd\r\n\tegész a\r\n\tbeolvas a\r\n\tegész b\r\n\tbeolvas b\r\n\tegész c\r\n\tbeolvas c\r\n\ttört diszkrimináns=b*b-(4*a*c)\r\n\tha diszkrimináns<0,0 akkor\r\n\t\tkiír \"Nincs valós gyöke!\"\r\n\tkülönben\r\n\t\tkiír \"Van legalább egy valós gyöke!\"\r\n\telágazás_vége\r\nprogram_vége";
        private SyntaxTree<Token> _syntaxTree;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<Token> tokenList = new LexicalAnalyzer(Code).Analyze().Tokens;

            SyntaxAnalyzerResult result = new SyntaxAnalyzer(tokenList).Start();
            _syntaxTree = result.SyntaxTree;

            new SyntaxTreeConverter(TreeView, _syntaxTree).SetTreeView();
            Console.WriteLine("SUCCESS? : " + result.IsSuccessful);
        }

        private void ButtonExpandAll_Click(object sender, RoutedEventArgs e)
        {
            (TreeView.Items[0] as TreeViewItem).ExpandAll();
            TreeView.Items.Refresh();
        }

        private void ButtonCollapseAll_Click(object sender, RoutedEventArgs e)
        {
            (TreeView.Items[0] as TreeViewItem).CollapseAll();
            TreeView.Items.Refresh();
        }

        private void ButtonPrintLeaves_Click(object sender, RoutedEventArgs e)
        {
            IList<Token> leaves = _syntaxTree.GetLeaves();
            string[] leaves2 = leaves.Select(s => s.ToString()).ToArray();
            Console.WriteLine(string.Join("\n", leaves2));
        }

    }
}
