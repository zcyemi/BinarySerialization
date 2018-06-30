using System;
using System.IO;
using Serialization;
using System.Diagnostics;

using Newtonsoft.Json;

namespace BinarySerialization
{
    class Program
    {
        static void Main(string[] args)
        {
            var testdata = new TestData();

            var watch = new Stopwatch();

            
            watch.Start();
            var jser = JsonConvert.SerializeObject(testdata);
            watch.Stop();
            Console.WriteLine(jser);

            Console.WriteLine("Json serialize:" + watch.ElapsedTicks +" - "+ watch.ElapsedMilliseconds+"ms");

            watch.Restart();
            byte[] data = BinarySeralizer.Serialize(testdata);
            watch.Stop();
            Console.WriteLine("Binary serialize:" + watch.ElapsedTicks +" - "+ watch.ElapsedMilliseconds+"ms");

            watch.Restart();
            var jdesdata = JsonConvert.DeserializeObject<TestData>(jser);
            watch.Stop();
            Console.WriteLine("Json deseralize:" + watch.ElapsedTicks +" - "+ watch.ElapsedMilliseconds+"ms");

            watch.Restart();
            var testdata1 = BinarySeralizer.Deserialize<TestData>(data);
            watch.Stop();
            Console.WriteLine("Binary deseralize:" + watch.ElapsedTicks +" - "+ watch.ElapsedMilliseconds+"ms");

            

            Console.WriteLine(JsonConvert.SerializeObject(testdata1));

        }
    }

    public class CC{
        public float cc = 10;
    }
    public class AA{
        public int xx;
        public CC c = new CC{cc = 433.9733f};
    }

    public class TestData
    {
        public string xxx = "DWDWW";
        public AA s1 = null;
        public AA s2 = new AA{xx = 42214};

        public CC classc = new CC{cc = 0.315854f};
    }
}
