using System;
using System.ComponentModel;
using System.Numerics;
using DebugUtils.Unity.Repr.Attributes;
using DebugUtils.Unity.Repr.Extensions;
using DebugUtils.Unity.Repr.Interfaces;
using DebugUtils.Unity.Repr.TypeHelpers;
using Newtonsoft.Json.Linq;

namespace DebugUtils.Unity.Repr.Formatters
{
    [ReprFormatter(typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int),
        typeof(uint), typeof(long), typeof(ulong), typeof(BigInteger)
        #if NET7_0_OR_GREATER
    , typeof(Int128), typeof(UInt128)
        #endif
    )]
    [ReprOptions(needsPrefix: true)]
    internal class IntegerFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            if (!String.IsNullOrEmpty(value: context.Config.IntFormatString))
            {
                return context.Config.IntFormatString switch
                {
                    "HB" => obj.FormatAsHexBytes(),
                    "B" or "b" => obj.FormatAsBinary(),
                    "X" => obj.FormatAsHex(),
                    "x" => obj.FormatAsHex()
                              .ToLowerInvariant(),
                    _ when (context.Config.IntFormatString.StartsWith("B")) ||
                           (context.Config.IntFormatString.StartsWith("b")) => obj
                       .FormatAsBinaryWithPadding(context.Config.IntFormatString),
                    _ when (context.Config.IntFormatString.StartsWith("X")) => obj
                       .FormatAsHexWithPadding(context.Config.IntFormatString),
                    _ when (context.Config.IntFormatString.StartsWith("x")) => obj
                       .FormatAsHexWithPadding(context.Config.IntFormatString)
                       .ToLowerInvariant(),
                    _ => obj switch
                    {
                        byte b => b.ToString(format: context.Config.IntFormatString),
                        sbyte sb => sb.ToString(format: context.Config.IntFormatString),
                        short s => s.ToString(format: context.Config.IntFormatString),
                        ushort us => us.ToString(format: context.Config.IntFormatString),
                        int i => i.ToString(format: context.Config.IntFormatString),
                        uint ui => ui.ToString(format: context.Config.IntFormatString),
                        long l => l.ToString(format: context.Config.IntFormatString),
                        ulong ul => ul.ToString(format: context.Config.IntFormatString),
                        BigInteger bi => bi.ToString(format: context.Config.IntFormatString),
                        _ => throw new InvalidEnumArgumentException(message: "Invalid Repr Config")
                    }
                };
            }
            return context.Config.IntMode switch
            {
                IntReprMode.Binary => obj.FormatAsBinary(),
                IntReprMode.Decimal => obj.ToString()!,
                IntReprMode.Hex => obj.FormatAsHex(),
                IntReprMode.HexBytes => obj.FormatAsHexBytes(),
                _ => throw new InvalidEnumArgumentException(message: "Invalid Repr Config")
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