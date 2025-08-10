#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DebugUtils.Unity.Repr;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UnityEngine;

namespace DebugUtils.Unity.Tests
{
    public class ReprTreeStressTest
    {
        private class NestedTreeData
        {
            public string Name { get; set; } = "";
            public int Level { get; set; }
            public NestedTreeData? Child { get; set; }
            public List<NestedTreeData> Children { get; set; } = new();
            public Dictionary<string, object> Properties { get; set; } = new();
            public NestedTreeData? Parent { get; set; }
        }

        private class CircularTreeNode
        {
            public string Id { get; set; } = "";
            public CircularTreeNode? Next { get; set; }
            public List<CircularTreeNode> References { get; set; } = new();
        }

        [Test]
        public void TestReprTree_DeepNesting_JsonStructure()
        {
            // Create deep linear chain
            var root = new NestedTreeData { Name = "Root", Level = 0 };
            var current = root;

            for (var i = 1; i <= 20; i++)
            {
                var child = new NestedTreeData
                {
                    Name = $"Level_{i}",
                    Level = i,
                    Parent = current
                };
                current.Child = child;
                current = child;
            }

            var config = new ReprConfig(MaxDepth: 8, EnablePrettyPrintForReprTree: true);
            var reprTreeJson = root.ReprTree(config: config);

            // Parse and validate JSON structure
            var jsonObject = JToken.Parse(json: reprTreeJson);

            Assert.IsNotNull(anObject: jsonObject);
            Assert.AreEqual(expected: JTokenType.Object, actual: jsonObject.Type);

            var rootObj = (JObject)jsonObject;

            // Verify root structure
            Assert.IsTrue(condition: rootObj.ContainsKey(propertyName: "type"));
            Assert.IsTrue(condition: rootObj.ContainsKey(propertyName: "kind"));
            Assert.IsTrue(condition: rootObj.ContainsKey(propertyName: "Name"));
            Assert.IsTrue(condition: rootObj.ContainsKey(propertyName: "Level"));
            Assert.IsTrue(condition: rootObj.ContainsKey(propertyName: "Child"));

            // Verify type information is not arrays
            Assert.AreEqual(expected: JTokenType.String,
                actual: rootObj[propertyName: "type"]!.Type);
            Assert.AreEqual(expected: JTokenType.String,
                actual: rootObj[propertyName: "kind"]!.Type);

            // Navigate to child and verify structure
            var childToken = rootObj[propertyName: "Child"]!;
            Assert.IsNotNull(anObject: childToken);
            Assert.AreEqual(expected: JTokenType.Object, actual: childToken.Type);

            var childObj = (JObject)childToken;
            Assert.AreEqual(expected: JTokenType.String,
                actual: childObj[propertyName: "type"]!.Type);

            UnityEngine.Debug.Log(
                message:
                $"ReprTree JSON Structure (first 500 chars):\n{reprTreeJson[..Math.Min(val1: 500, val2: reprTreeJson.Length)]}...");
        }

