using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace DerekWare.Collections
{
    public enum TreeNodeTypes
    {
        Unknown,

        /// <summary>
        ///     The node may have children, even if it is currently empty.
        /// </summary>
        Branch,

        /// <summary>
        ///     The node will never have children.
        /// </summary>
        Leaf
    }

    /// <summary>
    ///     Typeless version of a TreeNode; a generic type that has a single parent, zero or more children and zero or more
    ///     property key/value pairs. A TreeNode must be expanded before any of its properties are considered valid.
    /// </summary>
    public interface ITreeNode : IEnumerable
    {
        event EventHandler Collapsed;
        event EventHandler Collapsing;
        event EventHandler Expanded;
        event EventHandler Expanding;

        /// <summary>
        ///     The parent object in the tree hierarchy.
        /// </summary>
        object Parent { get; }

        /// <summary>
        ///     Object metadata.
        /// </summary>
        IEnumerable<KeyValuePair<object, object>> Properties { get; }

        /// <summary>
        ///     The node type.
        /// </summary>
        TreeNodeTypes TreeNodeType { get; }

        /// <summary>
        ///     Collapses the tree node, freeing any resources allocated during Expand.
        /// </summary>
        void Collapse();

        /// <summary>
        ///     Expands the tree node, allocating any resources necessary to use the object, including populating the child list
        ///     and properties.
        /// </summary>
        void Expand();
    }

    /// <summary>
    ///     Typed version of a TreeNode; a generic that has a single parent, zero or more children and zero or more property
    ///     values.
    /// </summary>
    public interface ITreeNode<out TParent, out TChild> : ITreeNode, IEnumerable<TChild>
        where TParent : class where TChild : class
    {
        new TParent Parent { get; }
    }

    public interface ITreeNode<out T> : ITreeNode<T, T>
        where T : class, ITreeNode<T>
    {
    }

    public class TreeNode<TParent, TChild> : ObservableList<TChild>, ITreeNode<TParent, TChild>
        where TParent : class where TChild : class
    {
        public event EventHandler Collapsed;
        public event EventHandler Collapsing;
        public event EventHandler Expanded;
        public event EventHandler Expanding;

        public TreeNode()
        {
        }

        public TreeNode(TParent parent, IEnumerable<TChild> children = null)
        {
            Parent = parent;
            AddRange(children.SafeEmpty());
        }

        public virtual IEnumerable<KeyValuePair<object, object>> Properties => Enumerable.Empty<KeyValuePair<object, object>>();

        public TParent Parent { get; protected set; }
        public TreeNodeTypes TreeNodeType { get; protected set; }

        protected virtual bool NeedPopulate => Count <= 0;

        object ITreeNode.Parent => Parent;

        /// <summary>
        ///     The node has been collapsed. Resources associated with child nodes may be freed.
        /// </summary>
        protected virtual void OnCollapsed()
        {
            Collapsed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        ///     The node is about to be collapsed. Resources associated with child nodes may be freed.
        /// </summary>
        protected virtual void OnCollapsing()
        {
            Collapsing?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            e.NewItems.ForEach<ITreeNode>(node =>
            {
                if(!ReferenceEquals(node.Parent, this))
                {
                    throw new InvalidOperationException("Parent mismatch");
                }
            });

            e.OldItems.ForEach<ITreeNode>(node =>
            {
                if(!ReferenceEquals(node.Parent, this))
                {
                    throw new InvalidOperationException("Parent mismatch");
                }
            });
        }

        /// <summary>
        ///     The node has been expanded. Resources associated with child nodes should have been allocated and the child list
        ///     populated.
        /// </summary>
        protected virtual void OnExpanded()
        {
            Expanded?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        ///     The node is about to be expanded. Resources associated with child nodes should be allocated and the child list
        ///     populated.
        /// </summary>
        protected virtual void OnExpanding()
        {
            Expanding?.Invoke(this, EventArgs.Empty);

            if(NeedPopulate)
            {
                Populate();
            }
        }

        /// <summary>
        ///     Called by OnExpanding when NeedPopulate is true.
        /// </summary>
        protected virtual void Populate()
        {
        }

        #region ITreeNode

        public void Collapse()
        {
            if(TreeNodeType == TreeNodeTypes.Leaf)
            {
                return;
            }

            OnCollapsing();
            OnCollapsed();
        }

        public void Expand()
        {
            if(TreeNodeType == TreeNodeTypes.Leaf)
            {
                throw new InvalidOperationException("Node is a leaf");
            }

            OnExpanding();
            OnExpanded();
        }

        #endregion
    }

    public class TreeNode<T> : TreeNode<T, T>
        where T : class, ITreeNode<T>
    {
        public TreeNode()
        {
        }

        public TreeNode(T parent, IEnumerable<T> children = null)
        {
            Parent = parent;
            AddRange(children.SafeEmpty());
        }
    }

    public static class Tree
    {
        /// <summary>
        ///     Ascends from a child node into a parent by collapsing the child.
        /// </summary>
        /// <returns>The parent node or null on error.</returns>
        public static object Ascend(this ITreeNode child)
        {
            if(child?.Parent is null)
            {
                return child;
            }

            child.Collapse();
            return child.Parent;
        }

        /// <summary>
        ///     Ascends from a child node into a parent by collapsing the child.
        /// </summary>
        /// <returns>The parent node or null on error.</returns>
        public static TParent Ascend<TParent, TChild>(this ITreeNode<TParent, TChild> child)
            where TParent : class where TChild : class
        {
            return (TParent)Ascend((ITreeNode)child);
        }

        /// <summary>
        ///     Descends into a child node by expanding it.
        /// </summary>
        /// <returns>The child node or null on error.</returns>
        public static ITreeNode Descend(this ITreeNode child)
        {
            if(child is null || (child.TreeNodeType == TreeNodeTypes.Leaf))
            {
                return null;
            }

            child.Expand();
            return child;
        }

        /// <summary>
        ///     Descends into a child node by expanding it.
        /// </summary>
        /// <returns>The child node or null on error.</returns>
        public static TChild Descend<TParent, TChild>(this ITreeNode<TParent, TChild> child)
            where TParent : class where TChild : class
        {
            return (TChild)Descend((ITreeNode)child);
        }
    }
}
