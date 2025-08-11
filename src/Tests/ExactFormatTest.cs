using System;
using DebugUtils.Unity.Repr;
using NUnit.Framework;
using Half = Unity.Mathematics.half;
using static DebugUtils.Tests.TestHelpers.FloatingPointTestHelpers;

namespace DebugUtils.Tests
{
    public class ExactFormatTest
    {
        private static readonly ReprConfig
            OldExactConfig = new(FloatMode: FloatReprMode.Exact_Old);

        private static readonly ReprConfig NewExactConfig =
            new(FloatMode: FloatReprMode.Exact);

        [Test]
        public void TestDecimal_Exact_Normal()
        {
            Assert.That(actual: 1.0m.Repr(config: OldExactConfig),
                expression: Does.Contain(expected: "1.0E+000"));
            StringAssert.Contains(expected: "-1.0E+000",
                actual: (-1.0m).Repr(config: OldExactConfig));
            StringAssert.Contains(expected: "3.1415926535897932384626433833E+000",
                actual: 3.1415926535897932384626433833m.Repr(config: OldExactConfig));
            StringAssert.Contains(expected: "1.23456789E+028",
                actual: 12345678900000000000000000000m.Repr(config: OldExactConfig));
            StringAssert.Contains(expected: "1.0E-028",
                actual: 0.0000000000000000000000000001m.Repr(config: OldExactConfig));
        }

        [Test]
        public void TestDecimal_ExactBeta_Normal()
        {
            StringAssert.Contains(expected: "1.0E+000",
                actual: 1.0m.Repr(config: NewExactConfig));
            StringAssert.Contains(expected: "-1.0E+000",
                actual: (-1.0m).Repr(config: NewExactConfig));
            StringAssert.Contains(expected: "3.1415926535897932384626433833E+000",
                actual: 3.1415926535897932384626433833m.Repr(config: NewExactConfig));
            StringAssert.Contains(expected: "1.23456789E+028",
                actual: 12345678900000000000000000000m.Repr(config: NewExactConfig));
            StringAssert.Contains(expected: "1.0E-028",
                actual: 0.0000000000000000000000000001m.Repr(config: NewExactConfig));
        }

        [Test]
        public void TestDecimal_Exact_Zero()
        {
            StringAssert.Contains(expected: "0.0E+000",
                actual: 0.0m.Repr(config: OldExactConfig));
            StringAssert.Contains(expected: "0.0E+000",
                actual: (-0.0m).Repr(config: OldExactConfig));
        }

        [Test]
        public void TestDecimal_ExactBeta_Zero()
        {
            StringAssert.Contains(expected: "0.0E+000",
                actual: 0.0m.Repr(config: NewExactConfig));
            StringAssert.Contains(expected: "0.0E+000",
                actual: (-0.0m).Repr(config: NewExactConfig));
        }

        [Test]
        public void TestDecimal_Exact_MaxMin()
        {
            StringAssert.Contains(expected: "7.9228162514264337593543950335E+028",
                actual: Decimal.MaxValue.Repr(config: OldExactConfig));
            StringAssert.Contains(expected: "-7.9228162514264337593543950335E+028",
                actual: Decimal.MinValue.Repr(config: OldExactConfig));
        }

        [Test]
        public void TestDecimal_ExactBeta_MaxMin()
        {
            StringAssert.Contains(expected: "7.9228162514264337593543950335E+028",
                actual: Decimal.MaxValue.Repr(config: NewExactConfig));
            StringAssert.Contains(expected: "-7.9228162514264337593543950335E+028",
                actual: Decimal.MinValue.Repr(config: NewExactConfig));
        }

        [Test]
        public void TestFloat_Exact_Normal()
        {
            var result0 = 0.0f.Repr(config: OldExactConfig);
            var resultNeg0 = (-0.0f).Repr(config: OldExactConfig);
            var result1 = 1.0f.Repr(config: OldExactConfig);
            var resultNeg1 = (-1.0f).Repr(config: OldExactConfig);
            var result15 = 1.5f.Repr(config: OldExactConfig);
            var result25 = 2.5f.Repr(config: OldExactConfig);

            Assert.NotNull(anObject: result0);
            Assert.NotNull(anObject: resultNeg0);
            Assert.NotNull(anObject: result1);
            Assert.NotNull(anObject: resultNeg1);
            Assert.NotNull(anObject: result15);
            Assert.NotNull(anObject: result25);
        }

