using System;
using NUnit.Framework;
using DebugUtils.Unity.CallStack;
using UnityEngine;
using UnityEngine.TestTools;

namespace DebugUtils.Unity.Tests
{
    public class CallStackTest
    {
        [Test]
        public void TestGetCallerName_Basic()
        {
            var callerName = CallStack.CallStack.GetCallerName();
            Assert.AreEqual(expected: "CallStackTest.TestGetCallerName_Basic",
                actual: callerName);
        }

        private class NestedClass
        {
            public string GetCallerNameFromNested()
            {
                return CallStack.CallStack.GetCallerName();
            }
        }

        [Test]
        public void TestGetCallerName_FromNestedClass()
        {
            var nested = new NestedClass();
            var callerName = nested.GetCallerNameFromNested();
            Assert.AreEqual(expected: "NestedClass.GetCallerNameFromNested",
                actual: callerName);
        }

        [Test]
        public void TestGetCallerName_FromLambda()
        {
            var lambdaCaller = new Func<string>(CallStack.CallStack.GetCallerName);
            var callerName = lambdaCaller();
            // The exact name for lambda can vary based on compiler, but it should contain the test method name
            Assert.AreEqual(expected: "CallStackTest.TestGetCallerName_FromLambda",
                actual: callerName);
        }

        [Test]
        public void TestGetCallerInfo_Basic()
        {
            var callerInfo = CallStack.CallStack.GetCallerInfo();
            Assert.AreEqual(
                expected: "CallStackTest.TestGetCallerInfo_Basic@CallStackTest.cs:43:9",
                actual: callerInfo.ToString());
        }
    }
}