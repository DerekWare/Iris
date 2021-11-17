using System;
using System.Collections.Generic;
using System.ComponentModel;
using DerekWare.Expressions;
using DerekWare.Reflection;
using DerekWare.Strings;

namespace DerekWare.Query
{
    /// <summary>
    ///     Clause operators; e.g. 'foo' Not Equals 'bar'.
    /// </summary>
    [Flags]
    public enum ClauseOperator : uint
    {
        [Alias("="), Alias("=="), Description("The property equals the given value")]
        Equals = 1u << 0,

        [Alias(">"), Description("A comparison of the two values indicates that a precedes b")]
        GreaterThan = 1u << 1,

        [Alias("<"), Description("A comparison of the two values indicates that b precedes a")]
        LessThan = 1u << 2,

        [Alias("Includes"), Alias("Has"), Description("The property in string form contains the given value")]
        Contains = 1u << 3,

        [Alias("BeginsWith"), Alias("Starts"), Description("The property in string form starts with the given value")]
        StartsWith = 1u << 4,

        [Alias("Ends"), Description("The property in string form ends with the given value")]
        EndsWith = 1u << 5,

        [Alias(">="), Description("A comparison of the two values indicates that a precedes or is equal to b")]
        GreaterThanOrEquals = GreaterThan | Equals,

        [Alias("<="), Description("A comparison of the two values indicates that b precedes or is equal to a")]
        LessThanOrEquals = LessThan | Equals,

        [Alias("!"), Description("Reverse the operator (e.g. equals becomes not equals)")]
        Not = 1u << 31
    }

    /// <summary>
    ///     Extends the IEvaluatable interface to support a basic statement, such as "A Equals B."
    /// </summary>
    public interface IClause : IEvaluatable
    {
        /// <summary>
        ///     The operator used to evaluate the clause.
        /// </summary>
        ClauseOperator Operator { get; set; }

        /// <summary>
        ///     Comparison type used when X and Y are string values.
        /// </summary>
        StringComparison StringComparison { get; set; }

        /// <summary>
        ///     The left value used in a comparison or the single value to evaluate.
        /// </summary>
        object X { get; set; }

        /// <summary>
        ///     The right value used in a comparison.
        /// </summary>
        object Y { get; set; }

        /// <summary>
        ///     Resolves X and Y and evaluates the expression.
        /// </summary>
        bool Evaluate(out IClause resolved);
    }

    public class Clause : IClause
    {
        static readonly Dictionary<string, ClauseOperator> ClauseOperatorMap =
            new Reflector(typeof(ClauseOperator)).GetFieldValues<ClauseOperator>(StringComparer.OrdinalIgnoreCase);

        public ClauseOperator Operator { get; set; }
        public StringComparison StringComparison { get; set; }
        public object X { get; set; }
        public object Y { get; set; }

        public override string ToString()
        {
            return $"[\"{X}\" {Operator} \"{Y}\"]";
        }

        #region Conversion

        internal static Clause Parse(Collections.Queue<string> args)
        {
            // "A Equals B"
            var x = args.Pop();
            var o = args.Pop();
            var y = args.Pop();

            return new Clause
            {
                X = x.IsNullOrEmpty() || x.Equals("null", StringComparison.OrdinalIgnoreCase) ? null : x,
                Operator = ParseClauseOperator(o),
                Y = y.IsNullOrEmpty() || y.Equals("null", StringComparison.OrdinalIgnoreCase) ? null : y
            };
        }

        #endregion

        #region IClause

        public bool Evaluate(out IClause resolved)
        {
            // TODO
            throw new NotImplementedException();
        }

        #endregion

        #region IEvaluatable

        public bool Evaluate()
        {
            return Evaluate(out var resolved);
        }

        #endregion

        public static ClauseOperator ParseClauseOperator(string value)
        {
            if(!TryParseClauseOperator(value, out var result))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            return result;
        }

        public static bool TryParseClauseOperator(string value, out ClauseOperator o)
        {
            // Text representation of an operator may be standard enum style, or it might be a concatenation such as NotContains.
            // Strip out any commas and spaces that might come from the standard ToString and parse the concatenated string.
            // Rather than using Enum.TryParse, we'll just use the operator map above, since it includes name attributes.
            //
            // TODO if one of the X or Y values has a comma in it, this won't work
            value = value.Split(",".ToCharArray()).Trim().Join(null);
            o = 0;

            while(!value.IsNullOrEmpty())
            {
                var index = value.IndexOfAny(ClauseOperatorMap.Keys, 0, StringComparison.OrdinalIgnoreCase);

                if(index?.Value != 0)
                {
                    return false;
                }

                var values = value.SplitAt(index.Value.Key.Length);
                o |= ClauseOperatorMap[values[0]];
                value = values[1];
            }

            return true;
        }
    }
}
