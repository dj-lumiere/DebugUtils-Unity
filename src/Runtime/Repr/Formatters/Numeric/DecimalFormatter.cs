#nullable enable
using DebugUtils.Unity.Repr.Attributes;
using DebugUtils.Unity.Repr.Extensions;
using DebugUtils.Unity.Repr.Interfaces;
using DebugUtils.Unity.Repr.TypeHelpers;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
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
    [ReprFormatter(typeof(decimal))]
    [ReprOptions(needsPrefix: false)]
    internal class DecimalFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var dec = (decimal)obj;
            return context.Config.FloatFormatString switch
            {
                "HP" => dec.FormatAsHexPower(),
                "EX" => dec.FormatAsExact(),
                _ => dec.ToString(format: context.Config.FloatFormatString,
                    provider: context.Config.Culture)
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