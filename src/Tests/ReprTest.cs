#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using DebugUtils.Unity.Repr;
using NUnit.Framework;
using Half = Unity.Mathematics.half;

namespace DebugUtils.Unity.Tests
{
// Test data structures from DebugUtilsTest.cs
    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public struct CustomStruct
    {
        public string Name;
        public int Value;
        public override string ToString()
        {
            return $"Custom({Name}, {Value})";
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }
        public override string ToString()
        {
            return $"{Name}({Age})";
        }
    }

    public class NoToStringClass
    {
        public string Data { get; set; }
        public int Number { get; set; }
        public NoToStringClass(string data, int number)
        {
            Data = data;
            Number = number;
        }
    }

    public record TestSettings(string EquipmentName, Dictionary<string, double> EquipmentSettings);

    public enum Colors
    {
        RED,
        GREEN,
        BLUE
    }

    public class Children
    {
        public string Name { get; set; } = "";
        public Children? Parent { get; set; }
    }

    public class ClassifiedData
    {
        public string Writer { get; set; }
        private string Data { get; set; }
        public ClassifiedData(string writer, string data)
        {
            Writer = writer;
            Data = data;
        }
    }

    public class ReprTest
    {
        [Test]
        public void ReadmeTestRepr()
        {
            var arr = new int[] { 1, 2, 3, 4 };
            Assert.AreEqual(expected: "System.Int32[]", actual: arr.ToString());

            var dict = new Dictionary<string, int> { { "a", 1 }, { "b", 2 } };
            Assert.AreEqual(
                expected: "System.Collections.Generic.Dictionary`2[System.String,System.Int32]",
                actual: dict.ToString());

            var data = new { Name = "Alice", Age = 30, Scores = new[] { 95, 87, 92 } };
            Assert.AreEqual(
                expected:
                "Anonymous(Name: \"Alice\", Age: int(30), Scores: 1DArray([int(95), int(87), int(92)]))",
                actual: data.Repr());

            Assert.AreEqual(expected: "1DArray([int(1), int(2), int(3)])",
                actual: new[] { 1, 2, 3 }.Repr());
            Assert.AreEqual(expected: "2DArray([[int(1), int(2)], [int(3), int(4)]])",
                actual: new[,] { { 1, 2 }, { 3, 4 } }.Repr());
            Assert.AreEqual(expected: "JaggedArray([[int(1), int(2)], [int(3), int(4), int(5)]])",
                actual: new int[][] { new[] { 1, 2 }, new[] { 3, 4, 5 } }.Repr());

            Assert.AreEqual(expected: "[int(1), int(2), int(3)]",
                actual: new List<int> { 1, 2, 3 }.Repr());
            Assert.AreEqual(expected: "{\"a\", \"b\"}",
                actual: new HashSet<string> { "a", "b" }.Repr());
            Assert.AreEqual(expected: "{\"x\": int(1)}",
                actual: new Dictionary<string, int> { { "x", 1 } }.Repr());

            Assert.AreEqual(expected: "int(42)", actual: 42.Repr());
            Assert.AreEqual(expected: "int(0x2A)",
                actual: 42.Repr(config: new ReprConfig(IntMode: IntReprMode.Hex)));
            Assert.AreEqual(expected: "int(0b101010)",
                actual: 42.Repr(config: new ReprConfig(IntMode: IntReprMode.Binary)));

            Assert.AreEqual(
                expected: "double(3.000000000000000444089209850062616169452667236328125E-001)",
                actual: (0.1 + 0.2)
               .Repr());
            Assert.AreEqual(
                expected: "double(2.99999999999999988897769753748434595763683319091796875E-001)",
                actual: 0.3.Repr());
            Assert.AreEqual(expected: "double(0.3)",
                actual: (0.1 + 0.2)
               .Repr(config: new ReprConfig(FloatMode: FloatReprMode.General)));

            var hideTypes = new ReprConfig(
                TypeMode: TypeReprMode.AlwaysHide,
                ContainerReprMode: ContainerReprMode.UseParentConfig
            );
            Assert.AreEqual(expected: "[1, 2, 3]",
                actual: new[] { 1, 2, 3 }.Repr(config: hideTypes));

            var showTypes = new ReprConfig(TypeMode: TypeReprMode.AlwaysShow);
            Assert.AreEqual(expected: "1DArray([int(1), int(2), int(3)])",
                actual: new[] { 1, 2, 3 }.Repr(config: showTypes));
        }

