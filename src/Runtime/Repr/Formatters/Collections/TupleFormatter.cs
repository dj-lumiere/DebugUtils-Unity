using System.Runtime.CompilerServices;
using System.Text;
using DebugUtils.Unity.Repr.Attributes;
using DebugUtils.Unity.Repr.Interfaces;
using DebugUtils.Unity.Repr.TypeHelpers;
using Newtonsoft.Json.Linq;

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
            // Apply container defaults if configured
            context = context.WithContainerConfig();

            var sb = new StringBuilder();
            sb.Append(value: '(');
            for (var i = 0; i < tuple.Length; i++)
            {
                if (context.Config.MaxElementsPerCollection >= 0 &&
                    i >= context.Config.MaxElementsPerCollection)
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

            if (context.Config.MaxElementsPerCollection >= 0 &&
                tuple.Length > context.Config.MaxElementsPerCollection)
            {
                var truncatedItemCount = tuple.Length -
                                         context.Config.MaxElementsPerCollection;
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
                return new JObject
                {
                    [propertyName: "type"] = new JValue(value: type.GetReprTypeName()),
                    [propertyName: "kind"] = new JValue(value: type.GetTypeKind()),
                    [propertyName: "maxDepthReached"] = new JValue(value: "true"),
                    [propertyName: "depth"] = new JValue(value: context.Depth)
                };
            }

            var result = new JObject();
            result.Add(propertyName: "type", value: new JValue(value: type.GetReprTypeName()));
            result.Add(propertyName: "kind", value: new JValue(value: type.GetTypeKind()));
            if (!type.IsValueType)
            {
                result.Add(propertyName: "hashCode",
                    value: new JValue(value: $"0x{RuntimeHelpers.GetHashCode(o: obj):X8}"));
            }

            result.Add(propertyName: "length", value: new JValue(value: tuple.Length));

            var entries = new JArray();
            for (var i = 0; i < tuple.Length; i++)
            {
                if (context.Config.MaxElementsPerCollection >= 0 &&
                    i >= context.Config.MaxElementsPerCollection)
                {
                    break;
                }

                entries.Add(item: tuple[index: i]
                   .FormatAsJToken(context: context.WithIncrementedDepth()));
            }

            if (context.Config.MaxElementsPerCollection >= 0 &&
                tuple.Length > context.Config.MaxElementsPerCollection)
            {
                var truncatedItemCount = tuple.Length -
                                         context.Config.MaxElementsPerCollection;
                entries.Add(item: $"... ({truncatedItemCount} more items)");
            }

            result.Add(propertyName: "value", value: entries);
            return result;
        }
    }
}