using System.Text.RegularExpressions;

namespace DerekWare.Expressions
{
    public class WildcardPattern : Regex
    {
        public WildcardPattern(string pattern, RegexOptions options = RegexOptions.IgnoreCase)
            : base(ToRegex(pattern), options)
        {
        }

        public static string ToRegex(string pattern)
        {
            pattern = Escape(pattern).Replace("\\*", ".*");
            pattern = string.Concat("^", pattern.Replace("\\?", "."), "$");
            return pattern;
        }
    }
}
