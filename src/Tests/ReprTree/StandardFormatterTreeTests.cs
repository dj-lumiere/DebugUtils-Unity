#nullable enable
using System;
using System.Collections.Generic;
using DebugUtils.Unity.Repr;
using NUnit.Framework;
using Newtonsoft.Json.Linq;

namespace DebugUtils.Unity.Tests
{

    public class StandardFormatterTreeTests
    {
        [Test]
        public void TestNullRepr()
        {
            var actualJson = JToken.Parse(json: ((string?)null).ReprTree());
            Assert.AreEqual(expected: "string", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.Null(anObject: actualJson[key: "length"]);
            Assert.Null(anObject: actualJson[key: "hashCode"]);
            var hasValues = actualJson[key: "value"]?.HasValues ?? false;
            Assert.IsFalse(condition: hasValues);
        }

        [Test]
        public void TestStringRepr()
        {
            var actualJson = JToken.Parse(json: "hello".ReprTree());
            Assert.AreEqual(expected: "string", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.AreEqual(expected: 5,
                actual: actualJson[key: "length"]!.Value<int>());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: "hello", actual: actualJson[key: "value"]
              ?.ToString());

            actualJson = JToken.Parse(json: "".ReprTree());
            Assert.AreEqual(expected: "string", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.AreEqual(expected: 0,
                actual: actualJson[key: "length"]!.Value<int>());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: "", actual: actualJson[key: "value"]
              ?.ToString());
        }

        [Test]
        public void TestCharRepr()
        {
            var actualJson = JToken.Parse(json: 'A'.ReprTree());
            Assert.AreEqual(expected: "char", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "struct", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.Null(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: "A", actual: actualJson[key: "value"]
              ?.ToString());
            Assert.AreEqual(expected: "0x0041", actual: actualJson[key: "unicodeValue"]
              ?.ToString());

            actualJson = JToken.Parse(json: '\n'.ReprTree());
            Assert.AreEqual(expected: "char", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "struct", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.Null(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: "\\n", actual: actualJson[key: "value"]
              ?.ToString());
            Assert.AreEqual(expected: "0x000A", actual: actualJson[key: "unicodeValue"]
              ?.ToString());

            actualJson = JToken.Parse(json: '\u007F'.ReprTree());
            Assert.AreEqual(expected: "char", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "struct", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.Null(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: "\\u007F", actual: actualJson[key: "value"]
              ?.ToString());
            Assert.AreEqual(expected: "0x007F", actual: actualJson[key: "unicodeValue"]
              ?.ToString());

            actualJson = JToken.Parse(json: '아'.ReprTree());
            Assert.AreEqual(expected: "char", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "struct", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.Null(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: "아", actual: actualJson[key: "value"]
              ?.ToString());
            Assert.AreEqual(expected: "0xC544", actual: actualJson[key: "unicodeValue"]
              ?.ToString());
        }

        [Test]
        public void TestBoolRepr()
        {
            var actualJson = JToken.Parse(json: true.ReprTree());
            Assert.AreEqual(expected: "bool", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "struct", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.Null(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: "true", actual: actualJson[key: "value"]
              ?.ToString());
        }

        [Test]
        public void TestDateTimeRepr()
        {
            var dateTime =
                new DateTime(year: 2025, month: 1, day: 1, hour: 0, minute: 0, second: 0);
            var localDateTime = DateTime.SpecifyKind(value: dateTime, kind: DateTimeKind.Local);
            var utcDateTime = DateTime.SpecifyKind(value: dateTime, kind: DateTimeKind.Utc);

            var actualJson = JToken.Parse(json: dateTime.ReprTree());
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "DateTime",
                [propertyName: "kind"] = "struct",
                [propertyName: "year"] = "2025",
                [propertyName: "month"] = "1",
                [propertyName: "day"] = "1",
                [propertyName: "hour"] = "0",
                [propertyName: "minute"] = "0",
                [propertyName: "second"] = "0",
                [propertyName: "millisecond"] = "0",
                [propertyName: "ticks"] = "638712864000000000",
                [propertyName: "timezone"] = "Unspecified"
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));

            actualJson = JToken.Parse(json: localDateTime.ReprTree());
            expectedJson = new JObject
            {
                [propertyName: "type"] = "DateTime",
                [propertyName: "kind"] = "struct",
                [propertyName: "year"] = "2025",
                [propertyName: "month"] = "1",
                [propertyName: "day"] = "1",
                [propertyName: "hour"] = "0",
                [propertyName: "minute"] = "0",
                [propertyName: "second"] = "0",
                [propertyName: "millisecond"] = "0",
                [propertyName: "ticks"] = "638712864000000000",
                [propertyName: "timezone"] = "Local"
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));

            actualJson = JToken.Parse(json: utcDateTime.ReprTree());
            expectedJson = new JObject
            {
                [propertyName: "type"] = "DateTime",
                [propertyName: "kind"] = "struct",
                [propertyName: "year"] = "2025",
                [propertyName: "month"] = "1",
                [propertyName: "day"] = "1",
                [propertyName: "hour"] = "0",
                [propertyName: "minute"] = "0",
                [propertyName: "second"] = "0",
                [propertyName: "millisecond"] = "0",
                [propertyName: "ticks"] = "638712864000000000",
                [propertyName: "timezone"] = "Utc"
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        [Test]
        public void TestTimeSpanRepr()
        {
            var timeSpan = TimeSpan.FromMinutes(value: 30);
            var actualJson = JToken.Parse(json: timeSpan.ReprTree());
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "TimeSpan",
                [propertyName: "kind"] = "struct",
                [propertyName: "day"] = "0",
                [propertyName: "hour"] = "0",
                [propertyName: "minute"] = "30",
                [propertyName: "second"] = "0",
                [propertyName: "millisecond"] = "0",
                [propertyName: "ticks"] = "18000000000",
                [propertyName: "isNegative"] = "false"
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        [Test]
        public void TestGuidRepr()
        {
            var guid = Guid.NewGuid();
            var actualJson = JToken.Parse(json: guid.ReprTree());
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "Guid",
                [propertyName: "kind"] = "struct",
                [propertyName: "value"] = guid.ToString()
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        
        [Test]
        public void TestNullableStructRepr()
        {
            var actualJson = JToken.Parse(json: ((int?)123).ReprTree());
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "int?",
                [propertyName: "kind"] = "struct",
                [propertyName: "value"] = "123"
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));

            actualJson = JToken.Parse(json: ((int?)null).ReprTree());
            expectedJson = new JObject
            {
                [propertyName: "type"] = "int?",
                [propertyName: "kind"] = "struct",
                [propertyName: "value"] = null
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        [Test]
        public void TestNullableClassRepr()
        {
            var actualJson = JToken.Parse(json: ((List<int>?)null).ReprTree());
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "List",
                [propertyName: "kind"] = "class",
                [propertyName: "value"] = null
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }
        
        [Test]
        public void TestTimeSpanRepr_Negative()
        {
            var config = new ReprConfig(IntMode: IntReprMode.Decimal);
            var timeSpan = TimeSpan.FromMinutes(value: -30);
            var actualJson = JToken.Parse(json: timeSpan.ReprTree(config: config));
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "TimeSpan",
                [propertyName: "kind"] = "struct",
                [propertyName: "day"] = "0",
                [propertyName: "hour"] = "0",
                [propertyName: "minute"] = "30",
                [propertyName: "second"] = "0",
                [propertyName: "millisecond"] = "0",
                [propertyName: "ticks"] = "18000000000",
                [propertyName: "isNegative"] = "true"
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        [Test]
        public void TestTimeSpanRepr_Zero()
        {
            var config = new ReprConfig(IntMode: IntReprMode.Decimal);
            var timeSpan = TimeSpan.Zero;
            var actualJson = JToken.Parse(json: timeSpan.ReprTree(config: config));
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "TimeSpan",
                [propertyName: "kind"] = "struct",
                [propertyName: "day"] = "0",
                [propertyName: "hour"] = "0",
                [propertyName: "minute"] = "0",
                [propertyName: "second"] = "0",
                [propertyName: "millisecond"] = "0",
                [propertyName: "ticks"] = "0",
                [propertyName: "isNegative"] = "false"
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        [Test]
        public void TestTimeSpanRepr_Positive()
        {
            var config = new ReprConfig(IntMode: IntReprMode.Decimal);
            var timeSpan = TimeSpan.FromMinutes(value: 30);
            var actualJson = JToken.Parse(json: timeSpan.ReprTree(config: config));
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "TimeSpan",
                [propertyName: "kind"] = "struct",
                [propertyName: "day"] = "0",
                [propertyName: "hour"] = "0",
                [propertyName: "minute"] = "30",
                [propertyName: "second"] = "0",
                [propertyName: "millisecond"] = "0",
                [propertyName: "ticks"] = "18000000000",
                [propertyName: "isNegative"] = "false"
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }


        [Test]
        public void TestDateTimeOffsetRepr_WithOffset()
        {
            var dateTimeOffset =
                new DateTimeOffset(year: 2025, month: 1, day: 1, hour: 0, minute: 0, second: 0,
                    offset: TimeSpan.FromHours(value: 1));
            var actualJson = JToken.Parse(json: dateTimeOffset.ReprTree());
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "DateTimeOffset",
                [propertyName: "kind"] = "struct",
                [propertyName: "year"] = "2025",
                [propertyName: "month"] = "1",
                [propertyName: "day"] = "1",
                [propertyName: "hour"] = "0",
                [propertyName: "minute"] = "0",
                [propertyName: "second"] = "0",
                [propertyName: "millisecond"] = "0",
                [propertyName: "ticks"] = "638712864000000000",
                [propertyName: "offset"] = new JObject
                {
                    [propertyName: "type"] = "TimeSpan",
                    [propertyName: "kind"] = "struct",
                    [propertyName: "day"] = "0",
                    [propertyName: "hour"] = "1",
                    [propertyName: "minute"] = "0",
                    [propertyName: "second"] = "0",
                    [propertyName: "millisecond"] = "0",
                    [propertyName: "ticks"] = "36000000000",
                    [propertyName: "isNegative"] = "false"
                }
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        [Test]
        public void TestDateTimeOffsetRepr_WithNegativeOffset()
        {
            var dateTimeOffset =
                new DateTimeOffset(dateTime: new DateTime(year: 2025, month: 1, day: 1),
                    offset: TimeSpan.FromHours(value: -1));
            var actualJson = JToken.Parse(json: dateTimeOffset.ReprTree());
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "DateTimeOffset",
                [propertyName: "kind"] = "struct",
                [propertyName: "year"] = "2025",
                [propertyName: "month"] = "1",
                [propertyName: "day"] = "1",
                [propertyName: "hour"] = "0",
                [propertyName: "minute"] = "0",
                [propertyName: "second"] = "0",
                [propertyName: "millisecond"] = "0",
                [propertyName: "ticks"] = "638712864000000000",
                [propertyName: "offset"] = new JObject
                {
                    [propertyName: "type"] = "TimeSpan",
                    [propertyName: "kind"] = "struct",
                    [propertyName: "day"] = "0",
                    [propertyName: "hour"] = "1",
                    [propertyName: "minute"] = "0",
                    [propertyName: "second"] = "0",
                    [propertyName: "millisecond"] = "0",
                    [propertyName: "ticks"] = "36000000000",
                    [propertyName: "isNegative"] = "true"
                }
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        [Test]
        public void TestDateTimeOffsetRepr()
        {
            var dateTimeOffset = new DateTimeOffset(dateTime: new DateTime(year: 2025, month: 1,
                day: 1, hour: 0, minute: 0, second: 0, kind: DateTimeKind.Utc));
            var actualJson = JToken.Parse(json: dateTimeOffset.ReprTree());
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "DateTimeOffset",
                [propertyName: "kind"] = "struct",
                [propertyName: "year"] = "2025",
                [propertyName: "month"] = "1",
                [propertyName: "day"] = "1",
                [propertyName: "hour"] = "0",
                [propertyName: "minute"] = "0",
                [propertyName: "second"] = "0",
                [propertyName: "millisecond"] = "0",
                [propertyName: "ticks"] = "638712864000000000",
                [propertyName: "offset"] = new JObject
                {
                    [propertyName: "type"] = "TimeSpan",
                    [propertyName: "kind"] = "struct",
                    [propertyName: "day"] = "0",
                    [propertyName: "hour"] = "0",
                    [propertyName: "minute"] = "0",
                    [propertyName: "second"] = "0",
                    [propertyName: "millisecond"] = "0",
                    [propertyName: "ticks"] = "0",
                    [propertyName: "isNegative"] = "false"
                }
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }
    }
}