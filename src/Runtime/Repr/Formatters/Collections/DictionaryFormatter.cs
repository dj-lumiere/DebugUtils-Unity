using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DebugUtils.Unity.Repr.Attributes;
using DebugUtils.Unity.Repr.Interfaces;
using DebugUtils.Unity.Repr.TypeHelpers;
using Newtonsoft.Json.Linq;

namespace DebugUtils.Unity.Repr.Formatters
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
                    [propertyName: "type"] = new JValue(value: type.GetReprTypeName()),
                    [propertyName: "kind"] = new JValue(value: type.GetTypeKind()),
                    [propertyName: "maxDepthReached"] = new JValue(value: "true"),
                    [propertyName: "depth"] = new JValue(value: context.Depth)
                };
            }

            var result = new JObject();
            var entries = new JArray();
            result.Add(propertyName: "type", value: new JValue(value: type.GetReprTypeName()));
            result.Add(propertyName: "kind", value: new JValue(value: type.GetTypeKind()));
            result.Add(propertyName: "hashCode",
                value: new JValue(value: $"0x{RuntimeHelpers.GetHashCode(o: obj):X8}"));
            var keyType = dict.GetType()
                              .GetGenericArguments()[0]
                              .GetReprTypeName();
            var valueType = dict.GetType()
                                .GetGenericArguments()[1]
                                .GetReprTypeName();
            result.Add(propertyName: "count", value: new JValue(value: dict.Count));
            result.Add(propertyName: "keyType", value: new JValue(value: keyType));
            result.Add(propertyName: "valueType", value: new JValue(value: valueType));
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
                    [propertyName: "key"] =
                        entry.Key.FormatAsJToken(context: context.WithIncrementedDepth()),
                    [propertyName: "value"] =
                        entry.Value?.FormatAsJToken(context: context.WithIncrementedDepth())
                };
                entries.Add(item: entryJson);
                count += 1;
            }

            if (context.Config.MaxElementsPerCollection >= 0 &&
                dict.Count > context.Config.MaxElementsPerCollection)
            {
                var truncatedItemCount = dict.Count -
                                         context.Config.MaxElementsPerCollection;
                entries.Add(item: $"... ({truncatedItemCount} more items)");
            }

            result.Add(propertyName: "value", value: entries);
            return result;
        }
    }
}