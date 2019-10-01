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

                if (FObj == null) { WriteLine(Field.Name + ":NULL", Writer, Indentation); }

                else if (OType == ObjectType.Serial) {
                    WriteLine(Field.Name, FObj, Writer, Indentation);
                }

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

                    string DimensionSet = "";
                    for (int i = 0; i < Set.Rank; i++) { DimensionSet += Set.GetLength(i) + ","; } DimensionSet = DimensionSet.TrimEnd(',');
                    WriteLine(Field.Name + ":[" + DimensionSet, Writer, Indentation);
                    WriteArray(Set, Writer, Indentation);
                    WriteLine("]", Writer, Indentation);
                }
            }
        }

        static void WriteArray(Array Set, StreamWriter Writer, int Indentation = 0, int Dimension = 0, int[] Path = null)
        {
            bool IsDeepest = Set.Rank-1==Dimension;
            if (Path == null) { Path = new int[Set.Rank]; }
            Indentation++;
            for (int i = 0; i < Set.GetLength(Dimension); i++)
            {
                if (IsDeepest)
                {
                    object Item = Set.GetValue(Path); ObjectType ItemType = Master.GetObjectType(Item.GetType());
                    if (ItemType == ObjectType.Serial) { WriteLine(Item.ToString(), Writer, Indentation); }
                    else
                    {
                        WriteLine("{", Writer, Indentation);
                        WriteObject(Item, Writer, Indentation);
                        WriteLine("}", Writer, Indentation);
                    }
                }
                else
                {
                    //WriteLine("[", Writer, Indentation);
                    WriteArray(Set, Writer, Indentation, Dimension + 1, Path);
                    //WriteLine("]", Writer, Indentation);
                }
                Path[Dimension]++;
            }
            Path[Dimension] = 0;
        }

        public static void WriteLine(string Line, StreamWriter Writer, int Indentation = 0)
        {
            Writer.WriteLine(new string(' ', Indentation * 2) + Line);
        }

        public static void WriteLine(string FieldName, object FieldValue, StreamWriter Writer, int Indentation = 0)
        {
            if (FieldValue.GetType() == typeof(string)) { FieldValue = "\"" + HttpUtility.UrlEncode(FieldValue.ToString()) + "\""; }
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
