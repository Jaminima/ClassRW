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
            TestObject TO = new TestObject(); TO.T0 = new TestObject(); TO.T0.T0 = new TestObject();
            StreamWriter Writer = new StreamWriter("./Out.txt", false);
            ClassReadWriter.WriteObject(TO,TO.GetType(),Writer);
            Writer.Flush(); Writer.Close();

            StreamReader Reader = new StreamReader("./Out.txt");
            var v = ClassReadWriter.ReadObject(TO.GetType(), Reader);
        }
    }

    public class TestObject
    {
        public string S = "yee haw";
        public int I = 9;
        public TestObject T0;
        public bool B = true;
    }

}
