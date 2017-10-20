using LexicalAnalysis.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LexicalAnalysis.Analyzer;
using SyntaxAnalysis.Analyzer;
using SyntaxAnalysis.Tree;

namespace VisualParseTree
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SyntaxTree<Token> _syntaxTree;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Stopwatch sw = Stopwatch.StartNew();

            LexicalAnalyzerResult lexicalAnalyzerResult = new LexicalAnalyzer(SyntaxAnalyzer.TestCode).Analyze();
            List<TerminalToken> tokenList = lexicalAnalyzerResult.Tokens;
            sw.Stop();
            Console.WriteLine($"Lexer: {sw.ElapsedMilliseconds} ms.");
            sw.Restart();

            SyntaxAnalyzerResult syntaxAnalyzerResult = new SyntaxAnalyzer(tokenList).Start();
            sw.Stop();
            Console.WriteLine($"Parser: {sw.ElapsedMilliseconds} ms.");
            sw.Restart();

            _syntaxTree = syntaxAnalyzerResult.SyntaxTree;
            SyntaxTreeConverter.FillTreeView(TreeView, _syntaxTree);
            Console.WriteLine($"Visual tree build: {sw.ElapsedMilliseconds} ms.");

            Console.WriteLine("IsSuccessful = " + syntaxAnalyzerResult.IsSuccessful);

            ButtonExpandAll_Click(null, null);
        }

        private TreeViewItem SelectedItem
            => (TreeView.SelectedItem ?? TreeView.Items[0]) as TreeViewItem;

        private void ButtonExpandAll_Click(object sender, RoutedEventArgs e)
            => SelectedItem.ExpandAll();

        private void ButtonCollapseAll_Click(object sender, RoutedEventArgs e)
            => SelectedItem.CollapseAll();

        private void ButtonPrintLeaves_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(string.Join("\n", _syntaxTree.GetLeaves().Select(s => s.ToString())));
        }

    }
}
