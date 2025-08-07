﻿namespace DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Formatters.Numeric
{
    /// <summary>
    ///     Encapsulates IEEE 754 floating-point format specifications.
    ///     Contains bit masks and offsets needed for precise floating-point analysis.
    /// </summary>
    internal record FloatSpec(
        int ExpBitSize,
        int MantissaBitSize,
        int TotalSize,
        long MantissaMask,
        long MantissaMsbMask,
        long ExpMask,
        int ExpOffset
    );
}