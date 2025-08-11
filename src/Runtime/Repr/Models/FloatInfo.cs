using System;

namespace DebugUtils.Unity.Repr.Models
{
    internal enum FloatTypeKind
    {
        Half,
        Float,
        Double
    }

    internal readonly struct FloatInfo
    {
        public readonly FloatSpec Spec;
        public readonly long Bits;
        public readonly int RealExponent;
        public readonly ulong Significand;
        public readonly FloatTypeKind TypeName;

        public FloatInfo(FloatSpec spec, long bits, int realExponent,
            ulong significand, FloatTypeKind typeName)
        {
            Spec = spec;
            Bits = bits;
            RealExponent = realExponent;
            Significand = significand;
            TypeName = typeName;
        }
        public bool IsNegative => Bits < 0;

        public long Mantissa => Bits & Spec.MantissaMask;

        public string ExpBits => Convert
                                .ToString(value: Bits >> Spec.MantissaBitSize & Spec.ExpMask,
                                     toBase: 2)
                                .PadLeft(totalWidth: Spec.ExpBitSize, paddingChar: '0');

        public string MantissaBits => Convert.ToString(value: Mantissa, toBase: 2)
                                             .PadLeft(totalWidth: Spec.MantissaBitSize,
                                                  paddingChar: '0');

        public bool IsPositiveInfinity
        {
            get
            {
                var rawExponent = Bits >> Spec.MantissaBitSize & Spec.ExpMask;
                var mantissa = Bits & Spec.MantissaMask;
                return !IsNegative && rawExponent == Spec.ExpMask && mantissa == 0;
            }
        }

        public bool IsNegativeInfinity
        {
            get
            {
                var rawExponent = Bits >> Spec.MantissaBitSize & Spec.ExpMask;
                var mantissa = Bits & Spec.MantissaMask;
                return IsNegative && rawExponent == Spec.ExpMask && mantissa == 0;
            }
        }

        public bool IsQuietNaN
        {
            get
            {
                var rawExponent = Bits >> Spec.MantissaBitSize & Spec.ExpMask;
                var mantissa = Bits & Spec.MantissaMask;
                return rawExponent == Spec.ExpMask && mantissa != 0 &&
                       (Bits & Spec.MantissaMsbMask) != 0;
            }
        }

        public bool IsSignalingNaN
        {
            get
            {
                var rawExponent = Bits >> Spec.MantissaBitSize & Spec.ExpMask;
                var mantissa = Bits & Spec.MantissaMask;
                return rawExponent == Spec.ExpMask && mantissa != 0 &&
                       (Bits & Spec.MantissaMsbMask) == 0;
            }
        }
    };
}