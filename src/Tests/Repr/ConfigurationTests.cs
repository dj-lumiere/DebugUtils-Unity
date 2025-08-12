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

    public class ConfigurationTests
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