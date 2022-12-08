using SpecializedStructs.Core.Heap;
using System.Collections;
using System.Xml.Schema;

namespace SpecializedStructs
{
    /// <summary>
    /// Struct to define a partial tree h-ary (binary, ternary, h-ary).
    /// </summary>
    public class Heap<T> : IEnumerable<HeapLeaf<T>>, IDisposable
    {
        #region Properties & Fields
        /// <summary>
        /// Index of the last left that was added, starts with 0 (root leaf).
        /// </summary>
        protected int lastLeafIndex = 0;

        /// <summary>
        /// The leafs of the heap tree.
        /// </summary>
        internal List<HeapLeaf<T>> Leafs { get; } = new List<HeapLeaf<T>>();
        /// <summary>
        /// The size of the tree, determines the number of children each leaf can have.
        /// </summary>
        public int Size { get; }
        /// <summary>
        /// Total number of leafs in this head tree.
        /// </summary>
        public int Count => Leafs.Count;
        /// <summary>
        /// Determine how this heap should sort your leafs. The defaults is <see cref="HeapType.Maximum"/>.
        /// </summary>
        public HeapType SortType { get; } = HeapType.Maximum;
        /// <summary>
        /// A custom function used to get a key for an instance of <see cref="T"/> to generate a <see cref="HeapLeaf{T}"/> and add it to the heap.
        /// If this function is provided, when the <see cref="ComputeKey(T)"/> is called, always will return the value provided for this function.
        /// </summary>
        public Func<T, long> CustomComputeKeyFn { get; set; } = (t) => t?.GetHashCode() ?? throw new ArgumentNullException(nameof(t));
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="Heap{T}"/> specifying the size and using <see cref="HeapType.Maximum"/> as the sort type.
        /// </summary>
        /// <param name="size">The size of the heap, see <see cref="Size"/>.</param>
        /// <exception cref="ArgumentException">When the size provided in <paramref name="size"/> is less than 2.</exception>
        public Heap(int size) : this(size, HeapType.Maximum)
        {
        }
        /// <summary>
        /// Creates a new instance of <see cref="Heap{T}"/> specifying the size and the type of sort.
        /// </summary>
        /// <param name="size">The size of the heap, see <see cref="Size"/>.</param>
        /// <param name="sortType">The type of sort, see <see cref="SortType"/>.</param>
        /// <exception cref="ArgumentException">When the size provided in <paramref name="size"/> is less than 2.</exception>
        public Heap(int size, HeapType sortType)
        {
            this.SortType = sortType;

            this.Size = (size >= 2)
                ? size
                : throw new ArgumentException("The size provided for the heap is not valid the size should be greater or equal to 2.");
        }
        #endregion

        #region Methods
        /// <summary>
        /// Determines the key value for the <paramref name="instance"/>, to generate a <see cref="HeapLeaf{T}"/> and add it to the heap.
        /// <para>
        /// The order to obitain the key value is:
        /// <list type="bullet">
        /// <item>value of the <see cref="CustomComputeKeyFn"/> when it is provided.</item>
        /// <item>value of <see cref="IHeapLeafKey.GetKey"/> when <typeparamref name="T"/> implements the <see cref="IHeapLeafKey"/>.</item>
        /// <item>otherwise returns the <see cref="Object.GetHashCode"/> value.</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="instance">The instance of <typeparamref name="T"/> type that will be inserted into the heap.</param>
        protected virtual long ComputeKey(T instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            if (this.CustomComputeKeyFn != null) return this.CustomComputeKeyFn(instance);
            else if (instance is IHeapLeafKey iInstance) return iInstance.GetKey();
            else return instance.GetHashCode();
        }

        /// <summary>
        /// Adds a new leaf to the heap tree and returns a instance of the <see cref="HeapLeaf{T}"/> for the <paramref name="instance"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Throws when the <paramref name="instance"/> is not provided.</exception>
        public virtual HeapLeaf<T> Add(T instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            var leaf = new HeapLeaf<T>(this.ComputeKey(instance), instance);
            leaf.Index = this.lastLeafIndex;

            this.Leafs.Add(leaf);
            this.lastLeafIndex++;

            bool wentUp;
            do
            {
                wentUp = this.Up(ref leaf);

            } while (wentUp);

            return leaf;
        }

