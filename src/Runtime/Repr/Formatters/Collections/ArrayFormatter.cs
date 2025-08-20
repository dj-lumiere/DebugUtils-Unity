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
    [ReprFormatter(typeof(Array))]
    [ReprOptions(needsPrefix: true)]
    internal class ArrayFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            // Apply container defaults if configured
            context = context.WithContainerConfig();
            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return "<Max Depth Reached>";
            }

            var array = (Array)obj;
            var rank = array.Rank;
            var content =
                array.ArrayToReprRecursive(indices: new int[rank], dimension: 0, context: context);
            return content;
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            // Apply container defaults if configured
            context = context.WithContainerConfig();
            var array = (Array)obj;
            var type = array.GetType();
            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return type.CreateMaxDepthReachedJson(depth: context.Depth);
            }

            var elementType = array.GetType()
                                   .GetElementType()
                                  ?.GetReprTypeName() ?? "object";
            var dimensions = new JArray();
            for (var i = 0; i < array.Rank; i += 1)
            {
                dimensions.Add(item: array.GetLength(dimension: i));
            }

            var rank = array.Rank;
            var content = array.ArrayToHierarchicalReprRecursive(indices: new int[rank],
                dimension: 0, context: context);
            return new JObject
            {
                {
                    "type",
                    type.GetReprTypeName()
                },
                {
                    "kind",
                    type.GetTypeKind()
                },
                {
                    "hashCode",
                    $"0x{RuntimeHelpers.GetHashCode(o: obj):X8}"
                },
                {
                    "rank",
                    array.Rank
                },
                {
                    "dimensions",
                    dimensions
                },
                {
                    "elementType",
                    elementType
                },
                {
                    "value",
                    content
                }
            };
        }
    }
}