        [Test]
        public void TestFloat_ExactBeta_Normal()
        {
            var result0 = 0.0f.Repr(config: NewExactConfig);
            var resultNeg0 = (-0.0f).Repr(config: NewExactConfig);
            var result1 = 1.0f.Repr(config: NewExactConfig);
            var resultNeg1 = (-1.0f).Repr(config: NewExactConfig);
            var result15 = 1.5f.Repr(config: NewExactConfig);
            var result25 = 2.5f.Repr(config: NewExactConfig);

            Assert.NotNull(anObject: result0);
            Assert.NotNull(anObject: resultNeg0);
            Assert.NotNull(anObject: result1);
            Assert.NotNull(anObject: resultNeg1);
            Assert.NotNull(anObject: result15);
            Assert.NotNull(anObject: result25);
        }

        [Test]
        public void TestFloat_Exact_Subnormal()
        {
            var minValue = 1.401298E-45f; // Smallest positive subnormal float
            var subnormal = 1e-40f; // A subnormal number

            var result1 = minValue.Repr(config: OldExactConfig);
            var result11 = minValue.Repr(config: NewExactConfig);
            var result2 = subnormal.Repr(config: OldExactConfig);

            TestContext.WriteLine(value: result1);
            TestContext.WriteLine(value: result11);

            Assert.NotNull(anObject: result1);
            Assert.NotNull(anObject: result2);
            Assert.That(actual: result1, expression: Is.Not.Empty);
            Assert.That(actual: result2, expression: Is.Not.Empty);
        }

        [Test]
        public void TestFloat_ExactBeta_Subnormal()
        {
            var minValue = 1.401298E-45f; // Smallest positive subnormal float
            var subnormal = 1e-40f; // A subnormal number

            var result1 = minValue.Repr(config: NewExactConfig);
            var result2 = subnormal.Repr(config: NewExactConfig);

            Assert.NotNull(anObject: result1);
            Assert.NotNull(anObject: result2);
            Assert.That(actual: result1, expression: Is.Not.Empty);
            Assert.That(actual: result2, expression: Is.Not.Empty);
        }

        [Test]
        public void TestFloat_Exact_SpecialValues()
        {
            StringAssert.Contains(expected: "NaN",
                actual: Single.NaN.Repr(config: OldExactConfig));
            StringAssert.Contains(expected: "Infinity",
                actual: Single.PositiveInfinity.Repr(config: OldExactConfig));
            StringAssert.Contains(expected: "Infinity",
                actual: Single.NegativeInfinity.Repr(config: OldExactConfig));
        }

        [Test]
        public void TestFloat_ExactBeta_SpecialValues()
        {
            StringAssert.Contains(expected: "NaN",
                actual: Single.NaN.Repr(config: NewExactConfig));
            StringAssert.Contains(expected: "Infinity",
                actual: Single.PositiveInfinity.Repr(config: NewExactConfig));
            StringAssert.Contains(expected: "Infinity",
                actual: Single.NegativeInfinity.Repr(config: NewExactConfig));
        }

        [Test]
        public void TestFloat_Exact_MaxMin()
        {
            var maxResult = Single.MaxValue.Repr(config: OldExactConfig);
            var minResult = Single.MinValue.Repr(config: OldExactConfig);
            var epsilonResult = Single.Epsilon.Repr(config: OldExactConfig);

            Assert.NotNull(anObject: maxResult);
            Assert.NotNull(anObject: minResult);
            Assert.NotNull(anObject: epsilonResult);
            Assert.That(actual: maxResult, expression: Is.Not.Empty);
            Assert.That(actual: minResult, expression: Is.Not.Empty);
            Assert.That(actual: epsilonResult, expression: Is.Not.Empty);
        }

        [Test]
        public void TestFloat_ExactBeta_MaxMin()
        {
            var maxResult = Single.MaxValue.Repr(config: NewExactConfig);
            var minResult = Single.MinValue.Repr(config: NewExactConfig);
            var epsilonResult = Single.Epsilon.Repr(config: NewExactConfig);

            Assert.NotNull(anObject: maxResult);
            Assert.NotNull(anObject: minResult);
            Assert.NotNull(anObject: epsilonResult);
            Assert.That(actual: maxResult, expression: Is.Not.Empty);
            Assert.That(actual: minResult, expression: Is.Not.Empty);
            Assert.That(actual: epsilonResult, expression: Is.Not.Empty);
        }

