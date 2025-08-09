using System;
using System.Runtime.CompilerServices;
using DebugUtils.Unity.Repr.Attributes;
using DebugUtils.Unity.Repr.Extensions;
using DebugUtils.Unity.Repr.Interfaces;
using DebugUtils.Unity.Repr.TypeHelpers;
using Unity.Plastic.Newtonsoft.Json.Linq;

namespace DebugUtils.Unity.Repr.Formatters
{
    [ReprFormatter(typeof(Array))]
    [ReprOptions(needsPrefix: true)]
    internal class ArrayFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return "<Max Depth Reached>";
            }

            var array = (Array)obj;
            // Apply container defaults if configured
            context = context.WithContainerConfig();

            var rank = array.Rank;
            var content = array.ArrayToReprRecursive(indices: new int[rank], dimension: 0,
                context: context);
            return content;
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var array = (Array)obj;
            var type = array.GetType();

            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return new JObject
                {
                    [propertyName: "type"] = type.GetReprTypeName(),
                    [propertyName: "kind"] = type.GetTypeKind(),
                    [propertyName: "maxDepthReached"] = "true",
                    [propertyName: "depth"] = context.Depth
                };
            }

            var result = new JObject();
            var elementType = array.GetType()
                                   .GetElementType()
                                  ?.GetReprTypeName() ?? "object";
            result.Add(propertyName: "type", value: type.GetReprTypeName());
            result.Add(propertyName: "kind", value: type.GetTypeKind());
            result.Add(propertyName: "hashCode",
                value: $"0x{RuntimeHelpers.GetHashCode(o: obj):X8}");
            var dimensions = new JArray();
            for (var i = 0; i < array.Rank; i += 1)
            {
                dimensions.Add(item: array.GetLength(dimension: i));
            }

            result.Add(propertyName: "rank", value: array.Rank);
            result.Add(propertyName: "dimensions", value: dimensions);
            result.Add(propertyName: "elementType", value: elementType);
            // Apply container defaults if configured
            context = context.WithContainerConfig();

            var rank = array.Rank;
            var content = array.ArrayToHierarchicalReprRecursive(indices: new int[rank],
                dimension: 0,
                context: context);
            result.Add(propertyName: "value", value: content);
            return result;
        }
    }
}