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
            var type = obj.GetType();
            return new JObject
            {
                [propertyName: "type"] = type.GetReprTypeName(),
                [propertyName: "kind"] = type.GetTypeKind(),
                [propertyName: "value"] = ToRepr(obj: obj, context: context)
            };
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
            var type = obj.GetType();
            return new JObject
            {
                [propertyName: "type"] = type.GetReprTypeName(),
                [propertyName: "kind"] = type.GetTypeKind(),
                [propertyName: "value"] = ToRepr(obj: obj, context: context)
            };
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
            return new JObject
            {
                [propertyName: "type"] = "Version",
                [propertyName: "kind"] = "class",
                [propertyName: "major"] = v.Major,
                [propertyName: "minor"] = v.Minor,
                [propertyName: "build"] = v.Build,
                [propertyName: "revision"] = v.Revision
            };
        }
    }
}