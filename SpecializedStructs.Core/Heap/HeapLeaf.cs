using System.Diagnostics.CodeAnalysis;

namespace SpecializedStructs.Core.Heap
{
    /// <summary>
    /// Struct to define a leaf of specific <see cref="Heap"/>. 
    /// This contains the key and the data representation for the leaf.
    /// </summary>
    public struct HeapLeaf<T>
    {
        #region Constants
        public static HeapLeaf<T> Empty = new HeapLeaf<T>() { Index = -1 };
        #endregion

        #region Properties & Fields
        /// <summary>
        /// The key to determining the position of this leaf in the heap tree.
        /// </summary>
        public long Key { get; }
        /// <summary>
        /// Representation of the data stored by this leaf.
        /// </summary>
        public T Data { get; }
        /// <summary>
        /// The index of the leaf in the heap three.
        /// </summary>
        public int Index { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="HeapLeaf{T}"/> with the key value and data to this leaf.
        /// </summary>
        /// <param name="key">The key value of the leaf.</param>
        /// <param name="data">The data that this leaf represents.</param>
        /// <exception cref="ArgumentNullException">Throws when the <paramref name="data"/> is not provided.</exception>
        public HeapLeaf(long key, T data)
        {
            this.Index = 0;
            Key = key;
            Data = data ??
                throw new ArgumentNullException(nameof(data));
        }
        #endregion

        #region Methods
        /// <summary>
        /// Swap the indexes between two leafs.
        /// </summary>
        public void SwapIndex(ref HeapLeaf<T> leaf)
        {
            var oldIndex = this.Index;
            this.Index = leaf.Index;
            leaf.Index = oldIndex;
        }

        /// <inheritdoc/>
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is HeapLeaf<T> leaf)
            {
                return leaf.Index == this.Index && leaf.Key == this.Key;
            }

            return false;
        }

        /// <inheritdoc/>
        public override string ToString() => this.Data?.ToString() ?? $"Data not provided for the leaf with key {this.Key} and index {this.Index}";

        /// <summary>
        /// Checks if the instance of <see cref="HeapLeaf{T}"/> is <see cref="Empty"/>.
        /// </summary>
        internal bool IsEmpty() => this == Empty;
        #endregion

        #region Operators
        /// <summary>
        /// Explicit cast operator to gets a <typeparamref name="T"/> instance from the <paramref name="leaf"/>.
        /// </summary>
        public static explicit operator T(HeapLeaf<T> leaf) => leaf.Data;
        /// <summary>
        /// Returns a value indicating the key and index of two <see cref="HeapLeaf{T}"/> are equal.
        /// </summary>
        public static bool operator ==(HeapLeaf<T> left, HeapLeaf<T> right) => left.Equals(right);
        /// <summary>
        /// Returns a value indicating the key and index of two <see cref="HeapLeaf{T}"/> are not equal.
        /// </summary>
        public static bool operator !=(HeapLeaf<T> left, HeapLeaf<T> right) => !left.Equals(right);
        #endregion
    }
}