        /// <summary>
        /// Checks if the <paramref name="instance"/> contain value inside the heap tree.
        /// </summary>
        public bool Contains(T instance) => this.Leafs.Any(leaf => leaf.Data?.Equals(instance) ?? false);

        /// <summary>
        /// Gets the parent node index for the <paramref name="leaf"/>.
        /// </summary>
        public int GetParentIndex(HeapLeaf<T> leaf)
        {
            if (leaf == HeapLeaf<T>.Empty) throw new ArgumentNullException(nameof(leaf));
            // if the leaf index is 0 indicates that is the root element and returns -1 as the parent index.
            else if (leaf.Index == 0) return -1;
            // if the leaf index is greater 0 and less or equal to heap size, returns the root node as the parent.
            else if (leaf.Index <= this.Size) return 0;
            // else compute the parent index using expression (x - 1) / s, where 'x' represents the leaf index and 's' the heap size.
            return (leaf.Index - 1) / this.Size;
        }

        /// <summary>
        /// Determines if the <paramref name="leaf"/> can be went up in the heap tree. Checking your key value is less or greater (depending <see cref="SortType"/> defined) than <paramref name="parentLeaf"/> key.
        /// </summary>
        public virtual bool CanUp(HeapLeaf<T> parentLeaf, HeapLeaf<T> leaf)
        {
            if (parentLeaf == HeapLeaf<T>.Empty) throw new ArgumentNullException(nameof(parentLeaf));
            else if (leaf == HeapLeaf<T>.Empty) throw new ArgumentNullException(nameof(leaf));
            else if (leaf == parentLeaf) throw new ArgumentException("The leaf and parent leaf provided to check that it can be went up in the heap, can't has the same value for the index and key.");

            switch (this.SortType)
            {
                case HeapType.Minimum: return leaf.Key < parentLeaf.Key;
                case HeapType.Maximum: return leaf.Key > parentLeaf.Key;
                default:
                    throw new Exception("The sort type of heap don't contain implementation to determine that a leaf can go up on the heap.");
            }
        }

