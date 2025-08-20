#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DebugUtils.Unity.Repr;
using NUnit.Framework;
using UnityEngine;

namespace DebugUtils.Unity.Tests
{
    public class StressTest
    {
        private class DeepNestedClass
        {
            public string Name { get; set; } = "";
            public int Level { get; set; }
            public DeepNestedClass? Child { get; set; }
            public DeepNestedClass? Parent { get; set; }
            public List<DeepNestedClass> Children { get; set; } = new();
            public Dictionary<string, object> Metadata { get; set; } = new();
        }

        private class CircularReferenceClass
        {
            public string Id { get; set; } = "";
            public CircularReferenceClass? Next { get; set; }
            public CircularReferenceClass? Previous { get; set; }
            public List<CircularReferenceClass> References { get; set; } = new();
        }

        [Test]
        public void TestDeepNesting_Linear()
        {
            // Create a deep linear chain
            var root = new DeepNestedClass { Name = "Root", Level = 0 };
            var current = root;

            for (var i = 1; i <= 50; i++)
            {
                var child = new DeepNestedClass
                {
                    Name = $"Level_{i}",
                    Level = i,
                    Parent = current
                };
                current.Child = child;
                current = child;
            }

            // Test with depth limiting
            var config = new ReprConfig(MaxDepth: 10);
            var repr = root.Repr(config: config);

            Assert.IsTrue(condition: repr.Contains(value: "Level_1"));
            Assert.IsTrue(condition: repr.Contains(value: "<Max Depth Reached>"));

            // Test ReprTree format
            var reprTree = root.ReprTree(config: config);
            Assert.IsNotNull(anObject: reprTree);
            Assert.IsTrue(condition: reprTree.Length > 0);
        }

        [Test]
        public void TestDeepNesting_Wide()
        {
            // Create a wide tree structure
            var root = new DeepNestedClass { Name = "Root", Level = 0 };

            // Add 20 children to root
            for (var i = 0; i < 20; i++)
            {
                var child = new DeepNestedClass
                {
                    Name = $"Child_{i}",
                    Level = 1,
                    Parent = root
                };

                // Each child has 5 grandchildren
                for (var j = 0; j < 5; j++)
                {
                    var grandchild = new DeepNestedClass
                    {
                        Name = $"GrandChild_{i}_{j}",
                        Level = 2,
                        Parent = child
                    };
                    child.Children.Add(item: grandchild);

                    // Add some metadata
                    grandchild.Metadata[key: $"key_{j}"] = $"value_{i}_{j}";
                    grandchild.Metadata[key: "numbers"] = new List<int> { i, j, i * j };
                }

                root.Children.Add(item: child);
            }

            var config = new ReprConfig(MaxItemsPerContainer: 10, MaxDepth: 3);

            var repr = root.Repr(config: config);
            Assert.IsTrue(condition: repr.Contains(value: "Child_0"));
            Assert.IsTrue(condition: repr.Contains(value: "... (") &&
                                     repr.Contains(value: "more items)"));
        }

        [Test]
        public void TestCircularReferences_Complex()
        {
            // Create a complex circular reference network
            var nodeA = new CircularReferenceClass { Id = "A" };
            var nodeB = new CircularReferenceClass { Id = "B" };
            var nodeC = new CircularReferenceClass { Id = "C" };
            var nodeD = new CircularReferenceClass { Id = "D" };

            // Create circular chain: A -> B -> C -> D -> A
            nodeA.Next = nodeB;
            nodeB.Next = nodeC;
            nodeC.Next = nodeD;
            nodeD.Next = nodeA;

            // Create reverse references
            nodeA.Previous = nodeD;
            nodeB.Previous = nodeA;
            nodeC.Previous = nodeB;
            nodeD.Previous = nodeC;

            // Cross-references
            nodeA.References.AddRange(collection: new[] { nodeB, nodeC, nodeD });
            nodeB.References.AddRange(collection: new[] { nodeA, nodeC });
            nodeC.References.Add(item: nodeA);

            var repr = nodeA.Repr();
            Assert.IsTrue(condition: repr.Contains(value: "Circular Reference"));
            Assert.IsTrue(condition: repr.Contains(value: "CircularReferenceClass"));

            var reprTree = nodeA.ReprTree();
            Assert.IsNotNull(anObject: reprTree);
        }

