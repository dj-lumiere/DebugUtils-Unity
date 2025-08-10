using System;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;
using DebugUtils.Unity.Repr.Models;
using UnityMath = Unity.Mathematics;
using Half = Unity.Mathematics.half;

namespace DebugUtils.Unity.Repr.Extensions
{
    internal static class FloatExtensions
    {
        private static readonly FloatSpec halfSpec = new(ExpBitSize: 5, MantissaBitSize: 10,
            TotalSize: 16, MantissaMask: 0x3FF, MantissaMsbMask: 0x3FF, ExpMask: 0x1F,
            ExpOffset: 15);

        private static readonly FloatSpec floatSpec = new(ExpBitSize: 8, MantissaBitSize: 23,
            TotalSize: 32, MantissaMask: 0x7FFFFF, MantissaMsbMask: 0x7FFFFF, ExpMask: 0xFF,
            ExpOffset: 127);

        private static readonly FloatSpec doubleSpec =
            new(ExpBitSize: 11, MantissaBitSize: 52, TotalSize: 64, MantissaMask: 0xFFFFFFFFFFFFFL,
                MantissaMsbMask: 0x8000000000000L, ExpMask: 0x7FFL, ExpOffset: 1023);

        public static FloatInfo AnalyzeHalf(this Half value)
        {
            var bits = value.value;
            var rawExponent = (int)(bits >> halfSpec.MantissaBitSize & halfSpec.ExpMask);
            var mantissa = bits & halfSpec.MantissaMask;

            return new FloatInfo(
                Spec: halfSpec,
                Bits: bits,
                IsNegative: bits >> 15 == 1,
                IsPositiveInfinity: value.IsPositiveInfinity(),
                IsNegativeInfinity: value.IsNegativeInfinity(),
                IsQuietNaN: value.IsQuietNaN(),
                IsSignalingNaN: value.IsSignalingNaN(),
                RealExponent: rawExponent - halfSpec.ExpOffset + (rawExponent == 0
                    ? 1
                    : 0),
                Mantissa: mantissa,
                Significand: (ulong)(rawExponent == 0
                    ? mantissa
                    : (1 << halfSpec.MantissaBitSize) + mantissa),
                ExpBits: Convert.ToString(value: rawExponent, toBase: 2)
                                .PadLeft(totalWidth: halfSpec.ExpBitSize, paddingChar: '0'),
                MantissaBits: Convert.ToString(value: mantissa, toBase: 2)
                                     .PadLeft(totalWidth: halfSpec.MantissaBitSize,
                                          paddingChar: '0'),
                TypeName: FloatTypeKind.Half
            );
        }
        public static FloatInfo AnalyzeFloat(this float value)
        {
            var bits = BitConverter.SingleToInt32Bits(value: value);
            var rawExponent = (int)(bits >> floatSpec.MantissaBitSize & floatSpec.ExpMask);
            var mantissa = bits & floatSpec.MantissaMask;

            return new FloatInfo(
                Spec: floatSpec,
                Bits: bits,
                IsNegative: bits < 0,
                IsPositiveInfinity: Single.IsPositiveInfinity(f: value),
                IsNegativeInfinity: Single.IsNegativeInfinity(f: value),
                IsQuietNaN: Single.IsNaN(f: value) && (bits & floatSpec.MantissaMsbMask) != 0,
                IsSignalingNaN: Single.IsNaN(f: value) && (bits & floatSpec.MantissaMsbMask) == 0,
                RealExponent: rawExponent - floatSpec.ExpOffset + (rawExponent == 0
                    ? 1
                    : 0),
                Mantissa: mantissa,
                Significand: (ulong)(rawExponent == 0
                    ? mantissa
                    : (1 << floatSpec.MantissaBitSize) + mantissa),
                ExpBits: Convert.ToString(value: rawExponent, toBase: 2)
                                .PadLeft(totalWidth: floatSpec.ExpBitSize, paddingChar: '0'),
                MantissaBits: Convert.ToString(value: mantissa, toBase: 2)
                                     .PadLeft(totalWidth: floatSpec.MantissaBitSize,
                                          paddingChar: '0'),
                TypeName: FloatTypeKind.Float
            );
        }
        public static FloatInfo AnalyzeDouble(this double value)
        {
            var bits = BitConverter.DoubleToInt64Bits(value: value);
            var rawExponent = (int)(bits >> doubleSpec.MantissaBitSize & doubleSpec.ExpMask);
            var mantissa = bits & doubleSpec.MantissaMask;

            return new FloatInfo(
                Spec: doubleSpec,
                Bits: bits,
                IsNegative: bits < 0,
                IsPositiveInfinity: Double.IsPositiveInfinity(d: value),
                IsNegativeInfinity: Double.IsNegativeInfinity(d: value),
                IsQuietNaN: Double.IsNaN(d: value) && (bits & doubleSpec.MantissaMsbMask) != 0,
                IsSignalingNaN: Double.IsNaN(d: value) && (bits & doubleSpec.MantissaMsbMask) == 0,
                RealExponent: rawExponent - doubleSpec.ExpOffset + (rawExponent == 0
                    ? 1
                    : 0),
                Mantissa: mantissa,
                Significand: (ulong)(rawExponent == 0
                    ? mantissa
                    : (1L << doubleSpec.MantissaBitSize) + mantissa),
                ExpBits: Convert.ToString(value: rawExponent, toBase: 2)
                                .PadLeft(totalWidth: doubleSpec.ExpBitSize, paddingChar: '0'),
                MantissaBits: Convert.ToString(value: mantissa, toBase: 2)
                                     .PadLeft(totalWidth: doubleSpec.MantissaBitSize,
                                          paddingChar: '0'),
                TypeName: FloatTypeKind.Double
            );
        }

