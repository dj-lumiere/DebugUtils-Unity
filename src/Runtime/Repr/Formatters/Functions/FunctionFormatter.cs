﻿#nullable enable
using DebugUtils.Unity.Repr.Attributes;
using DebugUtils.Unity.Repr.Extensions;
using DebugUtils.Unity.Repr.Interfaces;
using DebugUtils.Unity.Repr.Models;
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
                return type.CreateMaxDepthReachedJson(depth: context.Depth);
            }

            var functionDetails = del.Method.ToFunctionDetails();
            return new JObject
            {
                {
                    "type",
                    "Function"
                },
                {
                    "hashCode",
                    $"0x{RuntimeHelpers.GetHashCode(o: obj):X8}"
                },
                {
                    "properties",
                    functionDetails.FormatAsJToken(context: context)
                }
            };
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
                return type.CreateMaxDepthReachedJson(depth: context.Depth);
            }

            var parameters = new JArray();
            foreach (var parameter in details.Parameters)
            {
                parameters.Add(
                    item: parameter.FormatAsJToken(context: context.WithIncrementedDepth()));
            }

            return new JObject
            {
                {
                    "name",
                    details.Name
                },
                {
                    "returnType",
                    details.ReturnTypeReprName
                },
                {
                    "modifiers",
                    details.Modifiers.FormatAsJToken(context: context.WithIncrementedDepth())
                },
                {
                    "parameters",
                    parameters
                }
            };
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
                return type.CreateMaxDepthReachedJson(depth: context.Depth);
            }

            var modifiers = new JArray();
            foreach (var (condition, name)in new[]
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
                return type.CreateMaxDepthReachedJson(depth: context.Depth);
            }

            return new JObject
            {
                {
                    "name",
                    details.Name
                },
                {
                    "type",
                    details.TypeReprName
                },
                {
                    "modifier",
                    details.Modifier
                },
                {
                    "defaultValue",
                    details.DefaultValue.FormatAsJToken(context: context.WithIncrementedDepth())
                }
            };
        }
    }
}