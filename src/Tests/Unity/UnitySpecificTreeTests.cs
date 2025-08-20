#nullable enable
using Color = UnityEngine.Color;
using Color32 = UnityEngine.Color32;
using DebugUtils.Unity.Repr;
using GameObject = UnityEngine.GameObject;
using Object = UnityEngine.Object;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
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
                [propertyName: "x"] = "3.14_f32",
                [propertyName: "y"] = "2.71_f32",
                [propertyName: "magnitude"] = "4.147735_f32"
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
                [propertyName: "x"] = "1_f32",
                [propertyName: "y"] = "2_f32",
                [propertyName: "z"] = "3_f32",
                [propertyName: "magnitude"] = "3.741657_f32"
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
                [propertyName: "x"] = "1_f32",
                [propertyName: "y"] = "2_f32",
                [propertyName: "z"] = "3_f32",
                [propertyName: "w"] = "4_f32",
                [propertyName: "magnitude"] = "5.477226_f32"
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
                [propertyName: "x"] = "0_f32",
                [propertyName: "y"] = "0_f32",
                [propertyName: "z"] = "0_f32",
                [propertyName: "w"] = "1_f32",
                [propertyName: "eulerDegreeX"] = "0_f32",
                [propertyName: "eulerDegreeY"] = "0_f32",
                [propertyName: "eulerDegreeZ"] = "0_f32"
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
                [propertyName: "r"] = "1_f32",
                [propertyName: "g"] = "0.5_f32",
                [propertyName: "b"] = "0.2_f32",
                [propertyName: "a"] = "0.8_f32",
                [propertyName: "h"] = "22.5_f32",
                [propertyName: "s"] = "80_f32",
                [propertyName: "v"] = "100_f32",
                [propertyName: "rgbaForm"] = "#FF7F33CC"
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
                [propertyName: "r"] = "255_u8",
                [propertyName: "g"] = "127_u8",
                [propertyName: "b"] = "51_u8",
                [propertyName: "a"] = "204_u8",
                [propertyName: "h"] = "22.35294_f32",
                [propertyName: "s"] = "80_f32",
                [propertyName: "v"] = "100_f32",
                [propertyName: "rgbaForm"] = "#FF7F33CC"
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        [Test]
        public void TestColorReprString()
        {
            var color = new Color(r: 1.0f, g: 0.5f, b: 0.2f, a: 0.8f);
            var reprString = color.Repr();

            // Should contain percentage values and color square
            Assert.IsTrue(reprString.Contains("R100%"));
            Assert.IsTrue(reprString.Contains("G50%"));
            Assert.IsTrue(reprString.Contains("B20%"));
            Assert.IsTrue(reprString.Contains("A80%"));
            Assert.IsTrue(reprString.Contains("<color=#FF8033CC>■■■</color>"));
        }

        [Test]
        public void TestColor32ReprString()
        {
            var color = new Color32(r: 255, g: 127, b: 51, a: 204);
            var reprString = color.Repr();

            // Should contain hex values and color square
            Assert.IsTrue(reprString.Contains("RGBA = #FF7F33CC"));
            Assert.IsTrue(reprString.Contains("<color=#FF7F33CC>■■■</color>"));
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
                Assert.AreEqual(expected: "1_f32", actual: position[key: "x"]
                  ?.ToString());
                Assert.AreEqual(expected: "2_f32", actual: position[key: "y"]
                  ?.ToString());
                Assert.AreEqual(expected: "3_f32", actual: position[key: "z"]
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
                Assert.AreEqual(expected: "5_f32", actual: position[key: "x"]
                  ?.ToString());
                Assert.AreEqual(expected: "10_f32", actual: position[key: "y"]
                  ?.ToString());
                Assert.AreEqual(expected: "15_f32", actual: position[key: "z"]
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