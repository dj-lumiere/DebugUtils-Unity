#nullable enable
using DebugUtils.Unity.Repr;
using DebugUtils.Unity.Tests.TestModels;
using NUnit.Framework;
using System.Collections.Generic;

namespace DebugUtils.Unity.Tests
{
    public class ConfigurationTests
    {
        [Test]
        public void TestReprConfig_MaxDepth()
        {
            var nestedList = new List<object>
            {
                1,
                new List<object>
                {
                    2,
                    new List<object>
                    {
                        3
                    }
                }
            };
            var config = ReprConfig.Configure()
                                   .MaxDepth(depth: 1)
                                   .Build();
            Assert.AreEqual(expected: "[1_i32, <Max Depth Reached>]",
                actual: nestedList.Repr(config: config));
            config = ReprConfig.Configure()
                               .MaxDepth(depth: 0)
                               .Build();
            Assert.AreEqual(expected: "<Max Depth Reached>",
                actual: nestedList.Repr(config: config));
        }

        [Test]
        public void TestReprConfig_MaxElementsPerCollection()
        {
            var list = new List<int>
            {
                1,
                2,
                3,
                4,
                5
            };
            var config = ReprConfig.Configure()
                                   .MaxItems(count: 3)
                                   .Build();
            Assert.AreEqual(expected: "[1_i32, 2_i32, 3_i32, ... (2 more items)]",
                actual: list.Repr(config: config));
            config = ReprConfig.Configure()
                               .MaxItems(count: 0)
                               .Build();
            Assert.AreEqual(expected: "[... (5 more items)]", actual: list.Repr(config: config));
        }

        [Test]
        public void TestReprConfig_MaxStringLength()
        {
            var longString = "This is a very long string that should be truncated.";
            var config = ReprConfig.Configure()
                                   .MaxStringLength(length: 10)
                                   .Build();
            Assert.AreEqual(expected: "\"This is a ... (42 more letters)\"",
                actual: longString.Repr(config: config));
            config = ReprConfig.Configure()
                               .MaxStringLength(length: 0)
                               .Build();
            Assert.AreEqual(expected: "\"... (52 more letters)\"",
                actual: longString.Repr(config: config));
        }

        [Test]
        public void TestReprConfig_ShowNonPublicProperties()
        {
            var classified =
                new ClassifiedData(writer: "writer", dataValue: "secret", password: "REDACTED");
            var config = ReprConfig.Configure()
                                   .ViewMode(mode: MemberReprMode.PublicFieldAutoProperty)
                                   .Build();
            Assert.AreEqual(
                expected:
                "ClassifiedData(Age: 10_i32, Id: 5_i64, Name: \"Lumi\", Writer: \"writer\")",
                actual: classified.Repr(config: config));
            config = ReprConfig.Configure()
                               .ViewMode(mode: MemberReprMode.AllFieldAutoProperty)
                               .Build();
            Assert.AreEqual(
                expected:
                "ClassifiedData(Age: 10_i32, Id: 5_i64, Name: \"Lumi\", Writer: \"writer\", private_Date: DateTime(1970.01.01 00:00:00.0000000 UTC), private_Password: \"REDACTED\", private_Data: \"secret\", private_Key: Guid(9a374b45-3771-4e91-b5e9-64bfa545efe9))",
                actual: classified.Repr(config: config));
        }

        [Test]
        public void TestReadmeTestRepr()
        {
            var arr = new int[]
            {
                1,
                2,
                3,
                4
            };
            Assert.AreEqual(expected: "System.Int32[]", actual: arr.ToString());
            var dict = new Dictionary<string, int>
            {
                {
                    "a",
                    1
                },
                {
                    "b",
                    2
                }
            };
            Assert.AreEqual(
                expected: "System.Collections.Generic.Dictionary`2[System.String,System.Int32]",
                actual: dict.ToString());
            var data = new
            {
                Name = "Alice",
                Age = 30,
                Scores = new[]
                {
                    95,
                    87,
                    92
                }
            };
            Assert.AreEqual(
                expected:
                "Anonymous(Age: 30_i32, Name: \"Alice\", Scores: 1DArray([95_i32, 87_i32, 92_i32]))",
                actual: data.Repr());
            Assert.AreEqual(expected: "1DArray([1_i32, 2_i32, 3_i32])",
                actual: new[] { 1, 2, 3 }.Repr());
            Assert.AreEqual(expected: "2DArray([[1_i32, 2_i32], [3_i32, 4_i32]])",
                actual: new[,] { { 1, 2 }, { 3, 4 } }.Repr());
            Assert.AreEqual(expected: "JaggedArray([[1_i32, 2_i32], [3_i32, 4_i32, 5_i32]])",
                actual: new int[][] { new[] { 1, 2 }, new[] { 3, 4, 5 } }.Repr());
            Assert.AreEqual(expected: "[1_i32, 2_i32, 3_i32]",
                actual: new List<int> { 1, 2, 3 }.Repr());
            Assert.AreEqual(expected: "{\"a\", \"b\"}",
                actual: new HashSet<string> { "a", "b" }.Repr());
            Assert.AreEqual(expected: "{\"x\": 1_i32}",
                actual: new Dictionary<string, int> { { "x", 1 } }.Repr());
            Assert.AreEqual(expected: "42_i32", actual: 42.Repr());
            Assert.AreEqual(expected: "0x2A_i32", actual: 42.Repr(config: ReprConfig.Configure()
               .IntFormat(format: "X")
               .Build()));
            Assert.AreEqual(expected: "0b101010_i32", actual: 42.Repr(config: ReprConfig
               .Configure()
               .IntFormat(format: "B")
               .Build()));
            Assert.AreEqual(
                expected: "3.000000000000000444089209850062616169452667236328125E-001_f64",
                actual: (0.1 + 0.2).Repr());
            Assert.AreEqual(
                expected: "2.99999999999999988897769753748434595763683319091796875E-001_f64",
                actual: 0.3.Repr());
            Assert.AreEqual(expected: "0.3_f64", actual: (0.1 + 0.2).Repr(
                config: ReprConfig.Configure()
                                  .FloatFormat(format: "G")
                                  .Build()));
            var hideTypes = ReprConfig.Configure()
                                      .TypeMode(mode: TypeReprMode.AlwaysHide)
                                      .UseSimpleFormatsInContainers(use: false)
                                      .Build();
            Assert.AreEqual(expected: "[1_i32, 2_i32, 3_i32]",
                actual: new[] { 1, 2, 3 }.Repr(config: hideTypes));
            var showTypes = ReprConfig.Configure()
                                      .TypeMode(mode: TypeReprMode.AlwaysShow)
                                      .Build();
            Assert.AreEqual(expected: "1DArray([1_i32, 2_i32, 3_i32])",
                actual: new[] { 1, 2, 3 }.Repr(config: showTypes));
        }

