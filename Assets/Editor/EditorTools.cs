
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class EditorTools
{
    public static Object[] LoadAllAssetsAtFolder(string path)
    {
        if (!Directory.Exists(path))
        {
            throw new Exception(string.Format("path {0} not exit", path));
        }

        List<Object> list = new List<Object>();
        DirectoryInfo info = new DirectoryInfo(path);
        FileInfo[] infos = info.GetFiles();
        for (int i = 0; i < infos.Length; i++)
        {
            if (!infos[i].Name.EndsWith(".meta"))
            {
                Object obj = AssetDatabase.LoadMainAssetAtPath(SystemPath2UnityPath(infos[i].FullName));
                list.Add(obj);
            }
        }

        return list.ToArray();
    }

    public static string SystemPath2UnityPath(string path)
    {
        int index = path.IndexOf("Assets");
        return SubString2End(path, index);
    }

    /// <summary>
    /// 以Assets开始
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string UnityPath2SystemPath(string path)
    {
        return Application.dataPath + SubString2End(path, 6);
    }

    public static string SubString2End(string str, int index)
    {
        if (index < 0)
        {
            throw new Exception("SubString2End index 错误" + index);
        }

        if (index >= str.Length)
        {
            throw new Exception("SubString2End index 溢出错误" + index);
        }
        return str.Substring(index, str.Length - index);
    }

    public static int GetTextureStorageMemorySize(Texture texture)
    {
        Type t = typeof(Editor).Assembly.GetType("UnityEditor.TextureUtil");
        var method =  t.GetMethod("GetStorageMemorySize" , BindingFlags.Static | BindingFlags.Public|BindingFlags.Instance);
        int len = (int)method.Invoke(null  , new object[]{texture});
        return len;
    }

    public static string[] GetAllFileInFolder(string path , string findPattern = "")
    {
        List<string> list = new List<string>();
        GetAllFileInFolder(path, list , findPattern);
        return list.ToArray();
    }

    public static void GetAllFileInFolder(string path ,  List<string> pathList , string findPattern ="")
    {
        DirectoryInfo dir = new DirectoryInfo(path);
        FileSystemInfo[] infos = dir.GetFileSystemInfos(findPattern);
        for (int i = 0; i < infos.Length; i++)
        {
            if (infos[i] is DirectoryInfo)
            {
                GetAllFileInFolder(infos[i].FullName, pathList,  findPattern);
            }
            else if (infos[i] is FileInfo)
            {
                pathList.Add(infos[i].FullName);
            }
        }
    }

    public static void GetAllFileInFolder(string path , Func<string , bool> filter , ref List<string> pathList)
    {
        DirectoryInfo dir = new DirectoryInfo(path);
        FileSystemInfo[] infos =  dir.GetFileSystemInfos();
        for (int i = 0; i < infos.Length; i++)
        {
            if (infos[i] is DirectoryInfo)
            {
                GetAllFileInFolder(infos[i].FullName, filter, ref pathList);
            }
            else if(infos[i] is FileInfo)
            {
                if (filter != null)
                {
                    if (filter(infos[i].FullName))
                    {
                        pathList.Add(infos[i].FullName);
                    }
                }
                else
                {
                    pathList.Add(infos[i].FullName);
                }
            }
        }
    }
    public static void CopyField(object from, object to)
    {
        var type = from.GetType();
        FieldInfo[] fieldInfos =  type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );
        for (int i = 0; i < fieldInfos.Length; i++)
        {
            FieldInfo fieldInfo = fieldInfos[i];
            Debug.Log("try get FiledName " + fieldInfo.Name);
            FieldInfo targetFieldInfo = to.GetType().GetField(fieldInfo.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Debug.Log("FiledName " + targetFieldInfo.Name);
            if(targetFieldInfo != null && targetFieldInfo.FieldType == fieldInfo.FieldType)
            {
                Debug.Log("set " + fieldInfo.Name);
                targetFieldInfo.SetValue(to, fieldInfo.GetValue(from));
            }

        }
    }
}
