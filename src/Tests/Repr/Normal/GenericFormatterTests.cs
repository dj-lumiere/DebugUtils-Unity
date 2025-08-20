#nullable enable
using DebugUtils.Unity.Repr;
using DebugUtils.Unity.Tests.TestModels;
using NUnit.Framework;
using System.Collections.Generic;

namespace DebugUtils.Unity.Tests
{
    // Test data structures from ReprTest.cs
    public class GenericFormatterTests
    {
        // Custom Types
        [Test]
        public void TestCustomStructRepr_NoToString()
        {
            var point = new Point
            {
                X = 10,
                Y = 20
            };
            Assert.AreEqual(expected: "Point(X: 10_i32, Y: 20_i32)", actual: point.Repr());
        }

        [Test]
        public void TestCustomStructRepr_WithToString()
        {
            var custom = new CustomStruct
            {
                Name = "test",
                Value = 42
            };
            Assert.AreEqual(expected: "CustomStruct(Name: \"test\", Value: 42_i32)",
                actual: custom.Repr());
        }

        [Test]
        public void TestClassRepr_WithToString()
        {
            var person = new Person(name: "Alice", age: 30);
            Assert.AreEqual(expected: "Person(Age: 30_i32, Name: \"Alice\")",
                actual: person.Repr());
        }

        [Test]
        public void TestClassRepr_NoToString()
        {
            var noToString = new NoToStringClass(data: "data", number: 123);
            Assert.AreEqual(expected: "NoToStringClass(Data: \"data\", Number: 123_i32)",
                actual: noToString.Repr());
        }

        [Test]
        public void TestRecordRepr()
        {
            var settings = new TestSettings(EquipmentName: "Printer",
                EquipmentSettings: new Dictionary<string, double>
                    { [key: "Temp (C)"] = 200.0, [key: "PrintSpeed (mm/s)"] = 30.0 });
            var result = settings.Repr();
            Assert.AreEqual(
                expected:
                "TestSettings({ EquipmentName: \"Printer\", EquipmentSettings: {\"Temp (C)\": 200_f64, \"PrintSpeed (mm/s)\": 30_f64} })",
                actual: result);
        }

        [Test]
        public void TestEnumRepr()
        {
            Assert.AreEqual(expected: "Colors.GREEN (1_i32)", actual: Colors.GREEN.Repr());
        }

        [Test]
        public void TestCircularReference()
        {
            var a = new List<object>();
            a.Add(item: a);
            var repr = a.Repr();
            Assert.That(actual: repr,
                expression: Does.StartWith(expected: "[<Circular Reference to List @"));
            Assert.That(actual: repr, expression: Does.EndWith(expected: ">]"));
        }
    }
}