﻿using System;
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
            TestObject TO = new TestObject(); TO.L = new List<TestObject> { /*new TestObject(), new TestObject()*/ };
            StreamWriter Writer = new StreamWriter("./Out.txt", false);
            ClassReadWriter.WriteObject(TO, TO.GetType(), Writer);
            Writer.Flush(); Writer.Close();

            StreamReader Reader = new StreamReader("./Out.txt");
            var v = ClassReadWriter.ReadObject(TO.GetType(), Reader);
        }
    }

    public class TestObject
    {
        public List<TestObject> L;
        public int[] A = new int[] { 1, 5, 1, 2, 4 };
        public string S = "yee haw";
        public int I = 9;
        public bool B = true;
    }
}
