﻿using System;
using System.ComponentModel;
using DebugUtils.Unity.Repr.Attributes;
using DebugUtils.Unity.Repr.Extensions;
using DebugUtils.Unity.Repr.Interfaces;
using DebugUtils.Unity.Repr.TypeHelpers;
using Newtonsoft.Json.Linq;

namespace DebugUtils.Unity.Repr.Formatters
{
    [ReprFormatter(typeof(decimal))]
    [ReprOptions(needsPrefix: true)]
    internal class DecimalFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var dec = (decimal)obj;
            // Get the internal bits
            var bits = Decimal.GetBits(d: dec);
            var config = context.Config;

            // Extract components
            var lo = (uint)bits[0]; // Low 32 bits of 96-bit integer
            var mid = (uint)bits[1]; // Middle 32 bits  
            var hi = (uint)bits[2]; // High 32 bits
            var flags = bits[3]; // Scale and sign
            var scale = (byte)(flags >> 16); // How many digits after decimal
            var isNegative = (flags & 0x80000000) != 0;
            var scaleBits = Convert.ToString(value: scale, toBase: 2)
                                   .PadLeft(totalWidth: 8, paddingChar: '0');
            var hiBits = Convert.ToString(value: hi, toBase: 2)
                                .PadLeft(totalWidth: 32, paddingChar: '0');
            var midBits = Convert.ToString(value: mid, toBase: 2)
                                 .PadLeft(totalWidth: 32, paddingChar: '0');
            var loBits = Convert.ToString(value: lo, toBase: 2)
                                .PadLeft(totalWidth: 32, paddingChar: '0');

            return config.FloatMode switch
            {
                FloatReprMode.HexBytes =>
                    $"0x{flags:X8}{hi:X8}{mid:X8}{lo:X8}",
                FloatReprMode.BitField =>
                    $"{(isNegative ? 1 : 0)}|{scaleBits}|{hiBits}{midBits}{loBits}",
                FloatReprMode.Round =>
                    $"{dec.ToString(format: "F" + (config.FloatPrecision > 0 ? config.FloatPrecision : 0))}",
                FloatReprMode.Scientific =>
                    $"{dec.ToString(format: "E" + (config.FloatPrecision > 0 ? config.FloatPrecision - 1 : 0))}",
                FloatReprMode.Exact => $"{dec.FormatAsExact()}",
                FloatReprMode.General => $"{dec}",
                _ => throw new InvalidEnumArgumentException(message: "Invalid FloatReprMode")
            };
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var result = new JObject();
            var type = obj.GetType();
            result.Add(propertyName: "type", value: new JValue(value: type.GetReprTypeName()));
            result.Add(propertyName: "kind", value: new JValue(value: type.GetTypeKind()));
            result.Add(propertyName: "value",
                value: new JValue(value: ToRepr(obj: obj, context: context)));
            return result;
        }
    }
}