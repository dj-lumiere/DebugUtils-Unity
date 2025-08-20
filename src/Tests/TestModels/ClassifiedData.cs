#nullable enable
using System;
using System.Linq;
using System.Numerics;

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
        public DateTimeOffset RealDate => new DateTimeOffset(Date, TimeSpan.Zero);
        private int Hash => Password.GetHashCode();

        private int DataChecksum => unchecked(Data.Select(c => (int)c)
                                                  .Aggregate(0,
                                                       (seed, r) => seed * 31 + r.GetHashCode()));

        private BigInteger keyInt => Key.ToByteArray()
                                        .Aggregate(BigInteger.Zero, (seed, b) => seed * 256 + b);

        public ClassifiedData(string writer, string dataValue, string password)
        {
            Writer = writer;
            Data = dataValue;
            Password = password;
        }
    }
}