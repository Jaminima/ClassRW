using System;
using System.Collections;
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
                var FieldValue = Field.GetValue(SrcObject);
                if (FieldValue == null) { WriteItem(Field.Name + ":NULL", Writer, IndentationDepth); }
                else if (Field.FieldType.IsArray)
                { WriteSet(Field, (Array)FieldValue, Writer, IndentationDepth); }
                else if (Field.FieldType.IsGenericType&&Field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                { WriteSet(Field, IListToArray((IList)FieldValue), Writer, IndentationDepth); }
                else if (Field.FieldType.IsSerializable) { WriteItem(Field.Name + ":" + FieldValue, Writer, IndentationDepth); }
                else
                {
                    WriteItem(Field.Name + ":{", Writer, IndentationDepth);
                    WriteObject(FieldValue, Field.FieldType, Writer, IndentationDepth + 1);
                    WriteItem("}", Writer, IndentationDepth);
                }
            }
        }

        static void WriteSet(FieldInfo Field, Array FieldValue, StreamWriter Writer, int IndentationDepth = 0)
        {
            WriteItem(Field.Name + ":["+FieldValue.Length, Writer, IndentationDepth);
            foreach (object SubObject in FieldValue)
            {
                if (SubObject.GetType().IsSerializable) { WriteItem(SubObject.ToString(), Writer, IndentationDepth+1); }
                else
                {
                    WriteItem("{", Writer, IndentationDepth+1);
                    WriteObject(SubObject, SubObject.GetType(), Writer, IndentationDepth + 2);
                    WriteItem("}", Writer, IndentationDepth+1);
                }
            }
            WriteItem("]", Writer, IndentationDepth);
        }

        static Array IListToArray(IList List)
        {
            Array Arr=Array.CreateInstance(List[0].GetType(),List.Count);
            List.CopyTo(Arr, 0);
            return Arr;
        }

        static void WriteItem(string Data, StreamWriter Writer, int IndentationDepth=0)
        {
            Writer.WriteLine(new string(' ', IndentationDepth*2) + Data);
        }

        public static object ReadObject(Type ObjectType, StreamReader Reader)
        {
            object ReturnObject = Activator.CreateInstance(ObjectType,null);
            while (!Reader.EndOfStream)
            {
                string Line = Reader.ReadLine().TrimStart(" ".ToCharArray()); string[] LineArr = Line.Split(new string[] { ":" }, StringSplitOptions.None);
                if (!Line.Contains("}")&&!Line.Contains("]"))
                {
                    FieldInfo thisField = ObjectType.GetField(LineArr[0]);
                    if (thisField != null)
                    {
                        if (LineArr.Length == 2 && LineArr[1] == "NULL") { thisField.SetValue(ReturnObject, null); }
                        else if (LineArr[1].StartsWith("{")) { thisField.SetValue(ReturnObject, ReadObject(thisField.FieldType, Reader)); }
                        else if (LineArr[1].StartsWith("["))
                        {
                            var Set = Activator.CreateInstance(thisField.FieldType,int.Parse(LineArr[1].Replace("[","")));
                            for (int i = 0; i < int.Parse(LineArr[1].Trim('[')); i++) {
                                if (thisField.FieldType.IsArray) { ((Array)Set).SetValue(ReadObject(thisField.FieldType.GetElementType(), Reader),i); }
                                if (thisField.FieldType.IsGenericType && thisField.FieldType.GetGenericTypeDefinition() == typeof(List<>)) { ((IList)Set).Add(ReadObject(thisField.FieldType.GetGenericArguments()[0], Reader)); }
                            }
                            if (thisField.FieldType.IsArray) { thisField.SetValue(ReturnObject, Set); }
                            else { thisField.SetValue(ReturnObject, Set); }
                        }
                        else { thisField.SetValue(ReturnObject, Convert.ChangeType(LineArr[1], thisField.FieldType)); }
                    }
                }
                else { break; }
            }
            return ReturnObject;
        }
    }
}
