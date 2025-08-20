﻿#nullable enable
using DebugUtils.Unity.Repr.Attributes;
using DebugUtils.Unity.Repr.Extensions;
using DebugUtils.Unity.Repr.Interfaces;
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
    [ReprOptions(needsPrefix: true)]
    internal class SetFormatter : IReprFormatter
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
                if (context.Config.MaxItemsPerContainer >= 0 &&
                    i >= context.Config.MaxItemsPerContainer)
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
                    var remainingCount = itemCount - context.Config.MaxItemsPerContainer;
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

            return "{" + String.Join(separator: ", ", values: items) + "}";
        }
    }
}