        [Test]
        public void TestReprTree_LargeCollections_JsonFormat()
        {
            // Create large collections with various data types
            var testData = new
            {
                LargeArray = Enumerable.Range(start: 0, count: 200)
                                       .ToArray(),
                LargeDictionary = Enumerable.Range(start: 0, count: 100)
                                            .ToDictionary(keySelector: i => $"key_{i}",
                                                 elementSelector: i => i * i),
                MixedList = new List<object?>
                {
                    "string_value",
                    42,
                    3.14159,
                    true,
                    null,
                    new { nested = "object" },
                    new[] { 1, 2, 3, 4, 5 }
                },
                NestedStructure = new Dictionary<string, object>
                {
                    [key: "level1"] = new Dictionary<string, object>
                    {
                        [key: "level2"] = new List<Dictionary<string, object>>
                        {
                            new()
                            {
                                [key: "key1"] = "value1", [key: "numbers"] = new[] { 1, 2, 3 }
                            },
                            new()
                            {
                                [key: "key2"] = "value2", [key: "data"] = new { x = 10, y = 20 }
                            }
                        }
                    }
                }
            };

            var config = new ReprConfig(
                MaxElementsPerCollection: 50,
                MaxDepth: 5,
                EnablePrettyPrintForReprTree: false
            );

            var reprTreeJson = testData.ReprTree(config: config);
            var jsonObject = JToken.Parse(json: reprTreeJson);

            Assert.IsNotNull(anObject: jsonObject);
            Assert.AreEqual(expected: JTokenType.Object, actual: jsonObject.Type);

            var rootObj = (JObject)jsonObject;

            // Verify large array handling
            Assert.IsTrue(condition: rootObj.ContainsKey(propertyName: "LargeArray"));
            var largeArrayToken = rootObj[propertyName: "LargeArray"];
            Assert.AreEqual(expected: JTokenType.Object, actual: largeArrayToken!.Type);

            var arrayObj = (JObject)largeArrayToken;
            Assert.AreEqual(expected: "1DArray",
                actual: arrayObj[propertyName: "type"]!.ToString());
            Assert.IsTrue(condition: arrayObj.ContainsKey(propertyName: "value"));

            // Verify dictionary handling
            Assert.IsTrue(condition: rootObj.ContainsKey(propertyName: "LargeDictionary"));
            var dictToken = rootObj[propertyName: "LargeDictionary"];
            Assert.AreEqual(expected: JTokenType.Object, actual: dictToken!.Type);

            // Verify no [] artifacts in the JSON
            Assert.IsFalse(condition: reprTreeJson.Contains(value: "\"type\":[]"));
            Assert.IsFalse(condition: reprTreeJson.Contains(value: "\"kind\":[]"));
            Assert.IsFalse(condition: reprTreeJson.Contains(value: "\"value\":[]"));
        }

        [Test]
        public void TestReprTree_CircularReferences_JsonStructure()
        {
            var nodeA = new CircularTreeNode { Id = "NodeA" };
            var nodeB = new CircularTreeNode { Id = "NodeB" };
            var nodeC = new CircularTreeNode { Id = "NodeC" };

            // Create circular references
            nodeA.Next = nodeB;
            nodeB.Next = nodeC;
            nodeC.Next = nodeA;

            nodeA.References.AddRange(collection: new[] { nodeB, nodeC });
            nodeB.References.Add(item: nodeA);

            var reprTreeJson = nodeA.ReprTree();
            var jsonObject = JToken.Parse(json: reprTreeJson);

            Assert.IsNotNull(anObject: jsonObject);
            Assert.AreEqual(expected: JTokenType.Object, actual: jsonObject.Type);

            // The JSON should contain circular reference markers
            Assert.IsTrue(condition: reprTreeJson.Contains(value: "CircularReference") ||
                                     reprTreeJson.Contains(value: "Circular Reference"));

            // Verify structure is valid JSON
            Assert.DoesNotThrow(code: () => JToken.Parse(json: reprTreeJson));
        }

