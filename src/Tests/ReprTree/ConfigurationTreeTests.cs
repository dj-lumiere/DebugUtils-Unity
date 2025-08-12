#nullable enable
using System.Collections.Generic;
using DebugUtils.Unity.Repr;
using NUnit.Framework;
using Newtonsoft.Json.Linq;

namespace DebugUtils.Unity.Tests
{

    public class ConfigurationTreeTests
    {
        [Test]
        public void TestReadme()
        {
            var student = new Student
            {
                Name = "Alice",
                Age = 30,
                Hobbies = new List<string> { "reading", "coding" }
            };
            var actualJson = JToken.Parse(json: student.ReprTree());

            Assert.AreEqual(expected: "Student", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);

            var nameNode = (JObject)actualJson[key: "Name"]!;
            Assert.AreEqual(expected: "string", actual: nameNode[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: 5,
                actual: nameNode[propertyName: "length"]!.Value<int>());
            Assert.AreEqual(expected: "Alice", actual: nameNode[propertyName: "value"]
              ?.ToString());

            var ageNode = (JObject)actualJson[key: "Age"]!;
            Assert.AreEqual(expected: "int", actual: ageNode[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "30", actual: ageNode[propertyName: "value"]
              ?.ToString());

            var hobbiesNode = (JObject)actualJson[key: "Hobbies"]!;
            Assert.AreEqual(expected: "List", actual: hobbiesNode[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: 2,
                actual: hobbiesNode[propertyName: "count"]!.Value<int>());

            var hobbiesValue = (JArray)hobbiesNode[propertyName: "value"]!;
            Assert.AreEqual(expected: "reading",
                actual: hobbiesValue[index: 0][key: "value"]!.Value<string>());
            Assert.AreEqual(expected: "coding",
                actual: hobbiesValue[index: 1][key: "value"]!.Value<string>());
        }
        
        
        [Test]
        public void TestExample()
        {
            var person = new Person(name: "John", age: 30);
            var actualJson = JToken.Parse(json: person.ReprTree());

            Assert.AreEqual(expected: "Person", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(actualJson[key: "hashCode"]);

            var nameNode = (JObject)actualJson[key: "Name"]!;
            Assert.AreEqual(expected: "string", actual: nameNode[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: 4, actual: nameNode[propertyName: "length"]!.Value<int>());
            Assert.AreEqual(expected: "John", actual: nameNode[propertyName: "value"]
              ?.ToString());

            var ageNode = (JObject)actualJson[key: "Age"]!;
            Assert.AreEqual(expected: "int", actual: ageNode[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "30", actual: ageNode[propertyName: "value"]
              ?.ToString());
        }

        [Test]
        public void TestReprConfig_MaxDepth_ReprTree()
        {
            var nestedList = new List<object>
                { 1, new List<object> { 2, new List<object> { 3 } } };
            var config = new ReprConfig(MaxDepth: 1);
            var actualJson = JToken.Parse(json: nestedList.ReprTree(config: config));
            Assert.AreEqual(expected: "List", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: 2,
                actual: actualJson[key: "count"]!.Value<int>());
            Assert.AreEqual(expected: "int",
                actual: actualJson[key: "value"]![key: 0]![key: "type"]
                  ?.ToString());
            Assert.AreEqual(expected: "struct",
                actual: actualJson[key: "value"]![key: 0]![key: "kind"]
                  ?.ToString());
            Assert.AreEqual(expected: "1",
                actual: actualJson[key: "value"]![key: 0]![key: "value"]
                  ?.ToString());
            Assert.AreEqual(expected: "List",
                actual: actualJson[key: "value"]![key: 1]![key: "type"]
                  ?.ToString());
            Assert.AreEqual(expected: "class",
                actual: actualJson[key: "value"]![key: 1]![key: "kind"]
                  ?.ToString());
            Assert.AreEqual(expected: "true",
                actual: actualJson[key: "value"]![key: 1]![
                        key: "maxDepthReached"]
                  ?.ToString());
            Assert.AreEqual(expected: 1,
                actual: actualJson[key: "value"]![key: 1]![key: "depth"]!
                   .Value<int>());

            config = new ReprConfig(MaxDepth: 0);
            actualJson = JToken.Parse(json: nestedList.ReprTree(config: config));
            Assert.AreEqual(expected: "List", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.AreEqual(expected: "true", actual: actualJson[key: "maxDepthReached"]
              ?.ToString());
            Assert.AreEqual(expected: 0,
                actual: actualJson[key: "depth"]!.Value<int>());
        }

        [Test]
        public void TestReprConfig_MaxCollectionItems_ReprTree()
        {
            var list = new List<int> { 1, 2, 3, 4, 5 };
            var config = new ReprConfig(MaxElementsPerCollection: 3);
            var actualJson = JToken.Parse(json: list.ReprTree(config: config));
            Assert.AreEqual(expected: "List", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: 5,
                actual: actualJson[key: "count"]!.Value<int>());
            Assert.AreEqual(expected: 4, actual: ((JArray)actualJson[key: "value"]!)
               .Count);
            Assert.AreEqual(expected: "int",
                actual: actualJson[key: "value"]![key: 0]![key: "type"]
                  ?.ToString());
            Assert.AreEqual(expected: "int",
                actual: actualJson[key: "value"]![key: 1]![key: "type"]
                  ?.ToString());
            Assert.AreEqual(expected: "int",
                actual: actualJson[key: "value"]![key: 2]![key: "type"]
                  ?.ToString());
            Assert.AreEqual(expected: "... (2 more items)",
                actual: actualJson[key: "value"]![key: 3]
                  ?.ToString());

            config = new ReprConfig(MaxElementsPerCollection: 0);
            actualJson = JToken.Parse(json: list.ReprTree(config: config));
            Assert.AreEqual(expected: "List", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "... (5 more items)",
                actual: actualJson[key: "value"]![key: 0]
                  ?.ToString());
        }

        [Test]
        public void TestReprConfig_MaxStringLength_ReprTree()
        {
            var longString = "This is a very long string that should be truncated.";
            var config = new ReprConfig(MaxStringLength: 10);
            var actualJson = JToken.Parse(json: longString.ReprTree(config: config));
            Assert.AreEqual(expected: "string", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "This is a ... (42 more letters)",
                actual: actualJson[key: "value"]
                  ?.ToString());

            config = new ReprConfig(MaxStringLength: 0);
            actualJson = JToken.Parse(json: longString.ReprTree(config: config));
            Assert.AreEqual(expected: "string", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "... (52 more letters)",
                actual: actualJson[key: "value"]
                  ?.ToString());
        }

        [Test]
        public void TestReprConfig_ShowNonPublicProperties_ReprTree()
        {
            var classified = new ClassifiedData(writer: "writer", data: "secret");
            var config = new ReprConfig(ShowNonPublicProperties: false);
            var actualJson = JToken.Parse(json: classified.ReprTree(config: config));
            Assert.NotNull(anObject: actualJson);
            Assert.AreEqual(expected: "ClassifiedData", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);

            var writerNode = (JObject)actualJson[key: "Writer"]!;
            Assert.AreEqual(expected: "string", actual: writerNode[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: 6,
                actual: writerNode[propertyName: "length"]!.Value<int>());
            Assert.AreEqual(expected: "writer", actual: writerNode[propertyName: "value"]
              ?.ToString());


            config = new ReprConfig(ShowNonPublicProperties: true);
            actualJson = JToken.Parse(json: classified.ReprTree(config: config));
            Assert.NotNull(anObject: actualJson);
            Assert.AreEqual(expected: "ClassifiedData", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);


            writerNode = (JObject)actualJson[key: "Writer"]!;
            Assert.AreEqual(expected: "string", actual: writerNode[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: 6,
                actual: writerNode[propertyName: "length"]!.Value<int>());
            Assert.AreEqual(expected: "writer", actual: writerNode[propertyName: "value"]
              ?.ToString());

            var secretNode = actualJson[key: "private_Data"];
            Assert.NotNull(anObject: secretNode);
            Assert.AreEqual(expected: "string", actual: secretNode![key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: 6,
                actual: secretNode[key: "length"]!.Value<int>());
            Assert.AreEqual(expected: "secret", actual: secretNode[key: "value"]
              ?.ToString());
        }
        
        [Test]
        public void TestReprTree_WithFloats()
        {
          var floatData = new { Pi = 3.14159f, E = 2.71828, Large = 123456789.123456789 };
          var actualJson = JToken.Parse(json: floatData.ReprTree());

          Assert.AreEqual(expected: "Anonymous", actual: actualJson[key: "type"]
          ?.ToString());
          Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
          ?.ToString());

          // Check Pi property (float)
          var piProperty = (JObject)actualJson[key: "Pi"]!;
          Assert.AreEqual(expected: "float", actual: piProperty[key: "type"]
          ?.ToString());
          Assert.AreEqual(expected: "struct", actual: piProperty[key: "kind"]
          ?.ToString());
          Assert.AreEqual("3.14159",
            piProperty[key: "value"]!.ToString());

          // Check E property (double)
          var eProperty = (JObject)actualJson[key: "E"]!;
          Assert.AreEqual(expected: "double", actual: eProperty[key: "type"]
          ?.ToString());
          Assert.AreEqual(expected: "struct", actual: eProperty[key: "kind"]
          ?.ToString());
          Assert.AreEqual("2.71828",
            eProperty[key: "value"]!.ToString());

          // Check Large property (double)
          var largeProperty = (JObject)actualJson[key: "Large"]!;
          Assert.AreEqual(expected: "double", actual: largeProperty[key: "type"]
          ?.ToString());
          Assert.AreEqual("123456789.123457",
            largeProperty[key: "value"]!.ToString());
        }
    }
}