        [Test]
        public void ExampleTestRepr()
        {
            var list = new List<int> { 1, 2, 3 };
            Assert.AreEqual(expected: "[int(1), int(2), int(3)]", actual: list.Repr());

            var config = new ReprConfig(FloatMode: FloatReprMode.Exact);
            var f = 3.14f;
            Assert.AreEqual(expected: "float(3.1400001049041748046875E+000)",
                actual: f.Repr(config: config));

            int? nullable = 123;
            Assert.AreEqual(expected: "int?(123)", actual: nullable.Repr());

            var parent = new Children { Name = "Parent" };
            var child = new Children { Name = "Child", Parent = parent };
            parent.Parent = child;
            Assert.IsTrue(
                condition: parent.Repr()
                                 .StartsWith(
                                      value:
                                      "Children(Name: \"Parent\", Parent: Children(Name: \"Child\", Parent: <Circular Reference to Children @"));
            Assert.IsTrue(condition: parent.Repr()
                                           .EndsWith(value: ">))"));
        }

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

        // Integer Types
        [TestCase(arg1: IntReprMode.Binary, arg2: "byte(0b101010)")]
        [TestCase(arg1: IntReprMode.Decimal, arg2: "byte(42)")]
        [TestCase(arg1: IntReprMode.Hex, arg2: "byte(0x2A)")]
        [TestCase(arg1: IntReprMode.HexBytes, arg2: "byte(0x2A)")]
        public void TestByteRepr(IntReprMode mode, string expected)
        {
            var config = new ReprConfig(IntMode: mode);
            Assert.AreEqual(expected: expected, actual: ((byte)42).Repr(config: config));
        }

        [TestCase(arg1: IntReprMode.Binary, arg2: "int(-0b101010)")]
        [TestCase(arg1: IntReprMode.Decimal, arg2: "int(-42)")]
        [TestCase(arg1: IntReprMode.Hex, arg2: "int(-0x2A)")]
        [TestCase(arg1: IntReprMode.HexBytes, arg2: "int(0xFFFFFFD6)")]
        public void TestIntRepr(IntReprMode mode, string expected)
        {
            var config = new ReprConfig(IntMode: mode);
            Assert.AreEqual(expected: expected, actual: (-42).Repr(config: config));
        }

        [Test]
        public void TestBigIntRepr()
        {
            var config = new ReprConfig(IntMode: IntReprMode.Decimal);
            Assert.AreEqual(expected: "BigInteger(-42)",
                actual: new BigInteger(value: -42).Repr(config: config));
        }

        // Floating Point Types
        [Test]
        public void TestFloatRepr_Exact()
        {
            var config = new ReprConfig(FloatMode: FloatReprMode.Exact);
            Assert.AreEqual(expected: "float(3.1415927410125732421875E+000)", actual: Single
               .Parse(s: "3.1415926535")
               .Repr(config: config));
        }

        [Test]
        public void TestDoubleRepr_Round()
        {
            var config = new ReprConfig(FloatMode: FloatReprMode.Round, FloatPrecision: 5);
            Assert.AreEqual(expected: "double(3.14159)", actual: Double.Parse(s: "3.1415926535")
               .Repr(config: config));
        }

        [Test]
        public void TestHalfRepr_Scientific()
        {
            var config = new ReprConfig(FloatMode: FloatReprMode.Scientific, FloatPrecision: 5);
            Assert.AreEqual(expected: "half(3.14063E+000)", actual: new Half(v: 3.14159)
               .Repr(config: config));
        }

