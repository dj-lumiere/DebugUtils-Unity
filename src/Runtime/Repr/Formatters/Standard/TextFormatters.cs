using System;
using System.Runtime.CompilerServices;
using System.Text;
using DebugUtils.Unity.Repr.Attributes;
using DebugUtils.Unity.Repr.Interfaces;
using DebugUtils.Unity.Repr.TypeHelpers;
using Newtonsoft.Json.Linq;

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

            var result = new JObject();
            result.Add(propertyName: "type", value: new JValue(value: "string"));
            result.Add(propertyName: "kind", value: new JValue(value: "class"));
            result.Add(propertyName: "hashCode",
                value: new JValue(value: $"0x{RuntimeHelpers.GetHashCode(o: obj):X8}"));
            result.Add(propertyName: "length", value: new JValue(value: sLength));
            result.Add(propertyName: "value", value: new JValue(value: s));
            return result;
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
            var result = new JObject();
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

            result.Add(propertyName: "type", value: new JValue(value: type.GetReprTypeName()));
            result.Add(propertyName: "kind", value: new JValue(value: type.GetTypeKind()));
            result.Add(propertyName: "hashCode",
                value: new JValue(value: RuntimeHelpers.GetHashCode(o: obj)));
            result.Add(propertyName: "length", value: new JValue(value: sLength));
            result.Add(propertyName: "value", value: new JValue(value: s));
            return result;
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
            var result = new JObject();
            result.Add(propertyName: "type", value: new JValue(value: "char"));
            result.Add(propertyName: "kind", value: new JValue(value: "struct"));
            // should truncate ' prefix and suffix
            result.Add(propertyName: "value",
                value: new JValue(value: ToRepr(obj: c, context: context)[1..^1]));
            result.Add(propertyName: "unicodeValue", value: new JValue(value: $"0x{(int)c:X4}"));
            return result;
        }
    }
}