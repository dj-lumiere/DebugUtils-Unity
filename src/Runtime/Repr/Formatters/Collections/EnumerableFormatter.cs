using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Attributes;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Interfaces;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Records;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.TypeHelpers;
using Unity.Plastic.Newtonsoft.Json.Linq;

namespace DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Formatters.Collections
{
    [ReprFormatter(typeof(IEnumerable))]
    [ReprOptions(needsPrefix: true)]
    internal class EnumerableFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return "<Max Depth Reached>";
            }

            var list = (IEnumerable)obj;
            var type = list.GetType();
            // Apply container defaults if configured
            context = context.WithContainerConfig();

            var items = new List<string>();
            int? itemCount = null;

            if (type.GetProperty(name: "Count")
                   ?.GetValue(obj: obj) is { } value)
            {
                itemCount = (int)value;
            }

            var i = 0;
            var hitLimit = false;
            foreach (var item in list)
            {
                if (context.Config.MaxElementsPerCollection >= 0 &&
                    i >= context.Config.MaxElementsPerCollection)
                {
                    hitLimit = true;
                    break;
                }

                items.Add(item: item.Repr(context: context.WithIncrementedDepth()));
                i += 1;
            }

            if (hitLimit)
            {
                if (itemCount is not null)
                {
                    var remainingCount = itemCount - context.Config.MaxElementsPerCollection;
                    if (remainingCount > 0)
                    {
                        items.Add(item: $"... ({remainingCount} more items)");
                    }
                }
                else
                {
                    items.Add(item: "... (more items)");
                }
            }

            return "[" + String.Join(separator: ", ", values: items) + "]";
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var list = (IEnumerable)obj;
            var type = list.GetType();
            int? itemCount = null;

            if (type.GetProperty(name: "Count")
                   ?.GetValue(obj: obj) is { } value)
            {
                itemCount = (int)value;
            }

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
            var entries = new JArray();
            result.Add("type", type.GetReprTypeName());
            result.Add("kind", type.GetTypeKind());
            result.Add("hashCode", $"0x{RuntimeHelpers.GetHashCode(o: obj):X8}");
            if (itemCount is not null)
            {
                result.Add("count", itemCount);
            }

            if (list.GetType()
                    .GetGenericArguments()
                    .Length != 0)
            {
                var elementType = list.GetType()
                                      .GetGenericArguments()[0]
                                      .GetReprTypeName();
                result.Add("elementType", elementType);
            }

            var i = 0;
            var hitLimit = false;

            foreach (var item in list)
            {
                if (context.Config.MaxElementsPerCollection >= 0 &&
                    i >= context.Config.MaxElementsPerCollection)
                {
                    hitLimit = true;
                    break;
                }

                entries.Add(item: item.FormatAsJToken(context: context.WithIncrementedDepth()));
                i += 1;
            }

            if (hitLimit)
            {
                if (itemCount is not null)
                {
                    var remainingCount = itemCount - context.Config.MaxElementsPerCollection;
                    if (remainingCount > 0)
                    {
                        entries.Add($"... ({remainingCount} more items)");
                    }
                }
                else
                {
                    entries.Add("... (more items)");
                }
            }

            result.Add("value", entries);
            return result;
        }
    }
}