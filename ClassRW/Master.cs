using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassRW
{
    public static class Master
    {
        public static ObjectType GetObjectType(Type T)
        {
            if ((T.IsGenericType && T.GetGenericTypeDefinition() == typeof(List<>))) { return ObjectType.List; }
            else if (T.IsArray) { return ObjectType.Array; }
            else if (T.IsSerializable) { return ObjectType.Serial; }
            else { return ObjectType.Composite; }
        }
    }

    public enum ObjectType
    {
        Serial,
        Array,
        List,
        Composite
    }
}
