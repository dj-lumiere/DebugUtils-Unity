#nullable enable

using System;
using System.Collections.Generic;
using DebugUtils.Unity.Repr;
using NUnit.Framework;

namespace DebugUtils.Unity.Tests
{
    public class StandardFormatterTests
    {
        // Basic Types
        [Test]
        public void TestNullRepr()
        {
            Assert.AreEqual(expected: "null", actual: ((string?)null).Repr());
        }

        [Test]
        public void TestStringRepr()
        {
            Assert.AreEqual(expected: "\"hello\"", actual: "hello".Repr());
            Assert.AreEqual(expected: "\"\"", actual: "".Repr());
        }

        [Test]
        public void TestCharRepr()
        {
            Assert.AreEqual(expected: "'A'", actual: 'A'.Repr());
            Assert.AreEqual(expected: "'\\n'", actual: '\n'.Repr());
            Assert.AreEqual(expected: "'\\u007F'", actual: '\u007F'.Repr());
            Assert.AreEqual(expected: "'아'", actual: '아'.Repr());
        }


        [Test]
        public void TestBoolRepr()
        {
            Assert.AreEqual(expected: "true", actual: true.Repr());
        }

        [Test]
        public void TestDateTimeRepr()
        {
            var dateTime =
                new DateTime(year: 2025, month: 1, day: 1, hour: 0, minute: 0, second: 0);
            var localDateTime = DateTime.SpecifyKind(value: dateTime, kind: DateTimeKind.Local);
            var utcDateTime = DateTime.SpecifyKind(value: dateTime, kind: DateTimeKind.Utc);
            Assert.AreEqual(expected: "DateTime(2025-01-01 00:00:00 Unspecified)", actual:
                dateTime.Repr());
            Assert.AreEqual(expected: "DateTime(2025-01-01 00:00:00 Local)", actual:
                localDateTime.Repr());
            Assert.AreEqual(expected: "DateTime(2025-01-01 00:00:00 UTC)", actual:
                utcDateTime.Repr());
        }

        [Test]
        public void TestTimeSpanRepr()
        {
            Assert.AreEqual(expected: "TimeSpan(1800.000s)", actual: TimeSpan
               .FromMinutes(value: 30)
               .Repr());
        }

        [Test]
        public void TestGuidRepr()
        {
            var guid = Guid.NewGuid();
            Assert.AreEqual(expected: $"Guid({guid})", actual: guid.Repr());
        }

        [Test]
        public void TestTimeSpanRepr_Negative()
        {
            var config = new ReprConfig(IntMode: IntReprMode.Decimal);
            Assert.AreEqual(expected: "TimeSpan(-1800.000s)", actual: TimeSpan
               .FromMinutes(value: -30)
               .Repr(config: config));
        }

        [Test]
        public void TestTimeSpanRepr_Zero()
        {
            var config = new ReprConfig(IntMode: IntReprMode.Decimal);
            Assert.AreEqual(expected: "TimeSpan(0.000s)",
                actual: TimeSpan.Zero.Repr(config: config));
        }

        [Test]
        public void TestTimeSpanRepr_Positive()
        {
            var config = new ReprConfig(IntMode: IntReprMode.Decimal);
            Assert.AreEqual(expected: "TimeSpan(1800.000s)", actual: TimeSpan
               .FromMinutes(value: 30)
               .Repr(config: config));
        }

        [Test]
        public void TestDateTimeOffsetRepr()
        {
            Assert.AreEqual(expected: "DateTimeOffset(2025-01-01 00:00:00Z)",
                actual: new DateTimeOffset(dateTime: new DateTime(year: 2025, month: 1, day: 1,
                    hour: 0, minute: 0, second: 0,
                    kind: DateTimeKind.Utc)).Repr());
        }

        [Test]
        public void TestDateTimeOffsetRepr_WithOffset()
        {
            Assert.AreEqual(expected: "DateTimeOffset(2025-01-01 00:00:00+01:00:00)",
                actual: new DateTimeOffset(dateTime: new DateTime(year: 2025, month: 1, day: 1),
                    offset: TimeSpan.FromHours(value: 1)).Repr());
        }


