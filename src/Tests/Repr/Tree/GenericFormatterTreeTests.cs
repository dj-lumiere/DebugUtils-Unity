#nullable enable
using DebugUtils.Unity.Repr;
using DebugUtils.Unity.Tests.TestModels;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace DebugUtils.Unity.Tests
{
    public class GenericFormatterTreeTests
    {
        [Test]
        public void TestCustomStructRepr_NoToString()
        {
            var point = new Point
            {
                X = 10,
                Y = 20
            };
            var actualJson = JToken.Parse(json: point.ReprTree());
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "Point",
                [propertyName: "kind"] = "struct",
                [propertyName: "X"] = "10_i32",
                [propertyName: "Y"] = "20_i32"
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        [Test]
        public void TestCustomStructRepr_WithToString()
        {
            var custom = new CustomStruct
            {
                Name = "test",
                Value = 42
            };
            var actualJson = JToken.Parse(json: custom.ReprTree())!;
            Assert.AreEqual(expected: "CustomStruct", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "struct", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.Null(anObject: actualJson[key: "hashCode"]);
            var nameNode = (JObject)actualJson[key: "Name"]!;
            Assert.AreEqual(expected: "string", actual: nameNode[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: nameNode[propertyName: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: nameNode[propertyName: "hashCode"]);
            Assert.AreEqual(expected: 4, actual: nameNode[propertyName: "length"]!.Value<int>());
            Assert.AreEqual(expected: "test", actual: nameNode[propertyName: "value"]
              ?.ToString());
            var valueNode = actualJson[key: "Value"]!;
            Assert.AreEqual(expected: "42_i32", actual: valueNode.ToString());
        }

        [Test]
        public void TestClassRepr_WithToString()
        {
            var person = new Person(name: "Alice", age: 30);
            var actualJson = JToken.Parse(json: person.ReprTree())!;
            Assert.AreEqual(expected: "Person", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            var nameNode = (JObject)actualJson[key: "Name"]!;
            Assert.AreEqual(expected: "string", actual: nameNode[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: nameNode[propertyName: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: nameNode[propertyName: "hashCode"]);
            Assert.AreEqual(expected: 5, actual: nameNode[propertyName: "length"]!.Value<int>());
            Assert.AreEqual(expected: "Alice", actual: nameNode[propertyName: "value"]
              ?.ToString());
            var ageNode = actualJson[key: "Age"]!;
            Assert.AreEqual(expected: "30_i32", actual: ageNode.ToString());
        }

        [Test]
        public void TestClassRepr_NoToString()
        {
            var noToString = new NoToStringClass(data: "data", number: 123);
            var actualJson = JToken.Parse(json: noToString.ReprTree())!;
            Assert.AreEqual(expected: "NoToStringClass", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            var dataNode = (JObject)actualJson[key: "Data"]!;
            Assert.AreEqual(expected: "string", actual: dataNode[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: dataNode[propertyName: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: dataNode[propertyName: "hashCode"]);
            Assert.AreEqual(expected: 4, actual: dataNode[propertyName: "length"]!.Value<int>());
            Assert.AreEqual(expected: "data", actual: dataNode[propertyName: "value"]
              ?.ToString());
            var numberNode = actualJson[key: "Number"]!;
            Assert.AreEqual(expected: "123_i32", actual: numberNode.ToString());
        }

        [Test]
        public void TestRecordRepr()
        {
            var settings = new TestSettings(EquipmentName: "Printer",
                EquipmentSettings: new Dictionary<string, double>
                    { [key: "Temp (C)"] = 200.0, [key: "PrintSpeed (mm/s)"] = 30.0 });
            var actualJson = JToken.Parse(json: settings.ReprTree())!;
            Assert.AreEqual(expected: "TestSettings", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "record class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            var equipmentName = (JObject)actualJson[key: "EquipmentName"]!;
            Assert.AreEqual(expected: "string", actual: equipmentName[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: equipmentName[propertyName: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: equipmentName[propertyName: "hashCode"]);
            Assert.AreEqual(expected: 7,
                actual: equipmentName[propertyName: "length"]!.Value<int>());
            Assert.AreEqual(expected: "Printer", actual: equipmentName[propertyName: "value"]
              ?.ToString());
            var equipmentSettings = (JObject)actualJson[key: "EquipmentSettings"]!;
            Assert.AreEqual(expected: "Dictionary", actual: equipmentSettings[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: equipmentSettings[propertyName: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: equipmentSettings[propertyName: "hashCode"]);
            Assert.AreEqual(expected: 2,
                actual: equipmentSettings[propertyName: "count"]!.Value<int>());
            var settingsArray = (JArray)equipmentSettings[propertyName: "value"]!;
            Assert.AreEqual(expected: 2, actual: settingsArray.Count);
            // Since dictionary order isn't guaranteed, we check for presence of keys
            var tempSetting = settingsArray.FirstOrDefault(predicate: s =>
                s![key: "key"]![key: "value"]!.ToString() == "Temp (C)");
            Assert.NotNull(anObject: tempSetting);
            Assert.AreEqual(expected: "200_f64",
                actual: tempSetting[key: "value"]!.ToString());
            var speedSetting = settingsArray.FirstOrDefault(predicate: s =>
                s![key: "key"]![key: "value"]!.ToString() == "PrintSpeed (mm/s)");
            Assert.NotNull(anObject: speedSetting);
            Assert.AreEqual(expected: "30_f64", actual: speedSetting[key: "value"]!.ToString());
        }

        [Test]
        public void TestEnumRepr()
        {
            var actualJson = JToken.Parse(json: Colors.GREEN.ReprTree());
            var expectedJson = new JObject
            {
                [propertyName: "type"] = "Colors",
                [propertyName: "kind"] = "enum",
                [propertyName: "name"] = "GREEN",
                [propertyName: "value"] = "1_i32"
            };
            Assert.True(condition: JToken.DeepEquals(t1: actualJson, t2: expectedJson));
        }

        [Test]
        public void TestObjectReprTree()
        {
            var data = new
            {
                Name = "Alice",
                Age = 30
            };
            var actualJsonNode = JToken.Parse(json: data.ReprTree())!;
            Assert.AreEqual(expected: "Anonymous", actual: actualJsonNode[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJsonNode[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJsonNode[key: "hashCode"]);
            var nameNode = (JObject)actualJsonNode[key: "Name"]!;
            Assert.AreEqual(expected: "string", actual: nameNode[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: nameNode[propertyName: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: nameNode[propertyName: "hashCode"]);
            Assert.AreEqual(expected: 5, actual: nameNode[propertyName: "length"]!.Value<int>());
            Assert.AreEqual(expected: "Alice", actual: nameNode[propertyName: "value"]
              ?.ToString());
            var ageNode = actualJsonNode[key: "Age"]!;
            Assert.AreEqual(expected: "30_i32", actual: ageNode.ToString());
        }

        [Test]
        public void TestCircularReprTree()
        {
            List<object> a = new();
            a.Add(item: a);
            var actualJsonString = a.ReprTree();
            // Parse the JSON to verify structure
            var json = JToken.Parse(json: actualJsonString)!;
            // Verify top-level structure
            Assert.AreEqual(expected: "List", actual: json[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: 1, actual: json[key: "count"]!.Value<int>());
            // Verify circular reference structure
            var firstElement = json[key: "value"]![key: 0]!;
            Assert.AreEqual(expected: "CircularReference", actual: firstElement[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "List", actual: firstElement[key: "target"]![key: "type"]
              ?.ToString());
            Assert.That(actual: firstElement[key: "target"]![key: "hashCode"]
              ?.ToString(), expression: Does.StartWith(expected: "0x"));
        }
    }
}