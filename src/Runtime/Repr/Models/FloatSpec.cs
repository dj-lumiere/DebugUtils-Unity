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
    /// <summary>
    ///     Encapsulates IEEE 754 floating-point format specifications.
    ///     Contains bit masks and offsets needed for precise floating-point analysis.
    /// </summary>
    internal readonly struct FloatSpec
    {
        public int ExpBitSize { get; }
        public int MantissaBitSize { get; }
        public long MantissaMask { get; }
        public long MantissaMsbMask { get; }
        public long ExpMask { get; }
        public int ExpOffset { get; }

        public FloatSpec(int expBitSize, int mantissaBitSize, long mantissaMask,
            long mantissaMsbMask, long expMask, int expOffset)
        {
            ExpBitSize = expBitSize;
            MantissaBitSize = mantissaBitSize;
            MantissaMask = mantissaMask;
            MantissaMsbMask = mantissaMsbMask;
            ExpMask = expMask;
            ExpOffset = expOffset;
        }
    }
}