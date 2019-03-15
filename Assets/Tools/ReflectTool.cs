using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class ReflectTool
{
    public static T Instantiate<T>(Type[] parameterTypes, object[] parameters) where T : class
    {
        Type type = typeof(T);
        ConstructorInfo constructorInfo = type.GetConstructor(parameterTypes);
        return constructorInfo.Invoke(parameters) as T;
    }

    public static object Instantiate(Type type)
    {
        try
        {
            ConstructorInfo constructorInfo = type.GetConstructor(Type.EmptyTypes);
            return constructorInfo.Invoke(null);
        }
        catch (Exception)
        {
            Debug.Log(type);
        }
        return null;
    }

    public static T Instantiate<T>() where T : class
    {
        Type type = typeof(T);
        ConstructorInfo constructorInfo = type.GetConstructor(Type.EmptyTypes);
        return constructorInfo.Invoke(null) as T;
    }

    public static object Instantiate(Type type, Type[] parameterTypes, object[] parameters)
    {
        ConstructorInfo constructorInfo = type.GetConstructor(parameterTypes);
        return constructorInfo.Invoke(parameters);
    }

    public static List<T> Instantiate<T>(List<Type> types) where T : class
    {
        List<T> list = new List<T>();
        foreach (var type in types)
        {
            list.Add(Instantiate(type) as T);
        }
        return list;
    }

    public static bool HasAttribute<T>(this FieldInfo info) where  T:class
    {
        var attributes = info.GetCustomAttributes( typeof(T) , true);
        return attributes.Length > 0;
    }

    public static T GetAttribute<T>(this FieldInfo info) where T : class
    {
        var attributes = info.GetCustomAttributes(typeof(T), true);
        return attributes.Length > 0?attributes[0] as T:null;
    }

    public static bool HasAttribute<T>(this Type type) where T : class
    {
        object[] customAttributes = type.GetCustomAttributes(typeof(T), true);
        return customAttributes.Length > 0;
    }

    public static T GetAttribute<T>(this MemberInfo field) where T : class
    {
        object[] customAttributes = field.GetCustomAttributes(typeof(T), true);
        return customAttributes.Length > 0 ? customAttributes[0] as T : null;
    }

    public static T GetAttribute<T>(this Type type) where T : class
    {
        object[] customAttributes = type.GetCustomAttributes(typeof(T), true);
        return customAttributes.Length > 0 ? customAttributes[0] as T : null;
    }

    public static bool HasAttribute(this FieldInfo field, Type attribute)
    {
        object[] customAttributes = field.GetCustomAttributes(attribute, true);
        return customAttributes.Length > 0;
    }

    public static Attribute GetAttribute(this FieldInfo field, Type attribute)
    {
        object[] customAttributes = field.GetCustomAttributes(attribute, true);
        return customAttributes.Length > 0 ? customAttributes[0] as Attribute : null;
    }

    public static bool HasAttribute(this Type type, Type attribute)
    {
        object[] customAttributes = type.GetCustomAttributes(attribute, true);
        return customAttributes.Length > 0;
    }

    public static Attribute GetAttribute(this Type type, Type attribute)
    {
        object[] customAttributes = type.GetCustomAttributes(attribute, true);
        return customAttributes.Length > 0 ? customAttributes[0] as Attribute : null;
    }

}
