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
    [ReprFormatter(typeof(ITuple))]
    [ReprOptions(needsPrefix: false)]
    internal class TupleFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            // Apply container defaults if configured
            context = context.WithContainerConfig();
            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return "<Max Depth Reached>";
            }

            var tuple = (ITuple)obj;
            var sb = new StringBuilder();
            sb.Append(value: '(');
            for (var i = 0; i < tuple.Length; i += 1)
            {
                if (context.Config.MaxItemsPerContainer >= 0 &&
                    i >= context.Config.MaxItemsPerContainer)
                {
                    break;
                }

                if (i > 0)
                {
                    sb.Append(value: ", ");
                }

                sb.Append(value: tuple[index: i]
                   .Repr(context: context.WithIncrementedDepth()));
            }

            if (context.Config.MaxItemsPerContainer >= 0 &&
                tuple.Length > context.Config.MaxItemsPerContainer)
            {
                var truncatedItemCount = tuple.Length - context.Config.MaxItemsPerContainer;
                sb.Append(value: $"... {truncatedItemCount} more items");
            }

            sb.Append(value: ')');
            return sb.ToString();
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            // Apply container defaults if configured
            context = context.WithContainerConfig();
            var tuple = (ITuple)obj;
            var type = tuple.GetType();
            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return type.CreateMaxDepthReachedJson(depth: context.Depth);
            }

            var result = new JObject
            {
                {
                    "type",
                    type.GetReprTypeName()
                },
                {
                    "kind",
                    type.GetTypeKind()
                }
            };
            if (!type.IsValueType)
            {
                result.Add(propertyName: "hashCode",
                    value: $"0x{RuntimeHelpers.GetHashCode(o: obj):X8}");
            }

            result.Add(propertyName: "length", value: tuple.Length);
            var entries = new JArray();
            for (var i = 0; i < tuple.Length; i += 1)
            {
                if (context.Config.MaxItemsPerContainer >= 0 &&
                    i >= context.Config.MaxItemsPerContainer)
                {
                    break;
                }

                entries.Add(item: tuple[index: i]
                   .FormatAsJToken(context: context.WithIncrementedDepth()));
            }

            if (context.Config.MaxItemsPerContainer >= 0 &&
                tuple.Length > context.Config.MaxItemsPerContainer)
            {
                var truncatedItemCount = tuple.Length - context.Config.MaxItemsPerContainer;
                entries.Add(item: $"... ({truncatedItemCount} more items)");
            }

            result.Add(propertyName: "value", value: entries);
            return result;
        }
    }
}