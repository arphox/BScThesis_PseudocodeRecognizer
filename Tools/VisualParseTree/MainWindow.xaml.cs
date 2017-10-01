using LexicalAnalysis;
using VisualParseTree;
using LexicalAnalysis.Tokens;
using SyntaxAnalysis;
using SyntaxAnalysis.ST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

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

        const string _inputFolderPath = @"..\..\..\_input\";
        const string _outputFolderPath = @"..\..\..\_output\";
        private SyntaxTree<Token> syntaxTree;

        void Main()
        {
            string path = _inputFolderPath + "08_masodfoku.opl";

            List<Token> tokenList = new LexicalAnalyzer().PerformLexicalAnalysisOnFile(path);

            Tuple<SyntaxTree<Token>, bool> result = new SyntaxAnalyzer(tokenList).Start();
            syntaxTree = result.Item1;

            new SyntaxTreeConverter(treeView, syntaxTree).SetTreeView();
            Console.WriteLine("SUCCESS? : " + result.Item2);
        }

        private void ButtonExpandAll_Click(object sender, RoutedEventArgs e)
        {
            (treeView.Items[0] as TreeViewItem).ExpandAll();
            treeView.Items.Refresh();
        }

        private void ButtonCollapseAll_Click(object sender, RoutedEventArgs e)
        {
            (treeView.Items[0] as TreeViewItem).CollapseAll();
            treeView.Items.Refresh();
        }

        private void ButtonPrintLeaves_Click(object sender, RoutedEventArgs e)
        {
            List<Token> leaves = syntaxTree.GetLeaves();
            string[] leaves2 = leaves.Select(s => s.ToString()).ToArray();
            Console.WriteLine(string.Join("\n", leaves2));
        }
    }
}