        [Test]
        public void TestReprTree_SpecialValues_JsonSerialization()
        {
            var specialValues = new
            {
                FloatNaN = Single.NaN,
                FloatPositiveInfinity = Single.PositiveInfinity,
                FloatNegativeInfinity = Single.NegativeInfinity,
                DoubleNaN = Double.NaN,
                DoublePositiveInfinity = Double.PositiveInfinity,
                DoubleNegativeInfinity = Double.NegativeInfinity,
                DecimalMax = Decimal.MaxValue,
                DecimalMin = Decimal.MinValue,
                StringEmpty = "",
                StringNull = (string?)null,
                StringSpecial = "Special\0\r\n\tChars",
                UnicodeString = "Unicode: 🚀🎯🔥",
                IntMax = Int32.MaxValue,
                IntMin = Int32.MinValue,
                LongMax = Int64.MaxValue,
                ByteMax = Byte.MaxValue
            };

            var reprTreeJson = specialValues.ReprTree(
                config: new ReprConfig(EnablePrettyPrintForReprTree: true,
                    MaxPropertiesPerObject: -1));
            var jsonObject = JToken.Parse(json: reprTreeJson);

            Assert.IsNotNull(anObject: jsonObject);
            Assert.AreEqual(expected: JTokenType.Object, actual: jsonObject.Type);

            var rootObj = (JObject)jsonObject;

            // Verify all special values are represented
            Assert.IsTrue(condition: rootObj.ContainsKey(propertyName: "FloatNaN"));
            Assert.IsTrue(condition: rootObj.ContainsKey(propertyName: "FloatPositiveInfinity"));
            Assert.IsTrue(condition: rootObj.ContainsKey(propertyName: "StringEmpty"));
            Assert.IsTrue(condition: rootObj.ContainsKey(propertyName: "UnicodeString"));

            // Verify no serialization artifacts
            Assert.IsFalse(condition: reprTreeJson.Contains(value: "\"type\":[]"));
            Assert.IsFalse(condition: reprTreeJson.Contains(value: "\"kind\":[]"));

            UnityEngine.Debug.Log(
                message:
                $"Special Values JSON Sample:\n{reprTreeJson[..Math.Min(val1: 800, val2: reprTreeJson.Length)]}...");
        }

        [Test]
        public void TestReprTree_UnityObjects_JsonStructure()
        {
            var gameObject = new GameObject(name: "TestObject");
            var transform = gameObject.transform;

            var unityData = new
            {
                GameObject = gameObject,
                Transform = transform,
                Vector3 = new Vector3(x: 1.5f, y: -2.3f, z: 0.0f),
                Quaternion = Quaternion.identity,
                Color32 = new Color32(r: 255, g: 128, b: 64, a: 255),
                Color = new Color(r: 0.5f, g: 0.7f, b: 0.9f, a: 1.0f),
                Bounds = new Bounds(center: Vector3.zero, size: Vector3.one)
            };

            try
            {
                var reprTreeJson = unityData.ReprTree();
                var jsonObject = JToken.Parse(json: reprTreeJson);

                Assert.IsNotNull(anObject: jsonObject);
                Assert.AreEqual(expected: JTokenType.Object, actual: jsonObject.Type);

                var rootObj = (JObject)jsonObject;

                // Verify Unity objects are properly serialized
                Assert.IsTrue(condition: rootObj.ContainsKey(propertyName: "Vector3"));
                Assert.IsTrue(condition: rootObj.ContainsKey(propertyName: "Color32"));
                Assert.IsTrue(condition: rootObj.ContainsKey(propertyName: "Quaternion"));

                // Check Vector3 structure
                var vector3Token = rootObj[propertyName: "Vector3"];
                Assert.AreEqual(expected: JTokenType.Object, actual: vector3Token!.Type);

                var vector3Obj = (JObject)vector3Token;
                Assert.AreEqual(expected: JTokenType.String,
                    actual: vector3Obj[propertyName: "type"]!.Type);
                Assert.AreEqual(expected: "Vector3",
                    actual: vector3Obj[propertyName: "type"]!.ToString());

                // Check Color32 structure (should have RGBA, potentially HSV)
                var color32Token = rootObj[propertyName: "Color32"];
                Assert.AreEqual(expected: JTokenType.Object, actual: color32Token!.Type);

                var color32Obj = (JObject)color32Token;
                Assert.IsTrue(condition: color32Obj.ContainsKey(propertyName: "r"));
                Assert.IsTrue(condition: color32Obj.ContainsKey(propertyName: "g"));
                Assert.IsTrue(condition: color32Obj.ContainsKey(propertyName: "b"));
                Assert.IsTrue(condition: color32Obj.ContainsKey(propertyName: "a"));
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(obj: gameObject);
            }
        }

