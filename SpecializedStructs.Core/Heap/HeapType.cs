namespace SpecializedStructs.Core.Heap
{
    /// <summary>
    /// Defines how the heap will sort your leafs.
    /// </summary>
    public enum HeapType
    {
        /// <summary>
        /// When maximum, the parent leaf has a key greater or equal your childrens.
        /// </summary>
        Maximum,
        /// <summary>
        /// When minimum, the parent leaf has a key less or equal your childrens.
        /// </summary>
        Minimum
    }
}
