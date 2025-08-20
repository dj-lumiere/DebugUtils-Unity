using System.Runtime.InteropServices;
using Unity.Mathematics;

public static class BitConverter
{
    public static half UInt16BitsToHalf(ushort bits)
    {
        return MemoryMarshal.Cast<ushort, half>(
            span: MemoryMarshal.CreateSpan(reference: ref bits, length: 1))[index: 0];
    }

    public static ushort HalfToUInt16Bits(half value)
    {
        var temp = value;
        return MemoryMarshal.Cast<half, ushort>(
            span: MemoryMarshal.CreateSpan(reference: ref temp, length: 1))[index: 0];
    }

    // Float (32-bit) conversions
    public static float UInt32BitsToSingle(uint bits)
    {
        return MemoryMarshal.Cast<uint, float>(
            span: MemoryMarshal.CreateSpan(reference: ref bits, length: 1))[index: 0];
    }

    public static uint SingleToUInt32Bits(float value)
    {
        return MemoryMarshal.Cast<float, uint>(
            span: MemoryMarshal.CreateSpan(reference: ref value, length: 1))[index: 0];
    }

    // Double (64-bit) conversions
    public static double UInt64BitsToDouble(ulong bits)
    {
        return MemoryMarshal.Cast<ulong, double>(
            span: MemoryMarshal.CreateSpan(reference: ref bits, length: 1))[index: 0];
    }

    public static ulong DoubleToUInt64Bits(double value)
    {
        return MemoryMarshal.Cast<double, ulong>(
            span: MemoryMarshal.CreateSpan(reference: ref value, length: 1))[index: 0];
    }
}