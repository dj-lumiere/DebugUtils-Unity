using System;
using System.Runtime.CompilerServices;
using DebugUtils.Unity.Repr.Attributes;
using DebugUtils.Unity.Repr.Interfaces;
using DebugUtils.Unity.Repr.TypeHelpers;
using Unity.Plastic.Newtonsoft.Json.Linq;

namespace DebugUtils.Unity.Repr.Formatters
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
                    [propertyName: "type"] = "Function",
                    [propertyName: "kind"] = type.GetTypeKind(),
                    [propertyName: "maxDepthReached"] = true,
                    [propertyName: "depth"] = context.Depth
                };
            }

            var functionDetails = del.Method.ToFunctionDetails();
            var result = new JObject();
            result.Add(propertyName: "type", value: "Function");
            result.Add(propertyName: "hashCode",
                value: $"0x{RuntimeHelpers.GetHashCode(o: obj):X8}");
            result.Add(propertyName: "properties",
                value: functionDetails.FormatAsJToken(context: context));
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
                    [propertyName: "type"] = type.GetReprTypeName(),
                    [propertyName: "kind"] = type.GetTypeKind(),
                    [propertyName: "maxDepthReached"] = "true",
                    [propertyName: "depth"] = context.Depth
                };
            }

            var result = new JObject();
            result.Add(propertyName: "name",
                value: details.Name);
            result.Add(propertyName: "parameters",
                value: details.Parameters.FormatAsJToken(context: context.WithIncrementedDepth()));
            result.Add(propertyName: "type",
                value: details.ReturnTypeReprName);
            result.Add(propertyName: "modifier",
                value: details.Modifiers.FormatAsJToken(
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
                    [propertyName: "type"] = type.GetReprTypeName(),
                    [propertyName: "kind"] = type.GetTypeKind(),
                    [propertyName: "maxDepthReached"] = true,
                    [propertyName: "depth"] = context.Depth
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
                    modifiers.Add(item: name);
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
                    [propertyName: "type"] = type.GetReprTypeName(),
                    [propertyName: "kind"] = type.GetTypeKind(),
                    [propertyName: "maxDepthReached"] = "true",
                    [propertyName: "depth"] = context.Depth
                };
            }

            var result = new JObject();
            result.Add(propertyName: "name",
                value: details.Name);
            result.Add(propertyName: "type",
                value: details.TypeReprName);
            result.Add(propertyName: "modifier",
                value: details.Modifier);
            result.Add(propertyName: "defaultValue",
                value: details.DefaultValue.FormatAsJToken(
                    context: context.WithIncrementedDepth()));
            return result;
        }
    }
}