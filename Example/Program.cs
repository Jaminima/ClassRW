using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ClassRW;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            TestObject TO = new TestObject();
            StreamWriter Writer = new StreamWriter("./Out.txt",false);
            ClassWriter.WriteObject(TO, Writer);
            Writer.Flush(); Writer.Close();

            StreamReader Reader = new StreamReader("./Out.txt");
            var V = ClassReader.ReadObject(typeof(TestObject), Reader);
        }
    }

    public class TestObject
    {
        public List<TestObject2> TO = new List<TestObject2> { new TestObject2(), new TestObject2() };
        public List<string> L = new List<string> { "Koom", "By", "Ah" };
        public int[] A = new int[] { 1, 5, 1, 2, 4 };
        public string S = "yee\n haw";
        public int I = 9;
        public bool B = true;
    }

    public class TestObject2
    {
        public string S = "Arrggg";
        public int I = 90;
    }
}
