namespace DebugUtils.Unity.Repr.Models
{
    /// <summary>
    ///     Encapsulates IEEE 754 floating-point format specifications.
    ///     Contains bit masks and offsets needed for precise floating-point analysis.
    /// </summary>
    internal readonly struct FloatSpec
    {
        public readonly int ExpBitSize;
        public readonly int MantissaBitSize;
        public readonly int TotalSize;
        public readonly long MantissaMask;
        public readonly long MantissaMsbMask;
        public readonly long ExpMask;
        public readonly int ExpOffset;

        public FloatSpec(int expBitSize, int mantissaBitSize, int totalSize,
            long mantissaMask, long mantissaMsbMask, long expMask,
            int expOffset)
        {
            ExpBitSize = expBitSize;
            MantissaBitSize = mantissaBitSize;
            TotalSize = totalSize;
            MantissaMask = mantissaMask;
            MantissaMsbMask = mantissaMsbMask;
            ExpMask = expMask;
            ExpOffset = expOffset;
        }
    }
}