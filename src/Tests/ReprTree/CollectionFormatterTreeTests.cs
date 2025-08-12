#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using DebugUtils.Unity.Repr;
using NUnit.Framework;
using Newtonsoft.Json.Linq;

namespace DebugUtils.Unity.Tests
{
    public class CollectionFormatterTreeTests
    {
        // Collections
        [Test]
        public void TestListRepr()
        {
            // Test with an empty list
            var actualJson = JToken.Parse(json: new List<int>().ReprTree());
            Assert.AreEqual(expected: "List", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: 0,
                actual: actualJson[key: "count"]!.Value<int>());
            Assert.AreEqual(expected: actualJson[key: "value"]!.Value<JArray>()!
                                                               .Count, actual: 0);

            // Test with a list of integers
            actualJson = JToken.Parse(json: new List<int> { 1, 2, 3 }.ReprTree());
            Assert.AreEqual(expected: "List", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: 3,
                actual: actualJson[key: "count"]!.Value<int>());
            var valueArray = (JArray)actualJson[key: "value"]!;
            Assert.AreEqual(expected: 3, actual: valueArray.Count);
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "int", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "1"
                },
                t2: valueArray[index: 0]));
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "int", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "2"
                },
                t2: valueArray[index: 1]));
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "int", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "3"
                },
                t2: valueArray[index: 2]));

            // Test with a list of nullable strings
            actualJson = JToken.Parse(json: new List<string?> { "a", null, "c" }.ReprTree());
            Assert.AreEqual(expected: "List", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: 3,
                actual: actualJson[key: "count"]!.Value<int>());
            valueArray = (JArray)actualJson[key: "value"]!;
            Assert.AreEqual(expected: 3, actual: valueArray.Count);

            // Check the first element: "a"
            var item1 = valueArray[index: 0];
            Assert.AreEqual(expected: "string", actual: item1[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: item1[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: item1[key: "hashCode"]);
            Assert.AreEqual(expected: 1, actual: item1[key: "length"]!.Value<int>());
            Assert.AreEqual(expected: "a", actual: item1[key: "value"]
              ?.ToString());

            // Check the second element: null
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "object", [propertyName: "kind"] = "class",
                    [propertyName: "value"] = null
                },
                t2: valueArray[index: 1]));

            // Check the third element: "c"
            var item3 = valueArray[index: 2];
            Assert.AreEqual(expected: "string", actual: item3[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: item3[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: item3[key: "hashCode"]);
            Assert.AreEqual(expected: 1, actual: item3[key: "length"]!.Value<int>());
            Assert.AreEqual(expected: "c", actual: item3[key: "value"]
              ?.ToString());
        }

        [Test]
        public void TestEnumerableRepr()
        {
            var actualJson = JToken.Parse(json: Enumerable.Range(start: 1, count: 3)
                                                          .ReprTree());
            Assert.AreEqual(expected: "RangeIterator", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            var valueArray = (JArray)actualJson[key: "value"]!;
            Assert.AreEqual(expected: 3, actual: valueArray.Count);
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "int", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "1"
                },
                t2: valueArray[index: 0]));
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "int", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "2"
                },
                t2: valueArray[index: 1]));
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "int", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "3"
                },
                t2: valueArray[index: 2]));
        }

        [Test]
        public void TestNestedListRepr()
        {
            var nestedList = new List<List<int>> { new() { 1, 2 }, new() { 3, 4, 5 }, new() };
            var actualJson = JToken.Parse(json: nestedList.ReprTree());

            Assert.AreEqual(expected: "List", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: 3,
                actual: actualJson[key: "count"]!.Value<int>());

            var outerArray = (JArray)actualJson[key: "value"]!;
            Assert.AreEqual(expected: 3, actual: outerArray.Count);

            // Check the first nested list: { 1, 2 }
            var nested1 = outerArray[index: 0];
            Assert.AreEqual(expected: "List", actual: nested1[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: nested1[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: nested1[key: "hashCode"]);
            Assert.AreEqual(expected: 2, actual: nested1[key: "count"]!.Value<int>());
            var nested1Value = (JArray)nested1[key: "value"]!;
            Assert.AreEqual(expected: 2, actual: nested1Value.Count);
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "int", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "1"
                },
                t2: nested1Value[index: 0]));
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "int", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "2"
                },
                t2: nested1Value[index: 1]));

            // Check the second nested list: { 3, 4, 5 }
            var nested2 = outerArray[index: 1];
            Assert.AreEqual(expected: "List", actual: nested2[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: nested2[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: nested2[key: "hashCode"]);
            Assert.AreEqual(expected: 3, actual: nested2[key: "count"]!.Value<int>());
            var nested2Value = (JArray)nested2[key: "value"]!;
            Assert.AreEqual(expected: 3, actual: nested2Value.Count);
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "int", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "3"
                },
                t2: nested2Value[index: 0]));
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "int", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "4"
                },
                t2: nested2Value[index: 1]));
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "int", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "5"
                },
                t2: nested2Value[index: 2]));

            // Check the third nested list: { }
            var nested3 = outerArray[index: 2];
            Assert.AreEqual(expected: "List", actual: nested3[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: nested3[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: nested3[key: "hashCode"]);
            Assert.AreEqual(expected: 0, actual: nested3[key: "count"]!.Value<int>());
        }

        [Test]
        public void TestArrayRepr()
        {
            var actualJson = JToken.Parse(json: Array.Empty<int>()
                                                     .ReprTree());
            Assert.AreEqual(expected: "1DArray", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            Assert.True(condition: JToken.DeepEquals(t1: new JArray(content: 0),
                t2: actualJson[key: "dimensions"]!));

            actualJson = JToken.Parse(json: new[] { 1, 2, 3 }.ReprTree());
            Assert.AreEqual(expected: "1DArray", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            Assert.True(condition: JToken.DeepEquals(t1: new JArray(content: 3),
                t2: actualJson[key: "dimensions"]!));
            var valueArray = (JArray)actualJson[key: "value"]!;
            Assert.AreEqual(expected: 3, actual: valueArray.Count);
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "int", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "1"
                },
                t2: valueArray[index: 0]));
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "int", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "2"
                },
                t2: valueArray[index: 1]));
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "int", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "3"
                },
                t2: valueArray[index: 2]));
        }

        [Test]
        public void TestJaggedArrayRepr()
        {
            var jagged2D = new[] { new[] { 1, 2 }, new[] { 3 } };
            var actualJson = JToken.Parse(json: jagged2D.ReprTree());

            // Check outer jagged array properties
            Assert.AreEqual(expected: "JaggedArray", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: 1,
                actual: actualJson[key: "rank"]!.Value<int>());
            Assert.True(condition: JToken.DeepEquals(t1: new JArray(content: 2),
                t2: actualJson[key: "dimensions"]!));
            Assert.AreEqual(expected: "1DArray", actual: actualJson[key: "elementType"]
              ?.ToString());

            // Check the nested arrays structure
            var outerArray = (JArray)actualJson[key: "value"]!;
            Assert.AreEqual(expected: 2, actual: outerArray.Count);

            // First inner array: int[] { 1, 2 }
            var innerArray1Json = outerArray[index: 0];
            Assert.AreEqual(expected: "1DArray", actual: innerArray1Json[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: innerArray1Json[key: "kind"]
              ?.ToString());
            Assert.AreEqual(expected: 1,
                actual: innerArray1Json[key: "rank"]!.Value<int>());
            Assert.True(condition: JToken.DeepEquals(t1: new JArray(content: 2),
                t2: innerArray1Json[key: "dimensions"]!));
            Assert.AreEqual(expected: "int", actual: innerArray1Json[key: "elementType"]
              ?.ToString());

            var innerArray1Values = (JArray)innerArray1Json[key: "value"]!;
            Assert.AreEqual(expected: 2, actual: innerArray1Values.Count);
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "int",
                    [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "1" // String value due to repr config
                },
                t2: innerArray1Values[index: 0]));
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "int",
                    [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "2" // String value due to repr config
                },
                t2: innerArray1Values[index: 1]));

            // Second inner array: int[] { 3 }
            var innerArray2Json = outerArray[index: 1];
            Assert.AreEqual(expected: "1DArray", actual: innerArray2Json[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: innerArray2Json[key: "kind"]
              ?.ToString());
            Assert.AreEqual(expected: 1,
                actual: innerArray2Json[key: "rank"]!.Value<int>());
            Assert.True(condition: JToken.DeepEquals(t1: new JArray(content: 1),
                t2: innerArray2Json[key: "dimensions"]!));
            Assert.AreEqual(expected: "int", actual: innerArray2Json[key: "elementType"]
              ?.ToString());

            var innerArray2Values = (JArray)innerArray2Json[key: "value"]!;
            Assert.AreEqual(expected: 1, actual: innerArray2Values.Count);
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "int",
                    [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "3" // String value due to repr config
                },
                t2: innerArray2Values[index: 0]));
        }

        [Test]
        public void TestMultidimensionalArrayRepr()
        {
            var array2D = new[,] { { 1, 2 }, { 3, 4 } };
            var actualJson = JToken.Parse(json: array2D.ReprTree());

            Assert.AreEqual(expected: "2DArray", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: 2,
                actual: actualJson[key: "rank"]!.Value<int>());
            Assert.True(condition: JToken.DeepEquals(t1: new JArray(2, 2),
                t2: actualJson[key: "dimensions"]!));

            var outerArray = (JArray)actualJson[key: "value"]!;
            Assert.AreEqual(expected: 2, actual: outerArray.Count);

            var innerArray1 = (JArray)outerArray[index: 0];
            Assert.AreEqual(expected: 2, actual: innerArray1.Count);
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "int", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "1"
                },
                t2: innerArray1[index: 0]));
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "int", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "2"
                },
                t2: innerArray1[index: 1]));

            var innerArray2 = (JArray)outerArray[index: 1];
            Assert.AreEqual(expected: 2, actual: innerArray2.Count);
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "int", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "3"
                },
                t2: innerArray2[index: 0]));
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "int", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "4"
                },
                t2: innerArray2[index: 1]));
        }

        [Test]
        public void TestDictionaryRepr()
        {
            var dict = new Dictionary<string, int> { [key: "a"] = 1, [key: "b"] = 2 };
            var actualJson = JToken.Parse(json: dict.ReprTree());

            Assert.AreEqual(expected: "Dictionary", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: 2, actual: actualJson[key: "count"]!.Value<int>());

            var valueArray = (JArray)actualJson[key: "value"]!;
            Assert.AreEqual(expected: 2, actual: valueArray.Count);

            // Since dictionary order is not guaranteed, we check for the presence of keys
            var aItem = valueArray.FirstOrDefault(predicate: item =>
                item![key: "key"]![key: "value"]!.ToString() == "a");
            Assert.NotNull(anObject: aItem);
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "int", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "1"
                },
                t2: aItem[key: "value"]));

            var bItem = valueArray.FirstOrDefault(predicate: item =>
                item![key: "key"]![key: "value"]!.ToString() == "b");
            Assert.NotNull(anObject: bItem);
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "int", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "2"
                },
                t2: bItem[key: "value"]));
        }

        [Test]
        public void TestHashSetRepr()
        {
            var set = new HashSet<int> { 1, 2 };
            var actualJson = JToken.Parse(json: set.ReprTree());

            Assert.AreEqual(expected: "HashSet", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: 2, actual: actualJson[key: "count"]!.Value<int>());

            var valueArray = (JArray)actualJson[key: "value"]!;
            Assert.AreEqual(expected: 2, actual: valueArray.Count);

            // HashSet order is not guaranteed, so check for the presence of both values
            var values = valueArray.Select(selector: item => item![key: "value"]!.ToString())
                                   .ToList();
            Assert.Contains(expected: "1", actual: values);
            Assert.Contains(expected: "2", actual: values);
        }

        [Test]
        public void TestSortedSetRepr()
        {
            var set = new SortedSet<int> { 3, 1, 2 };
            var actualJson = JToken.Parse(json: set.ReprTree());

            Assert.AreEqual(expected: "SortedSet", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: 3,
                actual: actualJson[key: "count"]!.Value<int>());

            var valueArray = (JArray)actualJson[key: "value"]!;
            Assert.AreEqual(expected: 3, actual: valueArray.Count);
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "int", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "1"
                },
                t2: valueArray[index: 0]));
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "int", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "2"
                },
                t2: valueArray[index: 1]));
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "int", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "3"
                },
                t2: valueArray[index: 2]));
        }

        [Test]
        public void TestQueueRepr()
        {
            var queue = new Queue<string>();
            queue.Enqueue(item: "first");
            queue.Enqueue(item: "second");
            var actualJson = JToken.Parse(json: queue.ReprTree());

            Assert.AreEqual(expected: "Queue", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: 2,
                actual: actualJson[key: "count"]!.Value<int>());

            var valueArray = (JArray)actualJson[key: "value"]!;
            Assert.AreEqual(expected: 2, actual: valueArray.Count);

            var item1 = valueArray[index: 0];
            Assert.AreEqual(expected: "string", actual: item1[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: item1[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: item1[key: "hashCode"]);
            Assert.AreEqual(expected: 5, actual: item1[key: "length"]!.Value<int>());
            Assert.AreEqual(expected: "first", actual: item1[key: "value"]
              ?.ToString());

            var item2 = valueArray[index: 1];
            Assert.AreEqual(expected: "string", actual: item2[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: item2[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: item2[key: "hashCode"]);
            Assert.AreEqual(expected: 6, actual: item2[key: "length"]!.Value<int>());
            Assert.AreEqual(expected: "second", actual: item2[key: "value"]
              ?.ToString());
        }

        [Test]
        public void TestStackRepr()
        {
            var stack = new Stack<int>();
            stack.Push(item: 1);
            stack.Push(item: 2);
            var actualJson = JToken.Parse(json: stack.ReprTree());

            Assert.AreEqual(expected: "Stack", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: 2,
                actual: actualJson[key: "count"]!.Value<int>());

            var valueArray = (JArray)actualJson[key: "value"]!;
            Assert.AreEqual(expected: 2, actual: valueArray.Count);
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "int", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "2"
                },
                t2: valueArray[index: 0]));
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "int", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "1"
                },
                t2: valueArray[index: 1]));
        }

        [Test]
        public void TestListWithNullElements()
        {
            var listWithNull = new List<List<int>?> { new() { 1 }, null };
            var actualJson = JToken.Parse(json: listWithNull.ReprTree());

            Assert.AreEqual(expected: "List", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: 2,
                actual: actualJson[key: "count"]!.Value<int>());

            var valueArray = (JArray)actualJson[key: "value"]!;
            Assert.AreEqual(expected: 2, actual: valueArray.Count);

            var listNode = valueArray[index: 0];
            Assert.AreEqual(expected: "List", actual: listNode[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: listNode[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: listNode[key: "hashCode"]);
            Assert.AreEqual(expected: 1, actual: listNode[key: "count"]!.Value<int>());
            var innerValue = (JArray)listNode[key: "value"]!;
            Assert.AreEqual(expected: 1, actual: innerValue.Count);
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "int", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "1"
                },
                t2: innerValue[index: 0]));

            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "object", [propertyName: "kind"] = "class",
                    [propertyName: "value"] = null
                },
                t2: valueArray[index: 1]));
        }
        [Test]
        public void TestTupleRepr()
        {
            var actualJson = JToken.Parse(json: (1, "hello").ReprTree());

            Assert.AreEqual(expected: "ValueTuple", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "struct", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.AreEqual(expected: 2,
                actual: actualJson[key: "length"]!.Value<int>());

            var valueArray = (JArray)actualJson[key: "value"]!;
            Assert.AreEqual(expected: 2, actual: valueArray.Count);

            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "int", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "1"
                },
                t2: valueArray[index: 0]));

            var stringElement = valueArray[index: 1];
            Assert.AreEqual(expected: "string", actual: stringElement[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: stringElement[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: stringElement[key: "hashCode"]);
            Assert.AreEqual(expected: 5,
                actual: stringElement[key: "length"]!.Value<int>());
            Assert.AreEqual(expected: "hello", actual: stringElement[key: "value"]
              ?.ToString());
        }
    }
}