using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace VisualParseTree
{
    public static class TreeViewExtensions
    {
        // http://stackoverflow.com/a/37410171/4215389


        public static void ExpandAll(this TreeViewItem treeViewItem, bool isExpanded = true)
        {
            treeViewItem.IsExpanded = isExpanded;
            var stack = new Stack<TreeViewItem>(treeViewItem.Items.Cast<TreeViewItem>());
            while (stack.Count > 0)
            {
                TreeViewItem item = stack.Pop();

                foreach (var child in item.Items)
                {
                    var childContainer = child as TreeViewItem;
                    if (childContainer == null)
                    {
                        childContainer = item.ItemContainerGenerator.ContainerFromItem(child) as TreeViewItem;
                    }

                    stack.Push(childContainer);
                }

                item.IsExpanded = isExpanded;
            }
        }

        public static void CollapseAll(this TreeViewItem treeViewItem)
        {
            treeViewItem.ExpandAll(false);
        }
    }
}