        [Test]
        [Timeout(timeout: 15000)] // 15-second timeout for JSON operations
        public void TestReprTree_Performance_LargeJsonOutput()
        {
            // Create a complex nested structure
            var complexData = new
            {
                Metadata = new
                {
                    Version = "1.0.0",
                    Timestamp = DateTime.Now,
                    Id = Guid.NewGuid()
                },
                LargeDataSet = Enumerable.Range(start: 0, count: 500)
                                         .Select(selector: i => new
                                          {
                                              Index = i,
                                              Value = Math.Sin(a: i * 0.1),
                                              Category = $"Category_{i % 10}",
                                              Tags = new[]
                                                  { $"tag_{i}", $"group_{i % 5}", "common" },
                                              Nested = new Dictionary<string, object>
                                              {
                                                  [key: "key1"] = $"value_{i}",
                                                  [key: "key2"] = i * i,
                                                  [key: "key3"] = new
                                                      { X = i, Y = i * 2, Z = i * 3 }
                                              }
                                          })
                                         .ToArray(),
                Configuration = new ReprConfig(
                    MaxElementsPerCollection: 100,
                    MaxDepth: 4,
                    EnablePrettyPrintForReprTree: false
                )
            };

            var stopwatch = Stopwatch.StartNew();

            // Test non-pretty JSON
            var compactJson = complexData.ReprTree();
            var compactTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();

            // Test pretty-printed JSON
            var prettyConfig = new ReprConfig(EnablePrettyPrintForReprTree: true,
                MaxElementsPerCollection: 50);
            var prettyJson = complexData.ReprTree(config: prettyConfig);
            var prettyTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Stop();

            // Validate JSON structure
            Assert.DoesNotThrow(code: () => JToken.Parse(json: compactJson));
            Assert.DoesNotThrow(code: () => JToken.Parse(json: prettyJson));

            // Performance assertions
            Assert.Less(arg1: compactTime, arg2: 10000,
                message: $"Compact JSON took too long: {compactTime}ms");
            Assert.Less(arg1: prettyTime, arg2: 12000,
                message: $"Pretty JSON took too long: {prettyTime}ms");

            // Size assertions
            Assert.IsTrue(condition: compactJson.Length > 1000);
            Assert.IsTrue(condition: prettyJson.Length >
                                     compactJson.Length); // Pretty should be larger

            // Verify no serialization artifacts
            Assert.IsFalse(condition: compactJson.Contains(value: "\"type\":[]"));
            Assert.IsFalse(condition: compactJson.Contains(value: "\"kind\":[]"));
            Assert.IsFalse(condition: prettyJson.Contains(value: "\"type\":[]"));
            Assert.IsFalse(condition: prettyJson.Contains(value: "\"kind\":[]"));

            UnityEngine.Debug.Log(message: $"ReprTree Performance Results:");
            UnityEngine.Debug.Log(
                message: $"  Compact JSON: {compactTime}ms ({compactJson.Length:N0} chars)");
            UnityEngine.Debug.Log(
                message: $"  Pretty JSON: {prettyTime}ms ({prettyJson.Length:N0} chars)");
        }

