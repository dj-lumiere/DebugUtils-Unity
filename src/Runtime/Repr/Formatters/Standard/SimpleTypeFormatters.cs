#nullable enable
using DebugUtils.Unity.Repr.Attributes;
using DebugUtils.Unity.Repr.Extensions;
using DebugUtils.Unity.Repr.Interfaces;
using DebugUtils.Unity.Repr.TypeHelpers;
using Newtonsoft.Json.Linq;
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

namespace DebugUtils.Unity.Repr.Formatters
{
    [ReprFormatter(typeof(bool))]
    [ReprOptions(needsPrefix: false)]
    internal class BoolFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            return (bool)obj
                ? "true"
                : "false";
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var type = obj.GetType();
            if (context.Depth > 0)
            {
                return ToRepr(obj: obj, context: context)!;
            }

            return new JObject
            {
                [propertyName: "type"] = type.GetReprTypeName(),
                [propertyName: "kind"] = type.GetTypeKind(),
                [propertyName: "value"] = ToRepr(obj: obj, context: context)
            };
        }
    }

    [ReprFormatter(typeof(Enum))]
    [ReprOptions(needsPrefix: false)]
    internal class EnumFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var e = (Enum)obj;
            var underlyingType = Enum.GetUnderlyingType(enumType: e.GetType());
            var numericValue = Convert.ChangeType(value: e, conversionType: underlyingType);
            return $"{e.GetReprTypeName()}.{e} ({numericValue.Repr(context: context)})";
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var e = (Enum)obj;
            var underlyingType = Enum.GetUnderlyingType(enumType: e.GetType());
            var numericValue = Convert.ChangeType(value: e, conversionType: underlyingType);
            return new JObject
            {
                [propertyName: "type"] = e.GetReprTypeName(),
                [propertyName: "kind"] = "enum",
                [propertyName: "name"] = e.ToString(),
                [propertyName: "value"] =
                    numericValue.FormatAsJToken(context: context.WithIncrementedDepth())
            };
        }
    }
}