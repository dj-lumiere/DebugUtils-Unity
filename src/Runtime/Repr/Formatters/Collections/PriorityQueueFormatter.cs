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
    [ReprOptions(needsPrefix: true)]
    internal class PriorityQueueFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return "<Max Depth Reached>";
            }

            var type = obj.GetType();
            var countProperty = type.GetProperty(name: "Count");
            var count = (int)(countProperty?.GetValue(obj: obj) ?? 0);

            // Apply container defaults if configured
            context = context.WithContainerConfig();

            var unorderedItemsProperty = type.GetProperty(name: "UnorderedItems");
            var unorderedItems = (IEnumerable)unorderedItemsProperty!.GetValue(obj: obj)!;

            var items = new List<string>();
            var i = 0;
            var hitLimit = false;

            foreach (var item in unorderedItems)
            {
                if (context.Config.MaxElementsPerCollection >= 0 &&
                    i >= context.Config.MaxElementsPerCollection)
                {
                    hitLimit = true;
                    break;
                }

                if (item is ITuple tuple && tuple.Length == 2)
                {
                    var priority = tuple[index: 1];
                    var element = tuple[index: 0];
                    items.Add(
                        item:
                        $"{element.Repr(context: context.WithIncrementedDepth())} (priority: {priority.Repr(context: context.WithIncrementedDepth())})");
                }

                i += 1;
            }

            if (hitLimit)
            {
                var remainingCount = count - context.Config.MaxElementsPerCollection;
                if (remainingCount > 0)
                {
                    items.Add(item: $"... ({remainingCount} more items)");
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
            var type = obj.GetType();
            var countProperty = type.GetProperty(name: "Count");
            var count = (int)(countProperty?.GetValue(obj: obj) ?? 0);

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
            result.Add("type", type.GetReprTypeName());
            result.Add("kind", type.GetTypeKind());
            result.Add("hashCode", $"0x{RuntimeHelpers.GetHashCode(o: obj):X8}");
            result.Add("count", count);

            // Get generic type arguments for element and priority types
            if (type.IsGenericType)
            {
                var genericArgs = type.GetGenericArguments();
                if (genericArgs.Length >= 2)
                {
                    result.Add("elementType", genericArgs[0]
                       .GetReprTypeName());
                    result.Add("priorityType", genericArgs[1]
                       .GetReprTypeName());
                }
            }

            var entries = new JArray();
            var unorderedItemsProperty = type.GetProperty(name: "UnorderedItems");
            var unorderedItems = (IEnumerable)unorderedItemsProperty!.GetValue(obj: obj)!;

            var i = 0;
            var hitLimit = false;

            foreach (var item in unorderedItems)
            {
                if (context.Config.MaxElementsPerCollection >= 0 &&
                    i >= context.Config.MaxElementsPerCollection)
                {
                    hitLimit = true;
                    break;
                }

                if (item is ITuple tuple && tuple.Length == 2)
                {
                    var priority = tuple[index: 1]
                       .FormatAsJToken(context: context.WithIncrementedDepth());
                    var element = tuple[index: 0]
                       .FormatAsJToken(context: context.WithIncrementedDepth());
                    var entry = new JObject();
                    entry.Add("element", element);
                    entry.Add("priority", priority);
                    entries.Add(entry);
                }

                i += 1;
            }

            if (hitLimit)
            {
                var remainingCount = count - context.Config.MaxElementsPerCollection;
                if (remainingCount > 0)
                {
                    entries.Add($"... ({remainingCount} more items)");
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