        [Test]
        public void TestReprTree_JsonValidation_AllFormatters()
        {
            // Test all major formatter types to ensure valid JSON output
            var testObjects = new object?[]
            {
                // Basic types
                42,
                3.14159,
                "Hello World",
                true,
                'A',

                // Collections
                new[] { 1, 2, 3 },
                new List<string> { "a", "b", "c" },
                new Dictionary<string, int> { [key: "key1"] = 1, [key: "key2"] = 2 },
                new HashSet<int> { 1, 2, 3 },

                // Complex types
                new { Name = "Test", Value = 42 },
                DateTime.Now,
                TimeSpan.FromMinutes(value: 30),
                Guid.NewGuid(),

                // Unity types
                new Vector3(x: 1, y: 2, z: 3),
                new Color32(r: 255, g: 128, b: 64, a: 255),
                Quaternion.identity,

                // Special cases
                (string?)null,
                new int[0], // Empty array
                new List<object?> { 1, null, "test" } // Mixed with nulls
            };

            foreach (var testObj in testObjects)
            {
                var reprTreeJson = testObj.ReprTree();

                // Validate JSON structure
                JToken parsedJson;
                Assert.DoesNotThrow(code: () => parsedJson = JToken.Parse(json: reprTreeJson),
                    message: $"Failed to parse JSON for {testObj?.GetType().Name ?? "null"}");

                parsedJson = JToken.Parse(json: reprTreeJson);
                Assert.IsNotNull(anObject: parsedJson);

                // Ensure no array serialization artifacts
                Assert.IsFalse(condition: reprTreeJson.Contains(value: "\"type\":[]"),
                    message: $"Found type:[] artifact in {testObj?.GetType().Name ?? "null"}");
                Assert.IsFalse(condition: reprTreeJson.Contains(value: "\"kind\":[]"),
                    message: $"Found kind:[] artifact in {testObj?.GetType().Name ?? "null"}");
                if (testObj is Array array && array.Length == 0)
                {
                    // its value WILL be [].
                    continue;
                }

                Assert.IsFalse(condition: reprTreeJson.Contains(value: "\"value\":[]"),
                    message: $"Found value:[] artifact in {testObj?.GetType().Name ?? "null"}");
            }
        }

        [Test]
        public void TestReprTree_PrettyPrint_Formatting()
        {
            var testData = new
            {
                Name = "TestObject",
                Numbers = new[] { 1, 2, 3, 4, 5 },
                Nested = new
                {
                    InnerName = "Inner",
                    InnerValue = 42,
                    DeepNested = new Dictionary<string, object>
                    {
                        [key: "key1"] = "value1",
                        [key: "key2"] = new[] { "a", "b", "c" }
                    }
                }
            };

            // Test compact format
            var compactJson =
                testData.ReprTree(config: new ReprConfig(EnablePrettyPrintForReprTree: false));
            Assert.IsFalse(condition: compactJson.Contains(value: "\n"));
            Assert.IsFalse(condition: compactJson.Contains(value: "  ")); // No indentation

            // Test pretty format
            var prettyJson =
                testData.ReprTree(config: new ReprConfig(EnablePrettyPrintForReprTree: true));
            Assert.IsTrue(condition: prettyJson.Contains(value: "\n"));
            Assert.IsTrue(condition: prettyJson.Contains(value: "  ")); // Has indentation

            // Both should be valid JSON
            Assert.DoesNotThrow(code: () => JToken.Parse(json: compactJson));
            Assert.DoesNotThrow(code: () => JToken.Parse(json: prettyJson));

            // Pretty should be significantly larger due to formatting
            Assert.IsTrue(condition: prettyJson.Length > compactJson.Length * 1.3);

            UnityEngine.Debug.Log(
                message: $"Compact JSON ({compactJson.Length} chars):\n{compactJson}");
            UnityEngine.Debug.Log(
                message: $"\nPretty JSON ({prettyJson.Length} chars):\n{prettyJson}");
        }

        [Test]
        public void TestReprTree_ErrorRecovery()
        {
            // Test objects that might cause issues
            var problematicObjects = new object?[]
            {
                null,
                "",
                new object[0],
                new Dictionary<string, object>(),
                Single.NaN,
                Double.PositiveInfinity,
                new { RecursiveProperty = (object?)null }
            };

            // Set RecursiveProperty to create a potential issue
            var recursiveObj = new { RecursiveProperty = (object?)null };

            foreach (var obj in problematicObjects)
            {
                Assert.DoesNotThrow(code: () =>
                    {
                        var json = obj.ReprTree();
                        Assert.IsNotNull(anObject: json);
                        Assert.IsTrue(condition: json.Length > 0);

                        // Validate JSON
                        var parsed = JToken.Parse(json: json);
                        Assert.IsNotNull(anObject: parsed);
                    }, message: $"Failed for object: {obj?.GetType().Name ?? "null"}");
            }
        }
    }
}