#nullable enable
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
    /// <summary>
    ///     A generic formatter for any record type.
    ///     It uses reflection to represent the record's public properties.
    /// </summary>
    [ReprOptions(needsPrefix: true)]
    internal class RecordFormatter : IReprFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            context = context.WithContainerConfig();
            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return "<Max Depth Reached>";
            }

            var members = obj.GetObjectMembers(context: context);
            var parts = new List<string>();
            foreach (var member in members.PublicFields)
            {
                parts.Add(item: obj.ToReprParts(f: member, context: context));
            }

            foreach (var member in members.PublicAutoProps)
            {
                parts.Add(item: obj.ToReprParts(pair: member, context: context));
            }

            foreach (var member in members.PrivateFields)
            {
                parts.Add(item: obj.ToPrivateReprParts(f: member, context: context));
            }

            foreach (var member in members.PrivateAutoProps)
            {
                parts.Add(item: obj.ToPrivateReprParts(pair: member, context: context));
            }

            if (members.Truncated)
            {
                parts.Add(item: "...");
            }

            return $"{{ {String.Join(separator: ", ", values: parts)} }}";
        }
    }
}