        [Test]
        public void TestExampleTestRepr()
        {
            var list = new List<int>
            {
                1,
                2,
                3
            };
            Assert.AreEqual(expected: "[1_i32, 2_i32, 3_i32]", actual: list.Repr());
            var config = ReprConfig.Configure()
                                   .FloatFormat(format: "EX")
                                   .Build();
            var f = 3.14f;
            Assert.AreEqual(expected: "3.1400001049041748046875E+000_f32",
                actual: f.Repr(config: config));
            int? nullable = 123;
            Assert.AreEqual(expected: "123_i32?", actual: nullable.Repr());
            var parent = new Children
            {
                Name = "Parent"
            };
            var child = new Children
            {
                Name = "Child",
                Parent = parent
            };
            parent.Parent = child;
            Assert.That(actual: parent.Repr(),
                expression: Does.StartWith(
                    expected:
                    "Children(Name: \"Parent\", Parent: Children(Name: \"Child\", Parent: <Circular Reference to Children @"));
            Assert.That(actual: parent.Repr(), expression: Does.EndWith(expected: ">))"));
        }

        [Test]
        public void TestReprConfig_AllPublicMode()
        {
            var classified = new ClassifiedData(
                writer: "Lumi",
                dataValue: "Now Top Secret Accessing",
                password: "REDACTED"
            );

            var config = new ReprConfig(ViewMode: MemberReprMode.AllPublic);
            var actual = classified.Repr(config);

            // Should include all public fields and properties (including computed)
            Assert.That(actual, Does.Contain("Age: 10_i32"));
            Assert.That(actual, Does.Contain("Id: 5_i64"));
            Assert.That(actual, Does.Contain("Name: \"Lumi\""));
            Assert.That(actual, Does.Contain("Writer: \"Lumi\""));
            Assert.That(actual, Does.Contain("RealDate: DateTimeOffset(1970.01.01 00:00:00.0000000Z)"));

            // Should NOT include private members
            Assert.That(actual, Does.Not.Contain("private_"));
            Assert.That(actual, Does.Not.Contain("REDACTED"));
            Assert.That(actual, Does.Not.Contain("Now Top Secret Accessing"));
        }

        [Test]
        public void TestReprConfig_EverythingMode()
        {
            var classified = new ClassifiedData(
                writer: "Lumi",
                dataValue: "Now Top Secret Accessing",
                password: "REDACTED"
            );

            var config = new ReprConfig(ViewMode: MemberReprMode.Everything);
            var actual = classified.Repr(config);

            // Should include all public fields and properties
            Assert.That(actual, Does.Contain("Age: 10_i32"));
            Assert.That(actual, Does.Contain("Id: 5_i64"));
            Assert.That(actual, Does.Contain("Name: \"Lumi\""));
            Assert.That(actual, Does.Contain("Writer: \"Lumi\""));

            // Should include private fields
            Assert.That(actual, Does.Contain("private_Date: DateTime(1970.01.01 00:00:00.0000000 UTC)"));
            Assert.That(actual, Does.Contain("private_Password: \"REDACTED\""));
            Assert.That(actual, Does.Contain("private_Data: \"Now Top Secret Accessing\""));
            Assert.That(actual, Does.Contain("private_Key: Guid(9a374b45-3771-4e91-b5e9-64bfa545efe9)"));

            // Should include private computed properties
            Assert.That(actual, Does.Contain("private_DataChecksum:"));
            Assert.That(actual, Does.Contain("private_Hash:"));
            Assert.That(actual, Does.Contain("private_keyInt:"));
        }
    }
}