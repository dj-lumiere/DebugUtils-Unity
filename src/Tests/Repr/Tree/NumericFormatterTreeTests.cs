#nullable enable
using DebugUtils.Unity.Repr;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using System.Numerics;
using System;

namespace DebugUtils.Unity.Tests
{
    public class NumericFormatterTreeTests
    {
        [TestCase(arg1: "B", arg2: "0b101010")]
        [TestCase(arg1: "D", arg2: "42")]
        [TestCase(arg1: "X", arg2: "0x2A")]
        public void TestByteRepr(string format, string expectedValue)
        {
            var config = ReprConfig.Configure()
                                   .IntFormat(format: format)
                                   .Build();
            var actualJson = JToken.Parse(json: ((byte)42).ReprTree(config: config));
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "byte",
                [propertyName: "kind"] = "struct",
                [propertyName: "value"] = expectedValue
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        [TestCase(arg1: "B", arg2: "-0b101010")]
        [TestCase(arg1: "D", arg2: "-42")]
        [TestCase(arg1: "X", arg2: "-0x2A")]
        public void TestIntRepr(string format, string expectedValue)
        {
            var config = ReprConfig.Configure()
                                   .IntFormat(format: format)
                                   .Build();
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
            var config = ReprConfig.Configure()
                                   .Build();
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
            var config = ReprConfig.Configure()
                                   .FloatFormat(format: "EX")
                                   .Build();
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
            var config = ReprConfig.Configure()
                                   .FloatFormat(format: "F5")
                                   .Build();
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
        public void TestDecimalRepr_HexPower()
        {
            var config = ReprConfig.Configure()
                                   .FloatFormat(format: "HP")
                                   .Build();
            var value = 3.1415926535897932384626433832795m;
            var actualJson = JToken.Parse(json: value.ReprTree(config: config));
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "decimal",
                [propertyName: "kind"] = "struct",
                [propertyName: "value"] = "0x6582A536_0B143885_41B65F29p10-028"
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
                [propertyName: "value"] = "QuietNaN(0x400000)"
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
                [propertyName: "value"] = "QuietNaN(0x8000000000000)"
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
    [Fact]
    public void TestHalfRepr_Scientific()
    {
        var config = ReprConfig.Configure().FloatFormat("E5").Build();
        var value = Half.Parse(s: "3.14159");
        var actualJson = JsonNode.Parse(json: value.ReprTree(config: config));
        var expectedJson = new JsonObject
        {
            [propertyName: "type"] = "Half",
            [propertyName: "kind"] = "struct",
            [propertyName: "value"] = "3.14062E+000"
        };
        Assert.True(condition: JsonNode.DeepEquals(node1: actualJson, node2: expectedJson));
    }
    [Fact]
    public void TestHalfRepr_HexPower()
    {
        var config = ReprConfig.Configure().FloatFormat("HP").Build();
        var value = Half.Parse(s: "3.14159");
        var actualJson = JsonNode.Parse(json: value.ReprTree(config: config));
        var expectedJson = new JsonObject
        {
            [propertyName: "type"] = "Half",
            [propertyName: "kind"] = "struct",
            [propertyName: "value"] = "0x1.920p+001"
        };
        Assert.True(condition: JsonNode.DeepEquals(node1: actualJson, node2: expectedJson));
    }
    [Fact]
    public void TestHalfRepr_SpecialValues()
    {
        var actualJson = JsonNode.Parse(json: Half.NaN.ReprTree());
        var expectedJson = new JsonObject
        {
            [propertyName: "type"] = "Half",
            [propertyName: "kind"] = "struct",
            [propertyName: "value"] = "QuietNaN(0x200)"
        };
        Assert.True(condition: JsonNode.DeepEquals(node1: actualJson, node2: expectedJson));

        actualJson = JsonNode.Parse(json: Half.PositiveInfinity.ReprTree());
        expectedJson = new JsonObject
        {
            [propertyName: "type"] = "Half",
            [propertyName: "kind"] = "struct",
            [propertyName: "value"] = "Infinity"
        };
        Assert.True(condition: JsonNode.DeepEquals(node1: actualJson, node2: expectedJson));

        actualJson = JsonNode.Parse(json: Half.NegativeInfinity.ReprTree());
        expectedJson = new JsonObject
        {
            [propertyName: "type"] = "Half",
            [propertyName: "kind"] = "struct",
            [propertyName: "value"] = "-Infinity"
        };
        Assert.True(condition: JsonNode.DeepEquals(node1: actualJson, node2: expectedJson));
    }
        #endif
        #if NET7_0_OR_GREATER
    [Theory]
    [InlineData("B",
        "-0b10000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000")]
    [InlineData("D",
        "-170141183460469231731687303715884105728")]
    [InlineData("X",
        "-0x80000000000000000000000000000000")]
    public void TestInt128Repr(string format, string expectedValue)
    {
        var i128 = Int128.MinValue;
        var config = ReprConfig.Configure().IntFormat(format).Build();
        var actualJson = JsonNode.Parse(json: i128.ReprTree(config: config));
        var expectedJson = new JsonObject
        {
            [propertyName: "type"] = "Int128",
            [propertyName: "kind"] = "struct",
            [propertyName: "value"] = expectedValue
        };
        Assert.True(condition: JsonNode.DeepEquals(node1: actualJson, node2: expectedJson));
    }

    [Theory]
    [InlineData("B",
        "0b1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111")]
    [InlineData("D", "170141183460469231731687303715884105727")]
    [InlineData("X", "0x7FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF")]
    public void TestInt128Repr2(string format, string expectedValue)
    {
        var i128 = Int128.MaxValue;
        var config = ReprConfig.Configure().IntFormat(format).Build();
        var actualJson = JsonNode.Parse(json: i128.ReprTree(config: config));
        var expectedJson = new JsonObject
        {
            [propertyName: "type"] = "Int128",
            [propertyName: "kind"] = "struct",
            [propertyName: "value"] = expectedValue
        };
        Assert.True(condition: JsonNode.DeepEquals(node1: actualJson, node2: expectedJson));
    }
        #endif
    }
}