using System;
using System.Runtime.CompilerServices;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Attributes;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Interfaces;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Records;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.TypeHelpers;
using Unity.Plastic.Newtonsoft.Json.Linq;

namespace DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Formatters.Functions
{
    [ReprFormatter(typeof(Delegate))]
    [ReprOptions(needsPrefix: false)]
    internal class FunctionFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return "<Max Depth Reached>";
            }

            var del = (Delegate)obj;
            var functionDetails = del.Method.ToFunctionDetails();
            return functionDetails.ToString();
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var del = (Delegate)obj;
            var type = del.GetType();

            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return new JObject
                {
                    ["type"] = "Function",
                    ["kind"] = type.GetTypeKind(),
                    ["maxDepthReached"] = true,
                    ["depth"] = context.Depth
                };
            }

            var functionDetails = del.Method.ToFunctionDetails();
            var result = new JObject();
            result.Add("type", "Function");
            result.Add("hashCode", $"0x{RuntimeHelpers.GetHashCode(o: obj):X8}");
            result.Add("properties",
                functionDetails.FormatAsJToken(context: context));
            return result;
        }
    }

    [ReprFormatter(typeof(FunctionDetails))]
    [ReprOptions(needsPrefix: false)]
    internal class FunctionDetailsFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return "<Max Depth Reached>";
            }

            var details = (FunctionDetails)obj;
            return details.ToString();
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var details = (FunctionDetails)obj;
            var type = details.GetType();
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
            result.Add("name",
                details.Name);
            result.Add("parameters",
                details.Parameters.FormatAsJToken(context: context.WithIncrementedDepth()));
            result.Add("type",
                details.ReturnTypeReprName);
            result.Add("modifier",
                details.Modifiers.FormatAsJToken(
                    context: context.WithIncrementedDepth()));
            return result;
        }
    }

    [ReprFormatter(typeof(MethodModifiers))]
    [ReprOptions(needsPrefix: false)]
    internal class MethodModifiersFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return "<Max Depth Reached>";
            }

            var modifiers = (MethodModifiers)obj;
            return modifiers.ToString();
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var methodModifiers = (MethodModifiers)obj;
            var type = methodModifiers.GetType();
            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return new JObject
                {
                    ["type"] = type.GetReprTypeName(),
                    ["kind"] = type.GetTypeKind(),
                    ["maxDepthReached"] = true,
                    ["depth"] = context.Depth
                };
            }

            var modifiers = new JArray();

            foreach (var (condition, name) in new[]
                     {
                         (methodModifiers.IsPublic, "public"),
                         (methodModifiers.IsPrivate, "private"),
                         (methodModifiers.IsProtected, "protected"),
                         (methodModifiers.IsInternal, "internal"),
                         (methodModifiers.IsStatic, "static"),
                         (methodModifiers.IsAbstract, "abstract"),
                         (methodModifiers.IsVirtual, "virtual"),
                         (methodModifiers.IsOverride, "override"),
                         (methodModifiers.IsSealed, "sealed"),
                         (methodModifiers.IsAsync, "async"),
                         (methodModifiers.IsExtern, "extern"),
                         (methodModifiers.IsUnsafe, "unsafe"),
                         (methodModifiers.IsGeneric, "generic")
                     })
            {
                if (condition)
                {
                    modifiers.Add(name);
                }
            }

            return modifiers;
        }
    }

    [ReprFormatter(typeof(ParameterDetails))]
    [ReprOptions(needsPrefix: false)]
    internal class ParameterDetailsFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return "<Max Depth Reached>";
            }

            var details = (ParameterDetails)obj;
            return details.ToString();
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var details = (ParameterDetails)obj;
            var type = details.GetType();
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
            result.Add("name",
                details.Name);
            result.Add("type",
                details.TypeReprName);
            result.Add("modifier",
                details.Modifier);
            result.Add("defaultValue",
                details.DefaultValue.FormatAsJToken(
                    context: context.WithIncrementedDepth()));
            return result;
        }
    }
}