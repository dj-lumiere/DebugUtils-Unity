#nullable enable
using Unity.Mathematics;

namespace DebugUtils.Unity.Tests.TestHelpers
{
    public static class FloatingPointTestHelpers
    {
        // half (16-bit) conversions
        public static half NextHalf(this half value)
        {
            var bits = BitConverter.HalfToUInt16Bits(value: value);
            // Handle special cases
            if ((bits & 0x7FFF) == 0x7C00) // Infinity
            {
                return value; // Infinity + 1 = Infinity
            }

            if ((bits & 0x7FFF) > 0x7C00) // NaN
            {
                return value; // NaN + 1 = NaN
            }

            // Handle negative zero -> positive zero
            if (bits == 0x8000) // -0.0
            {
                return BitConverter.UInt16BitsToHalf(bits: 0x0001); // Smallest positive subnormal
            }

            // Handle sign bit
            if ((bits & 0x8000) != 0) // Negative number
            {
                bits--; // Move toward zero
            }
            else // Positive number or zero
            {
                bits++; // Move away from zero
            }

            return BitConverter.UInt16BitsToHalf(bits: bits);
        }


        public static float NextFloat(this float value)
        {
            var bits = BitConverter.SingleToUInt32Bits(value: value);
            // Handle special cases
            if ((bits & 0x7FFFFFFF) == 0x7F800000) // Infinity
            {
                return value; // Infinity + 1 = Infinity
            }

            if ((bits & 0x7FFFFFFF) > 0x7F800000) // NaN
            {
                return value; // NaN + 1 = NaN
            }

            // Handle negative zero -> positive zero
            if (bits == 0x80000000) // -0.0f
            {
                return
                    BitConverter
                       .UInt32BitsToSingle(bits: 0x00000001); // Smallest positive subnormal
            }

            // Handle sign bit
            if ((bits & 0x80000000) != 0) // Negative number
            {
                bits--; // Move toward zero
            }
            else // Positive number or zero
            {
                bits++; // Move away from zero
            }

            return BitConverter.UInt32BitsToSingle(bits: bits);
        }


        public static double NextDouble(this double value)
        {
            var bits = BitConverter.DoubleToUInt64Bits(value: value);
            // Handle special cases
            if ((bits & 0x7FFFFFFFFFFFFFFF) == 0x7FF0000000000000) // Infinity
            {
                return value; // Infinity + 1 = Infinity
            }

            if ((bits & 0x7FFFFFFFFFFFFFFF) > 0x7FF0000000000000) // NaN
            {
                return value; // NaN + 1 = NaN
            }

            // Handle negative zero -> positive zero
            if (bits == 0x8000000000000000) // -0.0
            {
                return BitConverter.UInt64BitsToDouble(
                    bits: 0x0000000000000001); // Smallest positive subnormal
            }

            // Handle sign bit
            if ((bits & 0x8000000000000000) != 0) // Negative number
            {
                bits--; // Move toward zero
            }
            else // Positive number or zero
            {
                bits++; // Move away from zero
            }

            return BitConverter.UInt64BitsToDouble(bits: bits);
        }

        // Common half constants
        public static class HalfConstants
        {
            public static readonly half Zero = BitConverter.UInt16BitsToHalf(bits: 0x0000);
            public static readonly half NegativeZero = BitConverter.UInt16BitsToHalf(bits: 0x8000);
            public static readonly half One = BitConverter.UInt16BitsToHalf(bits: 0x3C00);
            public static readonly half NegativeOne = BitConverter.UInt16BitsToHalf(bits: 0xBC00);

            public static readonly half PositiveInfinity =
                BitConverter.UInt16BitsToHalf(bits: 0x7C00);

            public static readonly half NegativeInfinity =
                BitConverter.UInt16BitsToHalf(bits: 0xFC00);

            public static readonly half NaN = BitConverter.UInt16BitsToHalf(bits: 0x7E00);
            public static readonly half MaxValue = BitConverter.UInt16BitsToHalf(bits: 0x7BFF);
            public static readonly half MinValue = BitConverter.UInt16BitsToHalf(bits: 0xFBFF);
            public static readonly half Epsilon = BitConverter.UInt16BitsToHalf(bits: 0x0001);
        }

        // Utility methods for generating test values
        public static half[] GetHalfWorstCaseValues()
        {
            return new half[]
            {
                BitConverter
                   .UInt16BitsToHalf(bits: 0x07FF), // Large mantissa, small exponent (subnormal)
                BitConverter.UInt16BitsToHalf(bits: 0x7BFF), // Max normal value with full mantissa
                BitConverter.UInt16BitsToHalf(bits: 0x0001), // Smallest subnormal
                BitConverter.UInt16BitsToHalf(bits: 0x03FF), // Largest subnormal
                BitConverter.UInt16BitsToHalf(bits: 0x3C01), // Just above 1.0
                BitConverter.UInt16BitsToHalf(bits: 0xBC01), // Just below -1.0
                BitConverter.UInt16BitsToHalf(bits: 0x7800), // Large number
                BitConverter.UInt16BitsToHalf(bits: 0x0400) // Small normal number
            };
        }

        public static float[] GetFloatWorstCaseValues()
        {
            return new float[]
            {
                BitConverter.UInt32BitsToSingle(
                    bits: 0x00FF_FFFF), // Large mantissa, small exponent
                BitConverter.UInt32BitsToSingle(
                    bits: 0x7F7F_FFFF), // Max normal value with full mantissa
                BitConverter.UInt32BitsToSingle(bits: 0x0000_0001), // Smallest subnormal
                BitConverter.UInt32BitsToSingle(bits: 0x007F_FFFF), // Largest subnormal
                BitConverter.UInt32BitsToSingle(bits: 0x3F80_0001), // Just above 1.0
                BitConverter.UInt32BitsToSingle(bits: 0xBF80_0001), // Just below -1.0
                BitConverter.UInt32BitsToSingle(bits: 0x7F00_0000), // Large number
                BitConverter.UInt32BitsToSingle(bits: 0x0100_0000) // Small normal number
            };
        }

        public static double[] GetDoubleWorstCaseValues()
        {
            return new double[]
            {
                BitConverter.UInt64BitsToDouble(
                    bits: 0x000F_FFFF_FFFF_FFFF), // Large mantissa, small exponent
                BitConverter.UInt64BitsToDouble(
                    bits: 0x3FF7_FFFF_FFFF_FFFF), // Max normal value with full mantissa
                BitConverter.UInt64BitsToDouble(bits: 0x0000_0000_0000_0001), // Smallest subnormal
                BitConverter.UInt64BitsToDouble(bits: 0x000F_FFFF_FFFF_FFFF), // Largest subnormal
                BitConverter.UInt64BitsToDouble(bits: 0x3FF0_0000_0000_0001), // Just above 1.0
                BitConverter.UInt64BitsToDouble(bits: 0xBFF0_0000_0000_0001), // Just below -1.0
                BitConverter.UInt64BitsToDouble(bits: 0x7FE0_0000_0000_0000), // Large number
                BitConverter.UInt64BitsToDouble(bits: 0x0010_0000_0000_0000) // Small normal number
            };
        }
    }
}