using System;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;
using DebugUtils.Unity.Repr.Models;
using UnityMath = Unity.Mathematics;
using Half = Unity.Mathematics.half;

namespace DebugUtils.Unity.Repr.Extensions
{
    /// <summary>
    /// IEEE 754 floating point analysis extensions.
    /// 
    /// CORE FUNCTIONALITY: Extract IEEE 754 binary representation components and convert
    /// to normalized form for exact decimal representation.
    /// 
    /// IEEE 754 FORMAT: [Sign][Exponent][Mantissa]
    /// - Normal numbers: value = (-1)^sign * (1.mantissa) * 2^(exponent - bias)
    /// - Subnormal numbers: value = (-1)^sign * (0.mantissa) * 2^(1 - bias)
    /// - Special cases: Infinity (exp = all 1s, mantissa = 0), NaN (exp = all 1s, mantissa ≠ 0)
    /// 
    /// OUTPUT: FloatInfo with RealExponent and Significand for exact decimal conversion
    /// </summary>
    internal static class FloatAnalysisExtensions
    {
        // IEEE 754 binary16 (Half): 1 sign + 5 exponent + 10 mantissa = 16 bits
        private static readonly FloatSpec halfSpec = new(expBitSize: 5, mantissaBitSize: 10,
            totalSize: 16, mantissaMask: 0x3FF, mantissaMsbMask: 0x200, expMask: 0x1F,
            expOffset: 15);

        // IEEE 754 binary32 (Float): 1 sign + 8 exponent + 23 mantissa = 32 bits
        private static readonly FloatSpec floatSpec = new(expBitSize: 8, mantissaBitSize: 23,
            totalSize: 32, mantissaMask: 0x7FFFFF, mantissaMsbMask: 0x400000, expMask: 0xFF,
            expOffset: 127);

        // IEEE 754 binary64 (Double): 1 sign + 11 exponent + 52 mantissa = 64 bits 
        private static readonly FloatSpec doubleSpec =
            new(expBitSize: 11, mantissaBitSize: 52, totalSize: 64, mantissaMask: 0xFFFFFFFFFFFFFL,
                mantissaMsbMask: 0x8000000000000L, expMask: 0x7FFL, expOffset: 1023);

        // IEEE 754 REAL EXPONENT CALCULATION:
        // Normal numbers: realExp = rawExp - bias - mantissaBits
        // Subnormal numbers: realExp = 1 - bias - mantissaBits (special case when rawExp = 0)
        // REASON: Subnormals have implicit leading 0, not 1, and use minimum exponent
        // IEEE 754 SIGNIFICAND CONSTRUCTION:
        // Normal: (2^mantissaBits + mantissa) - adds implicit leading 1
        // Subnormal: mantissa - no implicit leading 1 (rawExponent == 0)

        public static FloatInfo AnalyzeHalf(this Half value)
        {
            var bits = value.value;
            var rawExponent = (int)(bits >> halfSpec.MantissaBitSize & halfSpec.ExpMask);
            var mantissa = bits & halfSpec.MantissaMask;

            return new FloatInfo(
                spec: halfSpec,
                bits: bits,
                realExponent: rawExponent - halfSpec.ExpOffset + (rawExponent == 0
                    ? 1
                    : 0) - halfSpec.MantissaBitSize,
                significand: (ulong)(rawExponent == 0
                    ? mantissa
                    : (1 << halfSpec.MantissaBitSize) + mantissa),
                typeName: FloatTypeKind.Half
            );
        }
        public static FloatInfo AnalyzeFloat(this float value)
        {
            var bits = BitConverter.SingleToInt32Bits(value: value);
            var rawExponent = (int)(bits >> floatSpec.MantissaBitSize & floatSpec.ExpMask);
            var mantissa = bits & floatSpec.MantissaMask;

            return new FloatInfo(
                spec: floatSpec,
                bits: bits,
                realExponent: rawExponent - floatSpec.ExpOffset + (rawExponent == 0
                    ? 1
                    : 0) - floatSpec.MantissaBitSize,
                significand: (ulong)(rawExponent == 0
                    ? mantissa
                    : (1 << floatSpec.MantissaBitSize) + mantissa),
                typeName: FloatTypeKind.Float
            );
        }
        public static FloatInfo AnalyzeDouble(this double value)
        {
            var bits = BitConverter.DoubleToInt64Bits(value: value);
            var rawExponent = (int)(bits >> doubleSpec.MantissaBitSize & doubleSpec.ExpMask);
            var mantissa = bits & doubleSpec.MantissaMask;

            return new FloatInfo(
                spec: doubleSpec,
                bits: bits,
                realExponent: rawExponent - doubleSpec.ExpOffset + (rawExponent == 0
                    ? 1
                    : 0) - doubleSpec.MantissaBitSize,
                significand: (ulong)(rawExponent == 0
                    ? mantissa
                    : (1L << doubleSpec.MantissaBitSize) + mantissa),
                typeName: FloatTypeKind.Double
            );
        }
    }
}