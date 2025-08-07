using System;
using System.Runtime.CompilerServices;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Attributes;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Interfaces;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Records;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.TypeHelpers;
using Unity.Plastic.Newtonsoft.Json.Linq;

namespace DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Formatters.Collections
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
                    ["type"] = type.GetReprTypeName(),
                    ["kind"] = type.GetTypeKind(),
                    ["maxDepthReached"] = "true",
                    ["depth"] = context.Depth
                };
            }

            var result = new JObject();
            var elementType = array.GetType()
                                   .GetElementType()
                                  ?.GetReprTypeName() ?? "object";
            result.Add("type", type.GetReprTypeName());
            result.Add("kind", type.GetTypeKind());
            result.Add("hashCode", $"0x{RuntimeHelpers.GetHashCode(o: obj):X8}");
            var dimensions = new JArray();
            for (var i = 0; i < array.Rank; i++)
            {
                dimensions.Add(array.GetLength(dimension: i));
            }

            result.Add("rank", array.Rank);
            result.Add("dimensions", dimensions);
            result.Add("elementType", elementType);
            // Apply container defaults if configured
            context = context.WithContainerConfig();

            var rank = array.Rank;
            var content = array.ArrayToHierarchicalReprRecursive(indices: new int[rank], dimension: 0,
                context: context);
            result.Add("value", content);
            return result;
        }
    }
}