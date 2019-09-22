using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;

namespace ClassRW
{
    public static class ClassReadWriter
    {
        public static void WriteObject(object SrcObject, Type ObjectType, StreamWriter Writer, int IndentationDepth = 0)
        {
            foreach (FieldInfo Field in ObjectType.GetFields())
            {
                object FieldValue = Field.GetValue(SrcObject);
                if (FieldValue == null) { WriteItem(Field.Name + ":NULL", Writer, IndentationDepth); }
                else if (Field.FieldType.IsSerializable) { WriteItem(Field.Name + ":" + FieldValue, Writer, IndentationDepth); }
                else
                {
                    WriteItem(Field.Name + ":{", Writer, IndentationDepth);
                    WriteObject(FieldValue, Field.FieldType, Writer, IndentationDepth + 1);
                    WriteItem("}", Writer, IndentationDepth);
                }
            }
        }

        static void WriteItem(string Data, StreamWriter Writer, int IndentationDepth=0)
        {
            Writer.WriteLine(new string(' ', IndentationDepth*3) + Data);
        }

        public static object ReadObject(Type ObjectType, StreamReader Reader)
        {
            object ReturnObject = Activator.CreateInstance(ObjectType,null);
            while (!Reader.EndOfStream)
            {
                string Line = Reader.ReadLine().TrimStart(" ".ToCharArray()); string[] LineArr = Line.Split(new string[] { ":" }, StringSplitOptions.None);
                if (!Line.Contains("}"))
                {
                    FieldInfo thisField = ObjectType.GetField(LineArr[0]);
                    if (LineArr[1] == "NULL") { thisField.SetValue(ReturnObject, null); }
                    else if (LineArr[1].StartsWith("{")) { thisField.SetValue(ReturnObject,ReadObject(thisField.FieldType, Reader)); }
                    else  { thisField.SetValue(ReturnObject, Convert.ChangeType(LineArr[1], thisField.FieldType)); }
                }
                else { break; }
            }
            return ReturnObject;
        }
    }
}
