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
            var dt = (DateTime)obj;
            var kindSuffix = dt.Kind switch
            {
                DateTimeKind.Utc => "UTC",
                DateTimeKind.Local => "Local",
                _ => "Unspecified"
            };
            return
                $"{dt.Year:D4}.{dt.Month:D2}.{dt.Day:D2} {dt.Hour:D2}:{dt.Minute:D2}:{dt.Second:D2}.{dt.Millisecond:D3}{dt.Ticks % 10000:D4} {kindSuffix}";
        }

        public JToken ToReprTree(object obj, ReprContext context)
        {
            var datetime = (DateTime)obj;
            var result = new JObject
            {
                { "type", new JValue(value: "DateTime") },
                { "kind", new JValue(value: "struct") },
                { "year", new JValue(value: datetime.Year.ToString()) },
                { "month", new JValue(value: datetime.Month.ToString()) },
                { "day", new JValue(value: datetime.Day.ToString()) },
                { "hour", new JValue(value: datetime.Hour.ToString()) },
                { "minute", new JValue(value: datetime.Minute.ToString()) },
                { "second", new JValue(value: datetime.Second.ToString()) },
                { "millisecond", new JValue(value: datetime.Millisecond.ToString()) },
                { "subTicks", new JValue(value: (datetime.Ticks % 10000).ToString()) },
                { "totalTicks", new JValue(value: datetime.Ticks.ToString()) },
                { "timezone", new JValue(value: datetime.Kind.ToString()) }
            };
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
            var dtoTime =
                $"{dto.Year:D4}.{dto.Month:D2}.{dto.Day:D2} {dto.Hour:D2}:{dto.Minute:D2}:{dto.Second:D2}.{dto.Millisecond:D3}{dto.Ticks % 10000:D4}";
            if (dto.Offset == TimeSpan.Zero)
            {
                return dtoTime + "Z";
            }

            var offset = dto.Offset;
            return
                $"{dtoTime}{offset.ToRepr(context: context)}";
        }


        public JToken ToReprTree(object obj, ReprContext context)
        {
            var dto = (DateTimeOffset)obj;
            var result = new JObject
            {
                { "type", new JValue(value: "DateTimeOffset") },
                { "kind", new JValue(value: "struct") },
                { "year", new JValue(value: dto.Year.ToString()) },
                { "month", new JValue(value: dto.Month.ToString()) },
                { "day", new JValue(value: dto.Day.ToString()) },
                { "hour", new JValue(value: dto.Hour.ToString()) },
                { "minute", new JValue(value: dto.Minute.ToString()) },
                { "second", new JValue(value: dto.Second.ToString()) },
                { "millisecond", new JValue(value: dto.Millisecond.ToString()) },
                { "subTicks", new JValue(value: (dto.Ticks % 10000).ToString()) },
                { "totalTicks", new JValue(value: dto.Ticks.ToString()) },
                { "offset", dto.Offset.FormatAsJToken(context: context.WithIncrementedDepth()) }
            };
            return result;
        }
    }

    [ReprFormatter(typeof(TimeSpan))]
    [ReprOptions(needsPrefix: true)]
    internal class TimeSpanFormatter : IReprFormatter, IReprTreeFormatter
    {
        public string ToRepr(object obj, ReprContext context)
        {
            var ts = (TimeSpan)obj;
            var isNegative = ts.Ticks < 0;
            if (isNegative)
            {
                ts = ts.Negate();
            }

            var subDayPart =
                $"{ts.Hours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2}.{ts.Milliseconds:D3}{ts.Ticks % 10000:D4}";
            return (isNegative, ts.Days) switch
            {
                (true, 0) => $"-{subDayPart}",
                (true, _) => $"-{ts.Days}D-{subDayPart}",
                (false, 0) => subDayPart,
                (false, _) => $"{ts.Days}D {subDayPart}"
            };
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
            result.Add(propertyName: "subTicks",
                value: new JValue(value: (ts.Ticks % 10000).ToString()));
            result.Add(propertyName: "totalTicks", value: new JValue(value: ts.Ticks.ToString()));
            result.Add(propertyName: "isNegative", value: new JValue(value: isNegative.ToString()
               .ToLowerInvariant()));
            return result;
        }
    }
}