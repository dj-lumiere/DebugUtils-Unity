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

    public class FunctionFormatterTests
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
            // Added delay for truly async function.
            // However, this would not do much because it is only used for testing purposes
            // and not being called, only investigated the metadata of it.
            await Task.Delay(millisecondsDelay: 1);
            return a;
        }

        [Test]
        public void TestFunction()
        {
            var Add5 = new Func<int, int>((int a) => a + 1);
            var a = new Func<int, int, int>(Add);
            var b = new Func<int, long>(Add2);
            var e = new Func<int, Task<int>>(Lambda);

            Assert.AreEqual(expected: "internal int Lambda(int a)", actual: Add5.Repr());
            Assert.AreEqual(expected: "public static int Add(int a, int b)", actual: a.Repr());
            Assert.AreEqual(expected: "internal static long Add2(int a)", actual: b.Repr());
            Assert.AreEqual(expected: "private async Task<int> Lambda(int a)", actual: e.Repr());
        }
    }
}