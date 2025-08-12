using System;
using System.Collections.Generic;
using DebugUtils.Unity.Repr.Attributes;
using DebugUtils.Unity.Repr.Interfaces;
using DebugUtils.Unity.Repr.TypeHelpers;
using Newtonsoft.Json.Linq;

namespace DebugUtils.Unity.Repr.Formatters
{
    internal static class SpanFormatter
    {
        public static string ToRepr<T>(Span<T> obj, ReprContext context)
        {
            context = context.WithContainerConfig();
            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return "<Max Depth Reached>";
            }

            var items = new List<string>();
            var itemCount = obj.Length;
            var hitLimit = false;
            for (var i = 0; i < obj.Length; i += 1)
            {
                if (context.Config.MaxElementsPerCollection >= 0 &&
                    i >= context.Config.MaxElementsPerCollection)
                {
                    hitLimit = true;
                    break;
                }

                items.Add(item: obj[index: i]
                   .Repr(context: context.WithIncrementedDepth()));
            }

            if (hitLimit)
            {
                var remainingCount = itemCount - context.Config.MaxElementsPerCollection;
                if (remainingCount > 0)
                {
                    items.Add(item: $"... ({remainingCount} more items)");
                }
            }

            return "[" + String.Join(separator: ", ", values: items) + "]";
        }
        public static JToken ToReprTree<T>(Span<T> obj, ReprContext context)
        {
            context = context.WithContainerConfig();
            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return new JObject
                {
                    [propertyName: "type"] = new JValue(value: "Span"),
                    [propertyName: "kind"] = new JValue(value: "ref struct"),
                    [propertyName: "maxDepthReached"] = new JValue(value: "true"),
                    [propertyName: "depth"] = new JValue(value: context.Depth)
                };
            }

            var result = new JObject();
            var entries = new JArray();
            var itemCount = obj.Length;
            var hitLimit = false;
            result.Add(propertyName: "type", value: new JValue(value: "Span"));
            result.Add(propertyName: "kind", value: new JValue(value: "ref struct"));
            result.Add(propertyName: "length", value: new JValue(value: itemCount));
            result.Add(propertyName: "elementType",
                value: new JValue(value: typeof(T).GetReprTypeName()));
            ;

            for (var i = 0; i < obj.Length; i += 1)
            {
                if (context.Config.MaxElementsPerCollection >= 0 &&
                    i >= context.Config.MaxElementsPerCollection)
                {
                    hitLimit = true;
                    break;
                }

                entries.Add(item: obj[index: i]
                   .FormatAsJToken(context: context.WithIncrementedDepth()));
            }

            if (hitLimit)
            {
                var remainingCount = itemCount - context.Config.MaxElementsPerCollection;
                if (remainingCount > 0)
                {
                    entries.Add(item: new JValue(value: $"... ({remainingCount} more items)"));
                }
            }