        // Memory and Span Types
        [Test]
        public void TestMemoryRepr()
        {
            var array = new int[] { 1, 2, 3, 4, 5 };
            var memory = new Memory<int>(array: array, start: 1, length: 3);
            Assert.AreEqual(expected: "Memory([int(2), int(3), int(4)])",
                actual: memory.Repr());

            var emptyMemory = Memory<int>.Empty;
            Assert.AreEqual(expected: "Memory([])", actual: emptyMemory.Repr());
        }

        [Test]
        public void TestReadOnlyMemoryRepr()
        {
            var array = new int[] { 1, 2, 3, 4, 5 };
            var readOnlyMemory = new ReadOnlyMemory<int>(array: array, start: 1, length: 3);
            Assert.AreEqual(expected: "ReadOnlyMemory([int(2), int(3), int(4)])",
                actual: readOnlyMemory.Repr());

            var emptyReadOnlyMemory = ReadOnlyMemory<int>.Empty;
            Assert.AreEqual(expected: "ReadOnlyMemory([])",
                actual: emptyReadOnlyMemory.Repr());
        }

        [Test]
        public void TestSpanRepr()
        {
            var array = new int[] { 1, 2, 3, 4, 5 };
            var span = new Span<int>(array: array, start: 1, length: 3);
            Assert.AreEqual(expected: "Span([int(2), int(3), int(4)])", actual: span.Repr());

            var emptySpan = Span<int>.Empty;
            Assert.AreEqual(expected: "Span([])", actual: emptySpan.Repr());
        }

        [Test]
        public void TestReadOnlySpanRepr()
        {
            var array = new int[] { 1, 2, 3, 4, 5 };
            var readOnlySpan = new ReadOnlySpan<int>(array: array, start: 1, length: 3);
            Assert.AreEqual(expected: "ReadOnlySpan([int(2), int(3), int(4)])",
                actual: readOnlySpan.Repr());

            var emptyReadOnlySpan = ReadOnlySpan<int>.Empty;
            Assert.AreEqual(expected: "ReadOnlySpan([])", actual: emptyReadOnlySpan.Repr());
        }

        [Test]
        public void TestIndexRepr()
        {
            var fromStart = new Index(value: 5);
            Assert.AreEqual(expected: "Index(5)", actual: fromStart.Repr());

            var fromEnd = new Index(value: 3, fromEnd: true);
            Assert.AreEqual(expected: "Index(^3)", actual: fromEnd.Repr());

            var startIndex = Index.Start;
            Assert.AreEqual(expected: "Index(0)", actual: startIndex.Repr());

            var endIndex = Index.End;
            Assert.AreEqual(expected: "Index(^0)", actual: endIndex.Repr());
        }

        [Test]
        public void TestRangeRepr()
        {
            var range1 = new Range(start: 1, end: 5);
            Assert.AreEqual(expected: "Range(1..5)", actual: range1.Repr());

            var range2 = new Range(start: Index.Start, end: new Index(value: 3, fromEnd: true));
            Assert.AreEqual(expected: "Range(0..^3)", actual: range2.Repr());

            var allRange = Range.All;
            Assert.AreEqual(expected: "Range(0..^0)", actual: allRange.Repr());

            var rangeFromIndex = 2..^1;
            Assert.AreEqual(expected: "Range(2..^1)", actual: rangeFromIndex.Repr());
        }

        // Nullable Types
        [Test]
        public void TestNullableStructRepr()
        {
            Assert.AreEqual(expected: "int?(123)", actual: ((int?)123).Repr());
            Assert.AreEqual(expected: "int?(null)", actual: ((int?)null).Repr());
        }

        [Test]
        public void TestNullableClassRepr()
        {
            Assert.AreEqual(expected: "null", actual: ((List<int>?)null).Repr());
        }

        [Test]
        public void TestListWithNullElements()
        {
            var listWithNull = new List<List<int>?> { new() { 1 }, null };
            Assert.AreEqual(expected: "[[int(1)], null]", actual: listWithNull.Repr());
        }
    }
}