        /// <summary>
        /// Ups the <paramref name="leaf"/> in heap if it's possible.
        /// </summary>
        /// <returns>Returns true if the <paramref name="leaf"/> went up.</returns>
        public virtual bool Up(ref HeapLeaf<T> leaf)
        {
            var parentIndex = this.GetParentIndex(leaf);

            // If the parent index is greater than -1, this indicates that leaf is the root, checks if the leaf can be went up in the heap tree and, if possible, do that.
            if (parentIndex > -1 && this.CanUp(this.Leafs[parentIndex], leaf))
            {
                var parentLeaf = this.Leafs[parentIndex];
                this.Swap(ref parentLeaf, ref leaf);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Compute the index in <see cref="Leafs"/> for specific leaf using this exp: S * i + 1.
        /// </summary>
        internal int ComputeLeafIndex(int leafIndex) => this.Size * leafIndex + 1;

        /// <summary>
        /// Gets the children of <paramref name="leaf"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="leaf"/> is <see cref="HeapLeaf{T}.Empty"/>.</exception>
        public virtual IEnumerable<HeapLeaf<T>>? GetChildren(HeapLeaf<T> leaf)
        {
            if (leaf == HeapLeaf<T>.Empty) throw new ArgumentNullException(nameof(leaf));

            // Determines a initial index to obtain child leafs of the current leaf,
            // using this exp. sx+1 where 's' represents the size of the heap and 'x' is index of the current leaf.
            var startChildIndex = this.ComputeLeafIndex(leaf.Index);

            // if the start child index is greater or equal than list count returns null.
            if (startChildIndex >= this.Count) return null;

            var count = this.Size;
            if (startChildIndex + count > this.Count)
            {
                count = this.Count - startChildIndex;
            }

            return this.Leafs.GetRange(startChildIndex, count);
        }

        /// <summary>
        /// Determines if the <paramref name="leaf"/> can get down in the heap tree. Checking your key value is less or greater (depending <see cref="SortType"/> defined) than any key in the <paramref name="children"/>.
        /// </summary>
        public virtual bool CanDown(HeapLeaf<T> leaf, IEnumerable<HeapLeaf<T>>? children, out HeapLeaf<T> selectedChild)
        {
            if (leaf == HeapLeaf<T>.Empty) throw new ArgumentNullException(nameof(leaf));

            selectedChild = HeapLeaf<T>.Empty;

            // If this leaf doesn't contain children, returns false
            if (children == null || children.Count() == 0) return false;

            switch (this.SortType)
            {
                case HeapType.Minimum:
                    // If the heap is minimum sort children asceding and obtain the first item (that represents the minimum value of the children leafs).
                    var min = children.OrderBy(i => i.Key).ThenBy(i => i.Index).First();
                    if (leaf.Key > min.Key)
                    {
                        selectedChild = min;
                        return true;
                    }
                    break;
                case HeapType.Maximum:
                    // If the heap is minimum sort children desceding and obtain the first item (that represents the maximum value of the children leafs).
                    var max = children.OrderByDescending(i => i.Key).ThenBy(i => i.Index).First();
                    if (leaf.Key < max.Key)
                    {
                        selectedChild = max;
                        return true;
                    }
                    break;
                default:
                    throw new Exception("The sort type of heap don't contain implementation to determine that a leaf can go up on the heap.");
            }

            return false;
        }

        /// <summary>
        /// Downs the <paramref name="leaf"/> in heap if it's possible.
        /// </summary>
        /// <returns>Returns <see cref="true"/> if the <paramref name="leaf"/> get down in the heap.</returns>
        public virtual bool Down(ref HeapLeaf<T> leaf)
        {
            if (leaf.Index < this.Count)
            {
                var children = this.GetChildren(leaf);

                HeapLeaf<T> selectedChild;
                if (this.CanDown(leaf, children, out selectedChild) && selectedChild != HeapLeaf<T>.Empty)
                {
                    this.Swap(ref selectedChild, ref leaf);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Swap two leafs position in th <see cref="Leafs"/>.
        /// </summary>
        protected virtual void Swap(ref HeapLeaf<T> left, ref HeapLeaf<T> right)
        {
            left.SwapIndex(ref right);

            this.Leafs[left.Index] = left;
            this.Leafs[right.Index] = right;
        }

        /// <summary>
        /// Gets the <see cref="HeapLeaf{T}"/> at the top of the heap tree, removes that from the heap, and returns it.
        /// If the heap doesn't contain more leafs returns <see cref="HeapLeaf{T}.Empty"/>.
        /// </summary>
        public virtual HeapLeaf<T> Pop()
        {
            if (this.Count > 0)
            {
                var root = this.Top();

                if (this.Count > 2)
                {
                    // If the heap contains more than 2 leafs (root and two or more items) swap the root with the last leaf in the tree and downs this leaf.
                    var lastLeaf = this.Leafs[this.Count - 1];
                    this.Swap(ref root, ref lastLeaf);

                    this.Leafs.RemoveAt(root.Index);

                    bool getDown;
                    do
                    {
                        getDown = this.Down(ref lastLeaf);
                    } while (getDown);
                }

                return root;
            }
            return HeapLeaf<T>.Empty;
        }

        /// <summary>
        /// Gets the <see cref="HeapLeaf{T}"/> at the top of the heap tree, but can't remove that from the tree.
        /// </summary>
        public virtual HeapLeaf<T> Top() => (this.Count > 0) ? this.Leafs[0] : HeapLeaf<T>.Empty;

        /// <summary>
        /// Gets the <see cref="HeapLeaf{T}.Data"/> array, ordered by the leaf order.
        /// </summary>
        public virtual T[] GetData() => this.Leafs.Select(i => i.Data)?.ToArray() ?? new T[0];

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Leafs.Clear();
            this.Leafs.TrimExcess();
        }

        /// <inheritdoc/>
        public override string ToString() => string.Join(", ", this.Leafs);
        #endregion

        #region Enumerator

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        public IEnumerator<HeapLeaf<T>> GetEnumerator() => new HeapEnumerator(this);

        public struct HeapEnumerator : IEnumerator<HeapLeaf<T>>
        {
            private int currentIndex = -1;
            Heap<T> Heap { get; }

            public HeapEnumerator(Heap<T> heap)
            {
                this.Heap = heap;
            }

            object IEnumerator.Current => this.Current;
            public HeapLeaf<T> Current { get; private set; } = HeapLeaf<T>.Empty;

            public bool MoveNext()
            {
                this.currentIndex++;
                if (this.currentIndex >= this.Heap.Count) return false;

                this.Current = this.Heap.Leafs[this.currentIndex];
                return true;
            }

            public void Reset() => this.currentIndex = -1;

            public void Dispose() { }
        }
        #endregion
    }
}
