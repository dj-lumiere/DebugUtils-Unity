using System;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Attributes;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Interfaces;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Records;
using Unity.Plastic.Newtonsoft.Json.Linq;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.TypeHelpers;

// ReSharper disable BuiltInTypeReferenceStyle

namespace DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Formatters.Standard
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
            result.Add("type", type.GetReprTypeName());
            result.Add("kind", type.GetTypeKind());
            result.Add("value", ToRepr(obj: obj, context: context));
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
            result.Add("type", type.GetReprTypeName());
            result.Add("kind", type.GetTypeKind());
            result.Add("value", ToRepr(obj: obj, context: context));
            return result;
        }
    }
}