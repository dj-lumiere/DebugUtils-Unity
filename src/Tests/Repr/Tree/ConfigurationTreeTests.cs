#nullable enable
using DebugUtils.Unity.Repr;
using DebugUtils.Unity.Tests.TestModels;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

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
                Hobbies = new List<string>
                {
                    "reading",
                    "coding"
                }
            };
            var actualJson = JToken.Parse(json: student.ReprTree()) ;
            Assert.AreEqual(expected: "Student", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            var nameNode = (JObject)actualJson[key: "Name"]!;
            Assert.AreEqual(expected: "string", actual: nameNode[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: 5, actual: nameNode[propertyName: "length"]!.Value<int>());
            Assert.AreEqual(expected: "Alice", actual: nameNode[propertyName: "value"]
              ?.ToString());
            var ageNode = actualJson[key: "Age"]!;
            Assert.AreEqual(expected: "30_i32", actual: ageNode.ToString());
            var hobbiesNode = (JObject)actualJson[key: "Hobbies"]!;
            Assert.AreEqual(expected: "List", actual: hobbiesNode[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: 2, actual: hobbiesNode[propertyName: "count"]!.Value<int>());
            var hobbiesValue = (JArray)hobbiesNode[propertyName: "value"]!;
            Assert.AreEqual(expected: "reading",
                actual: hobbiesValue[index: 0]![key: "value"]!.ToString());
            Assert.AreEqual(expected: "coding",
                actual: hobbiesValue[index: 1]![key: "value"]!.ToString());
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
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            var nameNode = (JObject)actualJson[key: "Name"]!;
            Assert.AreEqual(expected: "string", actual: nameNode[propertyName: "type"]
              ?.ToString());
            Assert.AreEqual(expected: 4, actual: nameNode[propertyName: "length"]!.Value<int>());
            Assert.AreEqual(expected: "John", actual: nameNode[propertyName: "value"]
              ?.ToString());
            var ageNode = actualJson[key: "Age"]!;
            Assert.AreEqual(expected: "30_i32", actual: ageNode.ToString());
        }

        [Test]
        public void TestReprConfig_MaxDepth_ReprTree()
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
            var actualJson = JToken.Parse(json: nestedList.ReprTree(config: config))!;
            Assert.AreEqual(expected: "List", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: 2, actual: actualJson[key: "count"]!.Value<int>());
            Assert.AreEqual(expected: "1_i32",
                actual: actualJson[key: "value"]![key: 0]!.ToString());
            Assert.AreEqual(expected: "List",
                actual: actualJson[key: "value"]![key: 1]![key: "type"]
                  ?.ToString());
            Assert.AreEqual(expected: "class",
                actual: actualJson[key: "value"]![key: 1]![key: "kind"]
                  ?.ToString());
            Assert.AreEqual(expected: "true",
                actual: actualJson[key: "value"]![key: 1]![key: "maxDepthReached"]
                  ?.ToString());
            Assert.AreEqual(expected: 1,
                actual: actualJson[key: "value"]![key: 1]![key: "depth"]!.Value<int>());
            config = ReprConfig.Configure()
                               .MaxDepth(depth: 0)
                               .Build();
            actualJson = JToken.Parse(json: nestedList.ReprTree(config: config))!;
            Assert.AreEqual(expected: "List", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.AreEqual(expected: "true", actual: actualJson[key: "maxDepthReached"]
              ?.ToString());
            Assert.AreEqual(expected: 0, actual: actualJson[key: "depth"]!.Value<int>());
        }

        [Test]
        public void TestReprConfig_MaxCollectionItems_ReprTree()
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
            var actualJson = JToken.Parse(json: list.ReprTree(config: config))!;
            Assert.AreEqual(expected: "List", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: 5, actual: actualJson[key: "count"]!.Value<int>());
            Assert.AreEqual(expected: 4, actual: ((JArray)actualJson[key: "value"]!).Count);
            Assert.AreEqual(expected: "1_i32",
                actual: actualJson[key: "value"]![key: 0]!.ToString());
            Assert.AreEqual(expected: "2_i32",
                actual: actualJson[key: "value"]![key: 1]!.ToString());
            Assert.AreEqual(expected: "3_i32",
                actual: actualJson[key: "value"]![key: 2]!.ToString());
            Assert.AreEqual(expected: "... (2 more items)",
                actual: actualJson[key: "value"]![key: 3]
                  ?.ToString());
            config = ReprConfig.Configure()
                               .MaxItems(count: 0)
                               .Build();
            actualJson = JToken.Parse(json: list.ReprTree(config: config))!;
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
            var config = ReprConfig.Configure()
                                   .MaxStringLength(length: 10)
                                   .Build();
            var actualJson = JToken.Parse(json: longString.ReprTree(config: config))!;
            Assert.AreEqual(expected: "string", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "This is a ... (42 more letters)",
                actual: actualJson[key: "value"]
                  ?.ToString());
            config = ReprConfig.Configure()
                               .MaxStringLength(length: 0)
                               .Build();
            actualJson = JToken.Parse(json: longString.ReprTree(config: config))!;
            Assert.AreEqual(expected: "string", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "... (52 more letters)", actual: actualJson[key: "value"]
              ?.ToString());
        }

        [Test]
        public void TestReprConfig_ShowNonPublicProperties_ReprTree()
        {
            var classified =
                new ClassifiedData(writer: "writer", data: "secret", password: "REDACTED");
            var config = ReprConfig.Configure()
                                   .ViewMode(mode: MemberReprMode.PublicFieldAutoProperty)
                                   .Build();
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
            Assert.AreEqual(expected: 6, actual: writerNode[propertyName: "length"]!.Value<int>());
            Assert.AreEqual(expected: "writer", actual: writerNode[propertyName: "value"]
              ?.ToString());
            config = ReprConfig.Configure()
                               .ViewMode(mode: MemberReprMode.AllFieldAutoProperty)
                               .Build();
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
            Assert.AreEqual(expected: 6, actual: writerNode[propertyName: "length"]!.Value<int>());
            Assert.AreEqual(expected: "writer", actual: writerNode[propertyName: "value"]
              ?.ToString());
            var secretDataNode = actualJson[key: "private_Data"];
            Assert.NotNull(anObject: secretDataNode);
            Assert.AreEqual(expected: "string", actual: secretDataNode[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: 6, actual: secretDataNode[key: "length"]!.Value<int>());
            Assert.AreEqual(expected: "secret", actual: secretDataNode[key: "value"]
              ?.ToString());
            var secretPasswordNode = actualJson[key: "private_Password"];
            Assert.NotNull(anObject: secretPasswordNode);
            Assert.AreEqual(expected: "string", actual: secretPasswordNode[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: 8, actual: secretPasswordNode[key: "length"]!.Value<int>());
            Assert.AreEqual(expected: "REDACTED", actual: secretPasswordNode[key: "value"]
              ?.ToString());
        }

        [Test]
        public void TestReprTree_WithFloats()
        {
            var a = new
            {
                x = 3.14f,
                y = 2.71f
            };
            var actualJson = JToken.Parse(json: a.ReprTree())!;
            Assert.AreEqual(expected: "Anonymous", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: actualJson[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            Assert.AreEqual(expected: "3.14_f32", actual: actualJson[key: "x"]!.ToString());
            Assert.AreEqual(expected: "2.71_f32", actual: actualJson[key: "y"]!.ToString());
        }
    }
}