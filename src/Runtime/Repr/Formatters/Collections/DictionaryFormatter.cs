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
    [ReprFormatter(typeof(IDictionary))]
    internal class DictionaryFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return "<Max Depth Reached>";
            }

            var dict = (IDictionary)obj;
            // Apply container defaults if configured
            context = context.WithContainerConfig();

            if (dict.Count == 0)
            {
                return "{}";
            }

            var items = new List<string>();

            var count = 0;
            foreach (DictionaryEntry entry in dict)
            {
                if (context.Config.MaxElementsPerCollection >= 0 &&
                    count >= context.Config.MaxElementsPerCollection)
                {
                    break;
                }

                var key = entry.Key?.Repr(context: context.WithIncrementedDepth()) ?? "null";
                var value = entry.Value?.Repr(context: context.WithIncrementedDepth()) ?? "null";
                items.Add(item: $"{key}: {value}");
                count += 1;
            }


            if (context.Config.MaxElementsPerCollection >= 0 &&
                dict.Count > context.Config.MaxElementsPerCollection)
            {
                var truncatedItemCount = dict.Count -
                                         context.Config.MaxElementsPerCollection;
                items.Add(item: $"... {truncatedItemCount} more items");
            }

            return "{" + String.Join(separator: ", ", values: items) + "}";
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var dict = (IDictionary)obj;
            var type = dict.GetType();

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
            var keyType = dict.GetType()
                              .GetGenericArguments()[0]
                              .GetReprTypeName();
            var valueType = dict.GetType()
                                .GetGenericArguments()[1]
                                .GetReprTypeName();
            result.Add("count", dict.Count);
            result.Add("keyType", keyType);
            result.Add("valueType", valueType);
            var count = 0;
            foreach (DictionaryEntry entry in dict)
            {
                if (context.Config.MaxElementsPerCollection >= 0 &&
                    count >= context.Config.MaxElementsPerCollection)
                {
                    break;
                }

                var entryJson = new JObject
                {
                    ["key"] =
                        entry.Key.FormatAsJToken(context: context.WithIncrementedDepth()),
                    ["value"] =
                        entry.Value?.FormatAsJToken(context: context.WithIncrementedDepth()) ?? null
                };
                entries.Add(entryJson);
                count += 1;
            }

            if (context.Config.MaxElementsPerCollection >= 0 &&
                dict.Count > context.Config.MaxElementsPerCollection)
            {
                var truncatedItemCount = dict.Count -
                                         context.Config.MaxElementsPerCollection;
                entries.Add(item: $"... ({truncatedItemCount} more items)");
            }

            result.Add("value", entries);
            return result;
        }
    }
}