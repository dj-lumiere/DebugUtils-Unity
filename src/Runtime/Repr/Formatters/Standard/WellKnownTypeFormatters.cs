using System;
using DebugUtils.Unity.Repr.Attributes;
using DebugUtils.Unity.Repr.Interfaces;
using DebugUtils.Unity.Repr.TypeHelpers;
using Newtonsoft.Json.Linq;

namespace DebugUtils.Unity.Repr.Formatters
{
    [ReprFormatter(typeof(Guid))]
    [ReprOptions(needsPrefix: true)]
    internal class GuidFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            return $"{((Guid)obj).ToString()}";
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

    [ReprFormatter(typeof(Uri))]
    [ReprOptions(needsPrefix: true)]
    internal class UriFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            return $"{(Uri)obj}";
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

    [ReprFormatter(typeof(Version))]
    [ReprOptions(needsPrefix: true)]
    internal class VersionFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            return $"{((Version)obj).ToString()}";
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var v = (Version)obj;
            var result = new JObject();
            result.Add(propertyName: "type", value: new JValue(value: "Version"));
            result.Add(propertyName: "kind", value: new JValue(value: "class"));
            result.Add(propertyName: "major", value: new JValue(value: v.Major));
            result.Add(propertyName: "minor", value: new JValue(value: v.Minor));
            result.Add(propertyName: "build", value: new JValue(value: v.Build));
            result.Add(propertyName: "revision", value: new JValue(value: v.Revision));
            return result;
        }
    }
}