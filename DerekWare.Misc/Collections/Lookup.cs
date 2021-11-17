namespace DerekWare.Collections
{
    public interface ILookup<in TKey, TValue> : IReadOnlyLookup<TKey, TValue>
    {
        new TValue this[TKey key] { get; set; }
    }

    public interface IReadOnlyLookup<in TKey, TValue>
    {
        TValue this[TKey key] { get; }
        bool TryGetValue(TKey key, out TValue value);
    }
}
