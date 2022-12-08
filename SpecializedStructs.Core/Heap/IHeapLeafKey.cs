namespace SpecializedStructs.Core.Heap
{
    /// <summary>
    /// Defines a type of the <see cref="Heap{T}"/> can gets the key value for an instance of class that implements this interface.
    /// </summary>
    public interface IHeapLeafKey
    {
        /// <summary>
        /// Returns the key value for this instance.
        /// </summary>
        long GetKey();
    }
}