        [Test]
        public void TestLargeCollections()
        {
            // Test a large array
            var largeArray = Enumerable.Range(start: 0, count: 1000)
                                       .ToArray();
            var config = new ReprConfig(MaxItemsPerContainer: 50);

            var arrayRepr = largeArray.Repr(config: config);
            // since range doesn't have the count parameter.
            Assert.IsTrue(condition: arrayRepr.Contains(value: "more items"));

            // Test a large dictionary
            var largeDict = new Dictionary<string, int>();
            for (var i = 0; i < 500; i++)
            {
                largeDict[key: $"key_{i:D4}"] = i * i;
            }

            var dictRepr = largeDict.Repr(config: config);
            Assert.IsTrue(condition: dictRepr.Contains(value: "more items") ||
                                     dictRepr.Length > 1000);

            // Test a nested large collections
            var nestedCollections = new List<List<Dictionary<string, object>>>();
            for (var i = 0; i < 10; i++)
            {
                var innerList = new List<Dictionary<string, object>>();
                for (var j = 0; j < 20; j++)
                {
                    var dict = new Dictionary<string, object>
                    {
                        [key: "index"] = $"{i}_{j}",
                        [key: "data"] = Enumerable.Range(start: 0, count: 100)
                                                  .ToList(),
                        [key: "metadata"] = new { Level = i, Item = j }
                    };
                    innerList.Add(item: dict);
                }

                nestedCollections.Add(item: innerList);
            }

            var nestedRepr = nestedCollections.Repr(config: new ReprConfig(MaxDepth: 3));
            Assert.IsNotNull(anObject: nestedRepr);
            Assert.IsTrue(condition: nestedRepr.Length > 0);
        }

        [Test]
        public void TestLongStrings()
        {
            var shortString = "Hello World";
            var mediumString = new string(c: 'A', count: 200);
            var longString = new string(c: 'B', count: 2000);
            var veryLongString = new string(c: 'C', count: 10000);

            var config = new ReprConfig(MaxStringLength: 100);

            var shortRepr = shortString.Repr(config: config);
            Assert.IsFalse(condition: shortRepr.Contains(value: "more letters"));

            var mediumRepr = mediumString.Repr(config: config);
            Assert.IsTrue(condition: mediumRepr.Contains(value: "(100 more letters)"));

            var longRepr = longString.Repr(config: config);
            Assert.IsTrue(condition: longRepr.Contains(value: "(1900 more letters)"));

            var veryLongRepr = veryLongString.Repr(config: config);
            Assert.IsTrue(condition: veryLongRepr.Contains(value: "(9900 more letters)"));
        }

        [Test]
        public void TestSpecialValues()
        {
            // Test special numeric values
            var specialFloats = new object[]
            {
                Single.NaN,
                Single.PositiveInfinity,
                Single.NegativeInfinity,
                Double.NaN,
                Double.PositiveInfinity,
                Double.NegativeInfinity,
                Decimal.MaxValue,
                Decimal.MinValue
            };

            foreach (var value in specialFloats)
            {
                var repr = value.Repr();
                Assert.IsNotNull(anObject: repr);
                Assert.IsTrue(condition: repr.Length > 0);

                var reprTree = value.ReprTree();
                Assert.IsNotNull(anObject: reprTree);
            }

            // Test special string values
            var specialStrings = new[]
            {
                "",
                "\0",
                "\r\n\t",
                "Unicode: 🚀🎯🔥",
                new string(c: '\0', count: 50),
                "Mixed\0Content\r\nWith\tSpecial"
            };

            foreach (var str in specialStrings)
            {
                var repr = str.Repr();
                Assert.IsNotNull(anObject: repr);

                var reprTree = str.ReprTree();
                Assert.IsNotNull(anObject: reprTree);
            }
        }

        [Test]
        [Timeout(timeout: 10000)] // 10-second timeout
        public void TestPerformance_LargeObject()
        {
            // Create a moderately complex object that's still realistic
            var complexObject = new
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTime.Now,
                Data = Enumerable.Range(start: 0, count: 1000)
                                 .Select(selector: i => new
                                  {
                                      Index = i,
                                      Value = Math.Sin(a: i * 0.1),
                                      Metadata = new Dictionary<string, object>
                                      {
                                          [key: "key"] = $"item_{i}",
                                          [key: "tags"] = new[] { "tag1", "tag2", $"tag_{i % 10}" }
                                      }
                                  })
                                 .ToArray(),
                Config = new ReprConfig(MaxItemsPerContainer: 100)
            };

            var stopwatch = Stopwatch.StartNew();

            var repr = complexObject.Repr();
            var reprTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();
            var reprTree = complexObject.ReprTree();
            var reprTreeTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Stop();

            // Performance assertions (adjust these based on your requirements)
            Assert.Less(arg1: reprTime, arg2: 5000,
                message: $"Repr() took too long: {reprTime}ms");
            Assert.Less(arg1: reprTreeTime, arg2: 8000,
                message: $"ReprTree() took too long: {reprTreeTime}ms");

