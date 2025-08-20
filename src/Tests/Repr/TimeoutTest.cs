#nullable enable
using DebugUtils.Unity.Repr;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using System.Threading;
using System;

namespace DebugUtils.Unity.Tests
{
    public class TimeoutTest
    {
        // Test class with intentionally slow properties
        public class SlowObject
        {
            public string FastProperty => "I'm fast!";

            public string SlowProperty
            {
                get
                {
                    Thread.Sleep(millisecondsTimeout: 5); // 5ms - should timeout with 1ms limit
                    return "I'm slow!";
                }
            }

            public string VerySlowProperty
            {
                get
                {
                    Thread.Sleep(millisecondsTimeout: 10); // 10ms - definitely should timeout
                    return "I'm very slow!";
                }
            }
        }

        [Test]
        public void TestMemberTimeout_SlowProperty_ShouldTimeout()
        {
            var obj = new SlowObject();
            var config = ReprConfig.Configure()
                                   .MaxMemberTime(milliseconds: 1)
                                   .ViewMode(mode: MemberReprMode.AllPublic)
                                   .Build(); // 1ms timeout
            var result = obj.Repr(config: config);
            var expected =
                "SlowObject(FastProperty: \"I'm fast!\", SlowProperty: [Timed Out], VerySlowProperty: [Timed Out])";
            Assert.AreEqual(expected: expected, actual: result);
        }

        [Test]
        public void TestMemberTimeout_FastProperty_ShouldNotTimeout()
        {
            var obj = new SlowObject();
            var config = ReprConfig.Configure()
                                   .MaxMemberTime(milliseconds: 500)
                                   .ViewMode(mode: MemberReprMode.AllPublic)
                                   .Build(); // 500ms timeout - plenty of time
            var result = obj.Repr(config: config);
            var expected =
                "SlowObject(FastProperty: \"I'm fast!\", SlowProperty: \"I'm slow!\", VerySlowProperty: \"I'm very slow!\")";
            // All properties should work with a generous timeout
            Assert.AreEqual(expected: expected, actual: result);
        }

        [Test]
        public void TestMemberTimeout_JsonTree_ShouldTimeout()
        {
            var obj = new SlowObject();
            var config = ReprConfig.Configure()
                                   .MaxMemberTime(milliseconds: 1)
                                   .ViewMode(mode: MemberReprMode.AllPublic)
                                   .Build();
            var jsonResult = obj.ReprTree(config: config);
            // Parse the JSON result
            var jsonNode = JToken.Parse(json: jsonResult);
            Assert.NotNull(anObject: jsonNode);
            // Verify the root object structure
            Assert.AreEqual(expected: "SlowObject", actual: jsonNode[key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: jsonNode[key: "kind"]
              ?.ToString());
            Assert.NotNull(anObject: jsonNode[key: "hashCode"]);
            // Verify FastProperty - should work fine
            var fastProp = jsonNode[key: "FastProperty"];
            Assert.NotNull(anObject: fastProp);
            Assert.AreEqual(expected: "string", actual: fastProp![key: "type"]
              ?.ToString());
            Assert.AreEqual(expected: "class", actual: fastProp[key: "kind"]
              ?.ToString());
            Assert.AreEqual(expected: "I'm fast!", actual: fastProp[key: "value"]
              ?.ToString());
            Assert.AreEqual(expected: 9, actual: fastProp[key: "length"]
              ?.Value<int>());
            // Verify SlowProperty - should be timed out
            var slowProp = jsonNode[key: "SlowProperty"];
            Assert.NotNull(anObject: slowProp);
            Assert.AreEqual(expected: "[Timed Out]", actual: slowProp.ToString());
            // Verify VerySlowProperty - should be timed out
            var verySlowProp = jsonNode[key: "VerySlowProperty"];
            Assert.NotNull(anObject: verySlowProp);
            Assert.AreEqual(expected: "[Timed Out]", actual: verySlowProp.ToString());
        }

        // Test class with property that throws exceptions
        public class ErrorObject
        {
            public string GoodProperty => "I work fine";

            public string BadProperty =>
                throw new InvalidOperationException(message: "I always fail!");
        }

        [Test]
        public void TestMemberError_ShouldCatchException()
        {
            var obj = new ErrorObject();
            var config = ReprConfig.Configure()
                                   .ViewMode(mode: MemberReprMode.AllPublic)
                                   .MaxMemberTime(milliseconds: 5)
                                   .Build();
            var result = obj.Repr(config: config);
            // Should handle exceptions gracefully
            Assert.AreEqual(
                expected:
                "ErrorObject(BadProperty: [InvalidOperationException: I always fail!], GoodProperty: \"I work fine\")",
                actual: result);
        }

        public class InfiniteLoopClass
        {
            public string LoopingProperty
            {
                get
                {
                    // Infinite loop but not stack overflow - this can be timed out
                    while (true)
                    {
                        Thread.Sleep(millisecondsTimeout: 1);
                    }
                }
            }
        }

        [Test]
        public void TestMemberTimeout_InfiniteLoop_ShouldTimeout()
        {
            var obj = new InfiniteLoopClass();
            var config = ReprConfig.Configure()
                                   .MaxMemberTime(milliseconds: 10)
                                   .ViewMode(mode: MemberReprMode.AllPublic)
                                   .Build(); // 10ms timeout
            var result = obj.Repr(config: config);
            // Should handle infinite loops by timing out
            Assert.AreEqual(expected: "InfiniteLoopClass(LoopingProperty: [Timed Out])",
                actual: result);
        }
    }
}