#nullable enable
using DebugUtils.Unity.Repr;
using NUnit.Framework;
using System.Threading.Tasks;
using System;

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
            Assert.AreEqual(expected: "internal int Lambda(int a)", actual: add5.Repr());
            Assert.AreEqual(expected: "public static int Add(int a, int b)", actual: a.Repr());
            Assert.AreEqual(expected: "internal static long Add2(int a)", actual: b.Repr());
            Assert.AreEqual(expected: "private generic short Add3(short a)", actual: c.Repr());
            Assert.AreEqual(expected: "private static void Add4(in ref int a, out ref int b)",
                actual: d.Repr());
            Assert.AreEqual(expected: "private async Task<int> Lambda(int a)", actual: e.Repr());
        }
    }
}