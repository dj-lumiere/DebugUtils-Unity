#nullable enable

namespace DebugUtils.Unity.Tests
{
    public class ClassifiedData
    {
        public string Writer { get; set; }
        private string Data { get; set; }
        public ClassifiedData(string writer, string data)
        {
            Writer = writer;
            Data = data;
        }
    }
}