        [Test]
        public void TestDecimalRepr_RawHex()
        {
            var config = new ReprConfig(FloatMode: FloatReprMode.HexBytes);
            Assert.AreEqual(expected: "decimal(0x001C00006582A5360B14388541B65F29)",
                actual: 3.1415926535897932384626433832795m.Repr(
                    config: config));
        }

        [Test]
        public void TestHalfRepr_BitField()
        {
            var config = new ReprConfig(FloatMode: FloatReprMode.BitField);
            Assert.AreEqual(expected: "half(0|10000|1001001000)", actual: new Half(v: 3.14159)
               .Repr(config: config));
        }

        [Test]
        public void TestFloatRepr_SpecialValues()
        {
            Assert.AreEqual(expected: "float(Quiet NaN)", actual: Single.NaN.Repr());
            Assert.AreEqual(expected: "float(Infinity)", actual: Single.PositiveInfinity.Repr());
            Assert.AreEqual(expected: "float(-Infinity)", actual: Single.NegativeInfinity.Repr());
        }

        [Test]
        public void TestDoubleRepr_SpecialValues()
        {
            Assert.AreEqual(expected: "double(Quiet NaN)", actual: Double.NaN.Repr());
            Assert.AreEqual(expected: "double(Infinity)", actual: Double.PositiveInfinity.Repr());
            Assert.AreEqual(expected: "double(-Infinity)", actual: Double.NegativeInfinity.Repr());
        }

        [Test]
        public void TestHalfRepr_SpecialValues()
        {
            Assert.AreEqual(expected: "half(Quiet NaN)", actual: new Half(v: Single.NaN).Repr());
            Assert.AreEqual(expected: "half(Infinity)",
                actual: new Half(v: Single.PositiveInfinity).Repr());
            Assert.AreEqual(expected: "half(-Infinity)",
                actual: new Half(v: Single.NegativeInfinity).Repr());
        }

        // Collections
        [Test]
        public void TestListRepr()
        {
            Assert.AreEqual(expected: "[]", actual: new List<int>().Repr());
            Assert.AreEqual(expected: "[int(1), int(2), int(3)]",
                actual: new List<int> { 1, 2, 3 }.Repr());
            Assert.AreEqual(expected: "[\"a\", null, \"c\"]",
                actual: new List<string?> { "a", null, "c" }.Repr());
        }

        [Test]
        public void TestEnumerableRepr()
        {
            Assert.AreEqual(expected: "RangeIterator([int(1), int(2), int(3)])", actual: Enumerable
               .Range(start: 1, count: 3)
               .Repr());
        }

        [Test]
        public void TestNestedListRepr()
        {
            var nestedList = new List<List<int>> { new() { 1, 2 }, new() { 3, 4, 5 }, new() };
            Assert.AreEqual(expected: "[[int(1), int(2)], [int(3), int(4), int(5)], []]",
                actual: nestedList.Repr());
        }

        // Arrays
        [Test]
        public void TestArrayRepr()
        {
            Assert.AreEqual(expected: "1DArray([])", actual: Array.Empty<int>()
               .Repr());
            Assert.AreEqual(expected: "1DArray([int(1), int(2), int(3)])",
                actual: new[] { 1, 2, 3 }.Repr());
        }

        [Test]
        public void TestJaggedArrayRepr()
        {
            var jagged2D = new[]
                { new[] { 1, 2 }, new[] { 3 } };
            Assert.AreEqual(expected: "JaggedArray([[int(1), int(2)], [int(3)]])",
                actual: jagged2D.Repr());
        }

        [Test]
        public void TestMultidimensionalArrayRepr()
        {
            var array2D = new[,] { { 1, 2 }, { 3, 4 } };
            Assert.AreEqual(expected: "2DArray([[int(1), int(2)], [int(3), int(4)]])",
                actual: array2D.Repr());
        }