        public static string FormatAsRounding(this object obj, FloatInfo info,
            ReprContext context)
        {
            var config = context.Config;
            var roundingFormatString = "F" + (config.FloatPrecision > 0
                ? config.FloatPrecision
                : 0);
            return info.TypeName switch
            {
                FloatTypeKind.Half =>
                    $"{UnityMath.math.f16tof32(x: ((Half)obj).value).ToString(format: roundingFormatString)}",
                FloatTypeKind.Float =>
                    $"{((float)obj).ToString(format: roundingFormatString)}",
                FloatTypeKind.Double =>
                    $"{((double)obj).ToString(format: roundingFormatString)}",
                _ => throw new InvalidEnumArgumentException(message: "Invalid FloatTypeKind")
            };
        }
        public static string FormatAsGeneral(this object obj, FloatInfo info,
            ReprContext context)
        {
            return info.TypeName switch
            {
                FloatTypeKind.Half =>
                    $"{(Half)obj}",
                FloatTypeKind.Float =>
                    $"{(float)obj}",
                FloatTypeKind.Double =>
                    $"{(double)obj}",
                _ => throw new InvalidEnumArgumentException(message: "Invalid FloatTypeKind")
            };
        }
        public static string FormatAsScientific(this object obj, FloatInfo info,
            ReprContext context)
        {
            var config = context.Config;
            var scientificFormatString = "E" + (config.FloatPrecision > 0
                ? config.FloatPrecision - 1
                : 0);
            return info.TypeName switch
            {
                FloatTypeKind.Half =>
                    $"{((Half)obj).ToString(format: scientificFormatString)}",
                FloatTypeKind.Float =>
                    $"{((float)obj).ToString(format: scientificFormatString)}",
                FloatTypeKind.Double =>
                    $"{((double)obj).ToString(format: scientificFormatString)}",
                _ => throw new InvalidEnumArgumentException(message: "Invalid FloatTypeKind")
            };
        }
        public static string FormatAsExact(this object obj, FloatInfo info)
        {
            var realExponent = info.RealExponent - info.Spec.MantissaBitSize;
            var significand = info.Significand;
            var isNegative = info.IsNegative;
            var sign = isNegative
                ? "-"
                : "";
            if (significand == 0)
            {
                return $"{sign}0.0E0";
            }

            // Convert to exact decimal representation
            BigInteger numerator;
            int powerOf10Denominator;

            if (realExponent >= 0)
            {
                numerator = significand * BigInteger.Pow(value: 2, exponent: realExponent);
                powerOf10Denominator = 0;
            }
            else
            {
                // We want enough decimal places to represent 1/2^(-binaryExponent) exactly
                // Since 2^n × 5^n = spec.MantissaBitSize^n, we need n = -binaryExponent decimal places
                powerOf10Denominator = -realExponent;
                numerator = significand * BigInteger.Pow(value: 5, exponent: powerOf10Denominator);
            }

            // Now we have: numerator / halfSpec.MantissaBitSize^powerOf10Denominator
            var numeratorStr = numerator.ToString(provider: CultureInfo.InvariantCulture);
            var realPowerOf10 = numeratorStr.Length - powerOf10Denominator - 1;
            var integerPart = numeratorStr.Substring(startIndex: 0, length: 1);
            var fractionalPart = numeratorStr.Substring(startIndex: 1)
                                             .TrimEnd(trimChar: '0')
                                             .PadLeft(totalWidth: 1, paddingChar: '0');
            return $"{sign}{integerPart}.{fractionalPart}E{realPowerOf10}";
        }
    }

