using System;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Attributes;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Interfaces;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Records;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.TypeHelpers;
using Unity.Plastic.Newtonsoft.Json.Linq;

namespace DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Formatters.Standard
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
            var result = new JObject();
            var type = obj.GetType();
            result.Add("type", type.GetReprTypeName());
            result.Add("kind", type.GetTypeKind());
            result.Add("value", ToRepr(obj: obj, context: context));
            return result;
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
            var numericValue = Convert.ChangeType(e, conversionType: underlyingType);
            return
                $"{e.GetReprTypeName()}.{e} ({numericValue.Repr(context: context)})";
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var e = (Enum)obj;
            var result = new JObject();
            var underlyingType = Enum.GetUnderlyingType(enumType: e.GetType());
            var numericValue = Convert.ChangeType(e, conversionType: underlyingType);

            result.Add("type", e.GetReprTypeName());
            result.Add("kind", "enum");
            result.Add("name", e.ToString());
            result.Add("value", numericValue.FormatAsJToken(context: context));
            return result;
        }
    }
}