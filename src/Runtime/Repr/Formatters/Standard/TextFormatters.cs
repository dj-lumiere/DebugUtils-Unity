using System;
using System.Runtime.CompilerServices;
using System.Text;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Attributes;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Interfaces;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Records;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.TypeHelpers;
using Unity.Plastic.Newtonsoft.Json.Linq;

namespace DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Formatters.Standard
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
            if (s.Length > context.Config.MaxStringLength)
            {
                var truncatedLetterCount = s.Length - context.Config.MaxStringLength;
                s = s[..context.Config.MaxStringLength] +
                    $"... ({truncatedLetterCount} more letters)";
            }

            var result = new JObject();
            result.Add("type", "string");
            result.Add("kind", "class");
            result.Add("hashCode",
                $"0x{RuntimeHelpers.GetHashCode(o: obj):X8}");
            result.Add("length", s.Length);
            result.Add("value", s);
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
            result.Add("type", type.GetReprTypeName());
            result.Add("kind", type.GetTypeKind());
            result.Add("hashCode", RuntimeHelpers.GetHashCode(o: obj));
            result.Add("length", s.Length);
            result.Add("value", ToRepr(obj: obj, context: context));
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
            result.Add("type", "char");
            result.Add("kind", "struct");
            // should truncate ' prefix and suffix
            result.Add("value", ToRepr(obj: c, context: context)[1..^1]);
            result.Add("unicodeValue", $"0x{(int)c:X4}");
            return result;
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

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var rune = (Rune)obj;
            var result = new JObject();
            result.Add("type", "Rune");
            result.Add("kind", "struct");
            result.Add("value", rune.ToString());
            result.Add("unicodeValue", $"0x{rune.Value:X8}");
            return result;
        }
    }
    #endif
}