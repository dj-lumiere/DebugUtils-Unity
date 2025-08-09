using System;
using DebugUtils.Unity.Repr.Attributes;
using DebugUtils.Unity.Repr.Interfaces;
using Newtonsoft.Json.Linq;

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
            result.Add(propertyName: "type", value: new JValue(value: "DateTime"));
            result.Add(propertyName: "kind", value: new JValue(value: "struct"));
            result.Add(propertyName: "year", value: new JValue(value: datetime.Year.ToString()));
            result.Add(propertyName: "month", value: new JValue(value: datetime.Month.ToString()));
            result.Add(propertyName: "day", value: new JValue(value: datetime.Day.ToString()));
            result.Add(propertyName: "hour", value: new JValue(value: datetime.Hour.ToString()));
            result.Add(propertyName: "minute",
                value: new JValue(value: datetime.Minute.ToString()));
            result.Add(propertyName: "second",
                value: new JValue(value: datetime.Second.ToString()));
            result.Add(propertyName: "millisecond",
                value: new JValue(value: datetime.Millisecond.ToString()));
            result.Add(propertyName: "ticks", value: new JValue(value: datetime.Ticks.ToString()));
            result.Add(propertyName: "timezone",
                value: new JValue(value: datetime.Kind.ToString()));
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
            result.Add(propertyName: "type", value: new JValue(value: "DateTimeOffset"));
            result.Add(propertyName: "kind", value: new JValue(value: "struct"));
            result.Add(propertyName: "year", value: new JValue(value: dto.Year.ToString()));
            result.Add(propertyName: "month", value: new JValue(value: dto.Month.ToString()));
            result.Add(propertyName: "day", value: new JValue(value: dto.Day.ToString()));
            result.Add(propertyName: "hour", value: new JValue(value: dto.Hour.ToString()));
            result.Add(propertyName: "minute", value: new JValue(value: dto.Minute.ToString()));
            result.Add(propertyName: "second", value: new JValue(value: dto.Second.ToString()));
            result.Add(propertyName: "millisecond",
                value: new JValue(value: dto.Millisecond.ToString()));
            result.Add(propertyName: "ticks", value: new JValue(value: dto.Ticks.ToString()));
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
            result.Add(propertyName: "type", value: new JValue(value: "TimeSpan"));
            result.Add(propertyName: "kind", value: new JValue(value: "struct"));
            result.Add(propertyName: "day", value: new JValue(value: ts.Days.ToString()));
            result.Add(propertyName: "hour", value: new JValue(value: ts.Hours.ToString()));
            result.Add(propertyName: "minute", value: new JValue(value: ts.Minutes.ToString()));
            result.Add(propertyName: "second", value: new JValue(value: ts.Seconds.ToString()));
            result.Add(propertyName: "millisecond",
                value: new JValue(value: ts.Milliseconds.ToString()));
            result.Add(propertyName: "ticks", value: new JValue(value: ts.Ticks.ToString()));
            result.Add(propertyName: "isNegative", value: new JValue(value: isNegative.ToString()
               .ToLowerInvariant()));
            return result;
        }
    }
}