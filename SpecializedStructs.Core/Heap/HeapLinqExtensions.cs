using SpecializedStructs;
using SpecializedStructs.Core.Heap;

namespace System.Linq
{
    public static class HeapLinqExtensions
    {
        /// <summary>
        /// Filters a sequence of <see cref="HeapLeaf{T}"/> matches a specified predicate.
        /// </summary>
        public static bool Any<T>(this Heap<T> heap, Func<T, bool> predicate) => heap.Leafs.Any(i => predicate(i.Data));

        /// <summary>
        /// Filters all sequence of <see cref="HeapLeaf{T}"/> matches a specified predicate.
        /// </summary>
        public static bool All<T>(this Heap<T> heap, Func<T, bool> predicate) => heap.Leafs.All(i => predicate(i.Data));

        /// <summary>
        /// Gets the first or default value from the heap tree leafs that matches a specifed predicate.
        /// </summary>
        public static HeapLeaf<T> FirstOrDefault<T>(this Heap<T> heap, Func<T, bool> predicate) => heap.Leafs.FirstOrDefault(i => predicate(i.Data), HeapLeaf<T>.Empty);

        /// <summary>
        /// Checks if the heap tree contains a specfic leaf with the predicate.
        /// </summary>
        public static IEnumerable<HeapLeaf<T>> Where<T>(this Heap<T> heap, Func<T, bool> predicate) => heap.Leafs.Where(i => predicate(i.Data));
    }
}
