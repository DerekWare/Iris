namespace DerekWare.Query
{
    /// <summary>
    ///     Used to select the correct value from an item. For example, a list might use a key of 1 to indicate that element at
    ///     List[1] should be evaluated. In thise example, the selector would receive item = List, key = 1 and return List[1].
    /// </summary>
    public delegate object SelectorDelegate(object item, object key);
}
