#nullable enable
namespace DebugUtils.Unity.Tests.TestModels
{
    public class NoToStringClass
    {
        public string Data { get; set; }
        public int Number { get; set; }

        public NoToStringClass(string data, int number)
        {
            Data = data;
            Number = number;
        }
    }
}