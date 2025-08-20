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
    [ReprFormatter(typeof(string))]
    [ReprOptions(needsPrefix: false)]
    internal class StringFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var s = (string)obj;
            if (s.Length <= context.Config.MaxStringLength)
            {
                return $"\"{(string)obj}\"";
            }

            var truncatedLetterCount = s.Length - context.Config.MaxStringLength;
            s = s[..context.Config.MaxStringLength];
            return $"\"{s}... ({truncatedLetterCount} more letters)\"";
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var s = (string)obj;
            var sLength = s.Length;
            if (s.Length > context.Config.MaxStringLength)
            {
                var truncatedLetterCount = s.Length - context.Config.MaxStringLength;
                s = s[..context.Config.MaxStringLength] +
                    $"... ({truncatedLetterCount} more letters)";
            }

            return new JObject
            {
                [propertyName: "type"] = "string",
                [propertyName: "kind"] = "class",
                [propertyName: "hashCode"] = $"0x{RuntimeHelpers.GetHashCode(o: obj):X8}",
                [propertyName: "length"] = sLength,
                [propertyName: "value"] = s
            };
        }
    }

    [ReprFormatter(typeof(StringBuilder))]
    [ReprOptions(needsPrefix: true)]
    internal class StringBuilderFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var sb = (StringBuilder)obj;
            var s = sb.ToString();
            if (s.Length > context.Config.MaxStringLength)
            {
                var truncatedLetterCount = s.Length - context.Config.MaxStringLength;
                s = s[..context.Config.MaxStringLength] +
                    $"... ({truncatedLetterCount} more letters)";
            }

            return $"{s}";
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var type = obj.GetType();
            var sb = (StringBuilder)obj;
            var s = sb.ToString();
            var sLength = s.Length;
            if (s.Length > context.Config.MaxStringLength)
            {
                var truncatedLetterCount = s.Length - context.Config.MaxStringLength;
                s = s[..context.Config.MaxStringLength] +
                    $"... ({truncatedLetterCount} more letters)";
            }

            return new JObject
            {
                [propertyName: "type"] = type.GetReprTypeName(),
                [propertyName: "kind"] = type.GetTypeKind(),
                [propertyName: "hashCode"] = RuntimeHelpers.GetHashCode(o: obj),
                [propertyName: "length"] = sLength,
                [propertyName: "value"] = s
            };
        }
    }

    [ReprFormatter(typeof(char))]
    [ReprOptions(needsPrefix: false)]
    internal class CharFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var value = (char)obj;
            return value switch
            {
                '\'' => "'''", // Single quote
                '\"' => "'\"'", // Double quote
                '\\' => @"'\'", // Backslash
                '\0' => @"'\0'", // Null
                '\a' => @"'\a'", // Alert
                '\b' => @"'\b'", // Backspace
                '\f' => @"'\f'", // Form feed
                '\n' => @"'\n'", // Newline
                '\r' => @"'\r'", // Carriage return
                '\t' => @"'\t'", // Tab
                '\v' => @"'\v'", // Vertical tab
                '\u00a0' => "'nbsp'", // Non-breaking space
                '\u00ad' => "'shy'", // Soft Hyphen
                _ when Char.IsControl(c: value) => $"'\\u{(int)value:X4}'", // Control character
                _ => $"'{value}'"
            };
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var c = (char)obj;
            return new JObject
            {
                [propertyName: "type"] = "char",
                [propertyName: "kind"] = "struct",
                [propertyName: "value"] =
                    ToRepr(obj: c, context: context)[1..^1], // truncate ' prefix and suffix
                [propertyName: "unicodeValue"] = $"0x{(int)c:X4}"
            };
        }
    }
}
#if NET5_0_OR_GREATER
[ReprFormatter(typeof(Rune))]
[ReprOptions(needsPrefix: true)]
internal class RuneFormatter : IReprFormatter, IReprTreeFormatter
{
    public string ToRepr(object obj, ReprContext context)
    {
        return $"{(Rune)obj} @ \\U{((Rune)obj).Value:X8}";
    }

    public JsonNode ToReprTree(object obj, ReprContext context)
    {
        var rune = (Rune)obj;
        return new JsonObject
        {
            [propertyName: "type"] = "Rune",
            [propertyName: "kind"] = "struct",
            [propertyName: "value"] = rune.ToString(),
            [propertyName: "unicodeValue"] = $"0x{rune.Value:X8}"
        };
    }
}
#endif