using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DevTools {

    [MenuItem("#DevTools#/打开StreamingAssets目录")]
    public static void OpenStreamingPath()
    {
        var path = Application.streamingAssetsPath;
        System.Diagnostics.Process.Start(path);
    }
    [MenuItem("#DevTools#/打开Persistent目录")]
    public static void OpenPersistentPath()
    {
        var path = Application.persistentDataPath;
        System.Diagnostics.Process.Start(path);
    }
}