        // Dictionaries, Sets, Queues
        [Test]
        public void TestDictionaryRepr()
        {
            var dict = new Dictionary<string, int> { [key: "a"] = 1, [key: "b"] = 2 };
            // Note: Dictionary order is not guaranteed, so we check for both possibilities
            var possibleOutputs = new[]
            {
                "{\"a\": int(1), \"b\": int(2)}",
                "{\"b\": int(2), \"a\": int(1)}"
            };
            Assert.Contains(expected: dict.Repr(), actual: possibleOutputs);
        }

        [Test]
        public void TestHashSetRepr()
        {
            var set = new HashSet<int> { 1, 2 };
            // Note: HashSet order is not guaranteed, so we sort the string representation for a stable test
            var repr = set.Repr(); // e.g., "{int(1), int(2), int(3)}"
            var possibleOutputs = new[]
            {
                "{int(1), int(2)}",
                "{int(2), int(1)}"
            };
            Assert.Contains(expected: repr, actual: possibleOutputs);
        }

        [Test]
        public void TestSortedSetRepr()
        {
            var set = new SortedSet<int> { 3, 1, 2 };
            var repr = set.Repr();
            Assert.AreEqual(expected: "SortedSet({int(1), int(2), int(3)})", actual: repr);
        }

        [Test]
        public void TestQueueRepr()
        {
            var queue = new Queue<string>();
            queue.Enqueue(item: "first");
            queue.Enqueue(item: "second");
            Assert.AreEqual(expected: "Queue([\"first\", \"second\"])", actual: queue.Repr());
        }

        [Test]
        public void TestStackRepr()
        {
            var stack = new Stack<int>();
            stack.Push(item: 1);
            stack.Push(item: 2);
            Assert.AreEqual(expected: "Stack([int(2), int(1)])", actual: stack.Repr());
        }

        // Custom Types
        [Test]
        public void TestCustomStructRepr_NoToString()
        {
            var point = new Point { X = 10, Y = 20 };
            Assert.AreEqual(expected: "Point(X: int(10), Y: int(20))", actual: point.Repr());
        }

        [Test]
        public void TestCustomStructRepr_WithToString()
        {
            var custom = new CustomStruct { Name = "test", Value = 42 };
            Assert.AreEqual(expected: "Custom(test, 42)", actual: custom.Repr());
        }

        [Test]
        public void TestClassRepr_WithToString()
        {
            var person = new Person(name: "Alice", age: 30);
            Assert.AreEqual(expected: "Alice(30)", actual: person.Repr());
        }

        [Test]
        public void TestClassRepr_NoToString()
        {
            var noToString = new NoToStringClass(data: "data", number: 123);
            Assert.AreEqual(expected: "NoToStringClass(Data: \"data\", Number: int(123))",
                actual: noToString.Repr());
        }

        [Test]
        public void TestRecordRepr()
        {
            var settings = new TestSettings(EquipmentName: "Printer",
                EquipmentSettings: new Dictionary<string, double>
                    { [key: "Temp (C)"] = 200.0, [key: "PrintSpeed (mm/s)"] = 30.0 });
            // Note: Dictionary order is not guaranteed, so we check for both possibilities
            var possibleOutputs = new[]
            {
                "TestSettings({ EquipmentName: \"Printer\", EquipmentSettings: {\"PrintSpeed (mm/s)\": double(30), \"Temp (C)\": double(200)} })",
                "TestSettings({ EquipmentName: \"Printer\", EquipmentSettings: {\"Temp (C)\": double(200), \"PrintSpeed (mm/s)\": double(30)} })"
            };
            Assert.Contains(expected: settings.Repr(), actual: possibleOutputs);
        }

        [Test]
        public void TestEnumRepr()
        {
            Assert.AreEqual(expected: "Colors.GREEN (int(1))", actual: Colors.GREEN.Repr());
        }