            // Verify output quality
            Assert.IsNotNull(anObject: repr);
            Assert.IsTrue(condition: repr.Length > 100);
            Assert.IsNotNull(anObject: reprTree);
            Assert.IsTrue(condition: reprTree.Length > 200);

            UnityEngine.Debug.Log(message: $"Performance Test Results:");
            UnityEngine.Debug.Log(message: $"  Repr(): {reprTime}ms");
            UnityEngine.Debug.Log(message: $"  ReprTree(): {reprTreeTime}ms");
        }

        [Test]
        public void TestUnitySpecificObjects()
        {
            // Test Unity-specific types
            var gameObject = new GameObject(name: "StressTestObject");
            var transform = gameObject.transform;
            var color = new Color32(r: 255, g: 128, b: 64, a: 255);
            var vector = new Vector3(x: 1.5f, y: -2.3f, z: 0.0f);
            var quaternion = Quaternion.identity;

            try
            {
                var unityObjects = new object[]
                {
                    gameObject,
                    transform,
                    color,
                    vector,
                    quaternion,
                    new Vector2(x: 1.0f, y: 2.0f),
                    new Vector4(x: 1.0f, y: 2.0f, z: 3.0f, w: 4.0f),
                    new Color(r: 0.5f, g: 0.7f, b: 0.9f, a: 1.0f),
                    new Bounds(center: Vector3.zero, size: Vector3.one)
                };

                foreach (var obj in unityObjects)
                {
                    var repr = obj.Repr();
                    Assert.IsNotNull(anObject: repr);
                    Assert.IsTrue(condition: repr.Length > 0);

                    var reprTree = obj.ReprTree();
                    Assert.IsNotNull(anObject: reprTree);
                    Assert.IsTrue(condition: reprTree.Length > 10);
                }
            }
            finally
            {
                // Clean up Unity objects
                UnityEngine.Object.DestroyImmediate(obj: gameObject);
            }
        }

        [Test]
        public void TestMemoryPressure()
        {
            // Test that we don't leak memory with circular references
            var objects = new List<CircularReferenceClass>();

            // Create 100 objects with complex circular references
            for (var i = 0; i < 100; i++)
            {
                var obj = new CircularReferenceClass { Id = $"Object_{i}" };

                // Reference some previous objects
                if (objects.Count > 0)
                {
                    obj.Previous = objects[^1];
                    obj.References.AddRange(
                        collection: objects.Take(count: Math.Min(val1: 5, val2: objects.Count)));

                    // Create back-references
                    objects.Last()
                           .Next = obj;
                }

                objects.Add(item: obj);
            }

            // Test all objects - this should not cause memory leaks
            foreach (var obj in objects)
            {
                var repr = obj.Repr();
                Assert.IsNotNull(anObject: repr);
                Assert.IsTrue(condition: repr.Contains(value: "Circular Reference") ||
                                         repr.Length > 10);
            }

            // Force garbage collection to verify cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        [Test]
        public void TestEdgeCaseCollections()
        {
            // Empty collections
            var emptyArray = Array.Empty<int>();
            var emptyList = new List<string>();
            var emptyDict = new Dictionary<int, string>();

            Assert.DoesNotThrow(code: () => emptyArray.Repr());
            Assert.DoesNotThrow(code: () => emptyList.Repr());
            Assert.DoesNotThrow(code: () => emptyDict.Repr());

            // Collections with null elements
            var nullArray = new string?[] { "a", null, "c", null };
            var nullList = new List<object?> { 1, null, "test", null, 3.14 };

            var nullArrayRepr = nullArray.Repr();
            var nullListRepr = nullList.Repr();

            Assert.IsTrue(condition: nullArrayRepr.Contains(value: "null"));
            Assert.IsTrue(condition: nullListRepr.Contains(value: "null"));

            // Nested null collections
            var nestedNulls = new List<List<string?>?>
            {
                null,
                new() { "test", null },
                null,
                new() { null, null, "end" }
            };

            Assert.DoesNotThrow(code: () => nestedNulls.Repr());
        }

        private static DeepNestedClass CreateDeepStructure(int depth)
        {
            if (depth <= 0)
            {
                return new DeepNestedClass { Name = "Leaf", Level = 0 };
            }

            var node = new DeepNestedClass
            {
                Name = $"Node_Level_{depth}",
                Level = depth
            };

            node.Child = CreateDeepStructure(depth: depth - 1);
            if (node.Child != null)
            {
                node.Child.Parent = node;
            }

            return node;
        }
    }
}