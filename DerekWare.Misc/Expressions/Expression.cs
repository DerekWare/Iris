using System;
using System.Collections.Generic;
using System.Linq;
using DerekWare.Collections;
using DerekWare.Reflection;

namespace DerekWare.Expressions
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
        public Expression(IExpression parent, ExpressionType type = ExpressionType.And, IEnumerable<IEvaluatable> children = null)
            : base(parent, children)
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

            var result = this[0].ToString();

            for(var i = 1; i < Count; ++i)
            {
                result += $" {ExpressionType} {this[i]}";
            }

            return result;
        }

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
                    throw new FormatException("Unexpected expression type");
            }
        }

        #endregion
    }
}
