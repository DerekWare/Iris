using System;

namespace DerekWare.Expressions
{
    /// <summary>
    ///     Basic interface for any object that may be evaluated as a true/false statement.
    /// </summary>
    public interface IEvaluatable
    {
        bool Evaluate();
    }

    /// <summary>
    ///     Wrapper class to convert a lambda expression to an IEvaluatable.
    /// </summary>
    public class LambdaEvaluatable<T> : IEvaluatable
    {
        protected readonly Func<T, bool> Selector;
        protected readonly T Target;

        public LambdaEvaluatable(T target, Func<T, bool> selector)
        {
            Target = target;
            Selector = selector;
        }

        #region IEvaluatable

        public bool Evaluate()
        {
            return Selector(Target);
        }

        #endregion
    }

    public static class Evaluatable
    {
        public static IEvaluatable ToEvaluatable<T>(this Func<T, bool> selector, T target)
        {
            return new LambdaEvaluatable<T>(target, selector);
        }
    }
}
