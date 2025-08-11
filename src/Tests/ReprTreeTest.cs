#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using DebugUtils.Unity.Repr;
using NUnit.Framework;
using Half = Unity.Mathematics.half;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Object = UnityEngine.Object;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;

namespace DebugUtils.Unity.Tests
{
    public class Student
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
        public List<string> Hobbies { get; set; } = new();
    }

    public class ReprTreeTest
    {
        [Test]
        public void TestReadme()
        {
            var student = new Student
            {
                Name = "Alice",
                Age = 30,
                Hobbies = new List<string> { "reading", "coding" }
            };
            var actualJson = JToken.Parse(json: student.ReprTree());

            Assert.AreEqual(expected: "Student", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);

            var nameNode = (JObject)actualJson[key: "Name"]!;
            Assert.AreEqual(expected: "string", actual: nameNode[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: 5,
                actual: nameNode[propertyName: "length"]!.Value<int>());
            Assert.AreEqual(expected: "Alice", actual: nameNode[propertyName: "value"]
              ?.ToString());

            var ageNode = (JObject)actualJson[key: "Age"]!;
            Assert.AreEqual(expected: "int", actual: ageNode[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "30", actual: ageNode[propertyName: "value"]
              ?.ToString());

            var hobbiesNode = (JObject)actualJson[key: "Hobbies"]!;
            Assert.AreEqual(expected: "List", actual: hobbiesNode[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: 2,
                actual: hobbiesNode[propertyName: "count"]!.Value<int>());

            var hobbiesValue = (JArray)hobbiesNode[propertyName: "value"]!;
            Assert.AreEqual(expected: "reading",
                actual: hobbiesValue[index: 0][key: "value"]!.Value<string>());
            Assert.AreEqual(expected: "coding",
                actual: hobbiesValue[index: 1][key: "value"]!.Value<string>());
        }

        [Test]
        public void TestExample()
        {
            var person = new Person(name: "John", age: 30);
            var actualJson = JToken.Parse(json: person.ReprTree());

            Assert.AreEqual(expected: "Person", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);

            var nameNode = (JObject)actualJson[key: "Name"]!;
            Assert.AreEqual(expected: "string", actual: nameNode[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: 4,
                actual: nameNode[propertyName: "length"]!.Value<int>());
            Assert.AreEqual(expected: "John", actual: nameNode[propertyName: "value"]
              ?.ToString());

            var ageNode = (JObject)actualJson[key: "Age"]!;
            Assert.AreEqual(expected: "int", actual: ageNode[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "30", actual: ageNode[propertyName: "value"]
              ?.ToString());
        }

        [Test]
        public void TestNullRepr()
        {
            var actualJson = JToken.Parse(json: ((string?)null).ReprTree());
            Assert.AreEqual(expected: "string", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.Null(anObject: actualJson[key: "length"]);
            Assert.Null(anObject: actualJson[key: "hashCode"]);
            var hasValues = actualJson[key: "value"]?.HasValues ?? false;
            Assert.IsFalse(condition: hasValues);
        }

        [Test]
        public void TestStringRepr()
        {
            var actualJson = JToken.Parse(json: "hello".ReprTree());
            Assert.AreEqual(expected: "string", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.AreEqual(expected: 5,
                actual: actualJson[key: "length"]!.Value<int>());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: "hello", actual: actualJson[key: "value"]
              ?.ToString());

            actualJson = JToken.Parse(json: "".ReprTree());
            Assert.AreEqual(expected: "string", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.AreEqual(expected: 0,
                actual: actualJson[key: "length"]!.Value<int>());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: "", actual: actualJson[key: "value"]
              ?.ToString());
        }

        [Test]
        public void TestCharRepr()
        {
            var actualJson = JToken.Parse(json: 'A'.ReprTree());
            Assert.AreEqual(expected: "char", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "struct", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.Null(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: "A", actual: actualJson[key: "value"]
              ?.ToString());
            Assert.AreEqual(expected: "0x0041", actual: actualJson[key: "unicodeValue"]
              ?.ToString());

            actualJson = JToken.Parse(json: '\n'.ReprTree());
            Assert.AreEqual(expected: "char", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "struct", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.Null(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: "\\n", actual: actualJson[key: "value"]
              ?.ToString());
            Assert.AreEqual(expected: "0x000A", actual: actualJson[key: "unicodeValue"]
              ?.ToString());

            actualJson = JToken.Parse(json: '\u007F'.ReprTree());
            Assert.AreEqual(expected: "char", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "struct", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.Null(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: "\\u007F", actual: actualJson[key: "value"]
              ?.ToString());
            Assert.AreEqual(expected: "0x007F", actual: actualJson[key: "unicodeValue"]
              ?.ToString());

            actualJson = JToken.Parse(json: '아'.ReprTree());
            Assert.AreEqual(expected: "char", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "struct", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.Null(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: "아", actual: actualJson[key: "value"]
              ?.ToString());
            Assert.AreEqual(expected: "0xC544", actual: actualJson[key: "unicodeValue"]
              ?.ToString());
        }

        [Test]
        public void TestBoolRepr()
        {
            var actualJson = JToken.Parse(json: true.ReprTree());
            Assert.AreEqual(expected: "bool", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "struct", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.Null(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: "true", actual: actualJson[key: "value"]
              ?.ToString());
        }

        [Test]
        public void TestDateTimeRepr()
        {
            var dateTime =
                new DateTime(year: 2025, month: 1, day: 1, hour: 0, minute: 0, second: 0);
            var localDateTime = DateTime.SpecifyKind(value: dateTime, kind: DateTimeKind.Local);
            var utcDateTime = DateTime.SpecifyKind(value: dateTime, kind: DateTimeKind.Utc);

            var actualJson = JToken.Parse(json: dateTime.ReprTree());
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "DateTime",
                [propertyName: "kind"] = "struct",
                [propertyName: "year"] = "2025",
                [propertyName: "month"] = "1",
                [propertyName: "day"] = "1",
                [propertyName: "hour"] = "0",
                [propertyName: "minute"] = "0",
                [propertyName: "second"] = "0",
                [propertyName: "millisecond"] = "0",
                [propertyName: "ticks"] = "638712864000000000",
                [propertyName: "timezone"] = "Unspecified"
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));

            actualJson = JToken.Parse(json: localDateTime.ReprTree());
            expectedJson = new JObject
            {
                [propertyName: "type"] = "DateTime",
                [propertyName: "kind"] = "struct",
                [propertyName: "year"] = "2025",
                [propertyName: "month"] = "1",
                [propertyName: "day"] = "1",
                [propertyName: "hour"] = "0",
                [propertyName: "minute"] = "0",
                [propertyName: "second"] = "0",
                [propertyName: "millisecond"] = "0",
                [propertyName: "ticks"] = "638712864000000000",
                [propertyName: "timezone"] = "Local"
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));

            actualJson = JToken.Parse(json: utcDateTime.ReprTree());
            expectedJson = new JObject
            {
                [propertyName: "type"] = "DateTime",
                [propertyName: "kind"] = "struct",
                [propertyName: "year"] = "2025",
                [propertyName: "month"] = "1",
                [propertyName: "day"] = "1",
                [propertyName: "hour"] = "0",
                [propertyName: "minute"] = "0",
                [propertyName: "second"] = "0",
                [propertyName: "millisecond"] = "0",
                [propertyName: "ticks"] = "638712864000000000",
                [propertyName: "timezone"] = "Utc"
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        [Test]
        public void TestTimeSpanRepr()
        {
            var timeSpan = TimeSpan.FromMinutes(value: 30);
            var actualJson = JToken.Parse(json: timeSpan.ReprTree());
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "TimeSpan",
                [propertyName: "kind"] = "struct",
                [propertyName: "day"] = "0",
                [propertyName: "hour"] = "0",
                [propertyName: "minute"] = "30",
                [propertyName: "second"] = "0",
                [propertyName: "millisecond"] = "0",
                [propertyName: "ticks"] = "18000000000",
                [propertyName: "isNegative"] = "false"
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        // Integer Types

        [TestCase(arg1: IntReprMode.Binary, arg2: "0b101010")]
        [TestCase(arg1: IntReprMode.Decimal, arg2: "42")]
        [TestCase(arg1: IntReprMode.Hex, arg2: "0x2A")]
        [TestCase(arg1: IntReprMode.HexBytes, arg2: "0x2A")]
        public void TestByteRepr(IntReprMode mode, string expectedValue)
        {
            var config = new ReprConfig(IntMode: mode);
            var actualJson = JToken.Parse(json: ((byte)42).ReprTree(config: config));
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "byte",
                [propertyName: "kind"] = "struct",
                [propertyName: "value"] = expectedValue
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }


        [TestCase(arg1: IntReprMode.Binary, arg2: "-0b101010")]
        [TestCase(arg1: IntReprMode.Decimal, arg2: "-42")]
        [TestCase(arg1: IntReprMode.Hex, arg2: "-0x2A")]
        [TestCase(arg1: IntReprMode.HexBytes, arg2: "0xFFFFFFD6")]
        public void TestIntRepr(IntReprMode mode, string expectedValue)
        {
            var config = new ReprConfig(IntMode: mode);
            var actualJson = JToken.Parse(json: (-42).ReprTree(config: config));
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "int",
                [propertyName: "kind"] = "struct",
                [propertyName: "value"] = expectedValue
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        [Test]
        public void TestBigIntRepr()
        {
            var config = new ReprConfig(IntMode: IntReprMode.Decimal);
            var actualJson =
                JToken.Parse(json: new BigInteger(value: -42).ReprTree(config: config));
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "BigInteger",
                [propertyName: "kind"] = "struct",
                [propertyName: "value"] = "-42"
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        // Floating Point Types
        [Test]
        public void TestFloatRepr_Exact()
        {
            var config = new ReprConfig(FloatMode: FloatReprMode.Exact);
            var value = Single.Parse(s: "3.1415926535");
            var actualJson = JToken.Parse(json: value.ReprTree(config: config));
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "float",
                [propertyName: "kind"] = "struct",
                [propertyName: "value"] = "3.1415927410125732421875E+000"
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        [Test]
        public void TestDoubleRepr_Round()
        {
            var config = new ReprConfig(FloatMode: FloatReprMode.Round, FloatPrecision: 5);
            var value = Double.Parse(s: "3.1415926535");
            var actualJson = JToken.Parse(json: value.ReprTree(config: config));
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "double",
                [propertyName: "kind"] = "struct",
                [propertyName: "value"] = "3.14159"
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        [Test]
        public void TestDecimalRepr_RawHex()
        {
            var config = new ReprConfig(FloatMode: FloatReprMode.HexBytes);
            var value = 3.1415926535897932384626433832795m;
            var actualJson = JToken.Parse(json: value.ReprTree(config: config));
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "decimal",
                [propertyName: "kind"] = "struct",
                [propertyName: "value"] = "0x001C00006582A5360B14388541B65F29"
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        [Test]
        public void TestHalfRepr_Scientific()
        {
            var config = new ReprConfig(FloatMode: FloatReprMode.Scientific, FloatPrecision: 5);
            var value = new Half(v: 3.14159);
            var actualJson = JToken.Parse(json: value.ReprTree(config: config));
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "half",
                [propertyName: "kind"] = "struct",
                [propertyName: "value"] = "3.14063E+000"
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        [Test]
        public void TestHalfRepr_BitField()
        {
            var config = new ReprConfig(FloatMode: FloatReprMode.BitField);
            var value = new Half(v: 3.14159);
            var actualJson = JToken.Parse(json: value.ReprTree(config: config));
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "half",
                [propertyName: "kind"] = "struct",
                [propertyName: "value"] = "0|10000|1001001000"
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        [Test]
        public void TestFloatRepr_SpecialValues()
        {
            var actualJson = JToken.Parse(json: Single.NaN.ReprTree());
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "float",
                [propertyName: "kind"] = "struct",
                [propertyName: "value"] = "Quiet NaN"
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));

            actualJson = JToken.Parse(json: Single.PositiveInfinity.ReprTree());
            expectedJson = new JObject
            {
                [propertyName: "type"] = "float",
                [propertyName: "kind"] = "struct",
                [propertyName: "value"] = "Infinity"
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));

            actualJson = JToken.Parse(json: Single.NegativeInfinity.ReprTree());
            expectedJson = new JObject
            {
                [propertyName: "type"] = "float",
                [propertyName: "kind"] = "struct",
                [propertyName: "value"] = "-Infinity"
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        [Test]
        public void TestDoubleRepr_SpecialValues()
        {
            var actualJson = JToken.Parse(json: Double.NaN.ReprTree());
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "double",
                [propertyName: "kind"] = "struct",
                [propertyName: "value"] = "Quiet NaN"
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));

            actualJson = JToken.Parse(json: Double.PositiveInfinity.ReprTree());
            expectedJson = new JObject
            {
                [propertyName: "type"] = "double",
                [propertyName: "kind"] = "struct",
                [propertyName: "value"] = "Infinity"
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));

            actualJson = JToken.Parse(json: Double.NegativeInfinity.ReprTree());
            expectedJson = new JObject
            {
                [propertyName: "type"] = "double",
                [propertyName: "kind"] = "struct",
                [propertyName: "value"] = "-Infinity"
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        #if NET5_0_OR_GREATER
    [Test]
    public void TestHalfRepr_SpecialValues()
    {
        var actualJson = JToken.Parse(json: Half.NaN.ReprTree());
        var expectedJson = new JObject
        {
            ["type"] = "Half",
            ["kind"] = "struct",
            ["value"] = "Quiet NaN"
        };
        Assert.True(condition: JToken.DeepEquals(actualJson,  expectedJson));

        actualJson = JToken.Parse(json: Half.PositiveInfinity.ReprTree());
        expectedJson = new JObject
        {
            ["type"] = "Half",
            ["kind"] = "struct",
            ["value"] = "Infinity"
        };
        Assert.True(condition: JToken.DeepEquals(actualJson,  expectedJson));

        actualJson = JToken.Parse(json: Half.NegativeInfinity.ReprTree());
        expectedJson = new JObject
        {
            ["type"] = "Half",
            ["kind"] = "struct",
            ["value"] = "-Infinity"
        };
        Assert.True(condition: JToken.DeepEquals(actualJson,  expectedJson));
    }
        #endif

        #if NET7_0_OR_GREATER
        [TestCase(IntReprMode.Binary, "Int128(-0b10000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000)")]
        [TestCase(IntReprMode.Decimal, "Int128(-170141183460469231731687303715884105728)")]
        [TestCase(IntReprMode.Hex, "Int128(-0x80000000000000000000000000000000)")]
        [TestCase(IntReprMode.HexBytes, "Int128(0x80000000000000000000000000000000)")]
        public void TestInt128Repr(IntReprMode mode, string expectedValue)
        {
            var i128 = Int128.MinValue;
            var config = new ReprConfig(IntMode: mode);
            var actualJson = JToken.Parse(json: i128.ReprTree(config: config));
            var expectedJson = new JObject
            {
                ["type"] = "Int128",
                ["kind"] = "struct",
                ["value"] = expectedValue
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        [TestCase(IntReprMode.Binary, "Int128(0b1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111)")]
        [TestCase(IntReprMode.Decimal, "Int128(170141183460469231731687303715884105727)")]
        [TestCase(IntReprMode.Hex, "Int128(0x7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF)")]
        [TestCase(IntReprMode.HexBytes, "Int128(0x7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF)")]
        public void TestInt128Repr2(IntReprMode mode, string expectedValue)
        {
            var i128 = Int128.MaxValue;
            var config = new ReprConfig(IntMode: mode);
            var actualJson = JToken.Parse(json: i128.ReprTree(config: config));
            var expectedJson = new JObject
            {
                ["type"] = "Int128",
                ["kind"] = "struct",
                ["value"] = expectedValue
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }
        #endif

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
        public void TestCustomStructRepr_NoToString()
        {
            var point = new Point { X = 10, Y = 20 };
            var actualJson = JToken.Parse(json: point.ReprTree());
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "Point",
                [propertyName: "kind"] = "struct",
                [propertyName: "X"] = new JObject
                {
                    [propertyName: "type"] = "int", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "10"
                },
                [propertyName: "Y"] = new JObject
                {
                    [propertyName: "type"] = "int", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "20"
                }
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        [Test]
        public void TestCustomStructRepr_WithToString()
        {
            var custom = new CustomStruct { Name = "test", Value = 42 };
            var actualJson = JToken.Parse(json: custom.ReprTree());

            Assert.AreEqual(expected: "CustomStruct", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "struct", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.Null(anObject: actualJson[key: "hashCode"]);

            var nameNode = actualJson[key: "Name"]!;
            Assert.AreEqual(expected: "string", actual: nameNode[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: nameNode[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: nameNode[key: "hashCode"]);
            Assert.AreEqual(expected: 4,
                actual: nameNode[key: "length"]!.Value<int>());
            Assert.AreEqual(expected: "test", actual: nameNode[key: "value"]
              ?.ToString());

            var valueNode = actualJson[key: "Value"]!;
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "int", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "42"
                }, t2: valueNode));
        }

        [Test]
        public void TestClassRepr_WithToString()
        {
            var person = new Person(name: "Alice", age: 30);
            var actualJson = JToken.Parse(json: person.ReprTree());

            Assert.AreEqual(expected: "Person", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);

            var nameNode = (JObject)actualJson[key: "Name"]!;
            Assert.AreEqual(expected: "string", actual: nameNode[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: nameNode[propertyName: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: nameNode[propertyName: "hashCode"]);
            Assert.AreEqual(expected: 5,
                actual: nameNode[propertyName: "length"]!.Value<int>());
            Assert.AreEqual(expected: "Alice", actual: nameNode[propertyName: "value"]
              ?.ToString());

            var ageNode = (JObject)actualJson[key: "Age"]!;
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "int", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "30"
                }, t2: ageNode));
        }

        [Test]
        public void TestClassRepr_NoToString()
        {
            var noToString = new NoToStringClass(data: "data", number: 123);
            var actualJson = JToken.Parse(json: noToString.ReprTree());

            Assert.AreEqual(expected: "NoToStringClass", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);

            var dataNode = (JObject)actualJson[key: "Data"]!;
            Assert.AreEqual(expected: "string", actual: dataNode[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: dataNode[propertyName: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: dataNode[propertyName: "hashCode"]);
            Assert.AreEqual(expected: 4,
                actual: dataNode[propertyName: "length"]!.Value<int>());
            Assert.AreEqual(expected: "data", actual: dataNode[propertyName: "value"]
              ?.ToString());

            var numberNode = (JObject)actualJson[key: "Number"]!;
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "int", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "123"
                },
                t2: numberNode));
        }

        [Test]
        public void TestRecordRepr()
        {
            var settings = new TestSettings(EquipmentName: "Printer",
                EquipmentSettings: new Dictionary<string, double>
                    { [key: "Temp (C)"] = 200.0, [key: "PrintSpeed (mm/s)"] = 30.0 });
            var actualJson = JToken.Parse(json: settings.ReprTree());

            Assert.AreEqual(expected: "TestSettings", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "record class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);

            var equipmentName = (JObject)actualJson[key: "EquipmentName"]!;
            Assert.AreEqual(expected: "string", actual: equipmentName[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: equipmentName[propertyName: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: equipmentName[propertyName: "hashCode"]);
            Assert.AreEqual(expected: 7,
                actual: equipmentName[propertyName: "length"]!.Value<int>());
            Assert.AreEqual(expected: "Printer", actual: equipmentName[propertyName: "value"]
              ?.ToString());

            var equipmentSettings = (JObject)actualJson[key: "EquipmentSettings"]!;
            Assert.AreEqual(expected: "Dictionary", actual: equipmentSettings[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: equipmentSettings[propertyName: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: equipmentSettings[propertyName: "hashCode"]);
            Assert.AreEqual(expected: 2,
                actual: equipmentSettings[propertyName: "count"]!.Value<int>());

            var settingsArray = (JArray)equipmentSettings[propertyName: "value"]!;
            Assert.AreEqual(expected: 2, actual: settingsArray.Count);

            // Since dictionary order isn't guaranteed, we check for the presence of keys
            var tempSetting =
                settingsArray.FirstOrDefault(predicate: s =>
                    s![key: "key"]![key: "value"]!.ToString() == "Temp (C)");
            Assert.NotNull(anObject: tempSetting);
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "double", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "200"
                },
                t2: tempSetting[key: "value"]));

            var speedSetting =
                settingsArray.FirstOrDefault(predicate: s =>
                    s![key: "key"]![key: "value"]!.ToString() ==
                    "PrintSpeed (mm/s)");
            Assert.NotNull(anObject: speedSetting);
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "double", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "30"
                },
                t2: speedSetting[key: "value"]));
        }

        [Test]
        public void TestEnumRepr()
        {
            var actualJson = JToken.Parse(json: Colors.GREEN.ReprTree());
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "Colors",
                [propertyName: "kind"] = "enum",
                [propertyName: "name"] = "GREEN",
                [propertyName: "value"] = new JObject
                {
                    [propertyName: "type"] = "int", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "1"
                }
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
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

        [Test]
        public void TestNullableStructRepr()
        {
            var actualJson = JToken.Parse(json: ((int?)123).ReprTree());
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "int?",
                [propertyName: "kind"] = "struct",
                [propertyName: "value"] = "123"
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));

            actualJson = JToken.Parse(json: ((int?)null).ReprTree());
            expectedJson = new JObject
            {
                [propertyName: "type"] = "int?",
                [propertyName: "kind"] = "struct",
                [propertyName: "value"] = null
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        [Test]
        public void TestNullableClassRepr()
        {
            var actualJson = JToken.Parse(json: ((List<int>?)null).ReprTree());
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "List",
                [propertyName: "kind"] = "class",
                [propertyName: "value"] = null
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
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
        public void TestGuidRepr()
        {
            var guid = Guid.NewGuid();
            var actualJson = JToken.Parse(json: guid.ReprTree());
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "Guid",
                [propertyName: "kind"] = "struct",
                [propertyName: "value"] = guid.ToString()
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        [Test]
        public void TestTimeSpanRepr_Negative()
        {
            var config = new ReprConfig(IntMode: IntReprMode.Decimal);
            var timeSpan = TimeSpan.FromMinutes(value: -30);
            var actualJson = JToken.Parse(json: timeSpan.ReprTree(config: config));
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "TimeSpan",
                [propertyName: "kind"] = "struct",
                [propertyName: "day"] = "0",
                [propertyName: "hour"] = "0",
                [propertyName: "minute"] = "30",
                [propertyName: "second"] = "0",
                [propertyName: "millisecond"] = "0",
                [propertyName: "ticks"] = "18000000000",
                [propertyName: "isNegative"] = "true"
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        [Test]
        public void TestTimeSpanRepr_Zero()
        {
            var config = new ReprConfig(IntMode: IntReprMode.Decimal);
            var timeSpan = TimeSpan.Zero;
            var actualJson = JToken.Parse(json: timeSpan.ReprTree(config: config));
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "TimeSpan",
                [propertyName: "kind"] = "struct",
                [propertyName: "day"] = "0",
                [propertyName: "hour"] = "0",
                [propertyName: "minute"] = "0",
                [propertyName: "second"] = "0",
                [propertyName: "millisecond"] = "0",
                [propertyName: "ticks"] = "0",
                [propertyName: "isNegative"] = "false"
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        [Test]
        public void TestTimeSpanRepr_Positive()
        {
            var config = new ReprConfig(IntMode: IntReprMode.Decimal);
            var timeSpan = TimeSpan.FromMinutes(value: 30);
            var actualJson = JToken.Parse(json: timeSpan.ReprTree(config: config));
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "TimeSpan",
                [propertyName: "kind"] = "struct",
                [propertyName: "day"] = "0",
                [propertyName: "hour"] = "0",
                [propertyName: "minute"] = "30",
                [propertyName: "second"] = "0",
                [propertyName: "millisecond"] = "0",
                [propertyName: "ticks"] = "18000000000",
                [propertyName: "isNegative"] = "false"
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }


        [Test]
        public void TestDateTimeOffsetRepr_WithOffset()
        {
            var dateTimeOffset =
                new DateTimeOffset(year: 2025, month: 1, day: 1, hour: 0, minute: 0, second: 0,
                    offset: TimeSpan.FromHours(value: 1));
            var actualJson = JToken.Parse(json: dateTimeOffset.ReprTree());
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "DateTimeOffset",
                [propertyName: "kind"] = "struct",
                [propertyName: "year"] = "2025",
                [propertyName: "month"] = "1",
                [propertyName: "day"] = "1",
                [propertyName: "hour"] = "0",
                [propertyName: "minute"] = "0",
                [propertyName: "second"] = "0",
                [propertyName: "millisecond"] = "0",
                [propertyName: "ticks"] = "638712864000000000",
                [propertyName: "offset"] = new JObject
                {
                    [propertyName: "type"] = "TimeSpan",
                    [propertyName: "kind"] = "struct",
                    [propertyName: "day"] = "0",
                    [propertyName: "hour"] = "1",
                    [propertyName: "minute"] = "0",
                    [propertyName: "second"] = "0",
                    [propertyName: "millisecond"] = "0",
                    [propertyName: "ticks"] = "36000000000",
                    [propertyName: "isNegative"] = "false"
                }
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        [Test]
        public void TestDateTimeOffsetRepr_WithNegativeOffset()
        {
            var dateTimeOffset =
                new DateTimeOffset(dateTime: new DateTime(year: 2025, month: 1, day: 1),
                    offset: TimeSpan.FromHours(value: -1));
            var actualJson = JToken.Parse(json: dateTimeOffset.ReprTree());
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "DateTimeOffset",
                [propertyName: "kind"] = "struct",
                [propertyName: "year"] = "2025",
                [propertyName: "month"] = "1",
                [propertyName: "day"] = "1",
                [propertyName: "hour"] = "0",
                [propertyName: "minute"] = "0",
                [propertyName: "second"] = "0",
                [propertyName: "millisecond"] = "0",
                [propertyName: "ticks"] = "638712864000000000",
                [propertyName: "offset"] = new JObject
                {
                    [propertyName: "type"] = "TimeSpan",
                    [propertyName: "kind"] = "struct",
                    [propertyName: "day"] = "0",
                    [propertyName: "hour"] = "1",
                    [propertyName: "minute"] = "0",
                    [propertyName: "second"] = "0",
                    [propertyName: "millisecond"] = "0",
                    [propertyName: "ticks"] = "36000000000",
                    [propertyName: "isNegative"] = "true"
                }
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        [Test]
        public void TestDateTimeOffsetRepr()
        {
            var dateTimeOffset = new DateTimeOffset(dateTime: new DateTime(year: 2025, month: 1,
                day: 1, hour: 0, minute: 0, second: 0, kind: DateTimeKind.Utc));
            var actualJson = JToken.Parse(json: dateTimeOffset.ReprTree());
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "DateTimeOffset",
                [propertyName: "kind"] = "struct",
                [propertyName: "year"] = "2025",
                [propertyName: "month"] = "1",
                [propertyName: "day"] = "1",
                [propertyName: "hour"] = "0",
                [propertyName: "minute"] = "0",
                [propertyName: "second"] = "0",
                [propertyName: "millisecond"] = "0",
                [propertyName: "ticks"] = "638712864000000000",
                [propertyName: "offset"] = new JObject
                {
                    [propertyName: "type"] = "TimeSpan",
                    [propertyName: "kind"] = "struct",
                    [propertyName: "day"] = "0",
                    [propertyName: "hour"] = "0",
                    [propertyName: "minute"] = "0",
                    [propertyName: "second"] = "0",
                    [propertyName: "millisecond"] = "0",
                    [propertyName: "ticks"] = "0",
                    [propertyName: "isNegative"] = "false"
                }
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

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

        public static int Add(int a, int b)
        {
            return a + b;
        }

        internal static long Add2(int a)
        {
            return a;
        }

        private T Add3<T>(T a)
        {
            return a;
        }

        private static void Add4(in int a, out int b)
        {
            b = a + 1;
        }

        private async Task<int> Lambda(int a)
        {
            // Added delay for truly async function.
            // However, this would not do much because it is only used for testing purposes
            // and not being called, only investigated the metadata of it.
            await Task.Delay(millisecondsDelay: 1);
            return a;
        }

        public delegate void Add4Delegate(in int a, out int b);

        [Test]
        public void TestFunction()
        {
            var Add5 = new Func<int, int>((a) => a + 1);
            var a = new Func<int, int, int>(Add);
            var b = new Func<int, long>(Add2);
            var c = new Func<short, short>(Add3);
            Add4Delegate d = Add4;
            var e = new Func<int, Task<int>>(Lambda);

            var nullJsonObject = new JObject
            {
                [propertyName: "type"] = "object", [propertyName: "kind"] = "class",
                [propertyName: "value"] = null
            };

            var actualJson = JToken.Parse(json: Add5.ReprTree());
            Assert.AreEqual(expected: "Function", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            var props = (JObject)actualJson[key: "properties"]!;
            Assert.AreEqual(expected: "Lambda", actual: props[propertyName: "name"]
              ?.ToString());
            Assert.AreEqual(expected: "int", actual: props[propertyName: "returnType"]
              ?.ToString());
            Assert.True(condition: JToken.DeepEquals(t1: new JArray(content: "internal"),
                t2: props[propertyName: "modifiers"]));
            var parameters = (JArray)props[propertyName: "parameters"]!;
            Assert.AreEqual(expected: 1, actual: parameters.Count);
            Assert.AreEqual(expected: "int", actual: parameters[index: 0][key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "a", actual: parameters[index: 0][key: "name"]
              ?.ToString());
            Assert.AreEqual(expected: "", actual: parameters[index: 0][key: "modifier"]
              ?.ToString());
            Assert.True(condition: JToken.DeepEquals(t1: nullJsonObject,
                t2: parameters[index: 0][key: "defaultValue"]!));

            actualJson = JToken.Parse(json: a.ReprTree());
            Assert.AreEqual(expected: "Function", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            props = (JObject)actualJson[key: "properties"]!;
            Assert.AreEqual(expected: "Add", actual: props[propertyName: "name"]
              ?.ToString());
            Assert.AreEqual(expected: "int", actual: props[propertyName: "returnType"]
              ?.ToString());
            Assert.True(condition: JToken.DeepEquals(t1: new JArray("public", "static"),
                t2: props[propertyName: "modifiers"]));
            parameters = (JArray)props[propertyName: "parameters"]!;
            Assert.AreEqual(expected: "int", actual: parameters[index: 0][key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "a", actual: parameters[index: 0][key: "name"]
              ?.ToString());
            Assert.AreEqual(expected: "", actual: parameters[index: 0][key: "modifier"]
              ?.ToString());
            Assert.True(condition: JToken.DeepEquals(t1: nullJsonObject,
                t2: parameters[index: 0][key: "defaultValue"]!));
            Assert.AreEqual(expected: "int", actual: parameters[index: 1][key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "b", actual: parameters[index: 1][key: "name"]
              ?.ToString());
            Assert.AreEqual(expected: "", actual: parameters[index: 1][key: "modifier"]
              ?.ToString());
            Assert.True(condition: JToken.DeepEquals(t1: nullJsonObject,
                t2: parameters[index: 1][key: "defaultValue"]!));

            actualJson = JToken.Parse(json: b.ReprTree());
            Assert.AreEqual(expected: "Function", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            props = (JObject)actualJson[key: "properties"]!;
            Assert.AreEqual(expected: "Add2", actual: props[propertyName: "name"]
              ?.ToString());
            Assert.AreEqual(expected: "long", actual: props[propertyName: "returnType"]
              ?.ToString());
            Assert.True(condition: JToken.DeepEquals(t1: new JArray("internal", "static"),
                t2: props[propertyName: "modifiers"]));
            parameters = (JArray)props[propertyName: "parameters"]!;
            Assert.AreEqual(expected: 1, actual: parameters.Count);
            Assert.AreEqual(expected: "int", actual: parameters[index: 0][key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "a", actual: parameters[index: 0][key: "name"]
              ?.ToString());
            Assert.AreEqual(expected: "", actual: parameters[index: 0][key: "modifier"]
              ?.ToString());
            Assert.True(condition: JToken.DeepEquals(t1: nullJsonObject,
                t2: parameters[index: 0][key: "defaultValue"]!));

            actualJson = JToken.Parse(json: c.ReprTree());
            Assert.AreEqual(expected: "Function", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            props = (JObject)actualJson[key: "properties"]!;
            Assert.AreEqual(expected: "Add3", actual: props[propertyName: "name"]
              ?.ToString());
            Assert.AreEqual(expected: "short", actual: props[propertyName: "returnType"]
              ?.ToString());
            Assert.True(condition: JToken.DeepEquals(t1: new JArray("private", "generic"),
                t2: props[propertyName: "modifiers"]));
            parameters = (JArray)props[propertyName: "parameters"]!;
            Assert.AreEqual(expected: 1, actual: parameters.Count);
            Assert.AreEqual(expected: "short", actual: parameters[index: 0][key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "a", actual: parameters[index: 0][key: "name"]
              ?.ToString());
            Assert.AreEqual(expected: "", actual: parameters[index: 0][key: "modifier"]
              ?.ToString());
            Assert.True(condition: JToken.DeepEquals(t1: nullJsonObject,
                t2: parameters[index: 0][key: "defaultValue"]!));

            actualJson = JToken.Parse(json: d.ReprTree());
            Assert.AreEqual(expected: "Function", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            props = (JObject)actualJson[key: "properties"]!;
            Assert.AreEqual(expected: "Add4", actual: props[propertyName: "name"]
              ?.ToString());
            Assert.AreEqual(expected: "void", actual: props[propertyName: "returnType"]
              ?.ToString());
            Assert.True(condition: JToken.DeepEquals(t1: new JArray("private", "static"),
                t2: props[propertyName: "modifiers"]));
            parameters = (JArray)props[propertyName: "parameters"]!;
            Assert.AreEqual(expected: 2, actual: parameters.Count);
            Assert.AreEqual(expected: "ref int", actual: parameters[index: 0][key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "a", actual: parameters[index: 0][key: "name"]
              ?.ToString());
            Assert.AreEqual(expected: "in", actual: parameters[index: 0][key: "modifier"]
              ?.ToString());
            Assert.True(condition: JToken.DeepEquals(t1: nullJsonObject,
                t2: parameters[index: 0][key: "defaultValue"]!));
            Assert.AreEqual(expected: "ref int", actual: parameters[index: 1][key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "b", actual: parameters[index: 1][key: "name"]
              ?.ToString());
            Assert.AreEqual(expected: "out", actual: parameters[index: 1][key: "modifier"]
              ?.ToString());
            Assert.True(condition: JToken.DeepEquals(t1: nullJsonObject,
                t2: parameters[index: 1][key: "defaultValue"]!));

            actualJson = JToken.Parse(json: e.ReprTree());
            Assert.AreEqual(expected: "Function", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            props = (JObject)actualJson[key: "properties"]!;
            Assert.AreEqual(expected: "Lambda", actual: props[propertyName: "name"]
              ?.ToString());
            Assert.AreEqual(expected: "Task<int>", actual: props[propertyName: "returnType"]
              ?.ToString());
            Assert.True(condition: JToken.DeepEquals(t1: new JArray("private", "async"),
                t2: props[propertyName: "modifiers"]));
            parameters = (JArray)props[propertyName: "parameters"]!;
            Assert.AreEqual(expected: 1, actual: parameters.Count);
            Assert.AreEqual(expected: "int", actual: parameters[index: 0][key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "a", actual: parameters[index: 0][key: "name"]
              ?.ToString());
            Assert.AreEqual(expected: "", actual: parameters[index: 0][key: "modifier"]
              ?.ToString());
            Assert.True(condition: JToken.DeepEquals(t1: nullJsonObject,
                t2: parameters[index: 0][key: "defaultValue"]!));
        }

        [Test]
        public void TestObjectReprTree()
        {
            var data = new { Name = "Alice", Age = 30 };
            var actualJsonNode = JToken.Parse(json: data.ReprTree());

            Assert.AreEqual(expected: "Anonymous", actual: actualJsonNode[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJsonNode[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJsonNode[key: "hashCode"]);

            var nameNode = (JObject)actualJsonNode[key: "Name"]!;
            Assert.AreEqual(expected: "string", actual: nameNode[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: nameNode[propertyName: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: nameNode[propertyName: "hashCode"]);
            Assert.AreEqual(expected: 5,
                actual: nameNode[propertyName: "length"]!.Value<int>());
            Assert.AreEqual(expected: "Alice", actual: nameNode[propertyName: "value"]
              ?.ToString());

            var ageNode = (JObject)actualJsonNode[key: "Age"]!;
            Assert.True(condition: JToken.DeepEquals(
                t1: new JObject
                {
                    [propertyName: "type"] = "int", [propertyName: "kind"] = "struct",
                    [propertyName: "value"] = "30"
                }, t2: ageNode));
        }

        [Test]
        public void TestCircularReprTree()
        {
            List<object> a = new();
            a.Add(item: a);
            var actualJsonString = a.ReprTree();

            // Parse the JSON to verify structure
            var json = JToken.Parse(json: actualJsonString);

            // Verify top-level structure
            Assert.AreEqual(expected: "List", actual: json[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: 1, actual: json[key: "count"]!.Value<int>());

            // Verify circular reference structure
            var firstElement = json[key: "value"]![key: 0]!;
            Assert.AreEqual(expected: "CircularReference",
                actual: firstElement[key: "type"]
                  ?.ToString());
            Assert.AreEqual(expected: "List",
                actual: firstElement[key: "target"]![key: "type"]
                  ?.ToString());
            Assert.IsTrue(condition: firstElement[key: "target"]![key: "hashCode"]
                                   ?.ToString()
                                    .StartsWith(value: "0x"));
        }

        [Test]
        public void TestReprConfig_MaxDepth_ReprTree()
        {
            var nestedList = new List<object>
                { 1, new List<object> { 2, new List<object> { 3 } } };
            var config = new ReprConfig(MaxDepth: 1);
            var actualJson = JToken.Parse(json: nestedList.ReprTree(config: config));
            Assert.AreEqual(expected: "List", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: 2,
                actual: actualJson[key: "count"]!.Value<int>());
            Assert.AreEqual(expected: "int",
                actual: actualJson[key: "value"]![key: 0]![key: "type"]
                  ?.ToString());
            Assert.AreEqual(expected: "struct",
                actual: actualJson[key: "value"]![key: 0]![key: "kind"]
                  ?.ToString());
            Assert.AreEqual(expected: "1",
                actual: actualJson[key: "value"]![key: 0]![key: "value"]
                  ?.ToString());
            Assert.AreEqual(expected: "List",
                actual: actualJson[key: "value"]![key: 1]![key: "type"]
                  ?.ToString());
            Assert.AreEqual(expected: "class",
                actual: actualJson[key: "value"]![key: 1]![key: "kind"]
                  ?.ToString());
            Assert.AreEqual(expected: "true",
                actual: actualJson[key: "value"]![key: 1]![
                        key: "maxDepthReached"]
                  ?.ToString());
            Assert.AreEqual(expected: 1,
                actual: actualJson[key: "value"]![key: 1]![key: "depth"]!
                   .Value<int>());

            config = new ReprConfig(MaxDepth: 0);
            actualJson = JToken.Parse(json: nestedList.ReprTree(config: config));
            Assert.AreEqual(expected: "List", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.AreEqual(expected: "true", actual: actualJson[key: "maxDepthReached"]
              ?.ToString());
            Assert.AreEqual(expected: 0,
                actual: actualJson[key: "depth"]!.Value<int>());
        }

        [Test]
        public void TestReprConfig_MaxCollectionItems_ReprTree()
        {
            var list = new List<int> { 1, 2, 3, 4, 5 };
            var config = new ReprConfig(MaxElementsPerCollection: 3);
            var actualJson = JToken.Parse(json: list.ReprTree(config: config));
            Assert.AreEqual(expected: "List", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: 5,
                actual: actualJson[key: "count"]!.Value<int>());
            Assert.AreEqual(expected: 4, actual: ((JArray)actualJson[key: "value"]!)
               .Count);
            Assert.AreEqual(expected: "int",
                actual: actualJson[key: "value"]![key: 0]![key: "type"]
                  ?.ToString());
            Assert.AreEqual(expected: "int",
                actual: actualJson[key: "value"]![key: 1]![key: "type"]
                  ?.ToString());
            Assert.AreEqual(expected: "int",
                actual: actualJson[key: "value"]![key: 2]![key: "type"]
                  ?.ToString());
            Assert.AreEqual(expected: "... (2 more items)",
                actual: actualJson[key: "value"]![key: 3]
                  ?.ToString());

            config = new ReprConfig(MaxElementsPerCollection: 0);
            actualJson = JToken.Parse(json: list.ReprTree(config: config));
            Assert.AreEqual(expected: "List", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "... (5 more items)",
                actual: actualJson[key: "value"]![key: 0]
                  ?.ToString());
        }

        [Test]
        public void TestReprConfig_MaxStringLength_ReprTree()
        {
            var longString = "This is a very long string that should be truncated.";
            var config = new ReprConfig(MaxStringLength: 10);
            var actualJson = JToken.Parse(json: longString.ReprTree(config: config));
            Assert.AreEqual(expected: "string", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "This is a ... (42 more letters)",
                actual: actualJson[key: "value"]
                  ?.ToString());

            config = new ReprConfig(MaxStringLength: 0);
            actualJson = JToken.Parse(json: longString.ReprTree(config: config));
            Assert.AreEqual(expected: "string", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "... (52 more letters)",
                actual: actualJson[key: "value"]
                  ?.ToString());
        }

        [Test]
        public void TestReprConfig_ShowNonPublicProperties_ReprTree()
        {
            var classified = new ClassifiedData(writer: "writer", data: "secret");
            var config = new ReprConfig(ShowNonPublicProperties: false);
            var actualJson = JToken.Parse(json: classified.ReprTree(config: config));
            Assert.NotNull(anObject: actualJson);
            Assert.AreEqual(expected: "ClassifiedData", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);

            var writerNode = (JObject)actualJson[key: "Writer"]!;
            Assert.AreEqual(expected: "string", actual: writerNode[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: 6,
                actual: writerNode[propertyName: "length"]!.Value<int>());
            Assert.AreEqual(expected: "writer", actual: writerNode[propertyName: "value"]
              ?.ToString());


            config = new ReprConfig(ShowNonPublicProperties: true);
            actualJson = JToken.Parse(json: classified.ReprTree(config: config));
            Assert.NotNull(anObject: actualJson);
            Assert.AreEqual(expected: "ClassifiedData", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);


            writerNode = (JObject)actualJson[key: "Writer"]!;
            Assert.AreEqual(expected: "string", actual: writerNode[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: 6,
                actual: writerNode[propertyName: "length"]!.Value<int>());
            Assert.AreEqual(expected: "writer", actual: writerNode[propertyName: "value"]
              ?.ToString());

            var secretNode = actualJson[key: "private_Data"];
            Assert.NotNull(anObject: secretNode);
            Assert.AreEqual(expected: "string", actual: secretNode![key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: 6,
                actual: secretNode[key: "length"]!.Value<int>());
            Assert.AreEqual(expected: "secret", actual: secretNode[key: "value"]
              ?.ToString());
        }
    }
}