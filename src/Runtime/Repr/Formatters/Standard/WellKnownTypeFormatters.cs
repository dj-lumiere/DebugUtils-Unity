using System;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Attributes;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Interfaces;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Records;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.TypeHelpers;
using Unity.Plastic.Newtonsoft.Json.Linq;

namespace DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Formatters.Standard
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
            result.Add(propertyName: "type", value: type.GetReprTypeName());
            result.Add(propertyName: "kind", value: type.GetTypeKind());
            result.Add(propertyName: "value", value: ToRepr(obj: obj, context: context));
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
            result.Add(propertyName: "type", value: type.GetReprTypeName());
            result.Add(propertyName: "kind", value: type.GetTypeKind());
            result.Add(propertyName: "value", value: ToRepr(obj: obj, context: context));
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
            result.Add(propertyName: "type", value: "Version");
            result.Add(propertyName: "kind", value: "class");
            result.Add(propertyName: "major", value: v.Major);
            result.Add(propertyName: "minor", value: v.Minor);
            result.Add(propertyName: "build", value: v.Build);
            result.Add(propertyName: "revision", value: v.Revision);
            return result;
        }
    }
}