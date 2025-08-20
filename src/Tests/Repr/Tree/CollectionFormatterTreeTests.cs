#nullable enable
using DebugUtils.Unity.Repr;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System;

namespace DebugUtils.Unity.Tests
{
    public class CollectionFormatterTreeTests
    {
        [Test]
        public void TestListRepr()
        {
            // Test with an empty list
            var actualJson = JToken.Parse(json: new List<int>().ReprTree())!;
            Assert.AreEqual(expected: "List", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: 0, actual: actualJson[key: "count"]!.Value<int>());
            Assert.IsEmpty(collection: (JArray)actualJson[key: "value"]!);
            // Test with a list of integers
            actualJson = JToken.Parse(json: new List<int> { 1, 2, 3 }.ReprTree())!;
            Assert.AreEqual(expected: "List", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: 3, actual: actualJson[key: "count"]!.Value<int>());
            var valueArray = (JArray)actualJson[key: "value"]!;
            Assert.AreEqual(expected: 3, actual: valueArray.Count);
            Assert.AreEqual(expected: "1_i32", actual: valueArray[index: 0]!.ToString());
            Assert.AreEqual(expected: "2_i32", actual: valueArray[index: 1]!.ToString());
            Assert.AreEqual(expected: "3_i32", actual: valueArray[index: 2]!.ToString());
            // Test with a list of nullable strings
            actualJson = JToken.Parse(json: new List<string?> { "a", null, "c" }.ReprTree())!;
            Assert.AreEqual(expected: "List", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: 3, actual: actualJson[key: "count"]!.Value<int>());
            valueArray = (JArray)actualJson[key: "value"]!;
            Assert.AreEqual(expected: 3, actual: valueArray.Count);
            // Check first element: "a"
            var item1 = (JObject)valueArray[index: 0]!;
            Assert.AreEqual(expected: "string", actual: item1[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: item1[propertyName: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: item1[propertyName: "hashCode"]);
            Assert.AreEqual(expected: 1, actual: item1[propertyName: "length"]!.Value<int>());
            Assert.AreEqual(expected: "a", actual: item1[propertyName: "value"]
              ?.ToString());
            // Check second element: null
            Assert.IsEmpty(valueArray[index: 1]);
            // Check third element: "c"
            var item3 = (JObject)valueArray[index: 2]!;
            Assert.AreEqual(expected: "string", actual: item3[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: item3[propertyName: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: item3[propertyName: "hashCode"]);
            Assert.AreEqual(expected: 1, actual: item3[propertyName: "length"]!.Value<int>());
            Assert.AreEqual(expected: "c", actual: item3[propertyName: "value"]
              ?.ToString());
        }

        [Test]
        public void TestEnumerableRepr()
        {
            var actualJson = JToken.Parse(json: Enumerable.Range(start: 1, count: 3)
                                                          .ReprTree())!;
            Assert.AreEqual(expected: "RangeIterator", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            // Enumerable.Range doesn't have a Count property, so count field may not be present
            if (actualJson[key: "count"] != null)
            {
                Assert.AreEqual(expected: 3, actual: actualJson[key: "count"]!.Value<int>());
            }

            var valueArray = (JArray)actualJson[key: "value"]!;
            Assert.AreEqual(expected: 3, actual: valueArray.Count);
            Assert.AreEqual(expected: "1_i32", actual: valueArray[index: 0]!.ToString());
            Assert.AreEqual(expected: "2_i32", actual: valueArray[index: 1]!.ToString());
            Assert.AreEqual(expected: "3_i32", actual: valueArray[index: 2]!.ToString());
        }

        [Test]
        public void TestNestedListRepr()
        {
            var nestedList = new List<List<int>>
            {
                new()
                {
                    1,
                    2
                },
                new()
                {
                    3,
                    4,
                    5
                },
                new()
            };
            var actualJson = JToken.Parse(json: nestedList.ReprTree())!;
            Assert.AreEqual(expected: "List", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: 3, actual: actualJson[key: "count"]!.Value<int>());
            var outerArray = (JArray)actualJson[key: "value"]!;
            Assert.AreEqual(expected: 3, actual: outerArray.Count);
            // Check first nested list: { 1, 2 }
            var nested1 = (JObject)outerArray[index: 0]!;
            Assert.AreEqual(expected: "List", actual: nested1[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: nested1[propertyName: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: nested1[propertyName: "hashCode"]);
            Assert.AreEqual(expected: 2, actual: nested1[propertyName: "count"]!.Value<int>());
            var nested1Value = (JArray)nested1[propertyName: "value"]!;
            Assert.AreEqual(expected: 2, actual: nested1Value.Count);
            Assert.AreEqual(expected: "1_i32", actual: nested1Value[index: 0]!.ToString());
            Assert.AreEqual(expected: "2_i32", actual: nested1Value[index: 1]!.ToString());
            // Check second nested list: { 3, 4, 5 }
            var nested2 = (JObject)outerArray[index: 1]!;
            Assert.AreEqual(expected: "List", actual: nested2[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: nested2[propertyName: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: nested2[propertyName: "hashCode"]);
            Assert.AreEqual(expected: 3, actual: nested2[propertyName: "count"]!.Value<int>());
            var nested2Value = (JArray)nested2[propertyName: "value"]!;
            Assert.AreEqual(expected: 3, actual: nested2Value.Count);
            Assert.AreEqual(expected: "3_i32", actual: nested2Value[index: 0]!.ToString());
            Assert.AreEqual(expected: "4_i32", actual: nested2Value[index: 1]!.ToString());
            Assert.AreEqual(expected: "5_i32", actual: nested2Value[index: 2]!.ToString());
            // Check third nested list: { }
            var nested3 = (JObject)outerArray[index: 2]!;
            Assert.AreEqual(expected: "List", actual: nested3[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: nested3[propertyName: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: nested3[propertyName: "hashCode"]);
            Assert.AreEqual(expected: 0, actual: nested3[propertyName: "count"]!.Value<int>());
            Assert.IsEmpty(collection: (JArray)nested3[propertyName: "value"]!);
        }

        [Test]
        public void TestArrayRepr()
        {
            var actualJson = JToken.Parse(json: Array.Empty<int>()
                                                     .ReprTree())!;
            Assert.AreEqual(expected: "1DArray", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            Assert.True(condition: JToken.DeepEquals(t1: new JArray(content: 0),
                t2: actualJson[key: "dimensions"]!));
            Assert.IsEmpty(collection: (JArray)actualJson[key: "value"]!);
            actualJson = JToken.Parse(json: new[] { 1, 2, 3 }.ReprTree())!;
            Assert.AreEqual(expected: "1DArray", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            Assert.True(condition: JToken.DeepEquals(t1: new JArray(content: 3),
                t2: actualJson[key: "dimensions"]!));
            var valueArray = (JArray)actualJson[key: "value"]!;
            Assert.AreEqual(expected: 3, actual: valueArray.Count);
            Assert.AreEqual(expected: "1_i32", actual: valueArray[index: 0]!.ToString());
            Assert.AreEqual(expected: "2_i32", actual: valueArray[index: 1]!.ToString());
            Assert.AreEqual(expected: "3_i32", actual: valueArray[index: 2]!.ToString());
        }

        [Test]
        public void TestJaggedArrayRepr()
        {
            var jagged2D = new[]
            {
                new[]
                {
                    1,
                    2
                },
                new[]
                {
                    3
                }
            };
            var actualJson = JToken.Parse(json: jagged2D.ReprTree())!;
            // Check outer jagged array properties
            Assert.AreEqual(expected: "JaggedArray", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: 1, actual: actualJson[key: "rank"]!.Value<int>());
            Assert.True(condition: JToken.DeepEquals(t1: new JArray(content: 2),
                t2: actualJson[key: "dimensions"]!));
            Assert.AreEqual(expected: "1DArray", actual: actualJson[key: "elementType"]
              ?.ToString());
            // Check the nested arrays structure
            var outerArray = (JArray)actualJson[key: "value"]!;
            Assert.AreEqual(expected: 2, actual: outerArray.Count);
            // First inner array: int[] { 1, 2 }
            var innerArray1Json = outerArray[index: 0]!;
            Assert.AreEqual(expected: "1DArray", actual: innerArray1Json[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: innerArray1Json[key: "kind"]
              ?.ToString());
            Assert.AreEqual(expected: 1, actual: innerArray1Json[key: "rank"]!.Value<int>());
            Assert.True(condition: JToken.DeepEquals(t1: new JArray(content: 2),
                t2: innerArray1Json[key: "dimensions"]!));
            Assert.AreEqual(expected: "int", actual: innerArray1Json[key: "elementType"]
              ?.ToString());
            var innerArray1Values = (JArray)innerArray1Json[key: "value"]!;
            Assert.AreEqual(expected: 2, actual: innerArray1Values.Count);
            Assert.True(
                condition: JToken.DeepEquals(t1: "1_i32", t2: innerArray1Values[index: 0]));
            Assert.True(
                condition: JToken.DeepEquals(t1: "2_i32", t2: innerArray1Values[index: 1]));
            // Second inner array: int[] { 3 }
            var innerArray2Json = outerArray[index: 1]!;
            Assert.AreEqual(expected: "1DArray", actual: innerArray2Json[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: innerArray2Json[key: "kind"]
              ?.ToString());
            Assert.AreEqual(expected: 1, actual: innerArray2Json[key: "rank"]!.Value<int>());
            Assert.True(condition: JToken.DeepEquals(t1: new JArray(content: 1),
                t2: innerArray2Json[key: "dimensions"]!));
            Assert.AreEqual(expected: "int", actual: innerArray2Json[key: "elementType"]
              ?.ToString());
            var innerArray2Values = (JArray)innerArray2Json[key: "value"]!;
            Assert.AreEqual(expected: Enumerable.Count(source: innerArray2Values), actual: 1);
            Assert.True(
                condition: JToken.DeepEquals(t1: "3_i32", t2: innerArray2Values[index: 0]));
        }

        [Test]
        public void TestMultidimensionalArrayRepr()
        {
            var array2D = new[,]
            {
                {
                    1,
                    2
                },
                {
                    3,
                    4
                }
            };
            var actualJson = JToken.Parse(json: array2D.ReprTree())!;
            Assert.AreEqual(expected: "2DArray", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: 2, actual: actualJson[key: "rank"]!.Value<int>());
            Assert.True(condition: JToken.DeepEquals(t1: new JArray(2, 2),
                t2: actualJson[key: "dimensions"]!));
            var outerArray = (JArray)actualJson[key: "value"]!;
            Assert.AreEqual(expected: 2, actual: outerArray.Count);
            var innerArray1 = (JArray)outerArray[index: 0]!;
            Assert.AreEqual(expected: 2, actual: innerArray1.Count);
            Assert.AreEqual(expected: "1_i32", actual: innerArray1[index: 0]!.ToString());
            Assert.AreEqual(expected: "2_i32", actual: innerArray1[index: 1]!.ToString());
            var innerArray2 = (JArray)outerArray[index: 1]!;
            Assert.AreEqual(expected: 2, actual: innerArray2.Count);
            Assert.AreEqual(expected: "3_i32", actual: innerArray2[index: 0]!.ToString());
            Assert.AreEqual(expected: "4_i32", actual: innerArray2[index: 1]!.ToString());
        }

        [Test]
        public void TestHashSetRepr()
        {
            var set = new HashSet<int>
            {
                1,
                2
            };
            var actualJson = JToken.Parse(json: set.ReprTree())!;
            Assert.AreEqual(expected: "HashSet", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: 2, actual: actualJson[key: "count"]!.Value<int>());
            var possibleOutputs = new[]
            {
                new JArray
                {
                    "1_i32",
                    "2_i32"
                },
                new JArray
                {
                    "2_i32",
                    "1_i32"
                }
            };
            var valueArray = (JArray)actualJson[key: "value"]!;
            Assert.Contains(expected: valueArray, actual: possibleOutputs);
        }

        [Test]
        public void TestSortedSetRepr()
        {
            var set = new SortedSet<int>
            {
                3,
                1,
                2
            };
            var actualJson = JToken.Parse(json: set.ReprTree())!;
            Assert.AreEqual(expected: "SortedSet", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: 3, actual: actualJson[key: "count"]!.Value<int>());
            var valueArray = (JArray)actualJson[key: "value"]!;
            Assert.AreEqual(expected: 3, actual: valueArray.Count);
            Assert.True(condition: JToken.DeepEquals(t1: "1_i32", t2: valueArray[index: 0]));
            Assert.True(condition: JToken.DeepEquals(t1: "2_i32", t2: valueArray[index: 1]));
            Assert.True(condition: JToken.DeepEquals(t1: "3_i32", t2: valueArray[index: 2]));
        }

        [Test]
        public void TestQueueRepr()
        {
            var queue = new Queue<string>();
            queue.Enqueue(item: "first");
            queue.Enqueue(item: "second");
            var actualJson = JToken.Parse(json: queue.ReprTree())!;
            Assert.AreEqual(expected: "Queue", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: 2, actual: actualJson[key: "count"]!.Value<int>());
            var valueArray = (JArray)actualJson[key: "value"]!;
            Assert.AreEqual(expected: 2, actual: valueArray.Count);
            var item1 = (JObject)valueArray[index: 0]!;
            Assert.AreEqual(expected: "string", actual: item1[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: item1[propertyName: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: item1[propertyName: "hashCode"]);
            Assert.AreEqual(expected: 5, actual: item1[propertyName: "length"]!.Value<int>());
            Assert.AreEqual(expected: "first", actual: item1[propertyName: "value"]
              ?.ToString());
            var item2 = (JObject)valueArray[index: 1]!;
            Assert.AreEqual(expected: "string", actual: item2[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: item2[propertyName: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: item2[propertyName: "hashCode"]);
            Assert.AreEqual(expected: 6, actual: item2[propertyName: "length"]!.Value<int>());
            Assert.AreEqual(expected: "second", actual: item2[propertyName: "value"]
              ?.ToString());
        }

        [Test]
        public void TestStackRepr()
        {
            var stack = new Stack<int>();
            stack.Push(item: 1);
            stack.Push(item: 2);
            var actualJson = JToken.Parse(json: stack.ReprTree())!;
            Assert.AreEqual(expected: "Stack", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: 2, actual: actualJson[key: "count"]!.Value<int>());
            var valueArray = (JArray)actualJson[key: "value"]!;
            Assert.AreEqual(expected: 2, actual: valueArray.Count);
            Assert.True(condition: JToken.DeepEquals(t1: "2_i32", t2: valueArray[index: 0]));
            Assert.True(condition: JToken.DeepEquals(t1: "1_i32", t2: valueArray[index: 1]));
        }
        #if NET6_0_OR_GREATER
    [Fact]
    public void TestPriorityQueueRepr()
    {
        var pq = new PriorityQueue<string, int>();
        pq.Enqueue(element: "second", priority: 2);
        pq.Enqueue(element: "first", priority: 1);
        pq.Enqueue(element: "third", priority: 3);

        var actualJson = JsonNode.Parse(json: pq.ReprTree())!;

        Assert.Equal(expected: "PriorityQueue", actual: actualJson[propertyName: "type"]
          ?.ToString());
        Assert.Equal(expected: "class", actual: actualJson[propertyName: "kind"]
          ?.ToString());
        Assert.NotNull(@object: actualJson[propertyName: "hashCode"]);
        Assert.Equal(expected: 3, actual: actualJson[propertyName: "count"]!.GetValue<int>());
        Assert.Equal(expected: "string", actual: actualJson[propertyName: "elementType"]
          ?.ToString());
        Assert.Equal(expected: "int", actual: actualJson[propertyName: "priorityType"]
          ?.ToString());

        var valueArray = actualJson[propertyName: "value"]!.AsArray();
        Assert.Equal(expected: 3, actual: valueArray.Count);

        Assert.Contains(collection: valueArray, filter: item => item![propertyName: "element"]![propertyName: "value"]!.GetValue<string>() == "first" && item[propertyName: "priority"]!.GetValue<string>() == "1_i32");

        Assert.Contains(collection: valueArray, filter: item => item![propertyName: "element"]![propertyName: "value"]!.GetValue<string>() == "second" && item[propertyName: "priority"]!.GetValue<string>() == "2_i32");

        Assert.Contains(collection: valueArray, filter: item => item![propertyName: "element"]![propertyName: "value"]!.GetValue<string>() == "third" && item[propertyName: "priority"]!.GetValue<string>() == "3_i32");
    }
        #endif
    }
}