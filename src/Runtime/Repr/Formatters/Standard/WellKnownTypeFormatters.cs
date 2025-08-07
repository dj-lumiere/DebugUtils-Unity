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
            result.Add("type", type.GetReprTypeName());
            result.Add("kind", type.GetTypeKind());
            result.Add("value", ToRepr(obj: obj, context: context));
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
            result.Add("type", type.GetReprTypeName());
            result.Add("kind", type.GetTypeKind());
            result.Add("value", ToRepr(obj: obj, context: context));
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
            result.Add("type", "Version");
            result.Add("kind", "class");
            result.Add("major", v.Major);
            result.Add("minor", v.Minor);
            result.Add("build", v.Build);
            result.Add("revision", v.Revision);
            return result;
        }
    }
}