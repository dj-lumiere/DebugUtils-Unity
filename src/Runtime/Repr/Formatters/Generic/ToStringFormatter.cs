﻿
using DebugUtils.Unity.Repr.Attributes;
using DebugUtils.Unity.Repr.Interfaces;

namespace DebugUtils.Unity.Repr.Formatters
{
    // The default formatter that opts for ToString. This formatter should not be used when
// ToString method overrides object.ToString.
    [ReprOptions(needsPrefix: false)]
    internal class ToStringFormatter : IReprFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            if (context.Config.MaxDepth >= 0 && context.Depth >= context.Config.MaxDepth)
            {
                return "<Max Depth Reached>";
            }

            return obj.ToString() ?? "";
        }
    }
}