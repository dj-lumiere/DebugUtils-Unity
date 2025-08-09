using System;
using Newtonsoft.Json.Linq;
using DebugUtils.Unity.Repr.Attributes;
using DebugUtils.Unity.Repr.Interfaces;
using DebugUtils.Unity.Repr.TypeHelpers;

// ReSharper disable BuiltInTypeReferenceStyle

namespace DebugUtils.Unity.Repr.Formatters
{
    [ReprFormatter(typeof(IntPtr))]
    [ReprOptions(needsPrefix: true)]
    internal class IntPtrFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            return IntPtr.Size == 4
                ? $"0x{((IntPtr)obj).ToInt32():X8}"
                : $"0x{((IntPtr)obj).ToInt64():X16}";
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

    [ReprFormatter(typeof(UIntPtr))]
    [ReprOptions(needsPrefix: true)]
    internal class UIntPtrFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            return IntPtr.Size == 4
                ? $"0x{((UIntPtr)obj).ToUInt32():X8}"
                : $"0x{((UIntPtr)obj).ToUInt64():X16}";
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var result = new JObject();
            var type = obj.GetType();
            result.Add(propertyName: "type", value: new JValue(value: type.GetReprTypeName()));
            result.Add(propertyName: "kind", value: new JValue(value: type.GetTypeKind()));
            result.Add(propertyName: "value",
                value: new JValue(value: ToRepr(obj: obj, context: context)));
            ;
            return result;
        }
    }
}