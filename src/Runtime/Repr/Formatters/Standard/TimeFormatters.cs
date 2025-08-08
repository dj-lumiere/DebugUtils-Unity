using System;
using DebugUtils.Unity.Repr.Attributes;
using DebugUtils.Unity.Repr.Interfaces;
using Unity.Plastic.Newtonsoft.Json.Linq;

namespace DebugUtils.Unity.Repr.Formatters
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
            return datetime.ToString(format: "yyyy-MM-dd HH:mm:ss") + kindSuffix;
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var datetime = (DateTime)obj;
            var result = new JObject();
            result.Add(propertyName: "type", value: "DateTime");
            result.Add(propertyName: "kind", value: "struct");
            result.Add(propertyName: "year", value: datetime.Year.ToString());
            result.Add(propertyName: "month", value: datetime.Month.ToString());
            result.Add(propertyName: "day", value: datetime.Day.ToString());
            result.Add(propertyName: "hour", value: datetime.Hour.ToString());
            result.Add(propertyName: "minute", value: datetime.Minute.ToString());
            result.Add(propertyName: "second", value: datetime.Second.ToString());
            result.Add(propertyName: "millisecond", value: datetime.Millisecond.ToString());
            result.Add(propertyName: "ticks", value: datetime.Ticks.ToString());
            result.Add(propertyName: "timezone", value: datetime.Kind.ToString());
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
                return dto.ToString(format: "yyyy-MM-dd HH:mm:ss") + "Z";
            }

            var offset = dto.Offset.ToString(format: "c");
            if (!offset.StartsWith(value: "+"))
            {
                offset = "+" + offset;
            }

            return dto.ToString(format: "yyyy-MM-dd HH:mm:ss") + offset;
        }


        public JToken ToReprTree(object obj, ReprContext context)
        {
            var dto = (DateTimeOffset)obj;
            var result = new JObject();
            result.Add(propertyName: "type", value: "DateTimeOffset");
            result.Add(propertyName: "kind", value: "struct");
            result.Add(propertyName: "year", value: dto.Year.ToString());
            result.Add(propertyName: "month", value: dto.Month.ToString());
            result.Add(propertyName: "day", value: dto.Day.ToString());
            result.Add(propertyName: "hour", value: dto.Hour.ToString());
            result.Add(propertyName: "minute", value: dto.Minute.ToString());
            result.Add(propertyName: "second", value: dto.Second.ToString());
            result.Add(propertyName: "millisecond", value: dto.Millisecond.ToString());
            result.Add(propertyName: "ticks", value: dto.Ticks.ToString());
            result.Add(propertyName: "offset",
                value: dto.Offset.FormatAsJToken(context: context.WithIncrementedDepth()));
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
            result.Add(propertyName: "type", value: "TimeSpan");
            result.Add(propertyName: "kind", value: "struct");
            result.Add(propertyName: "day", value: ts.Days.ToString());
            result.Add(propertyName: "hour", value: ts.Hours.ToString());
            result.Add(propertyName: "minute", value: ts.Minutes.ToString());
            result.Add(propertyName: "second", value: ts.Seconds.ToString());
            result.Add(propertyName: "millisecond", value: ts.Milliseconds.ToString());
            result.Add(propertyName: "ticks", value: ts.Ticks.ToString());
            result.Add(propertyName: "isNegative", value: isNegative.ToString()
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