        [Test]
        public void TestTupleRepr()
        {
            Assert.AreEqual(expected: "(int(1), \"hello\")", actual: (1, "hello").Repr());
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
        public static int Add(int a, int b)
        {
            return a + b;
        }

        internal static long Add2(int a)
        {
            return a;
        }

        private T Add3<T>(T a)
        {
            return a;
        }

        private static void Add4(in int a, out int b)
        {
            b = a + 1;
        }

        private async Task<int> Lambda(int a)
        {
            // Added delay for truly async function.
            // However, this would not do much because it is only used for testing purposes
            // and not being called, only investigated the metadata of it.
            await Task.Delay(millisecondsDelay: 1);
            return a;
        }

        [Test]
        public void TestFunction()
        {
            var Add5 = new Func<int, int>((int a) => a + 1);
            var a = new Func<int, int, int>(Add);
            var b = new Func<int, long>(Add2);
            var e = new Func<int, Task<int>>(Lambda);

            Assert.AreEqual(expected: "internal int Lambda(int a)", actual: Add5.Repr());
            Assert.AreEqual(expected: "public static int Add(int a, int b)", actual: a.Repr());
            Assert.AreEqual(expected: "internal static long Add2(int a)", actual: b.Repr());
            Assert.AreEqual(expected: "private async Task<int> Lambda(int a)", actual: e.Repr());
        }

        [Test]
        public void TestCircularReference()
        {
            var a = new List<object>();
            a.Add(item: a);
            var repr = a.Repr();
            // object hash code can be different.
            Assert.IsTrue(condition: repr.StartsWith(value: "[<Circular Reference to List @"));
            Assert.IsTrue(condition: repr.EndsWith(value: ">]"));
        }

        [Test]
        public void TestReprConfig_MaxDepth()
        {
            var nestedList = new List<object>
                { 1, new List<object> { 2, new List<object> { 3 } } };
            var config = new ReprConfig(MaxDepth: 1);
            Assert.AreEqual(expected: "[int(1), <Max Depth Reached>]",
                actual: nestedList.Repr(config: config));

            config = new ReprConfig(MaxDepth: 0);
            Assert.AreEqual(expected: "<Max Depth Reached>",
                actual: nestedList.Repr(config: config));
        }

        [Test]
        public void TestReprConfig_MaxElementsPerCollection()
        {
            var list = new List<int> { 1, 2, 3, 4, 5 };
            var config = new ReprConfig(MaxElementsPerCollection: 3);
            Assert.AreEqual(expected: "[int(1), int(2), int(3), ... (2 more items)]",
                actual: list.Repr(config: config));

            config = new ReprConfig(MaxElementsPerCollection: 0);
            Assert.AreEqual(expected: "[... (5 more items)]", actual: list.Repr(config: config));
        }

        [Test]
        public void TestReprConfig_MaxStringLength()
        {
            var longString = "This is a very long string that should be truncated.";
            var config = new ReprConfig(MaxStringLength: 10);
            Assert.AreEqual(expected: "\"This is a ... (42 more letters)\"",
                actual: longString.Repr(config: config));

            config = new ReprConfig(MaxStringLength: 0);
            Assert.AreEqual(expected: "\"... (52 more letters)\"",
                actual: longString.Repr(config: config));
        }

        [Test]
        public void TestReprConfig_ShowNonPublicProperties()
        {
            var classified = new ClassifiedData(writer: "writer", data: "secret");
            var config = new ReprConfig(ShowNonPublicProperties: false);
            Assert.AreEqual(expected: "ClassifiedData(Writer: \"writer\")",
                actual: classified.Repr(config: config));

            config = new ReprConfig(ShowNonPublicProperties: true);
            Assert.AreEqual(
                expected: "ClassifiedData(Writer: \"writer\", private_Data: \"secret\")",
                actual: classified.Repr(config: config));
        }
    }
}