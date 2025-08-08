using System;
using DebugUtils.Unity.Repr.Attributes;
using DebugUtils.Unity.Repr.Interfaces;
using DebugUtils.Unity.Repr.TypeHelpers;
using Unity.Plastic.Newtonsoft.Json.Linq;

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
            var result = new JObject();
            var type = obj.GetType();
            result.Add(propertyName: "type", value: type.GetReprTypeName());
            result.Add(propertyName: "kind", value: type.GetTypeKind());
            result.Add(propertyName: "value", value: ToRepr(obj: obj, context: context));
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
            var numericValue = Convert.ChangeType(value: e, conversionType: underlyingType);
            return
                $"{e.GetReprTypeName()}.{e} ({numericValue.Repr(context: context)})";
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var e = (Enum)obj;
            var result = new JObject();
            var underlyingType = Enum.GetUnderlyingType(enumType: e.GetType());
            var numericValue = Convert.ChangeType(value: e, conversionType: underlyingType);

            result.Add(propertyName: "type", value: e.GetReprTypeName());
            result.Add(propertyName: "kind", value: "enum");
            result.Add(propertyName: "name", value: e.ToString());
            result.Add(propertyName: "value",
                value: numericValue.FormatAsJToken(context: context));
            return result;
        }
    }
}