#nullable enable
using System;
using System.Numerics;
using DebugUtils.Unity.Repr;
using NUnit.Framework;
using Half = Unity.Mathematics.half;
using Newtonsoft.Json.Linq;

namespace DebugUtils.Unity.Tests
{

    public class NumericFormatterTreeTests
    {
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
    }
}