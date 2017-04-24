using System.Collections.Generic;
using System.Linq;

namespace SyntaxAnalysis.ST
{
    public class TreeNode<T>
    {
        public TreeNode<T> Parent { get; private set; }
        public T Value { get; set; }
        public List<TreeNode<T>> Children { get; private set; } = new List<TreeNode<T>>();

        public TreeNode(TreeNode<T> parent)
        {
            this.Parent = parent;
        }
        public TreeNode(TreeNode<T> parent, T value) : this(parent)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            string children = string.Join(", ", Children.Select(s => s.Value));
            return $"TreeNode Value={Value.ToString()}, children:{children}, Parent: {Parent.Value}";
        }
    }
}