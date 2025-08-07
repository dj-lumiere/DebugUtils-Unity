using System;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Attributes;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Interfaces;
using DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Records;
using Unity.Plastic.Newtonsoft.Json.Linq;

namespace DebugUtils.Unity.DebugUtils.Unity.src.Runtime.Repr.Formatters.Standard
{
    [ReprFormatter(typeof(DateTime))]
    [ReprOptions(needsPrefix: true)]
    internal class DateTimeFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var datetime = (DateTime)obj;
            var kindSuffix = datetime.Kind switch
            {
                DateTimeKind.Utc => " UTC",
                DateTimeKind.Local => " Local",
                _ => " Unspecified"
            };
            return datetime.ToString("yyyy-MM-dd HH:mm:ss") + kindSuffix;
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var datetime = (DateTime)obj;
            var result = new JObject();
            result.Add("type", "DateTime");
            result.Add("kind", "struct");
            result.Add("year", datetime.Year.ToString());
            result.Add("month", datetime.Month.ToString());
            result.Add("day", datetime.Day.ToString());
            result.Add("hour", datetime.Hour.ToString());
            result.Add("minute", datetime.Minute.ToString());
            result.Add("second", datetime.Second.ToString());
            result.Add("millisecond", datetime.Millisecond.ToString());
            result.Add("ticks", datetime.Ticks.ToString());
            result.Add("timezone", datetime.Kind.ToString());
            return result;
        }
    }

    [ReprFormatter(typeof(DateTimeOffset))]
    [ReprOptions(needsPrefix: true)]
    internal class DateTimeOffsetFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var dto = (DateTimeOffset)obj;
            if (dto.Offset == TimeSpan.Zero)
            {
                return dto.ToString("yyyy-MM-dd HH:mm:ss") + "Z";
            }

            var offset = dto.Offset.ToString("c");
            if (!offset.StartsWith("+"))
            {
                offset = "+" + offset;
            }

            return dto.ToString("yyyy-MM-dd HH:mm:ss") + offset;
        }


        public JToken ToReprTree(object obj, ReprContext context)
        {
            var dto = (DateTimeOffset)obj;
            var result = new JObject();
            result.Add("type", "DateTimeOffset");
            result.Add("kind", "struct");
            result.Add("year", dto.Year.ToString());
            result.Add("month", dto.Month.ToString());
            result.Add("day", dto.Day.ToString());
            result.Add("hour", dto.Hour.ToString());
            result.Add("minute", dto.Minute.ToString());
            result.Add("second", dto.Second.ToString());
            result.Add("millisecond", dto.Millisecond.ToString());
            result.Add("ticks", dto.Ticks.ToString());
            result.Add("offset",
                dto.Offset.FormatAsJToken(context: context.WithIncrementedDepth()));
            return result;
        }
    }

    [ReprFormatter(typeof(TimeSpan))]
    [ReprOptions(needsPrefix: true)]
    internal class TimeSpanFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            return $"{((TimeSpan)obj).TotalSeconds:0.000}s";
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var ts = (TimeSpan)obj;
            var isNegative = ts.Ticks < 0;
            if (isNegative)
            {
                ts = ts.Negate();
            }

            var result = new JObject();
            result.Add("type", "TimeSpan");
            result.Add("kind", "struct");
            result.Add("day", ts.Days.ToString());
            result.Add("hour", ts.Hours.ToString());
            result.Add("minute", ts.Minutes.ToString());
            result.Add("second", ts.Seconds.ToString());
            result.Add("millisecond", ts.Milliseconds.ToString());
            result.Add("ticks", ts.Ticks.ToString());
            result.Add("isNegative", isNegative.ToString()
               .ToLowerInvariant());
            return result;
        }
    }
    #if NET6_0_OR_GREATER
[ReprFormatter(typeof(DateOnly))]
[ReprOptions(needsPrefix: true)]
internal class DateOnlyFormatter : IReprFormatter, IReprTreeFormatter
{
    public string ToRepr(object obj, ReprContext context)
    {
        return ((DateOnly)obj).ToString("yyyy-MM-dd");
    }

    public JToken ToReprTree(object obj, ReprContext context)
    {
        var dateOnly = (DateOnly)obj;
        var result = new JObject();
        result.Add("type", "DateOnly");
        result.Add("kind", "struct");
        result.Add("year", dateOnly.Year.ToString());
        result.Add("month", dateOnly.Month.ToString());
        result.Add("day", dateOnly.Day.ToString());
        return result;
    }
}

[ReprFormatter(typeof(TimeOnly))]
[ReprOptions(needsPrefix: true)]
internal class TimeOnlyFormatter : IReprFormatter, IReprTreeFormatter
{
    public string ToRepr(object obj, ReprContext context)
    {
        return ((TimeOnly)obj).ToString("HH:mm:ss");
    }

    public JToken ToReprTree(object obj, ReprContext context)
    {
        var to = (TimeOnly)obj;
        var result = new JObject();
        result.Add("type", "TimeOnly");
        result.Add("kind", "struct");
        result.Add("hour", to.Hour.ToString());
        result.Add("minute", to.Minute.ToString());
        result.Add("second", to.Second.ToString());
        result.Add("millisecond", to.Millisecond.ToString());
        result.Add("ticks", to.Ticks.ToString());
        return result;
    }
}
    #endif
}