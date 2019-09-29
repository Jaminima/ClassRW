using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Web;

namespace ClassRW
{
    public static class ClassReader
    {
        public static object ReadObject(Type ObjType,StreamReader Reader)
        {
            Object Obj = Activator.CreateInstance(ObjType,null);
            while (!Reader.EndOfStream)
            {
                string Line = Reader.ReadLine().TrimStart(' '); string[] LineArr = Line.Split(':');
                if (LineArr.Length == 2)
                {
                    FieldInfo LineField = ObjType.GetField(LineArr[0]); Type LineType = LineField.FieldType;
                    ObjectType LineObjType = Master.GetObjectType(LineType);

                    if (Line.EndsWith("NULL")) { LineField.SetValue(Obj, null); }

                    else if (LineObjType == ObjectType.Serial) { 
                        if (LineType == typeof(string)) { LineArr[1] = HttpUtility.UrlDecode(LineArr[1].Trim('\"')); }
                        LineField.SetValue(Obj, Convert.ChangeType(LineArr[1],LineType)); 
                    }

                    else if (LineObjType == ObjectType.Composite)
                    {
                        LineField.SetValue(Obj, ReadObject(LineType, Reader));
                    }

                    else if (LineObjType == ObjectType.Array || LineObjType == ObjectType.List)
                    {
                        Array Set; int Length = int.Parse(LineArr[1].TrimStart('[')); Type ArrayType; ObjectType ArrayObjType;
                        if (LineObjType == ObjectType.Array) { ArrayType = LineType.GetElementType(); }
                        else { ArrayType = LineType.GetGenericArguments()[0]; }
                        ArrayObjType = Master.GetObjectType(ArrayType);

                        Set = Array.CreateInstance(ArrayType, Length);
                        for (int i = 0; i < Length; i++) {
                            if (ArrayObjType == ObjectType.Serial) { Set.SetValue(Convert.ChangeType(Reader.ReadLine(), ArrayType), i); }
                            else { Set.SetValue(ReadObject(ArrayType, Reader), i); }
                        }

                        if (LineObjType == ObjectType.List) {
                            var LSet = Activator.CreateInstance(LineType, new object[] { Set.Length });
                            foreach (object Item in Set) { ((IList)LSet).Add(Item); }
                            LineField.SetValue(Obj, LSet);
                        }
                        else { LineField.SetValue(Obj, Set); }
                    }
                }
                else if (Line.EndsWith("}")) { return Obj; }
            }
            return Obj;
        }
    }
}
