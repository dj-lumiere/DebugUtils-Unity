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
    /// <summary>
    ///     The default object formatter that handles any type not specifically registered.
    ///     It uses reflection to represent the object's fields and properties based on ViewMode.
    /// </summary>
    [ReprOptions(needsPrefix: true)]
    internal class ObjectFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            context = context.WithContainerConfig();
            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return "<Max Depth Reached>";
            }

            var members = obj.GetObjectMembers(context: context);
            var parts = new List<string>();
            foreach (var member in members.PublicFields)
            {
                parts.Add(item: obj.ToReprParts(f: member, context: context));
            }

            foreach (var member in members.PublicAutoProps)
            {
                parts.Add(item: obj.ToReprParts(pair: member, context: context));
            }

            if (context.Config.MaxMemberTimeMs > 0)
            {
                foreach (var member in members.PublicProperties)
                {
                    parts.Add(item: obj.ToReprParts(p: member, context: context));
                }
            }

            foreach (var member in members.PrivateFields)
            {
                parts.Add(item: obj.ToPrivateReprParts(f: member, context: context));
            }

            foreach (var member in members.PrivateAutoProps)
            {
                parts.Add(item: obj.ToPrivateReprParts(pair: member, context: context));
            }

            if (context.Config.MaxMemberTimeMs > 0)
            {
                foreach (var member in members.PrivateProperties)
                {
                    parts.Add(item: obj.ToPrivateReprParts(p: member, context: context));
                }
            }

            if (members.Truncated)
            {
                parts.Add(item: "...");
            }

            return String.Join(separator: ", ", values: parts);
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            context = context.WithContainerConfig();
            var type = obj.GetType();
            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return type.CreateMaxDepthReachedJson(depth: context.Depth);
            }

            var result = new JObject();
            result.Add(propertyName: "type", value: type.GetReprTypeName());
            result.Add(propertyName: "kind", value: type.GetTypeKind());
            if (!type.IsValueType)
            {
                result.Add(propertyName: "hashCode",
                    value: $"0x{RuntimeHelpers.GetHashCode(o: obj):X8}");
            }

            var members = obj.GetObjectMembers(context: context);
            foreach (var (k, v) in
                     members.PublicFields.Select(p => obj.ToReprTreeParts(p, context)))
            {
                result.Add(k, v);
            }

            foreach (var (k, v) in members.PublicAutoProps.Select(p =>
                         obj.ToReprTreeParts(p, context)))
            {
                result.Add(k, v);
            }

            foreach (var (k, v) in members.PublicProperties.Select(p =>
                         obj.ToReprTreeParts(p, context)))
            {
                result.Add(k, v);
            }

            foreach (var (k, v) in members.PrivateFields.Select(p =>
                         obj.ToPrivateReprTreeParts(p, context)))
            {
                result.Add(k, v);
            }

            foreach (var (k, v) in members.PrivateAutoProps.Select(p =>
                         obj.ToPrivateReprTreeParts(p, context)))
            {
                result.Add(k, v);
            }

            foreach (var (k, v) in members.PrivateProperties.Select(p =>
                         obj.ToPrivateReprTreeParts(p, context)))
            {
                result.Add(k, v);
            }

            return result;
        }
    }
}