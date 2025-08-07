using System.Runtime.CompilerServices;
using System.Text;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Attributes;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Interfaces;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Records;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.TypeHelpers;
using Unity.Plastic.Newtonsoft.Json.Linq;

namespace DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Formatters.Collections
{
    [ReprFormatter(typeof(ITuple))]
    [ReprOptions(needsPrefix: false)]
    internal class TupleFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return "<Max Depth Reached>";
            }

            var tuple = (ITuple)obj;
            // Apply container defaults if configured
            context = context.WithContainerConfig();

            var sb = new StringBuilder();
            sb.Append('(');
            for (var i = 0; i < tuple.Length; i++)
            {
                if (context.Config.MaxElementsPerCollection >= 0 &&
                    i >= context.Config.MaxElementsPerCollection)
                {
                    break;
                }

                if (i > 0)
                {
                    sb.Append(", ");
                }

                sb.Append(tuple[index: i]
                   .Repr(context: context.WithIncrementedDepth()));
            }

            if (context.Config.MaxElementsPerCollection >= 0 &&
                tuple.Length > context.Config.MaxElementsPerCollection)
            {
                var truncatedItemCount = tuple.Length -
                                         context.Config.MaxElementsPerCollection;
                sb.Append($"... {truncatedItemCount} more items");
            }

            sb.Append(')');
            return sb.ToString();
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var tuple = (ITuple)obj;
            var type = tuple.GetType();
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
            if (!type.IsValueType)
            {
                result.Add("hashCode",
                    $"0x{RuntimeHelpers.GetHashCode(o: obj):X8}");
            }

            result.Add("length", tuple.Length);

            var entries = new JArray();
            for (var i = 0; i < tuple.Length; i++)
            {
                if (context.Config.MaxElementsPerCollection >= 0 &&
                    i >= context.Config.MaxElementsPerCollection)
                {
                    break;
                }

                entries.Add(tuple[index: i]
                   .FormatAsJToken(context: context.WithIncrementedDepth()));
            }

            if (context.Config.MaxElementsPerCollection >= 0 &&
                tuple.Length > context.Config.MaxElementsPerCollection)
            {
                var truncatedItemCount = tuple.Length -
                                         context.Config.MaxElementsPerCollection;
                entries.Add(item: $"... ({truncatedItemCount} more items)");
            }

            result.Add("value", entries);
            return result;
        }
    }
}