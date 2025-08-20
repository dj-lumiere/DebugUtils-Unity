#nullable enable
using DebugUtils.Unity.Repr.Extensions;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
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
        public FloatSpec Spec { get; }
        public long Bits { get; }
        public int RealExponent { get; }
        public ulong Significand { get; }
        public FloatTypeKind TypeName { get; }

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

        public bool IsSubnormal => (Bits & Spec.ExpMask) == 0;
    }
}