#nullable enable
namespace DebugUtils.Unity.Tests.TestModels
{
    public struct CustomStruct
    {
        public string Name;
        public int Value;
        public override string ToString()
        {
            return $"Custom({Name}, {Value})";
        }
    }
}