        [Test]
        public void TestDouble_Exact_Normal()
        {
            var result0 = 0.0.Repr(config: OldExactConfig);
            var resultNeg0 = (-0.0).Repr(config: OldExactConfig);
            var result1 = 1.0.Repr(config: OldExactConfig);
            var resultNeg1 = (-1.0).Repr(config: OldExactConfig);
            var result15 = 1.5.Repr(config: OldExactConfig);
            var result25 = 2.5.Repr(config: OldExactConfig);

            Assert.NotNull(anObject: result0);
            Assert.NotNull(anObject: resultNeg0);
            Assert.NotNull(anObject: result1);
            Assert.NotNull(anObject: resultNeg1);
            Assert.NotNull(anObject: result15);
            Assert.NotNull(anObject: result25);
        }

        [Test]
        public void TestDouble_ExactBeta_Normal()
        {
            var result0 = 0.0.Repr(config: NewExactConfig);
            var resultNeg0 = (-0.0).Repr(config: NewExactConfig);
            var result1 = 1.0.Repr(config: NewExactConfig);
            var resultNeg1 = (-1.0).Repr(config: NewExactConfig);
            var result15 = 1.5.Repr(config: NewExactConfig);
            var result25 = 2.5.Repr(config: NewExactConfig);

            Assert.NotNull(anObject: result0);
            Assert.NotNull(anObject: resultNeg0);
            Assert.NotNull(anObject: result1);
            Assert.NotNull(anObject: resultNeg1);
            Assert.NotNull(anObject: result15);
            Assert.NotNull(anObject: result25);
        }

        [Test]
        public void TestDouble_Exact_Subnormal()
        {
            var minValue = 4.9406564584124654E-324; // Smallest positive subnormal double
            var subnormal = 1e-320; // A subnormal number

            var result1 = minValue.Repr(config: OldExactConfig);
            var result2 = subnormal.Repr(config: OldExactConfig);

            Assert.NotNull(anObject: result1);
            Assert.NotNull(anObject: result2);
            Assert.That(actual: result1, expression: Is.Not.Empty);
            Assert.That(actual: result2, expression: Is.Not.Empty);
        }

        [Test]
        public void TestDouble_ExactBeta_Subnormal()
        {
            var minValue = 4.9406564584124654E-324; // Smallest positive subnormal double
            var subnormal = 1e-320; // A subnormal number

            var result1 = minValue.Repr(config: NewExactConfig);
            var result2 = subnormal.Repr(config: NewExactConfig);

            Assert.NotNull(anObject: result1);
            Assert.NotNull(anObject: result2);
            Assert.That(actual: result1, expression: Is.Not.Empty);
            Assert.That(actual: result2, expression: Is.Not.Empty);
        }

        [Test]
        public void TestDouble_Exact_SpecialValues()
        {
            StringAssert.Contains(expected: "NaN",
                actual: Double.NaN.Repr(config: OldExactConfig));
            StringAssert.Contains(expected: "Infinity",
                actual: Double.PositiveInfinity.Repr(config: OldExactConfig));
            StringAssert.Contains(expected: "Infinity",
                actual: Double.NegativeInfinity.Repr(config: OldExactConfig));
        }

        [Test]
        public void TestDouble_ExactBeta_SpecialValues()
        {
            StringAssert.Contains(expected: "NaN",
                actual: Double.NaN.Repr(config: NewExactConfig));
            StringAssert.Contains(expected: "Infinity",
                actual: Double.PositiveInfinity.Repr(config: NewExactConfig));
            StringAssert.Contains(expected: "Infinity",
                actual: Double.NegativeInfinity.Repr(config: NewExactConfig));
        }

        [Test]
        public void TestDouble_Exact_MaxMin()
        {
            var maxResult = Double.MaxValue.Repr(config: OldExactConfig);
            var minResult = Double.MinValue.Repr(config: OldExactConfig);
            var epsilonResult = Double.Epsilon.Repr(config: OldExactConfig);

            Assert.NotNull(anObject: maxResult);
            Assert.NotNull(anObject: minResult);
            Assert.NotNull(anObject: epsilonResult);
            Assert.That(actual: maxResult, expression: Is.Not.Empty);
            Assert.That(actual: minResult, expression: Is.Not.Empty);
            Assert.That(actual: epsilonResult, expression: Is.Not.Empty);
        }

