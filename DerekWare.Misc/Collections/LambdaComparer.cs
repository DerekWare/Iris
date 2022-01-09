using System.Collections.Generic;

namespace DerekWare.Collections
{
    public delegate int ComparerDelegate<in T>(T x, T y);

    public class LambdaComparer<T> : IComparer<T>
    {
        readonly ComparerDelegate<T> Comparer;

        public LambdaComparer(ComparerDelegate<T> comparer)
        {
            Comparer = comparer;
        }

        #region Equality

        public int Compare(T x, T y)
        {
            return Comparer(x, y);
        }

        #endregion
    }
}
