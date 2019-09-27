using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Web;

namespace ClassRW
{
    public static class ClassWriter
    {
        public static void WriteObject(object SrcObject,StreamWriter Writer,int Indentation=-1)
        {
            Type SrcType = SrcObject.GetType();
            Indentation++;
            foreach (FieldInfo Field in SrcType.GetFields())
            {
                Type FType = Field.FieldType; ObjectType OType = Master.GetObjectType(FType); Object FObj = Field.GetValue(SrcObject);

                if (OType == ObjectType.Serial) { WriteLine(Field.Name, FObj, Writer, Indentation); }

                else if (OType == ObjectType.Composite) 
                {
                    WriteLine(Field.Name + ":{", Writer, Indentation);
                    WriteObject(FObj, Writer, Indentation);
                    WriteLine("}", Writer, Indentation);
                }

                else if (OType == ObjectType.Array || OType == ObjectType.List)
                {
                    Array Set;
                    if (OType == ObjectType.Array) { Set = (Array)FObj; }
                    else { Set = IListToArray((IList)FObj); }

                    WriteLine(Field.Name + ":[" + Set.Length, Writer, Indentation);
                    foreach (object Item in Set)
                    {
                        ObjectType ItemType = Master.GetObjectType(Item.GetType());
                        if (ItemType == ObjectType.Serial) { WriteLine(Item.ToString(), Writer, Indentation + 1); }
                        else
                        {
                            WriteLine("{", Writer, Indentation + 1);
                            WriteObject(Item, Writer, Indentation + 1);
                            WriteLine("}", Writer, Indentation + 1);
                        }
                    }
                    WriteLine("]", Writer, Indentation);
                }
            }
        }

        public static void WriteLine(string Line, StreamWriter Writer, int Indentation = 0)
        {
            Writer.WriteLine(new string(' ', Indentation * 2) + Line);
        }

        public static void WriteLine(string FieldName, object FieldValue, StreamWriter Writer, int Indentation = 0)
        {
            if (FieldValue.GetType() == typeof(string)) { FieldValue = "\"" + HttpUtility.JavaScriptStringEncode(FieldValue.ToString()) + "\""; }
            WriteLine(FieldName + ":" + FieldValue.ToString(),Writer,Indentation);
        }

        public static Array IListToArray(IList Set)
        {
            if (Set.Count == 0) { return null; }
            Array Arr = Array.CreateInstance(Set[0].GetType(), Set.Count);
            Set.CopyTo(Arr, 0);
            return Arr;
        }
    }
}