        [Test]
        public void TestDouble_ExactBeta_MaxMin()
        {
            var maxResult = Double.MaxValue.Repr(config: NewExactConfig);
            var minResult = Double.MinValue.Repr(config: NewExactConfig);
            var epsilonResult = Double.Epsilon.Repr(config: NewExactConfig);

            Assert.NotNull(anObject: maxResult);
            Assert.NotNull(anObject: minResult);
            Assert.NotNull(anObject: epsilonResult);
            Assert.That(actual: maxResult, expression: Is.Not.Empty);
            Assert.That(actual: minResult, expression: Is.Not.Empty);
            Assert.That(actual: epsilonResult, expression: Is.Not.Empty);
        }

        [Test]
        public void TestHalf_Exact_Normal()
        {
            var result0 = ((ushort)0x0000).BitsToHalf()
                                          .Repr(config: OldExactConfig); // Zero
            var resultNegZero = ((ushort)0x8000).BitsToHalf()
                                                .Repr(config: OldExactConfig); // Negative zero
            var result1 = ((Half)1.0f).Repr(config: OldExactConfig);
            var resultNeg1 = ((Half)(-1.0f)).Repr(config: OldExactConfig);

            Assert.NotNull(anObject: result0);
            Assert.NotNull(anObject: resultNegZero);
            Assert.NotNull(anObject: result1);
            Assert.NotNull(anObject: resultNeg1);
        }

        [Test]
        public void TestHalf_ExactBeta_Normal()
        {
            var result0 = ((ushort)0x0000).BitsToHalf()
                                          .Repr(config: NewExactConfig); // Zero
            var resultNegZero = ((ushort)0x8000).BitsToHalf()
                                                .Repr(config: NewExactConfig); // Negative zero
            var result1 = ((Half)1.0f).Repr(config: NewExactConfig);
            var resultNeg1 = ((Half)(-1.0f)).Repr(config: NewExactConfig);

            Assert.NotNull(anObject: result0);
            Assert.NotNull(anObject: resultNegZero);
            Assert.NotNull(anObject: result1);
            Assert.NotNull(anObject: resultNeg1);
        }

        [Test]
        public void TestHalf_Exact_Subnormal()
        {
            var minValue = (Half)5.9604645E-8f; // Smallest positive subnormal Half
            var result = minValue.Repr(config: OldExactConfig);

            Assert.NotNull(anObject: result);
            Assert.That(actual: result, expression: Is.Not.Empty);
        }

        [Test]
        public void TestHalf_ExactBeta_Subnormal()
        {
            var minValue = (Half)5.9604645E-8f; // Smallest positive subnormal Half
            var result = minValue.Repr(config: NewExactConfig);

            Assert.NotNull(anObject: result);
            Assert.That(actual: result, expression: Is.Not.Empty);
        }

        [Test]
        public void TestHalf_Exact_SpecialValues()
        {
            StringAssert.Contains(expected: "NaN",
                actual: ((ushort)0x7E00).BitsToHalf()
                                        .Repr(config: OldExactConfig)); // NaN
            StringAssert.Contains(expected: "Infinity",
                actual: ((ushort)0x7C00).BitsToHalf()
                                        .Repr(config: OldExactConfig)); // Positive Infinity
            StringAssert.Contains(expected: "Infinity",
                actual: ((ushort)0xFC00).BitsToHalf()
                                        .Repr(config: OldExactConfig)); // Negative Infinity
        }

        [Test]
        public void TestHalf_ExactBeta_SpecialValues()
        {
            StringAssert.Contains(expected: "NaN",
                actual: ((ushort)0x7E00).BitsToHalf()
                                        .Repr(config: NewExactConfig)); // NaN
            StringAssert.Contains(expected: "Infinity",
                actual: ((ushort)0x7C00).BitsToHalf()
                                        .Repr(config: NewExactConfig)); // Positive Infinity
            StringAssert.Contains(expected: "Infinity",
                actual: ((ushort)0xFC00).BitsToHalf()
                                        .Repr(config: NewExactConfig)); // Negative Infinity
        }

        [Test]
        public void TestHalf_Exact_MaxMin()
        {
            var maxResult = ((ushort)0x7BFF).BitsToHalf()
                                            .Repr(config: OldExactConfig); // Max value
            var minResult = ((ushort)0xFBFF).BitsToHalf()
                                            .Repr(config: OldExactConfig); // Min value
            var epsilonResult = ((ushort)0x0001).BitsToHalf()
                                                .Repr(config: OldExactConfig); // Epsilon

            Assert.NotNull(anObject: maxResult);
            Assert.NotNull(anObject: minResult);
            Assert.NotNull(anObject: epsilonResult);
            Assert.That(actual: maxResult, expression: Is.Not.Empty);
            Assert.That(actual: minResult, expression: Is.Not.Empty);
            Assert.That(actual: epsilonResult, expression: Is.Not.Empty);
        }

