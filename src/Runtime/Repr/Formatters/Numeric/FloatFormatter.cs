#nullable enable
using DebugUtils.Unity.Repr.Attributes;
using DebugUtils.Unity.Repr.Extensions;
using DebugUtils.Unity.Repr.Interfaces;
using DebugUtils.Unity.Repr.Models;
using DebugUtils.Unity.Repr.TypeHelpers;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace DebugUtils.Unity.Repr.Formatters
{
    [ReprFormatter(typeof(float), typeof(double)
        #if NET5_0_OR_GREATER
    , typeof(Half)
        #endif
    )]
    [ReprOptions(needsPrefix: false)]
    internal class FloatFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var info = AnalyzeFloat(obj: obj);
            return FormatWithCustomString(obj: obj, info: info,
                formatString: context.Config.FloatFormatString, culture: context.Config.Culture);
        }

        private static FloatInfo AnalyzeFloat(object obj)
        {
            return obj switch
            {
                #if NET5_0_OR_GREATER
            Half h => h.AnalyzeHalf(),
                #endif
                float f => f.AnalyzeFloat(),
                double d => d.AnalyzeDouble(),
                _ => throw new ArgumentException(message: "Invalid type")
            };
        }

        private static string FormatWithCustomString(object obj, FloatInfo info,
            string formatString, CultureInfo? culture)
        {
            return formatString switch
            {
                _ when info.IsPositiveInfinity => "Infinity",
                _ when info.IsNegativeInfinity => "-Infinity",
                _ when info.IsQuietNaN => FormatQuietNaN(info: info),
                _ when info.IsSignalingNaN => FormatSignalingNaN(info: info),
                "EX" => obj.FormatAsExact(info: info),
                "HP" => obj.FormatAsHexPower(info: info),
                _ => FormatWithBuiltInToString(obj: obj, formatString: formatString,
                    culture: culture)
            };
        }

        private static string FormatWithBuiltInToString(object obj, string formatString,
            CultureInfo? culture)
        {
            return obj switch
            {
                #if NET5_0_OR_GREATER
            Half h => h.ToString(format: formatString, provider: culture),
                #endif
                float f => f.ToString(format: formatString, provider: culture),
                double d => d.ToString(format: formatString, provider: culture),
                _ => throw new InvalidEnumArgumentException(message: "Invalid FloatTypeKind")
            };
        }

        private static string FormatQuietNaN(FloatInfo info)
        {
            return info.TypeName switch
            {
                FloatTypeKind.Half => $"QuietNaN(0x{info.Mantissa:X3})",
                FloatTypeKind.Float => $"QuietNaN(0x{info.Mantissa:X3})",
                FloatTypeKind.Double => $"QuietNaN(0x{info.Mantissa:X3})",
                _ => throw new InvalidEnumArgumentException(message: "Invalid FloatTypeKind")
            };
        }

        private static string FormatSignalingNaN(FloatInfo info)
        {
            return info.TypeName switch
            {
                FloatTypeKind.Half => $"SignalingNaN(0x{info.Mantissa:X3})",
                FloatTypeKind.Float => $"SignalingNaN(0x{info.Mantissa:X3})",
                FloatTypeKind.Double => $"SignalingNaN(0x{info.Mantissa:X3})",
                _ => throw new InvalidEnumArgumentException(message: "Invalid FloatTypeKind")
            };
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var type = obj.GetType();
            if (context.Depth > 0)
            {
                return obj.Repr(context: context)!;
            }

            return new JObject
            {
                [propertyName: "type"] = type.GetReprTypeName(),
                [propertyName: "kind"] = type.GetTypeKind(),
                [propertyName: "value"] = ToRepr(obj: obj, context: context)
            };
        }
    }
}