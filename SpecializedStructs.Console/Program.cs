// See https://aka.ms/new-console-template for more information
using SpecializedStructs;
using SpecializedStructs.Core.Heap;

Console.WriteLine("Hello, World!");

var heap = new Heap<int>(2);

var random = new Random();

for (int i = 0; i < 6; i++)
{
    var value = random.Next(1, 100);
    heap.Add(value);
    Console.WriteLine("Adiciona o {0} ao heap", value);
    Console.WriteLine("Heap {0}", heap);
}

HeapLeaf<int> leaf;
do
{
    leaf = heap.Pop();
    Console.WriteLine("Pop the heap: {0}", leaf.Data);
    Console.WriteLine("Heap {0}", heap);
} while (leaf != HeapLeaf<int>.Empty);