#nullable enable
using DebugUtils.Unity.Repr;
using NUnit.Framework;
using System.Collections.Generic;
using System;

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

        #if NET5_0_OR_GREATER
    [Fact]
    public void TestRuneRepr()
    {
        Assert.Equal(expected: "Rune(💜 @ \\U0001F49C)", actual: new Rune(value: 0x1f49c).Repr());
    }
        #endif
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
            Assert.AreEqual(expected: "DateTime(2025.01.01 00:00:00.0000000 Unspecified)",
                actual: dateTime.Repr());
            Assert.AreEqual(expected: "DateTime(2025.01.01 00:00:00.0000000 Local)",
                actual: localDateTime.Repr());
            Assert.AreEqual(expected: "DateTime(2025.01.01 00:00:00.0000000 UTC)",
                actual: utcDateTime.Repr());
        }

        [Test]
        public void TestTimeSpanRepr()
        {
            Assert.AreEqual(expected: "TimeSpan(1D 00:00:00.0000000)", actual: TimeSpan
               .FromDays(value: 1)
               .Repr());
        }

        [Test]
        public void TestTimeSpanRepr_Negative()
        {
            var config = ReprConfig.Configure()
                                   .Build();
            Assert.AreEqual(expected: "TimeSpan(-00:30:00.0000000)", actual: TimeSpan
               .FromMinutes(value: -30)
               .Repr(config: config));
        }

        [Test]
        public void TestTimeSpanRepr_Negative_WithDays()
        {
            var config = ReprConfig.Configure()
                                   .Build();
            Assert.AreEqual(expected: "TimeSpan(-1D-01:00:00.0000000)",
                actual: new TimeSpan(days: -1, hours: -1, minutes: 0, seconds: 0).Repr(
                    config: config));
        }

        [Test]
        public void TestTimeSpanRepr_Zero()
        {
            var config = ReprConfig.Configure()
                                   .Build();
            Assert.AreEqual(expected: "TimeSpan(00:00:00.0000000)",
                actual: TimeSpan.Zero.Repr(config: config));
        }

        [Test]
        public void TestTimeSpanRepr_Positive()
        {
            var config = ReprConfig.Configure()
                                   .Build();
            Assert.AreEqual(expected: "TimeSpan(00:30:00.0000000)", actual: TimeSpan
               .FromMinutes(value: 30)
               .Repr(config: config));
        }

        [Test]
        public void TestDateTimeOffsetRepr()
        {
            Assert.AreEqual(expected: "DateTimeOffset(2025.01.01 12:34:56.7890000Z)",
                actual: new DateTimeOffset(dateTime: new DateTime(year: 2025, month: 1, day: 1,
                        hour: 12, minute: 34, second: 56, millisecond: 789,
                        kind: DateTimeKind.Utc))
                   .Repr());
        }

        [Test]
        public void TestDateTimeOffsetRepr_WithOffset()
        {
            Assert.AreEqual(
                expected: "DateTimeOffset(2025.01.01 00:00:00.0000000+01:00:00.0000000)",
                actual: new DateTimeOffset(dateTime: new DateTime(year: 2025, month: 1, day: 1),
                    offset: TimeSpan.FromHours(value: 1)).Repr());
        }

        #if NET6_0_OR_GREATER
    [Fact]
    public void TestDateOnly()
    {
        Assert.Equal(expected: "DateOnly(2025.01.01)",
            actual: new DateOnly(year: 2025, month: 1, day: 1).Repr());
    }

    [Fact]
    public void TestTimeOnly()
    {
        Assert.Equal(expected: "TimeOnly(01:02:03.0000000)",
            actual: new TimeOnly(hour: 1, minute: 2, second: 3).Repr());
    }
        #endif
        [Test]
        public void TestGuidRepr()
        {
            var guid = Guid.NewGuid();
            Assert.AreEqual(expected: $"Guid({guid})", actual: guid.Repr());
        }

        // Memory and Span Types
        [Test]
        public void TestMemoryRepr()
        {
            var array = new[]
            {
                1,
                2,
                3,
                4,
                5
            };
            var memory = new Memory<int>(array: array, start: 1, length: 3);
            Assert.AreEqual(expected: "Memory([2_i32, 3_i32, 4_i32])", actual: memory.Repr());
            var emptyMemory = Memory<int>.Empty;
            Assert.AreEqual(expected: "Memory([])", actual: emptyMemory.Repr());
        }

        [Test]
        public void TestReadOnlyMemoryRepr()
        {
            var array = new[]
            {
                1,
                2,
                3,
                4,
                5
            };
            var readOnlyMemory = new ReadOnlyMemory<int>(array: array, start: 1, length: 3);
            Assert.AreEqual(expected: "ReadOnlyMemory([2_i32, 3_i32, 4_i32])",
                actual: readOnlyMemory.Repr());
            var emptyReadOnlyMemory = ReadOnlyMemory<int>.Empty;
            Assert.AreEqual(expected: "ReadOnlyMemory([])", actual: emptyReadOnlyMemory.Repr());
        }

        [Test]
        public void TestSpanRepr()
        {
            var array = new[]
            {
                1,
                2,
                3,
                4,
                5
            };
            var span = new Span<int>(array: array, start: 1, length: 3);
            Assert.AreEqual(expected: "Span([2_i32, 3_i32, 4_i32])", actual: span.Repr());
            var emptySpan = Span<int>.Empty;
            Assert.AreEqual(expected: "Span([])", actual: emptySpan.Repr());
        }

        [Test]
        public void TestReadOnlySpanRepr()
        {
            var array = new[]
            {
                1,
                2,
                3,
                4,
                5
            };
            var readOnlySpan = new ReadOnlySpan<int>(array: array, start: 1, length: 3);
            Assert.AreEqual(expected: "ReadOnlySpan([2_i32, 3_i32, 4_i32])",
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
            Assert.AreEqual(expected: "123_i32?", actual: ((int?)123).Repr());
            Assert.AreEqual(expected: "null_i32?", actual: ((int?)null).Repr());
        }

        [Test]
        public void TestNullableClassRepr()
        {
            Assert.AreEqual(expected: "null", actual: ((List<int>?)null).Repr());
        }

        [Test]
        public void TestListWithNullElements()
        {
            var listWithNull = new List<List<int>?>
            {
                new()
                {
                    1
                },
                null
            };
            Assert.AreEqual(expected: "[[1_i32], null]", actual: listWithNull.Repr());
        }
    }
}