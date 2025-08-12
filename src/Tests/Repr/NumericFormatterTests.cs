#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using DebugUtils.Unity.Repr;
using NUnit.Framework;
using Half = Unity.Mathematics.half;

namespace DebugUtils.Unity.Tests
{

    public class NumericFormatterTests
    {
        // Integer Types
        [TestCase(arg1: IntReprMode.Binary, arg2: "byte(0b101010)")]
        [TestCase(arg1: IntReprMode.Decimal, arg2: "byte(42)")]
        [TestCase(arg1: IntReprMode.Hex, arg2: "byte(0x2A)")]
        [TestCase(arg1: IntReprMode.HexBytes, arg2: "byte(0x2A)")]
        public void TestByteRepr(IntReprMode mode, string expected)
        {
            var config = new ReprConfig(IntMode: mode);
            Assert.AreEqual(expected: expected, actual: ((byte)42).Repr(config: config));
        }

        [TestCase(arg1: IntReprMode.Binary, arg2: "int(-0b101010)")]
        [TestCase(arg1: IntReprMode.Decimal, arg2: "int(-42)")]
        [TestCase(arg1: IntReprMode.Hex, arg2: "int(-0x2A)")]
        [TestCase(arg1: IntReprMode.HexBytes, arg2: "int(0xFFFFFFD6)")]
        public void TestIntRepr(IntReprMode mode, string expected)
        {
            var config = new ReprConfig(IntMode: mode);
            Assert.AreEqual(expected: expected, actual: (-42).Repr(config: config));
        }

        [Test]
        public void TestBigIntRepr()
        {
            var config = new ReprConfig(IntMode: IntReprMode.Decimal);
            Assert.AreEqual(expected: "BigInteger(-42)",
                actual: new BigInteger(value: -42).Repr(config: config));
        }

        // Floating Point Types
        [Test]
        public void TestFloatRepr_Exact()
        {
            var config = new ReprConfig(FloatMode: FloatReprMode.Exact);
            Assert.AreEqual(expected: "float(3.1415927410125732421875E+000)", actual: Single
               .Parse(s: "3.1415926535")
               .Repr(config: config));
        }

        [Test]
        public void TestDoubleRepr_Round()
        {
            var config = new ReprConfig(FloatMode: FloatReprMode.Round, FloatPrecision: 5);
            Assert.AreEqual(expected: "double(3.14159)", actual: Double.Parse(s: "3.1415926535")
               .Repr(config: config));
        }

        [Test]
        public void TestHalfRepr_Scientific()
        {
            var config = new ReprConfig(FloatMode: FloatReprMode.Scientific, FloatPrecision: 5);
            Assert.AreEqual(expected: "half(3.14063E+000)", actual: new Half(v: 3.14159)
               .Repr(config: config));
        }

        [Test]
        public void TestDecimalRepr_RawHex()
        {
            var config = new ReprConfig(FloatMode: FloatReprMode.HexBytes);
            Assert.AreEqual(expected: "decimal(0x001C00006582A5360B14388541B65F29)",
                actual: 3.1415926535897932384626433832795m.Repr(
                    config: config));
        }

        [Test]
        public void TestHalfRepr_BitField()
        {
            var config = new ReprConfig(FloatMode: FloatReprMode.BitField);
            Assert.AreEqual(expected: "half(0|10000|1001001000)", actual: new Half(v: 3.14159)
               .Repr(config: config));
        }

        [Test]
        public void TestFloatRepr_SpecialValues()
        {
            Assert.AreEqual(expected: "float(Quiet NaN)", actual: Single.NaN.Repr());
            Assert.AreEqual(expected: "float(Infinity)", actual: Single.PositiveInfinity.Repr());
            Assert.AreEqual(expected: "float(-Infinity)", actual: Single.NegativeInfinity.Repr());
        }

        [Test]
        public void TestDoubleRepr_SpecialValues()
        {
            Assert.AreEqual(expected: "double(Quiet NaN)", actual: Double.NaN.Repr());
            Assert.AreEqual(expected: "double(Infinity)", actual: Double.PositiveInfinity.Repr());
            Assert.AreEqual(expected: "double(-Infinity)", actual: Double.NegativeInfinity.Repr());
        }

        [Test]
        public void TestHalfRepr_SpecialValues()
        {
            Assert.AreEqual(expected: "half(Quiet NaN)", actual: new Half(v: Single.NaN).Repr());
            Assert.AreEqual(expected: "half(Infinity)",
                actual: new Half(v: Single.PositiveInfinity).Repr());
            Assert.AreEqual(expected: "half(-Infinity)",
                actual: new Half(v: Single.NegativeInfinity).Repr());
        }
    }
}