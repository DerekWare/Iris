using System;
using System.ComponentModel;
using DerekWare.Reflection;

namespace DerekWare.Query
{
    [Flags]
    public enum Operator : uint
    {
        [Name("="), Name("=="), Description("The property equals the given value")]
        Equals = 1u << 0,

        [Name(">"), Description("A comparison of the two values indicates that a precedes b")]
        GreaterThan = 1u << 1,

        [Name("<"), Description("A comparison of the two values indicates that b precedes a")]
        LessThan = 1u << 2,

        [Name("Includes"), Name("Has"), Description("The property contains the given value")]
        Contains = 1u << 3,

        [Name("BeginsWith"), Name("Starts"), Description("The property begins with the given value")]
        StartsWith = 1u << 4,

        [Name("Ends"), Description("The property ends with the given value")]
        EndsWith = 1u << 5,

        [Name(">="), Description("A comparison of the two values indicates that a precedes or is equal to b")]
        GreaterThanOrEquals = GreaterThan | Equals,

        [Name("<="), Description("A comparison of the two values indicates that b precedes or is equal to a")]
        LessThanOrEquals = LessThan | Equals,

        [Name("!"), Description("Reverse the operator (e.g. equals becomes not equals)")]
        Not = 1u << 31
    }
}
