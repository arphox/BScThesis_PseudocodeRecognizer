using LexicalAnalysis;
using LexicalAnalysis.Tokens;
using SyntaxAnalysis;
using SyntaxAnalysis.ST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using LexicalAnalysis.LexicalAnalyzer;

namespace VisualParseTree
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Main();
        }

        private SyntaxTree<Token> _syntaxTree;

        void Main()
        {
            string path = "08_masodfoku.opl";

            List<Token> tokenList = new LexicalAnalyzer().Analyze(ReadUtf8File(path)).Tokens;

            Tuple<SyntaxTree<Token>, bool> result = new SyntaxAnalyzer(tokenList).Start();
            _syntaxTree = result.Item1;

            new SyntaxTreeConverter(TreeView, _syntaxTree).SetTreeView();
            Console.WriteLine("SUCCESS? : " + result.Item2);
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
            List<Token> leaves = _syntaxTree.GetLeaves();
            string[] leaves2 = leaves.Select(s => s.ToString()).ToArray();
            Console.WriteLine(string.Join("\n", leaves2));
        }

        internal static string ReadUtf8File(string path)
        {
            return File.ReadAllText(path);
        }
    }
}
