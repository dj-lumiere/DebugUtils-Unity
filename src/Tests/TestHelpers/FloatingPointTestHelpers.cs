using System;
using System.Runtime.InteropServices;
using Half = Unity.Mathematics.half;

namespace DebugUtils.Tests.TestHelpers
{
    public static class FloatingPointTestHelpers
    {
        // Half (16-bit) conversions
        public static Half BitsToHalf(this ushort bits)
        {
            return MemoryMarshal.Cast<ushort, Half>(
                span: MemoryMarshal.CreateSpan(reference: ref bits, length: 1))[index: 0];
        }

        public static ushort HalfToBits(this Half value)
        {
            var temp = value;
            return MemoryMarshal.Cast<Half, ushort>(
                span: MemoryMarshal.CreateSpan(reference: ref temp, length: 1))[index: 0];
        }

        public static Half NextHalf(this Half value)
        {
            var bits = HalfToBits(value: value);

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
                return BitsToHalf(bits: 0x0001); // Smallest positive subnormal
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

            return BitsToHalf(bits: bits);
        }

        // Float (32-bit) conversions
        public static float BitsToFloat(this uint bits)
        {
            return MemoryMarshal.Cast<uint, float>(
                span: MemoryMarshal.CreateSpan(reference: ref bits, length: 1))[index: 0];
        }

        public static uint FloatToBits(this float value)
        {
            return MemoryMarshal.Cast<float, uint>(
                span: MemoryMarshal.CreateSpan(reference: ref value, length: 1))[index: 0];
        }

        public static float NextFloat(this float value)
        {
            var bits = FloatToBits(value: value);

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
                return BitsToFloat(bits: 0x00000001); // Smallest positive subnormal
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

            return BitsToFloat(bits: bits);
        }

        // Double (64-bit) conversions
        public static double BitsToDouble(this ulong bits)
        {
            return MemoryMarshal.Cast<ulong, double>(
                span: MemoryMarshal.CreateSpan(reference: ref bits, length: 1))[index: 0];
        }

        public static ulong DoubleToBits(this double value)
        {
            return MemoryMarshal.Cast<double, ulong>(
                span: MemoryMarshal.CreateSpan(reference: ref value, length: 1))[index: 0];
        }

        public static double NextDouble(this double value)
        {
            var bits = DoubleToBits(value: value);

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
                return BitsToDouble(bits: 0x0000000000000001); // Smallest positive subnormal
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

            return BitsToDouble(bits: bits);
        }

        // Common Half constants
        public static class HalfConstants
        {
            public static readonly Half Zero = BitsToHalf(bits: 0x0000);
            public static readonly Half NegativeZero = BitsToHalf(bits: 0x8000);
            public static readonly Half One = BitsToHalf(bits: 0x3C00);
            public static readonly Half NegativeOne = BitsToHalf(bits: 0xBC00);
            public static readonly Half PositiveInfinity = BitsToHalf(bits: 0x7C00);
            public static readonly Half NegativeInfinity = BitsToHalf(bits: 0xFC00);
            public static readonly Half NaN = BitsToHalf(bits: 0x7E00);
            public static readonly Half MaxValue = BitsToHalf(bits: 0x7BFF);
            public static readonly Half MinValue = BitsToHalf(bits: 0xFBFF);
            public static readonly Half Epsilon = BitsToHalf(bits: 0x0001);
        }

        // Utility methods for generating test values
        public static Half[] GetHalfWorstCaseValues()
        {
            return new Half[]
            {
                BitsToHalf(bits: 0x07FF), // Large mantissa, small exponent (subnormal)
                BitsToHalf(bits: 0x7BFF), // Max normal value with full mantissa
                BitsToHalf(bits: 0x0001), // Smallest subnormal
                BitsToHalf(bits: 0x03FF), // Largest subnormal
                BitsToHalf(bits: 0x3C01), // Just above 1.0
                BitsToHalf(bits: 0xBC01), // Just below -1.0
                BitsToHalf(bits: 0x7800), // Large number
                BitsToHalf(bits: 0x0400) // Small normal number
            };
        }

        public static float[] GetFloatWorstCaseValues()
        {
            return new float[]
            {
                BitsToFloat(bits: 0x00FF_FFFF), // Large mantissa, small exponent
                BitsToFloat(bits: 0x7F7F_FFFF), // Max normal value with full mantissa
                BitsToFloat(bits: 0x0000_0001), // Smallest subnormal
                BitsToFloat(bits: 0x007F_FFFF), // Largest subnormal
                BitsToFloat(bits: 0x3F80_0001), // Just above 1.0
                BitsToFloat(bits: 0xBF80_0001), // Just below -1.0
                BitsToFloat(bits: 0x7F00_0000), // Large number
                BitsToFloat(bits: 0x0100_0000) // Small normal number
            };
        }

        public static double[] GetDoubleWorstCaseValues()
        {
            return new double[]
            {
                BitsToDouble(bits: 0x000F_FFFF_FFFF_FFFF), // Large mantissa, small exponent
                BitsToDouble(bits: 0x3FF7_FFFF_FFFF_FFFF), // Max normal value with full mantissa
                BitsToDouble(bits: 0x0000_0000_0000_0001), // Smallest subnormal
                BitsToDouble(bits: 0x000F_FFFF_FFFF_FFFF), // Largest subnormal
                BitsToDouble(bits: 0x3FF0_0000_0000_0001), // Just above 1.0
                BitsToDouble(bits: 0xBFF0_0000_0000_0001), // Just below -1.0
                BitsToDouble(bits: 0x7FE0_0000_0000_0000), // Large number
                BitsToDouble(bits: 0x0010_0000_0000_0000) // Small normal number
            };
        }
    }
}