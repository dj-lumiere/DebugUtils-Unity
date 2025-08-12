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

    public class GenericFormatterTests
    {
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
        public void TestCircularReference()
        {
            var a = new List<object>();
            a.Add(item: a);
            var repr = a.Repr();
            // object hash code can be different.
            Assert.IsTrue(condition: repr.StartsWith(value: "[<Circular Reference to List @"));
            Assert.IsTrue(condition: repr.EndsWith(value: ">]"));
        }
    }
}