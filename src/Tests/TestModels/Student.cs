#nullable enable
using System.Collections.Generic;

namespace DebugUtils.Unity.Tests
{
    public class Student
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
        public List<string> Hobbies { get; set; } = new();
    }
}