    internal static class UnityHalfExtensions
    {
        // Half-precision float bit layout (IEEE 754):
        // Sign: 1 bit (bit 15)
        // Exponent: 5 bits (bits 14-10) 
        // Mantissa: 10 bits (bits 9-0)
        private const ushort ExponentMask = 0x7C00; // 0111 1100 0000 0000
        private const ushort MantissaMask = 0x03FF; // 0000 0011 1111 1111
        private const ushort PositiveInfinityBits = 0x7C00;
        private const ushort NegativeInfinityBits = 0xFC00;
        private const ushort ExponentAllOnes = 0x7C00;

        public static bool IsPositiveInfinity(this Half value)
        {
            return value.value == PositiveInfinityBits;
        }
        public static bool IsNegativeInfinity(this Half value)
        {
            return value.value == NegativeInfinityBits;
        }
        public static bool IsQuietNaN(this Half value)
        {
            // Quiet NaN: exponent = all 1s, mantissa MSB = 1, rest can be anything
            return (value.value & ExponentMask) == ExponentAllOnes &&
                   (value.value & MantissaMask) != 0 &&
                   (value.value & 0x0200) != 0; // Check mantissa MSB (bit 9)
        }
        public static bool IsSignalingNaN(this Half value)
        {
            // Signaling NaN: exponent = all 1s, mantissa MSB = 0, but mantissa != 0
            return (value.value & ExponentMask) == ExponentAllOnes &&
                   (value.value & MantissaMask) != 0 &&
                   (value.value & 0x0200) == 0; // Check mantissa MSB (bit 9)
        }

        public static bool IsFinite(this Half value)
        {
            return (value.value & ExponentMask) != ExponentAllOnes;
        }

        public static bool IsNormal(this Half value)
        {
            var exp = (ushort)(value.value & ExponentMask);
            return exp != 0 && exp != ExponentAllOnes;
        }

        public static bool IsSubnormal(this Half value)
        {
            return (value.value & ExponentMask) == 0 && (value.value & MantissaMask) != 0;
        }

        public static bool IsZero(this Half value)
        {
            return (value.value & 0x7FFF) == 0; // Ignore sign bit
        }

        public static string ToString(this Half value, string format)
        {
            return UnityMath.math
                            .f16tof32(x: value.value)
                            .ToString(format: format);
        }
    }
}