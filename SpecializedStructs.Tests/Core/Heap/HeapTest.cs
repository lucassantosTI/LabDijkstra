using Microsoft.VisualStudio.TestPlatform.CrossPlatEngine.Client;

namespace SpecializedStructs.Tests.Core.Heap
{
    public class HeapTest
    {
        // Commons
        private int[] _inputData;

        // binary
        private Heap<int> _binaryHeap;
        private string _binaryExpected;
        private int _firstItemPopedExpected;
        private int _secondItemPopedExpected;
        private string _binaryExpectedPopTwoItems;

        // ternary
        private Heap<int> _ternaryHeap;
        private string _ternaryExpected;
        private string _ternaryExpectedPopTwoItems;

        [SetUp]
        public void Setup()
        {
            // initializes commons values for the heap
            this._inputData = new int[] { 5, 10, 8, 9, 5, 6, 7, 4, 3, 2, 1, 15 };
            this._firstItemPopedExpected = 15;
            this._secondItemPopedExpected = 10;

            // initializes the binary heap and your expected result, when apply the _inputData to it.
            this._binaryHeap = new Heap<int>(2);
            this._binaryExpected = "15, 9, 10, 5, 5, 8, 7, 4, 3, 2, 1, 6";
            this._binaryExpectedPopTwoItems = "9, 5, 8, 4, 5, 6, 7, 1, 3, 2";

            // initializes the ternary heap and your expected result, when apply the _inputData to it.
            this._ternaryHeap = new Heap<int>(3);
            this._ternaryExpected = "15, 7, 8, 10, 5, 5, 6, 4, 3, 2, 1, 9";
            this._ternaryExpectedPopTwoItems = "9, 7, 8, 1, 5, 5, 6, 4, 3, 2";

            // add items to heaps
            this.AddInputData(this._binaryHeap);
            this.AddInputData(this._ternaryHeap);
        }

        private void AddInputData(Heap<int> heap)
        {
            for (int i = 0; i < this._inputData.Length; i++)
            {
                heap.Add(this._inputData[i]);
            }
        }

        /// <summary>
        /// Tests the array sorting using a binary max-heap.
        /// </summary>
        [Test(Description = "Test the array sorting using a binary max-heap."), Order(1)]
        public void TestSortingBinaryHeap() =>
            Assert.That(
                this._binaryHeap.ToString(), Is.EqualTo(_binaryExpected),
                "The sorting using a binary heap was fail.\r\nInput: [{0}]", string.Join(", ", this._inputData));

        /// <summary>
        /// Tests pop two items from the binary max-heap.
        /// </summary>
        [Test(Description = "Test pop two items from the binary max-heap"), Order(2)]
        public void TestBinaryMaxHeapPopTwoItems()
        {
            var pop1 = this._binaryHeap.Pop();
            var pop2 = this._binaryHeap.Pop();
            Assert.IsTrue(
                pop1.Data == this._firstItemPopedExpected &&
                pop2.Data == this._secondItemPopedExpected &&
                this._binaryHeap.ToString() == this._binaryExpectedPopTwoItems,
                @"After the pop two items from the heap the values returned for one or all poped items is not valid or the heap resulted is not valid.
Poped items [Actual x Esxected]: 
First   {0} x {1}
Second  {2} x {3}
Heap data:
Actual      [{4}]
Execpted    [{5}]",
                pop1.Data, this._firstItemPopedExpected,
                pop2.Data, this._secondItemPopedExpected,
                this._binaryHeap, this._binaryExpectedPopTwoItems);
        }

        /// <summary>
        /// Tests the array sorting using a ternary max-heap.
        /// </summary>
        [Test(Description = "Test the array sorting using a ternary max-heap."), Order(3)]
        public void TestTernaryMaxHeapSorting() =>
            Assert.That(
                this._ternaryHeap.ToString(), Is.EqualTo(_ternaryExpected),
                "The sorting using a ternary heap was fail.\r\nInput: [{0}]", string.Join(", ", this._inputData));

        /// <summary>
        /// Tests pop two items from the ternary max-heap.
        /// </summary>
        [Test(Description = "Test pop two items from the ternary max-heap"), Order(4)]
        public void TestTernaryMaxHeapPopTwoItems()
        {
            var pop1 = this._ternaryHeap.Pop();
            var pop2 = this._ternaryHeap.Pop();
            Assert.IsTrue(
                pop1.Data == this._firstItemPopedExpected &&
                pop2.Data == this._secondItemPopedExpected &&
                this._ternaryHeap.ToString() == this._ternaryExpectedPopTwoItems,
                @"After the pop two items from the heap the values returned for one or all poped items is not valid or the heap resulted is not valid.
Poped items [Actual x Esxected]: 
First   {0} x {1}
Second  {2} x {3}
Heap data:
Actual      [{4}]
Execpted    [{5}]",
                pop1.Data, this._firstItemPopedExpected,
                pop2.Data, this._secondItemPopedExpected,
                this._ternaryHeap, this._ternaryExpectedPopTwoItems);
        }
    }
}
