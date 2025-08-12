#nullable enable

namespace DebugUtils.Unity.Tests
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