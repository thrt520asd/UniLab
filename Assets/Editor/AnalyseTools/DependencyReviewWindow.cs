using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class DependencyReviewWindow :EditorWindow
{
    [MenuItem("Tools/DependencyReviewWindow")]
    public static void Open()
    {
        EditorWindow.GetWindow<DependencyReviewWindow>();
    }

    [MenuItem("Assets/查看依赖 &d")]
    public static void ReivewDep()
    {
        DependencyReviewWindow window = EditorWindow.GetWindow<DependencyReviewWindow>();
        if (Selection.activeObject == null)
        {
            return;
        }
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        window.ShowDeps(path);
    }


    private string savePath;
    private bool autoCache = true;
    void OnEnable()
    {
        savePath = Application.dataPath + "/../dep.json";
        this.titleContent = new GUIContent("依赖分析器");
    }

    private static Dictionary<string, DepencyInfo> infoDic;
//    private DepencyInfo showInfo;
//    private DepencyInfo lastInfo;
    private Stack<DepencyInfo> showInfoStack = new Stack<DepencyInfo>();
    void OnGUI()
    {
        if (GUILayout.Button("删除缓存" , GUILayout.Width(100)))
        {
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
                infoDic = null;
                this.ShowNotification(new GUIContent("删除缓存成功"));
            }
            else
            {
                this.ShowNotification(new GUIContent("缓存不存在"));
            }
        }

        if (GUILayout.Button("返回" , GUILayout.Width(100)))
        {
            showInfoStack.Pop();
        }
        autoCache = GUILayout.Toggle(autoCache, "自动保存缓存");
        
        ShowInfo();
        
    }

    private void ShowInfo()
    {
        if (showInfoStack.Count > 0)
        {
            var showInfo = showInfoStack.Peek();
            Object obj = AssetDatabase.LoadMainAssetAtPath(showInfo.path);
            if (obj == null) return;
            GUILayout.Label("名字 ： " + obj.name);
            GUILayout.Label("被引用：");
            DrawDepInfo(showInfo.dependcies);
            GUILayout.Label("引用：");
            DrawDepInfo(showInfo.refs);
        }
    }

    private void DrawDepInfo(List<DepencyInfo> infoList)
    {
        for (int i = 0; i < infoList.Count; i++)
        {
            DepencyInfo dep = infoList[i];
            Object depObj = AssetDatabase.LoadMainAssetAtPath(dep.path);
            if (depObj == null)
            {
                continue;
            }
            GUILayout.BeginHorizontal();
            {

                GUILayout.Label(depObj.name);
                GUILayout.Label(dep.path);
                if (GUILayout.Button("选择", GUILayout.Width(70)))
                {
                    EditorGUIUtility.PingObject(depObj);
                }
                if (GUILayout.Button("查看依赖", GUILayout.Width(70)))
                {
                    showInfoStack.Push(dep);
                }

            }
            GUILayout.EndHorizontal();
        }
    }

    public void ShowDeps(string path)
    {
        if (infoDic == null)
        {
            Analyse();
        }

        DepencyInfo info;
        if (infoDic.TryGetValue(path, out info))
        {
            showInfoStack.Push(info);
        }
        else
        {
            Debug.LogError("找不到依赖信息 " + path);
            this.ShowNotification(new GUIContent("找不到依赖信息 " + path + "\n 清除缓存试试"));
        }
    }


    void Analyse()
    {
        if (File.Exists(savePath))
        {
            infoDic = LitJson.JsonMapper.ToObject<Dictionary<string, DepencyInfo>>(File.ReadAllText(savePath));
            DepHandler();
        }
        else
        {
            infoDic = new Dictionary<string, DepencyInfo>();
            string[] path = AssetDatabase.GetAllAssetPaths();
            for (int i = 0; i < path.Length; i++)
            {

                infoDic[path[i]] = new DepencyInfo() { path = path[i] };
                EditorUtility.DisplayProgressBar("采集Asset信息", path[i], (float)i / (float)path.Length);
            }


            int count = infoDic.Count;
            int n = 0;
            foreach (var info in infoDic.Values)
            {
                string[] deps = AssetDatabase.GetDependencies(info.path);
                for (int i = 0; i < deps.Length; i++)
                {
                    DepencyInfo depInfo;
                    if (infoDic.TryGetValue(deps[i], out depInfo))
                    {
                        depInfo.depList.Add(info.path);
                        info.refList.Add(depInfo.path);
                    }
                    else
                    {
                        Debug.Log("no dep info " + deps[i]);
                    }
                }

                n++;
                EditorUtility.DisplayProgressBar("收集依赖信息", info.path, (float)n / (float)count);
            }
            EditorUtility.ClearProgressBar();
            if (autoCache)
            {
                string json = LitJson.JsonMapper.ToJson(infoDic);
                File.WriteAllText(savePath, json);
            }
            DepHandler();
        }
        
    }

    private void DepHandler()
    {
        foreach (var info in infoDic.Values)
        {
            for (int i = 0; i < info.depList.Count; i++)
            {
                string path = info.depList[i];
                DepencyInfo depInfo;
                if (infoDic.TryGetValue(path, out depInfo))
                {
                    info.dependcies.Add(depInfo);
                }
            }
            for (int i = 0; i < info.refList.Count; i++)
            {
                string path = info.refList[i];
                DepencyInfo depInfo;
                if (infoDic.TryGetValue(path, out depInfo))
                {
                    info.refs.Add(depInfo);
                }
            }
        }
    }

    
    [Serializable]
    class DepencyInfo
    {
        public string path;
        [NonSerialized]
        public List<DepencyInfo> dependcies = new List<DepencyInfo>();

        public List<string> depList = new List<string>();

        [NonSerialized]
        public List<DepencyInfo> refs = new List<DepencyInfo>();

        public List<string> refList = new List<string>();
    }
}
