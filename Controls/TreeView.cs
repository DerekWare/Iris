using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DerekWare.Iris
{
    class TreeView : System.Windows.Forms.TreeView
    {
        protected new bool DesignMode => Extensions.IsDesignMode();

        // Intelligently adds or updates nodes using the correct node type and automatically sorted
        public class TreeNode : System.Windows.Forms.TreeNode
        {
            public TreeNode(string text)
            {
                Text = text;
            }

            public IEnumerable<TreeNode> AllChildNodes => GetAllChildNodes(Nodes);

            public virtual T Add<T>(T child)
                where T : TreeNode
            {
                return Add(Nodes, child);
            }

            public static T Add<T>(TreeNodeCollection parent, T child)
                where T : TreeNode
            {
                parent.Insert(FindInsertionPoint(parent, child), child);
                return child;
            }

            public static T Find<T>(TreeNodeCollection parent, string text)
                where T : TreeNode
            {
                return Find<T>(parent, i => i.Text.Equals(text));
            }

            public static T Find<T>(TreeNodeCollection parent, Func<T, bool> wherePredicate)
                where T : TreeNode
            {
                return parent.OfType<T>().FirstOrDefault(wherePredicate);
            }

            public static int FindInsertionPoint(TreeNodeCollection parent, TreeNode child)
            {
                var index = 0;

                foreach(TreeNode i in parent)
                {
                    if(string.Compare(child.Text, i.Text, StringComparison.CurrentCulture) < 0)
                    {
                        break;
                    }

                    ++index;
                }

                return index;
            }

            public static IEnumerable<TreeNode> GetAllChildNodes(TreeNodeCollection parent)
            {
                foreach(TreeNode node in parent)
                {
                    yield return node;
                }

                foreach(TreeNode node in parent)
                {
                    foreach(var i in GetAllChildNodes(node.Nodes))
                    {
                        yield return i;
                    }
                }
            }

            public static void Remove<T>(TreeNodeCollection parent, Func<T, bool> wherePredicate)
                where T : TreeNode
            {
                parent.OfType<T>().Where(wherePredicate).ToList().ForEach(parent.Remove);
            }
        }
    }
}
