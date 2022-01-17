using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DerekWare.Collections;

namespace DerekWare.Iris
{
    class TreeView : System.Windows.Forms.TreeView
    {
        protected new bool DesignMode => base.DesignMode || Extensions.IsDesignMode();

        public class TreeNode : System.Windows.Forms.TreeNode
        {
            public TreeNode(string text)
            {
                Text = text;
            }

            public IReadOnlyList<TreeNode> AllNodes => GetAllNodes<TreeNode>(Nodes).ToList();

            public virtual T Add<T>(T child)
                where T : TreeNode
            {
                return Add(Nodes, child);
            }

            public static T Add<T>(TreeNodeCollection parent, T child)
                where T : TreeNode
            {
                parent.InsertSortedInUntypedList<TreeNode>(child, (x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.Text, y.Text));
                return child;
            }

            public static T Find<T>(TreeNodeCollection parent, string text)
                where T : TreeNode
            {
                return Find<T>(parent, i => Equals(i.Text, text));
            }

            public static T Find<T>(TreeNodeCollection parent, Func<T, bool> predicate)
                where T : TreeNode
            {
                return parent.OfType<T>().FirstOrDefault(predicate);
            }

            public static IEnumerable<T> GetAllNodes<T>(TreeNodeCollection parent)
                where T : TreeNode
            {
                foreach(var node in parent.OfType<T>())
                {
                    yield return node;

                    foreach(var i in GetAllNodes<T>(node.Nodes))
                    {
                        yield return i;
                    }
                }
            }

            public static void Remove<T>(TreeNodeCollection parent, Func<T, bool> predicate)
                where T : TreeNode
            {
                parent.RemoveWhere(predicate);
            }
        }
    }
}
