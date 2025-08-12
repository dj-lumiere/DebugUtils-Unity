#nullable enable
using DebugUtils.Unity.Repr;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;

namespace DebugUtils.Unity.Tests
{

    public class UnitySpecificTreeTests
    {
        // Unity-specific Tests
        [Test]
        public void TestVector2Repr()
        {
            var vector = new Vector2(x: 3.14f, y: 2.71f);
            var actualJson = JToken.Parse(json: vector.ReprTree());
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "Vector2",
                [propertyName: "kind"] = "struct",
                [propertyName: "x"] = new JObject
                {
                    [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "3.14"
                },
                [propertyName: "y"] = new JObject
                {
                    [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "2.71"
                },
                [propertyName: "magnitude"] = new JObject
                {
                    [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "4.147735"
                },
                [propertyName: "normalized"] = new JObject
                {
                    [propertyName: "x"] = new JObject
                    {
                        [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                        [propertyName: "value"] = "0.7570398"
                    },
                    [propertyName: "y"] = new JObject
                    {
                        [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                        [propertyName: "value"] = "0.6533687"
                    }
                }
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        [Test]
        public void TestVector3Repr()
        {
            var vector = new Vector3(x: 1.0f, y: 2.0f, z: 3.0f);
            var actualJson = JToken.Parse(json: vector.ReprTree());
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "Vector3",
                [propertyName: "kind"] = "struct",
                [propertyName: "x"] = new JObject
                {
                    [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "1"
                },
                [propertyName: "y"] = new JObject
                {
                    [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "2"
                },
                [propertyName: "z"] = new JObject
                {
                    [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "3"
                },
                [propertyName: "magnitude"] = new JObject
                {
                    [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "3.741657"
                },
                [propertyName: "normalized"] = new JObject
                {
                    [propertyName: "x"] = new JObject
                    {
                        [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                        [propertyName: "value"] = "0.2672612"
                    },
                    [propertyName: "y"] = new JObject
                    {
                        [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                        [propertyName: "value"] = "0.5345225"
                    },
                    [propertyName: "z"] = new JObject
                    {
                        [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                        [propertyName: "value"] = "0.8017837"
                    }
                }
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        [Test]
        public void TestVector4Repr()
        {
            var vector = new Vector4(x: 1.0f, y: 2.0f, z: 3.0f, w: 4.0f);
            var actualJson = JToken.Parse(json: vector.ReprTree());
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "Vector4",
                [propertyName: "kind"] = "struct",
                [propertyName: "x"] = new JObject
                {
                    [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "1"
                },
                [propertyName: "y"] = new JObject
                {
                    [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "2"
                },
                [propertyName: "z"] = new JObject
                {
                    [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "3"
                },
                [propertyName: "w"] = new JObject
                {
                    [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "4"
                },
                [propertyName: "magnitude"] = new JObject
                {
                    [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "5.477226"
                },
                [propertyName: "normalized"] = new JObject
                {
                    [propertyName: "x"] = new JObject
                    {
                        [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                        [propertyName: "value"] = "0.1825742"
                    },
                    [propertyName: "y"] = new JObject
                    {
                        [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                        [propertyName: "value"] = "0.3651484"
                    },
                    [propertyName: "z"] = new JObject
                    {
                        [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                        [propertyName: "value"] = "0.5477225"
                    },
                    [propertyName: "w"] = new JObject
                    {
                        [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                        [propertyName: "value"] = "0.7302967"
                    }
                }
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        [Test]
        public void TestQuaternionRepr()
        {
            var quaternion = new Quaternion(x: 0.0f, y: 0.0f, z: 0.0f, w: 1.0f);
            var actualJson = JToken.Parse(json: quaternion.ReprTree());
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "Quaternion",
                [propertyName: "kind"] = "struct",
                [propertyName: "x"] = new JObject
                {
                    [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "0"
                },
                [propertyName: "y"] = new JObject
                {
                    [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "0"
                },
                [propertyName: "z"] = new JObject
                {
                    [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "0"
                },
                [propertyName: "w"] = new JObject
                {
                    [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "1"
                },
                [propertyName: "eulerAngles"] = new JObject
                {
                    [propertyName: "x"] = new JObject
                    {
                        [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                        [propertyName: "value"] = "0"
                    },
                    [propertyName: "y"] = new JObject
                    {
                        [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                        [propertyName: "value"] = "0"
                    },
                    [propertyName: "z"] = new JObject
                    {
                        [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                        [propertyName: "value"] = "0"
                    }
                }
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        [Test]
        public void TestColorRepr()
        {
            var color =
                new Color(r: 1.0f, g: 0.5f, b: 0.2f,
                    a: 0.8f); // Red=1.0, Green=0.5, Blue=0.2, Alpha=0.8
            var actualJson = JToken.Parse(json: color.ReprTree());
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "Color",
                [propertyName: "kind"] = "struct",
                [propertyName: "r"] = new JObject
                {
                    [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "1"
                },
                [propertyName: "g"] = new JObject
                {
                    [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "0.5"
                },
                [propertyName: "b"] = new JObject
                {
                    [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "0.2"
                },
                [propertyName: "a"] = new JObject
                {
                    [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "0.8"
                },
                [propertyName: "h"] = new JObject
                {
                    [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "0.0625"
                },
                [propertyName: "s"] = new JObject
                {
                    [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "0.8"
                },
                [propertyName: "v"] = new JObject
                {
                    [propertyName: "type"] = "float", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "1"
                }
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        [Test]
        public void TestColor32Repr()
        {
            var color = new Color32(r: 255, g: 127, b: 51, a: 204);
            var actualJson = JToken.Parse(json: color.ReprTree());
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "Color32",
                [propertyName: "kind"] = "struct",
                [propertyName: "r"] = new JObject
                {
                    [propertyName: "type"] = "byte", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "255"
                },
                [propertyName: "g"] = new JObject
                {
                    [propertyName: "type"] = "byte", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "127"
                },
                [propertyName: "b"] = new JObject
                {
                    [propertyName: "type"] = "byte", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "51"
                },
                [propertyName: "a"] = new JObject
                {
                    [propertyName: "type"] = "byte", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "204"
                },
                [propertyName: "h"] = new JObject
                {
                    [propertyName: "type"] = "byte", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "15"
                },
                [propertyName: "s"] = new JObject
                {
                    [propertyName: "type"] = "byte", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "204"
                },
                [propertyName: "v"] = new JObject
                {
                    [propertyName: "type"] = "byte", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "255"
                }
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        [Test]
        public void TestGameObjectRepr()
        {
            var gameObject = new GameObject(name: "TestGameObject")
            {
                transform =
                {
                    position = new Vector3(x: 1.0f, y: 2.0f, z: 3.0f)
                }
            };

            try
            {
                var actualJson = JToken.Parse(json: gameObject.ReprTree());

                Assert.AreEqual(expected: "GameObject", actual: actualJson[key: "type"]
                  ?.ToString());
                Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
                  ?.ToString());
                Assert.AreEqual(expected: "TestGameObject", actual: actualJson[key: "name"]
                  ?.ToString());
                Assert.NotNull(anObject: actualJson[key: "hashCode"]);
                Assert.NotNull(anObject: actualJson[key: "path"]);

                // Check position structure
                var position = actualJson[key: "position"]!;
                Assert.AreEqual(expected: "Vector3", actual: position[key: "type"]
                  ?.ToString());
                Assert.AreEqual(expected: "struct", actual: position[key: "kind"]
                  ?.ToString());
                Assert.AreEqual(expected: "1", actual: position[key: "x"]![key: "value"]
                  ?.ToString());
                Assert.AreEqual(expected: "2", actual: position[key: "y"]![key: "value"]
                  ?.ToString());
                Assert.AreEqual(expected: "3", actual: position[key: "z"]![key: "value"]
                  ?.ToString());
            }
            finally
            {
                Object.DestroyImmediate(obj: gameObject);
            }
        }

        [Test]
        public void TestTransformRepr()
        {
            var gameObject = new GameObject(name: "TestTransformObject")
            {
                transform =
                {
                    position = new Vector3(x: 5.0f, y: 10.0f, z: 15.0f)
                }
            };
            var transform = gameObject.transform;

            try
            {
                var actualJson = JToken.Parse(json: transform.ReprTree());

                Assert.AreEqual(expected: "Transform", actual: actualJson[key: "type"]
                  ?.ToString());
                Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
                  ?.ToString());
                Assert.AreEqual(expected: "TestTransformObject", actual: actualJson[key: "name"]
                  ?.ToString());
                Assert.NotNull(anObject: actualJson[key: "hashCode"]);
                Assert.NotNull(anObject: actualJson[key: "path"]);

                // Check position structure
                var position = actualJson[key: "position"]!;
                Assert.AreEqual(expected: "Vector3", actual: position[key: "type"]
                  ?.ToString());
                Assert.AreEqual(expected: "struct", actual: position[key: "kind"]
                  ?.ToString());
                Assert.AreEqual(expected: "5", actual: position[key: "x"]![key: "value"]
                  ?.ToString());
                Assert.AreEqual(expected: "10", actual: position[key: "y"]![key: "value"]
                  ?.ToString());
                Assert.AreEqual(expected: "15", actual: position[key: "z"]![key: "value"]
                  ?.ToString());
            }
            finally
            {
                Object.DestroyImmediate(obj: gameObject);
            }
        }

        [Test]
        public void TestGameObjectWithChildrenRepr()
        {
            var parent = new GameObject(name: "Parent");
            var child1 = new GameObject(name: "Child1");
            var child2 = new GameObject(name: "Child2");

            child1.transform.SetParent(p: parent.transform);
            child2.transform.SetParent(p: parent.transform);

            try
            {
                var actualJson = JToken.Parse(json: parent.ReprTree());

                Assert.AreEqual(expected: "GameObject", actual: actualJson[key: "type"]
                  ?.ToString());
                Assert.AreEqual(expected: "Parent", actual: actualJson[key: "name"]
                  ?.ToString());

                // Check children array exists and has the correct count
                var children = (JArray)actualJson[key: "children"]!;
                Assert.AreEqual(expected: 2, actual: children.Count);

                // Check first child
                var firstChild = children[index: 0];
                Assert.AreEqual(expected: "GameObject", actual: firstChild[key: "type"]
                  ?.ToString());
                Assert.AreEqual(expected: "Child1", actual: firstChild[key: "name"]
                  ?.ToString());

                // Check second child
                var secondChild = children[index: 1];
                Assert.AreEqual(expected: "GameObject", actual: secondChild[key: "type"]
                  ?.ToString());
                Assert.AreEqual(expected: "Child2", actual: secondChild[key: "name"]
                  ?.ToString());
            }
            finally
            {
                Object.DestroyImmediate(obj: child2);
                Object.DestroyImmediate(obj: child1);
                Object.DestroyImmediate(obj: parent);
            }
        }
    }
}