        [Test]
        public void TestHalf_ExactBeta_MaxMin()
        {
            var maxResult = ((ushort)0x7BFF).BitsToHalf()
                                            .Repr(config: NewExactConfig); // Max value
            var minResult = ((ushort)0xFBFF).BitsToHalf()
                                            .Repr(config: NewExactConfig); // Min value
            var epsilonResult = ((ushort)0x0001).BitsToHalf()
                                                .Repr(config: NewExactConfig); // Epsilon

            Assert.NotNull(anObject: maxResult);
            Assert.NotNull(anObject: minResult);
            Assert.NotNull(anObject: epsilonResult);
            Assert.That(actual: maxResult, expression: Is.Not.Empty);
            Assert.That(actual: minResult, expression: Is.Not.Empty);
            Assert.That(actual: epsilonResult, expression: Is.Not.Empty);
        }

        [TestCase(arg: 1.0f)]
        [TestCase(arg: 2.0f)]
        [TestCase(arg: 3.14159f)]
        [TestCase(arg: 0.1f)]
        [TestCase(arg: 1e-10f)]
        [TestCase(arg: 1e20f)]
        public void TestFloat_Exact_Consistency(float value)
        {
            var result = value.Repr(config: OldExactConfig);
            Assert.NotNull(anObject: result);
            Assert.That(actual: result, expression: Is.Not.Empty);
        }

        [TestCase(arg: 1.0f)]
        [TestCase(arg: 2.0f)]
        [TestCase(arg: 3.14159f)]
        [TestCase(arg: 0.1f)]
        [TestCase(arg: 1e-10f)]
        [TestCase(arg: 1e20f)]
        public void TestFloat_ExactBeta_Consistency(float value)
        {
            var result = value.Repr(config: NewExactConfig);
            Assert.NotNull(anObject: result);
            Assert.That(actual: result, expression: Is.Not.Empty);
        }

        [TestCase(arg: 1.0)]
        [TestCase(arg: 2.0)]
        [TestCase(arg: 3.14159265358979)]
        [TestCase(arg: 0.1)]
        [TestCase(arg: 1e-100)]
        [TestCase(arg: 1e100)]
        public void TestDouble_Exact_Consistency(double value)
        {
            var result = value.Repr(config: OldExactConfig);
            Assert.NotNull(anObject: result);
            Assert.That(actual: result, expression: Is.Not.Empty);
        }

        [TestCase(arg: 1.0)]
        [TestCase(arg: 2.0)]
        [TestCase(arg: 3.14159265358979)]
        [TestCase(arg: 0.1)]
        [TestCase(arg: 1e-100)]
        [TestCase(arg: 1e100)]
        public void TestDouble_ExactBeta_Consistency(double value)
        {
            var result = value.Repr(config: NewExactConfig);
            Assert.NotNull(anObject: result);
            Assert.That(actual: result, expression: Is.Not.Empty);
        }

        [TestCase(arg: 1.0)]
        [TestCase(arg: 2.0)]
        [TestCase(arg: 3.141592653589793238462643383279)]
        [TestCase(arg: 0.1)]
        [TestCase(arg: 0.0000000000000000000000000001)]
        [TestCase(arg: 79228162514264328797450928128.0)]
        public void TestDecimal_Exact_Consistency(double doubleValue)
        {
            var decimalValue = (decimal)doubleValue;
            var result = decimalValue.Repr(config: OldExactConfig);
            Assert.NotNull(anObject: result);
            Assert.That(actual: result, expression: Is.Not.Empty);
        }

        [TestCase(arg: 1.0)]
        [TestCase(arg: 2.0)]
        [TestCase(arg: 3.141592653589793238462643383279)]
        [TestCase(arg: 0.1)]
        [TestCase(arg: 0.0000000000000000000000000001)]
        [TestCase(arg: 79228162514264328797450928128.0)]
        public void TestDecimal_ExactBeta_Consistency(double doubleValue)
        {
            var decimalValue = (decimal)doubleValue;
            var result = decimalValue.Repr(config: NewExactConfig);
            Assert.NotNull(anObject: result);
            Assert.That(actual: result, expression: Is.Not.Empty);
        }

