#nullable enable
using DebugUtils.Unity.Repr;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace DebugUtils.Unity.Tests
{
    public class FunctionFormatterTreeTests
    {
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
            await Task.Delay(millisecondsDelay: 1);
            return a;
        }

        public delegate void Add4Delegate(in int a, out int b);

        [Test]
        public void TestFunctionHierarchical()
        {
            var add5 = new Func<int, int>((a) => a + 1);
            var a = new Func<int, int, int>(Add);
            var b = new Func<int, long>(Add2);
            var c = new Func<short, short>(Add3);
            Add4Delegate d = Add4;
            var e = new Func<int, Task<int>>(Lambda);

            var nullJsonObject = new JObject
            {
                [propertyName: "type"] = "object",
                [propertyName: "kind"] = "class",
                [propertyName: "value"] = null
            };
            var actualJson = JToken.Parse(json: add5.ReprTree())!;
            Assert.AreEqual(expected: "Function", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            var props = (JObject)actualJson[key: "properties"]!;
            Assert.AreEqual(expected: "Lambda", actual: props[propertyName: "name"]
              ?.ToString());
            Assert.AreEqual(expected: "int", actual: props[propertyName: "returnType"]
              ?.ToString());
            Assert.True(condition: JToken.DeepEquals(t1: new JArray(content: "internal"),
                t2: props[propertyName: "modifiers"]));
            var parameterArray = (JArray)props[propertyName: "parameters"]!;
            Assert.AreEqual(expected: Enumerable.Count(source: parameterArray), actual: 1);
            Assert.AreEqual(expected: "int", actual: parameterArray[index: 0]![key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "a", actual: parameterArray[index: 0]![key: "name"]
              ?.ToString());
            Assert.AreEqual(expected: "", actual: parameterArray[index: 0]![key: "modifier"]
              ?.ToString());
            Assert.IsEmpty(parameterArray[index: 0]![key: "defaultValue"]!);
            actualJson = JToken.Parse(json: a.ReprTree())!;
            Assert.AreEqual(expected: "Function", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            props = (JObject)actualJson[key: "properties"]!;
            Assert.AreEqual(expected: "Add", actual: props[propertyName: "name"]
              ?.ToString());
            Assert.AreEqual(expected: "int", actual: props[propertyName: "returnType"]
              ?.ToString());
            Assert.True(condition: JToken.DeepEquals(t1: new JArray("public", "static"),
                t2: props[propertyName: "modifiers"]));
            parameterArray = (JArray)props[propertyName: "parameters"]!;
            Assert.AreEqual(expected: 2, actual: parameterArray.Count);
            Assert.AreEqual(expected: "int", actual: parameterArray[index: 0]![key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "a", actual: parameterArray[index: 0]![key: "name"]
              ?.ToString());
            Assert.AreEqual(expected: "", actual: parameterArray[index: 0]![key: "modifier"]
              ?.ToString());
            Assert.IsEmpty(parameterArray[index: 0]![key: "defaultValue"]!);
            Assert.AreEqual(expected: "int", actual: parameterArray[index: 1]![key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "b", actual: parameterArray[index: 1]![key: "name"]
              ?.ToString());
            Assert.AreEqual(expected: "", actual: parameterArray[index: 1]![key: "modifier"]
              ?.ToString());
            Assert.IsEmpty(parameterArray[index: 1]![key: "defaultValue"]);
            actualJson = JToken.Parse(json: b.ReprTree())!;
            Assert.AreEqual(expected: "Function", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            props = (JObject)actualJson[key: "properties"]!;
            Assert.AreEqual(expected: "Add2", actual: props[propertyName: "name"]
              ?.ToString());
            Assert.AreEqual(expected: "long", actual: props[propertyName: "returnType"]
              ?.ToString());
            Assert.True(condition: JToken.DeepEquals(t1: new JArray("internal", "static"),
                t2: props[propertyName: "modifiers"]));
            parameterArray = (JArray)props[propertyName: "parameters"]!;
            Assert.AreEqual(expected: Enumerable.Count(source: parameterArray), actual: 1);
            Assert.AreEqual(expected: "int", actual: parameterArray[index: 0]![key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "a", actual: parameterArray[index: 0]![key: "name"]
              ?.ToString());
            Assert.AreEqual(expected: "", actual: parameterArray[index: 0]![key: "modifier"]
              ?.ToString());
            Assert.IsEmpty(parameterArray[index: 0]![key: "defaultValue"]);
            actualJson = JToken.Parse(json: c.ReprTree())!;
            Assert.AreEqual(expected: "Function", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            props = (JObject)actualJson[key: "properties"]!;
            Assert.AreEqual(expected: "Add3", actual: props[propertyName: "name"]
              ?.ToString());
            Assert.AreEqual(expected: "short", actual: props[propertyName: "returnType"]
              ?.ToString());
            Assert.True(condition: JToken.DeepEquals(t1: new JArray("private", "generic"),
                t2: props[propertyName: "modifiers"]));
            parameterArray = (JArray)props[propertyName: "parameters"]!;
            Assert.AreEqual(expected: Enumerable.Count(source: parameterArray), actual: 1);
            Assert.AreEqual(expected: "short", actual: parameterArray[index: 0]![key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "a", actual: parameterArray[index: 0]![key: "name"]
              ?.ToString());
            Assert.AreEqual(expected: "", actual: parameterArray[index: 0]![key: "modifier"]
              ?.ToString());
            Assert.IsEmpty(parameterArray[index: 0]![key: "defaultValue"]);
            actualJson = JToken.Parse(json: d.ReprTree())!;
            Assert.AreEqual(expected: "Function", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            props = (JObject)actualJson[key: "properties"]!;
            Assert.AreEqual(expected: "Add4", actual: props[propertyName: "name"]
              ?.ToString());
            Assert.AreEqual(expected: "void", actual: props[propertyName: "returnType"]
              ?.ToString());
            Assert.True(condition: JToken.DeepEquals(t1: new JArray("private", "static"),
                t2: props[propertyName: "modifiers"]));
            parameterArray = (JArray)props[propertyName: "parameters"]!;
            Assert.AreEqual(expected: 2, actual: parameterArray.Count);
            Assert.AreEqual(expected: "ref int", actual: parameterArray[index: 0]![key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "a", actual: parameterArray[index: 0]![key: "name"]
              ?.ToString());
            Assert.AreEqual(expected: "in", actual: parameterArray[index: 0]![key: "modifier"]
              ?.ToString());
            Assert.IsEmpty(parameterArray[index: 0]![key: "defaultValue"]);
            Assert.AreEqual(expected: "ref int", actual: parameterArray[index: 1]![key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "b", actual: parameterArray[index: 1]![key: "name"]
              ?.ToString());
            Assert.AreEqual(expected: "out", actual: parameterArray[index: 1]![key: "modifier"]
              ?.ToString());
            Assert.IsEmpty(parameterArray[index: 1]![key: "defaultValue"]);
            actualJson = JToken.Parse(json: e.ReprTree())!;
            Assert.AreEqual(expected: "Function", actual: actualJson[key: "type"]
              ?.ToString());
            Assert.NotNull(anObject: actualJson[key: "hashCode"]);
            props = (JObject)actualJson[key: "properties"]!;
            Assert.AreEqual(expected: "Lambda", actual: props[propertyName: "name"]
              ?.ToString());
            Assert.AreEqual(expected: "Task<int>", actual: props[propertyName: "returnType"]
              ?.ToString());
            Assert.True(condition: JToken.DeepEquals(t1: new JArray("private", "async"),
                t2: props[propertyName: "modifiers"]));
            parameterArray = (JArray)props[propertyName: "parameters"]!;
            Assert.AreEqual(expected: Enumerable.Count(source: parameterArray), actual: 1);
            Assert.AreEqual(expected: "int", actual: parameterArray[index: 0]![key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "a", actual: parameterArray[index: 0]![key: "name"]
              ?.ToString());
            Assert.AreEqual(expected: "", actual: parameterArray[index: 0]![key: "modifier"]
              ?.ToString());
            Assert.IsEmpty(parameterArray[index: 0]![key: "defaultValue"]);
        }
    }
}