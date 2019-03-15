using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

public class LuaSerializer
{

    public static string Serialize(object val)
    {
        if (val == null)
        {
            return "nil";
        }

        Type type = val.GetType();
        if (type == typeof(float))
        {
            return val.ToString();
        }
        else if (type == typeof(string))
        {
            return val.ToString();
        }
        else if (type == typeof(int))
        {
            return val.ToString();
        }
        else if (type == typeof(bool))
        {
            return val.ToString().ToLower();
        }
        else if (type == typeof(double) || type == typeof(uint) || type == typeof(ulong) || type == typeof(ushort))
        {
//            return Convert.ToDouble(val).ToString();
            return val.ToString();
        }
        else if (type == typeof(string))
        {
            return String.Format("\"{0}\"", val);
        }
        else
        {

            return SerializeClassType(val);
        }
    }

    private static string SerializeClassType(object value)
    {
        if (value == null)
        {
            return "nil";
        }

        Type type = value.GetType();
        List<FieldInfo> fields = new List<FieldInfo>();
        var publicFields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
        var privateFields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
        foreach (var info in publicFields)
        {
            if (!info.HasAttribute<NonSerializedAttribute>())
            {
                fields.Add(info);
            }
        }

        foreach (var info in privateFields)
        {
            if (info.HasAttribute<SerializeField>())
            {
                fields.Add(info);
            }
        }
        List<string> tableFields = new List<string>();

        for (int i = 0; i < fields.Count; i++)
        {
            var info = fields[i];
            tableFields.Add(string.Format("{0} = {1}" , fields[i].Name , Serialize(fields[i].GetValue(value))));
        }

        if (value is IDictionary)
        {
            var dic = value as IDictionary;
            foreach (var key in dic.Keys)
            {
                if (key is int)
                {
                    tableFields.Add(string.Format("[{0}] = {1}" , key , Serialize(dic[key])));
                }
                else
                {
                    tableFields.Add(string.Format("{0} = {1}" , key , Serialize(dic[key])));
                }
            }
        }else if (value is IEnumerable)
        {
            var enumerable = value as IEnumerable;
            foreach (var enu in enumerable)
            {
                tableFields.Add(Serialize(enu));
            }
        }
        StringBuilder tableBuilder = new StringBuilder();
        tableBuilder.Append("{");
        for (int i = 0; i < tableFields.Count; i++)
        {
            var tableField = tableFields[i];
            tableBuilder.Append(tableField);
            if (i < tableFields.Count - 1)
            {
                tableBuilder.Append(", ");
            }
        }
        tableBuilder.Append("}");

        return tableBuilder.ToString();
    }
}