            result.Add(propertyName: "value", value: entries);
            return result;
        }
    }

    internal static class ReadOnlySpanFormatter
    {
        public static string ToRepr<T>(ReadOnlySpan<T> obj, ReprContext context)
        {
            context = context.WithContainerConfig();
            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return "<Max Depth Reached>";
            }

            var items = new List<string>();
            var itemCount = obj.Length;
            var hitLimit = false;
            for (var i = 0; i < obj.Length; i += 1)
            {
                if (context.Config.MaxElementsPerCollection >= 0 &&
                    i >= context.Config.MaxElementsPerCollection)
                {
                    hitLimit = true;
                    break;
                }

                items.Add(item: obj[index: i]
                   .Repr(context: context.WithIncrementedDepth()));
            }

            if (hitLimit)
            {
                var remainingCount = itemCount - context.Config.MaxElementsPerCollection;
                if (remainingCount > 0)
                {
                    items.Add(item: $"... ({remainingCount} more items)");
                }
            }

            return "[" + String.Join(separator: ", ", values: items) + "]";
        }
        public static JToken ToReprTree<T>(ReadOnlySpan<T> obj, ReprContext context)
        {
            context = context.WithContainerConfig();
            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return new JObject
                {
                    [propertyName: "type"] = new JValue(value: "ReadOnlySpan"),
                    [propertyName: "kind"] = new JValue(value: "ref struct"),
                    [propertyName: "maxDepthReached"] = new JValue(value: "true"),
                    [propertyName: "depth"] = new JValue(value: context.Depth)
                };
            }

            var result = new JObject();
            var entries = new JArray();
            var itemCount = obj.Length;
            var hitLimit = false;
            result.Add(propertyName: "type", value: new JValue(value: "ReadOnlySpan"));
            result.Add(propertyName: "kind", value: new JValue(value: "ref struct"));
            result.Add(propertyName: "length", value: new JValue(value: itemCount));
            result.Add(propertyName: "elementType",
                value: new JValue(value: typeof(T).GetReprTypeName()));

            for (var i = 0; i < obj.Length; i += 1)
            {
                if (context.Config.MaxElementsPerCollection >= 0 &&
                    i >= context.Config.MaxElementsPerCollection)
                {
                    hitLimit = true;
                    break;
                }

                entries.Add(item: obj[index: i]
                   .FormatAsJToken(context: context.WithIncrementedDepth()));
            }

            if (hitLimit)
            {
                var remainingCount = itemCount - context.Config.MaxElementsPerCollection;
                if (remainingCount > 0)
                {
                    entries.Add(item: new JValue(value: $"... ({remainingCount} more items)"));
                }
            }

            result.Add(propertyName: "value", value: entries);
            return result;
        }
    }

    [ReprOptions(needsPrefix: true)]
    internal class MemoryFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var memoryType = obj.GetType();
            var toArrayMethod = memoryType.GetMethod(name: "ToArray");
            var array = (Array)toArrayMethod!.Invoke(obj: obj, parameters: null)!;
            context = context.WithContainerConfig();
            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return "<Max Depth Reached>";
            }

            var items = new List<string>();
            var itemCount = array.Length;
            var hitLimit = false;
            for (var i = 0; i < array.Length; i += 1)
            {
                if (context.Config.MaxElementsPerCollection >= 0 &&
                    i >= context.Config.MaxElementsPerCollection)
                {
                    hitLimit = true;
                    break;
                }

                items.Add(item: array.GetValue(index: i)
                                     .Repr(context: context.WithIncrementedDepth()));
            }

            if (!hitLimit)
            {
                return "[" + String.Join(separator: ", ", values: items) + "]";
            }

            var remainingCount = itemCount - context.Config.MaxElementsPerCollection;
            if (remainingCount > 0)
            {
                items.Add(item: $"... ({remainingCount} more items)");
            }

            return "[" + String.Join(separator: ", ", values: items) + "]";
        }
        public JToken ToReprTree(object obj, ReprContext context)
        {
            var memoryType = obj.GetType();
            var toArrayMethod = memoryType.GetMethod(name: "ToArray");
            var array = (Array)toArrayMethod!.Invoke(obj: obj, parameters: null)!;
            context = context.WithContainerConfig();
            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return new JObject
                {
                    [propertyName: "type"] = new JValue(value: "Memory"),
                    [propertyName: "kind"] = new JValue(value: "struct"),
                    [propertyName: "maxDepthReached"] = new JValue(value: "true"),
                    [propertyName: "depth"] = new JValue(value: context.Depth)
                };
            }

            var result = new JObject();
            var entries = new JArray();
            var itemCount = array.Length;
            var hitLimit = false;
            var elementType = array.GetType()
                                   .GetElementType()
                                  ?.GetReprTypeName() ?? "object";
            result.Add(propertyName: "type", value: new JValue(value: "Memory"));
            result.Add(propertyName: "kind", value: new JValue(value: "struct"));
            result.Add(propertyName: "length", value: new JValue(value: itemCount));
            result.Add(propertyName: "elementType", value: new JValue(value: elementType));

            for (var i = 0; i < array.Length; i += 1)
            {
                if (context.Config.MaxElementsPerCollection >= 0 &&
                    i >= context.Config.MaxElementsPerCollection)
                {
                    hitLimit = true;
                    break;
                }

                entries.Add(item: array.GetValue(index: i)
                                       .FormatAsJToken(context: context.WithIncrementedDepth()));
            }

            if (hitLimit)
            {
                var remainingCount = itemCount - context.Config.MaxElementsPerCollection;
                if (remainingCount > 0)
                {
                    entries.Add(item: new JValue(value: $"... ({remainingCount} more items)"));
                }
            }

            result.Add(propertyName: "value", value: entries);
            return result;
        }
    }

    [ReprOptions(needsPrefix: true)]
    internal class ReadOnlyMemoryFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var memoryType = obj.GetType();
            var toArrayMethod = memoryType.GetMethod(name: "ToArray");
            var array = (Array)toArrayMethod!.Invoke(obj: obj, parameters: null)!;
            context = context.WithContainerConfig();
            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return "<Max Depth Reached>";
            }

            var items = new List<string>();
            var itemCount = array.Length;
            var hitLimit = false;
            for (var i = 0; i < array.Length; i += 1)
            {
                if (context.Config.MaxElementsPerCollection >= 0 &&
                    i >= context.Config.MaxElementsPerCollection)
                {
                    hitLimit = true;
                    break;
                }

                items.Add(item: array.GetValue(index: i)
                                     .Repr(context: context.WithIncrementedDepth()));
            }

            if (hitLimit)
            {
                var remainingCount = itemCount - context.Config.MaxElementsPerCollection;
                if (remainingCount > 0)
                {
                    items.Add(item: $"... ({remainingCount} more items)");
                }
            }

            return "[" + String.Join(separator: ", ", values: items) + "]";
        }
        public JToken ToReprTree(object obj, ReprContext context)
        {
            var memoryType = obj.GetType();
            var toArrayMethod = memoryType.GetMethod(name: "ToArray");
            var array = (Array)toArrayMethod!.Invoke(obj: obj, parameters: null)!;
            context = context.WithContainerConfig();
            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return new JObject
                {
                    [propertyName: "type"] = new JValue(value: "ReadOnlyMemory"),
                    [propertyName: "kind"] = new JValue(value: "struct"),
                    [propertyName: "maxDepthReached"] = new JValue(value: "true"),
                    [propertyName: "depth"] = new JValue(value: context.Depth)
                };
            }

            var result = new JObject();
            var entries = new JArray();
            var itemCount = array.Length;
            var hitLimit = false;
            var elementType = array.GetType()
                                   .GetElementType()
                                  ?.GetReprTypeName() ?? "object";
            result.Add(propertyName: "type", value: new JValue(value: "ReadOnlyMemory"));
            result.Add(propertyName: "kind", value: new JValue(value: "struct"));
            result.Add(propertyName: "length", value: new JValue(value: itemCount));
            result.Add(propertyName: "elementType", value: new JValue(value: elementType));

            for (var i = 0; i < array.Length; i += 1)
            {
                if (context.Config.MaxElementsPerCollection >= 0 &&
                    i >= context.Config.MaxElementsPerCollection)
                {
                    hitLimit = true;
                    break;
                }

                entries.Add(item: array.GetValue(index: i)
                                       .FormatAsJToken(context: context.WithIncrementedDepth()));
            }

            if (hitLimit)
            {
                var remainingCount = itemCount - context.Config.MaxElementsPerCollection;
                if (remainingCount > 0)
                {
                    entries.Add(item: new JValue(value: $"... ({remainingCount} more items)"));
                }
            }

            result.Add(propertyName: "value", value: entries);
            return result;
        }
    }

    [ReprFormatter(typeof(Index))]
    [ReprOptions(needsPrefix: true)]
    internal class IndexFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var index = (Index)obj;
            return index.ToString();
        }
        public JToken ToReprTree(object obj, ReprContext context)
        {
            var index = (Index)obj;
            var result = new JObject
            {
                { "type", new JValue(value: "Index") },
                { "kind", new JValue(value: "struct") },
                { "value", new JValue(value: index.ToString()) },
                {
                    "isFromEnd", new JValue(value: index.IsFromEnd
                                                        .ToString()
                                                        .ToLowerInvariant())
                }
            };
            return result;
        }
    }

    [ReprFormatter(typeof(Range))]
    [ReprOptions(needsPrefix: true)]
    internal class RangeFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var range = (Range)obj;
            return range.ToString();
        }
        public JToken ToReprTree(object obj, ReprContext context)
        {
            var range = (Range)obj;
            var result = new JObject
            {
                { "type", new JValue(value: "Range") },
                { "kind", new JValue(value: "struct") },
                { "start", range.Start.FormatAsJToken(context: context) },
                { "end", range.End.FormatAsJToken(context: context) }
            };
            return result;
        }
    }
}