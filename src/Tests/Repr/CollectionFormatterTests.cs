#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using DebugUtils.Unity.Repr;
using NUnit.Framework;

namespace DebugUtils.Unity.Tests
{

    public class CollectionFormatterTests
    {
        // Collections
        [Test]
        public void TestListRepr()
        {
            Assert.AreEqual(expected: "[]", actual: new List<int>().Repr());
            Assert.AreEqual(expected: "[int(1), int(2), int(3)]",
                actual: new List<int> { 1, 2, 3 }.Repr());
            Assert.AreEqual(expected: "[\"a\", null, \"c\"]",
                actual: new List<string?> { "a", null, "c" }.Repr());
        }

        [Test]
        public void TestEnumerableRepr()
        {
            Assert.AreEqual(expected: "RangeIterator([int(1), int(2), int(3)])", actual: Enumerable
               .Range(start: 1, count: 3)
               .Repr());
        }

        [Test]
        public void TestNestedListRepr()
        {
            var nestedList = new List<List<int>> { new() { 1, 2 }, new() { 3, 4, 5 }, new() };
            Assert.AreEqual(expected: "[[int(1), int(2)], [int(3), int(4), int(5)], []]",
                actual: nestedList.Repr());
        }

        // Arrays
        [Test]
        public void TestArrayRepr()
        {
            Assert.AreEqual(expected: "1DArray([])", actual: Array.Empty<int>()
               .Repr());
            Assert.AreEqual(expected: "1DArray([int(1), int(2), int(3)])",
                actual: new[] { 1, 2, 3 }.Repr());
        }

        [Test]
        public void TestJaggedArrayRepr()
        {
            var jagged2D = new[]
                { new[] { 1, 2 }, new[] { 3 } };
            Assert.AreEqual(expected: "JaggedArray([[int(1), int(2)], [int(3)]])",
                actual: jagged2D.Repr());
        }

        [Test]
        public void TestMultidimensionalArrayRepr()
        {
            var array2D = new[,] { { 1, 2 }, { 3, 4 } };
            Assert.AreEqual(expected: "2DArray([[int(1), int(2)], [int(3), int(4)]])",
                actual: array2D.Repr());
        }

        // Dictionaries, Sets, Queues
        [Test]
        public void TestDictionaryRepr()
        {
            var dict = new Dictionary<string, int> { [key: "a"] = 1, [key: "b"] = 2 };
            // Note: Dictionary order is not guaranteed, so we check for both possibilities
            var possibleOutputs = new[]
            {
                "{\"a\": int(1), \"b\": int(2)}",
                "{\"b\": int(2), \"a\": int(1)}"
            };
            Assert.Contains(expected: dict.Repr(), actual: possibleOutputs);
        }

        [Test]
        public void TestHashSetRepr()
        {
            var set = new HashSet<int> { 1, 2 };
            // Note: HashSet order is not guaranteed, so we sort the string representation for a stable test
            var repr = set.Repr(); // e.g., "{int(1), int(2), int(3)}"
            var possibleOutputs = new[]
            {
                "{int(1), int(2)}",
                "{int(2), int(1)}"
            };
            Assert.Contains(expected: repr, actual: possibleOutputs);
        }

        [Test]
        public void TestSortedSetRepr()
        {
            var set = new SortedSet<int> { 3, 1, 2 };
            var repr = set.Repr();
            Assert.AreEqual(expected: "SortedSet({int(1), int(2), int(3)})", actual: repr);
        }

        [Test]
        public void TestQueueRepr()
        {
            var queue = new Queue<string>();
            queue.Enqueue(item: "first");
            queue.Enqueue(item: "second");
            Assert.AreEqual(expected: "Queue([\"first\", \"second\"])", actual: queue.Repr());
        }

        [Test]
        public void TestStackRepr()
        {
            var stack = new Stack<int>();
            stack.Push(item: 1);
            stack.Push(item: 2);
            Assert.AreEqual(expected: "Stack([int(2), int(1)])", actual: stack.Repr());
        }
        
        
        [Test]
        public void TestTupleRepr()
        {
            Assert.AreEqual(expected: "(int(1), \"hello\")", actual: (1, "hello").Repr());
        }
    }
}