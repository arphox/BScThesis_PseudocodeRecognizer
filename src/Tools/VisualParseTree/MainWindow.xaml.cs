using LexicalAnalysis.Analyzer;
using LexicalAnalysis.Tokens;
using SyntaxAnalysis.Analyzer;
using SyntaxAnalysis.Tree;
using System;
using System.Diagnostics;
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
        private ParseTree<Token> _parseTree;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // ReSharper disable once UnusedVariable
            const string code = "program_kezd\r\n" +
                                "egész e = 2\r\n" +
                                "tört t = -2,4\r\n" +
                                "logikai l = hamis\r\n" +
                                "szöveg sz = \"alma\"\r\n" +
                                "szöveg[] szt = létrehoz[10]\r\n" +
                                "szt[0] = 2\r\n" +
                                "e = 3\r\n" +
                                "ha igaz akkor\r\n" +
                                "    beolvas e\r\n" +
                                "különben\r\n" +
                                "    kiír t\r\n" +
                                "elágazás_vége\r\n" +
                                "ha hamis akkor\r\n" +
                                "    beolvas e\r\n" +
                                "elágazás_vége\r\n" +
                                "ciklus_amíg hamis\r\n" +
                                "    kilép\r\n" +
                                "ciklus_vége\r\n" +
                                "egész i = 0\r\n" +
                                "i = - i\r\n" +
                                "i = ! hamis\r\n" +
                                "i = i\r\n" +
                                "i = -9,4\r\n" +
                                "i = 9 / i\r\n" +
                                "i = -9 mod i\r\n" +
                                "i = 9 . - i\r\n" +
                                "i = ! 9 - ! i\r\n" +
                                "egész[] et = létrehoz[10 + 2]\r\n" +
                                "et = létrehoz[e + 2]\r\n" +
                                "szt[0 - 3] = 2\r\n" +
                                "e = törtből_egészbe(e + 2)\r\n" +
                                "egész e2 = törtből_egészbe(e + 2)\r\n" +
                                "program_vége";


            Stopwatch sw = Stopwatch.StartNew();

            LexicalAnalyzerResult lexicalAnalyzerResult = new LexicalAnalyzer(code).Start();
            sw.Stop();
            Console.WriteLine($"Lexer: {sw.ElapsedMilliseconds} ms.");
            sw.Restart();

            SyntaxAnalyzerResult syntaxAnalyzerResult = new SyntaxAnalyzer(lexicalAnalyzerResult).Start();
            sw.Stop();
            Console.WriteLine($"Parser: {sw.ElapsedMilliseconds} ms.");
            sw.Restart();

            _parseTree = syntaxAnalyzerResult.ParseTree;
            ParseTreeConverter.FillTreeView(TreeView, _parseTree);
            Console.WriteLine($"Visual tree build: {sw.ElapsedMilliseconds} ms.");

            Console.WriteLine("IsSuccessful = " + syntaxAnalyzerResult.IsSuccessful);

            ButtonExpandAll_Click(null, null);

            if (!syntaxAnalyzerResult.IsSuccessful)
                MessageBox.Show("The syntax analysis failed!!! :( ");
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
