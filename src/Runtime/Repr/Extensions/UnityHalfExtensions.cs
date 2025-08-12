using Unity.Mathematics;

namespace DebugUtils.Unity.Repr.Extensions
{
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

        public static bool IsPositiveInfinity(this half value)
        {
            return value.value == PositiveInfinityBits;
        }
        public static bool IsNegativeInfinity(this half value)
        {
            return value.value == NegativeInfinityBits;
        }
        public static bool IsQuietNaN(this half value)
        {
            // Quiet NaN: exponent = all 1s, mantissa MSB = 1, rest can be anything
            return (value.value & ExponentMask) == ExponentAllOnes &&
                   (value.value & MantissaMask) != 0 &&
                   (value.value & 0x0200) != 0; // Check mantissa MSB (bit 9)
        }
        public static bool IsSignalingNaN(this half value)
        {
            // Signaling NaN: exponent = all 1s, mantissa MSB = 0, but mantissa != 0
            return (value.value & ExponentMask) == ExponentAllOnes &&
                   (value.value & MantissaMask) != 0 &&
                   (value.value & 0x0200) == 0; // Check mantissa MSB (bit 9)
        }

        public static bool IsNaN(this half value)
        {
            return IsQuietNaN(value: value) || IsSignalingNaN(value: value);
        }

        public static bool IsFinite(this half value)
        {
            return (value.value & ExponentMask) != ExponentAllOnes;
        }

        public static bool IsNormal(this half value)
        {
            var exp = (ushort)(value.value & ExponentMask);
            return exp != 0 && exp != ExponentAllOnes;
        }

        public static bool IsSubnormal(this half value)
        {
            return (value.value & ExponentMask) == 0 && (value.value & MantissaMask) != 0;
        }

        public static bool IsZero(this half value)
        {
            return (value.value & 0x7FFF) == 0; // Ignore sign bit
        }

        public static string ToString(this half value, string format)
        {
            return math
                  .f16tof32(x: value.value)
                  .ToString(format: format);
        }
    }
}