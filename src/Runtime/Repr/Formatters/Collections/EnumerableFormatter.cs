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
    [ReprFormatter(typeof(IEnumerable))]
    [ReprOptions(needsPrefix: true)]
    internal class EnumerableFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            // Apply container defaults if configured
            context = context.WithContainerConfig();
            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return "<Max Depth Reached>";
            }

            var list = (IEnumerable)obj;
            var type = list.GetType();

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
            // Apply container defaults if configured
            context = context.WithContainerConfig();
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
            if (itemCount is not null)
            {
                result.Add(propertyName: "count", value: new JValue(value: itemCount));
            }

            if (list.GetType()
                    .GetGenericArguments()
                    .Length != 0)
            {
                var elementType = list.GetType()
                                      .GetGenericArguments()[0]
                                      .GetReprTypeName();
                result.Add(propertyName: "elementType", value: new JValue(value: elementType));
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
                        entries.Add(item: $"... ({remainingCount} more items)");
                    }
                }
                else
                {
                    entries.Add(item: "... (more items)");
                }
            }

            result.Add(propertyName: "value", value: entries);
            return result;
        }
    }
}