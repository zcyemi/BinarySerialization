using System;
using System.IO;
using Serialization;

using Newtonsoft.Json;

namespace BinarySerialization
{
    class Program
    {
        static void Main(string[] args)
        {
            var testdata = new TestData();

            Console.WriteLine(JsonConvert.SerializeObject(testdata));
            byte[] data = BinarySeralizer.Serialize(testdata);
            var testdata1 = BinarySeralizer.Deserialize<TestData>(data);

            Console.WriteLine(JsonConvert.SerializeObject(testdata1));

        }
    }

    public class TestData{
        public UInt16 uint1 = UInt16.MaxValue;
        public UInt32 uint2 = UInt32.MaxValue;
        public UInt64 uint3 = UInt64.MaxValue;
        public Int16 int1 = Int16.MaxValue;
        public Int32 int2 = Int32.MaxValue;
        public Int64 int3 = Int32.MaxValue;
        public bool b = true;
        public byte by = 132;
        public float f = 342352.432f;
        public double d = -4312334.6534;
        public string s1 = "dwacaw -43d|";
        public string s2 = "";
        public string s3 = null;

    }
}