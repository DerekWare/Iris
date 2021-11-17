using System;
using System.Linq;
using DerekWare.Collections;
using DerekWare.Diagnostics;
using DerekWare.Expressions;
using DerekWare.Reflection;
using DerekWare.Strings;
using Enum = DerekWare.Reflection.Enum;

namespace DerekWare.Query
{
    /// <summary>
    ///     Groups multiple expressions together for evaluation.
    /// </summary>
    public enum ExpressionType
    {
        /// <summary>
        ///     The expression evaluates as true if all child expressions evaluate as true.
        /// </summary>
        [Name("&&")]
        And,

        /// <summary>
        ///     The expression evaluates as true if any child expressions evaluate as true.
        /// </summary>
        [Name("||")]
        Or
    }

    /// <summary>
    ///     Basic interface for a tree of objects that may be evaluated as a true/false statement and grouped together as an
    ///     and/or statement when evaluating children.
    /// </summary>
    public interface IExpression : IEvaluatable, ITreeNode<IExpression, IEvaluatable>
    {
        ExpressionType ExpressionType { get; }
    }

    public class Expression : TreeNode<IExpression, IEvaluatable>, IExpression
    {
        public Expression(IExpression parent, ExpressionType type)
            : base(parent)
        {
            ExpressionType = type;
        }

        public ExpressionType ExpressionType { get; set; }

        public override string ToString()
        {
            if(Count <= 0)
            {
                return base.ToString();
            }

            var result = $"{this[0]}";

            for(var i = 1; i < Count; ++i)
            {
                result += $" {ExpressionType} {this[i]}";
            }

            return result;
        }

        #region Conversion

        public static Expression Parse(string text)
        {
            return Parse(text.SplitArguments().ToQueue());
        }

        internal static Expression Parse(Queue<string> args)
        {
            // The number of arguments should either be 3 (key operator value)
            // or, to include the joining clauses, expression_count / 3 + 
            // expression_count - 1.
            //
            // TODO look for markers to allow for explicit grouping

            var parent = new Expression(null, ExpressionType.Or);
            Expression child = null;

            while(args.Count > 0)
            {
                // Parse the clause
                var clause = Clause.Parse(args);

                // If the expression type (and/or) changes, start a new group
                var type = args.Count > 0 ? Enum.Parse<ExpressionType>(args.Pop(), true) : 0;

                if(type != child?.ExpressionType)
                {
                    parent.Add(child = new Expression(parent, type));
                }

                child.Add(clause);
            }

            Debug.Trace(parent, string.Empty);

            return parent;
        }

        #endregion

        #region IEvaluatable

        public bool Evaluate()
        {
            switch(ExpressionType)
            {
                case ExpressionType.And:
                    return this.All(i => i.Evaluate());

                case ExpressionType.Or:
                    return this.Any(i => i.Evaluate());

                default:
                    throw new ArgumentOutOfRangeException(nameof(ExpressionType));
            }
        }

        #endregion
    }
}
