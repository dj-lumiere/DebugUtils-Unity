using System.Collections.Generic;
using System.Threading.Tasks;
using DebugUtils.Unity.Repr.TypeHelpers;
using NUnit.Framework;

namespace DebugUtils.Tests
{
    public class TypeNamingTest
    {
        [Test]
        public void GetReprTypeNameTest()
        {
            var intType = typeof(int);
            Assert.AreEqual(expected: "int", actual: intType.GetReprTypeName());

            var listType = typeof(List<string>);
            Assert.AreEqual(expected: "List", actual: listType.GetReprTypeName());

            var intNullableType = typeof(int?);
            Assert.AreEqual(expected: typeof(int?), actual: typeof(int?));
            Assert.AreEqual(expected: "int?", actual: intNullableType.GetReprTypeName());

            var taskType = typeof(Task<bool>);
            Assert.AreEqual(expected: "Task<bool>", actual: taskType.GetReprTypeName());

            var dictType = typeof(Dictionary<string, List<int>>);
            Assert.AreEqual(expected: "Dictionary", actual: dictType.GetReprTypeName());

            var multiDimArrayType = typeof(int[,]);
            Assert.AreEqual(expected: "2DArray", actual: multiDimArrayType.GetReprTypeName());

            var jaggedArrayType = typeof(int[][]);
            Assert.AreEqual(expected: "JaggedArray", actual: jaggedArrayType.GetReprTypeName());

            var nestedType = typeof(OuterClass.NestedClass);
            Assert.AreEqual(expected: "NestedClass", actual: nestedType.GetReprTypeName());

            var genericMethodType = typeof(TypeNamingTest).GetMethod(name: nameof(GenericMethod))
                                                         ?.GetGenericArguments()[0];
            Assert.Null(anObject: genericMethodType?.GetReprTypeName());
        }

        public class OuterClass
        {
            public class NestedClass
            {
            }
        }

        private void GenericMethod<T>()
        {
        }
    }
}