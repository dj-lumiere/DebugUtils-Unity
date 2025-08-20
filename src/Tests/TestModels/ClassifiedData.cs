#nullable enable
using System;

namespace DebugUtils.Unity.Tests.TestModels
{
    public class ClassifiedData
    {
        public long Id = 5;
        public string Writer { get; set; }
        private string Data { get; set; }

        private DateTime Date = DateTime.UnixEpoch;
        public int Age = 10;
        private Guid Key { get; set; } = new(g: "9a374b45-3771-4e91-b5e9-64bfa545efe9");
        public string Name { get; set; } = "Lumi";
        private string Password;

        public ClassifiedData(string writer, string data, string password)
        {
            Writer = writer;
            Data = data;
            Password = password;
        }
    }
}