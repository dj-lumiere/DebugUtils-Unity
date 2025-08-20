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
    [ReprFormatter(typeof(IDictionary))]
    internal class DictionaryFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            // Apply container defaults if configured
            context = context.WithContainerConfig();
            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return "<Max Depth Reached>";
            }

            var dict = (IDictionary)obj;
            if (dict.Count == 0)
            {
                return "{}";
            }

            var items = new List<string>();
            var count = 0;
            foreach (DictionaryEntry entry in dict)
            {
                if (context.Config.MaxItemsPerContainer >= 0 &&
                    count >= context.Config.MaxItemsPerContainer)
                {
                    break;
                }

                var key = entry.Key?.Repr(context: context.WithIncrementedDepth()) ?? "null";
                var value = entry.Value?.Repr(context: context.WithIncrementedDepth()) ?? "null";
                items.Add(item: $"{key}: {value}");
                count += 1;
            }

            if (context.Config.MaxItemsPerContainer >= 0 &&
                dict.Count > context.Config.MaxItemsPerContainer)
            {
                var truncatedItemCount = dict.Count - context.Config.MaxItemsPerContainer;
                items.Add(item: $"... {truncatedItemCount} more items");
            }

            return "{" + String.Join(separator: ", ", values: items) + "}";
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            // Apply container defaults if configured
            context = context.WithContainerConfig();
            var dict = (IDictionary)obj;
            var type = dict.GetType();
            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return type.CreateMaxDepthReachedJson(depth: context.Depth);
            }

            var entries = new JArray();
            var keyType = dict.GetType()
                              .GetGenericArguments()[0]
                              .GetReprTypeName();
            var valueType = dict.GetType()
                                .GetGenericArguments()[1]
                                .GetReprTypeName();
            var count = 0;
            foreach (DictionaryEntry entry in dict)
            {
                if (context.Config.MaxItemsPerContainer >= 0 &&
                    count >= context.Config.MaxItemsPerContainer)
                {
                    break;
                }

                var entryJson = new JObject
                {
                    [propertyName: "key"] =
                        entry.Key.FormatAsJToken(context: context.WithIncrementedDepth()),
                    [propertyName: "value"] =
                        entry.Value.FormatAsJToken(context: context.WithIncrementedDepth())
                };
                entries.Add(item: entryJson);
                count += 1;
            }

            if (context.Config.MaxItemsPerContainer >= 0 &&
                dict.Count > context.Config.MaxItemsPerContainer)
            {
                var truncatedItemCount = dict.Count - context.Config.MaxItemsPerContainer;
                entries.Add(item: $"... ({truncatedItemCount} more items)");
            }

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
                    "count",
                    dict.Count
                },
                {
                    "keyType",
                    keyType
                },
                {
                    "valueType",
                    valueType
                },
                {
                    "value",
                    entries
                }
            };
        }
    }
}