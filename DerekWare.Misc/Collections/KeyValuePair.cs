using System.Collections.Generic;

namespace DerekWare.Collections
{
    public static class KeyValuePair
    {
        public static KeyValuePair<TKey, TValue> Create<TKey, TValue>(TKey key, TValue value)
        {
            return new KeyValuePair<TKey, TValue>(key, value);
        }

        public static KeyValuePair<TKey, TValue> ToKeyValuePair<TKey, TValue>(this TKey key, TValue value)
        {
            return new KeyValuePair<TKey, TValue>(key, value);
        }
    }
}