        [Test]
        public void TestHalf_Exact_WorstCaseScenarios()
        {
            // Worst case bit patterns for Half precision
            var half1 = ((ushort)0x07FF).BitsToHalf(); // Large mantissa, small exponent
            var half2 = ((ushort)0x7BFF).BitsToHalf(); // Max normal value with full mantissa

            var result1 = half1.Repr(config: OldExactConfig);
            var result2 = half2.Repr(config: OldExactConfig);

            Assert.NotNull(anObject: result1);
            Assert.NotNull(anObject: result2);
            Assert.That(actual: result1, expression: Is.Not.Empty);
            Assert.That(actual: result2, expression: Is.Not.Empty);
        }

        [Test]
        public void TestHalf_ExactBeta_WorstCaseScenarios()
        {
            // Worst case bit patterns for Half precision
            var half1 = ((ushort)0x07FF).BitsToHalf(); // Large mantissa, small exponent
            var half2 = ((ushort)0x7BFF).BitsToHalf(); // Max normal value with full mantissa

            var result1 = half1.Repr(config: NewExactConfig);
            var result2 = half2.Repr(config: NewExactConfig);

            Assert.NotNull(anObject: result1);
            Assert.NotNull(anObject: result2);
            Assert.That(actual: result1, expression: Is.Not.Empty);
            Assert.That(actual: result2, expression: Is.Not.Empty);
        }

        [Test]
        public void TestFloat_Exact_WorstCaseScenarios()
        {
            // Worst case bit patterns for Single precision
            var float1 = ((uint)0x00FF_FFFF).BitsToFloat(); // Large mantissa, small exponent
            var float2 = ((uint)0x7FFF_FFFF).BitsToFloat(); // Max normal value with full mantissa

            var result1 = float1.Repr(config: OldExactConfig);
            var result2 = float2.Repr(config: OldExactConfig);

            Assert.NotNull(anObject: result1);
            Assert.NotNull(anObject: result2);
            Assert.That(actual: result1, expression: Is.Not.Empty);
            Assert.That(actual: result2, expression: Is.Not.Empty);
        }

        [Test]
        public void TestFloat_ExactBeta_WorstCaseScenarios()
        {
            // Worst case bit patterns for Single precision
            var float1 = ((uint)0x00FF_FFFF).BitsToFloat(); // Large mantissa, small exponent
            var float2 = ((uint)0x7FFF_FFFF).BitsToFloat(); // Max normal value with full mantissa

            var result1 = float1.Repr(config: NewExactConfig);
            var result2 = float2.Repr(config: NewExactConfig);

            Assert.NotNull(anObject: result1);
            Assert.NotNull(anObject: result2);
            Assert.That(actual: result1, expression: Is.Not.Empty);
            Assert.That(actual: result2, expression: Is.Not.Empty);
        }

        [Test]
        public void TestDouble_Exact_WorstCaseScenarios()
        {
            // Worst case bit patterns for Double precision
            var double1 =
                ((ulong)0x000F_FFFF_FFFF_FFFF).BitsToDouble(); // Large mantissa, small exponent
            var double2 =
                ((ulong)0x3FF7_FFFF_FFFF_FFFF)
               .BitsToDouble(); // Max normal value with full mantissa

            var result1 = double1.Repr(config: OldExactConfig);
            var result2 = double2.Repr(config: OldExactConfig);

            Assert.NotNull(anObject: result1);
            Assert.NotNull(anObject: result2);
            Assert.That(actual: result1, expression: Is.Not.Empty);
            Assert.That(actual: result2, expression: Is.Not.Empty);
        }

        [Test]
        public void TestDouble_ExactBeta_WorstCaseScenarios()
        {
            // Worst case bit patterns for Double precision
            var double1 =
                ((ulong)0x000F_FFFF_FFFF_FFFF).BitsToDouble(); // Large mantissa, small exponent
            var double2 =
                ((ulong)0x3FF7_FFFF_FFFF_FFFF)
               .BitsToDouble(); // Max normal value with full mantissa

            var result1 = double1.Repr(config: NewExactConfig);
            var result2 = double2.Repr(config: NewExactConfig);

            Assert.NotNull(anObject: result1);
            Assert.NotNull(anObject: result2);
            Assert.That(actual: result1, expression: Is.Not.Empty);
            Assert.That(actual: result2, expression: Is.Not.Empty);
        }
    }
}