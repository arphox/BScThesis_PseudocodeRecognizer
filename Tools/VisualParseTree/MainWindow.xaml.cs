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
        private ParseTree<Token> _parseTree;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // ReSharper disable once UnusedVariable
            const string code = "program_kezd\r\n" +
                                "egész[] tömb = létrehoz[10]\r\n" +
                                "egész i = 0\r\n" +
                                "egész összeg = 0\r\n" +
                                "ciklus_amíg i < 10\r\n" +
                                "   egész temp = tömb[i]\r\n" +
                                "   összeg = összeg + temp\r\n" +
                                "   i = i + 1\r\n" +
                                "ciklus_vége\r\n" +
                                "kiír összeg\r\n" +
                                "program_vége";


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

            _parseTree = syntaxAnalyzerResult.ParseTree;
            ParseTreeConverter.FillTreeView(TreeView, _parseTree);
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
            Console.WriteLine(string.Join("\n", _parseTree.GetLeaves().Select(s => s.ToString())));
        }

    }
}
