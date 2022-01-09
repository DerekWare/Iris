namespace DerekWare.Collections
{
    public interface IMap<in TKey, TValue> : IReadOnlyMap<TKey, TValue>
    {
        new TValue this[TKey key] { get; set; }
    }

    public interface IReadOnlyMap<in TKey, TValue>
    {
        TValue this[TKey key] { get; }
        bool TryGetValue(TKey key, out TValue value);
    }
}
