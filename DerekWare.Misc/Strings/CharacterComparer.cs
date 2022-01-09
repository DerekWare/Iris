using System.Collections.Generic;

namespace DerekWare.Strings
{
    public class CharacterComparer : IEqualityComparer<char>, IComparer<char>
    {
        public static readonly CharacterComparer Default = new(false);
        public static readonly CharacterComparer DefaultIgnoreCase = new(true);

        public readonly bool IgnoreCase;

        public CharacterComparer(bool ignoreCase)
        {
            IgnoreCase = ignoreCase;
        }

        #region Equality

        public bool Equals(char x, char y)
        {
            if(IgnoreCase)
            {
                x = char.ToLower(x);
                y = char.ToLower(y);
            }

            return x.Equals(y);
        }

        public int GetHashCode(char obj)
        {
            if(IgnoreCase)
            {
                obj = char.ToLower(obj);
            }

            return obj.GetHashCode();
        }

        public int Compare(char x, char y)
        {
            if(IgnoreCase)
            {
                x = char.ToLower(x);
                y = char.ToLower(y);
            }

            return x.CompareTo(y);
        }

        #endregion
    }
}
