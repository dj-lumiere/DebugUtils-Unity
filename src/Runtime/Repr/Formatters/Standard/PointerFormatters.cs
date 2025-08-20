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

// ReSharper disable BuiltInTypeReferenceStyle
namespace DebugUtils.Unity.Repr.Formatters
{
    [ReprFormatter(typeof(IntPtr))]
    [ReprOptions(needsPrefix: false)]
    internal class IntPtrFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            return IntPtr.Size == 4
                ? $"0x{((IntPtr)obj).ToInt32():X8}"
                : $"0x{((IntPtr)obj).ToInt64():X16}";
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var type = obj.GetType();
            if (context.Depth > 0)
            {
                return obj.Repr(context: context)!;
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
                    "value",
                    ToRepr(obj: obj, context: context)
                }
            };
        }
    }

    [ReprFormatter(typeof(UIntPtr))]
    [ReprOptions(needsPrefix: false)]
    internal class UIntPtrFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            return IntPtr.Size == 4
                ? $"0x{((UIntPtr)obj).ToUInt32():X8}"
                : $"0x{((UIntPtr)obj).ToUInt64():X16}";
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var type = obj.GetType();
            if (context.Depth > 0)
            {
                return obj.Repr(context: context)!;
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
                    "value",
                    ToRepr(obj: obj, context: context)
                }
            };
        }
    }
}