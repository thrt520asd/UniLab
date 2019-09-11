using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Profiling;

public class TextureReviewWindow : EditorWindow {
    public GUIStyle background ;
    public GUIStyle header  ;
    private SplitterState m_Splitter;
    private static List<TextureAndImporter> texInfoList;
    private List<string> pathList;
    private Vector2 m_ScrollPosition;
    private TextureImporterType type = TextureImporterType.Default;
    private float height = 20;
    private static List<TextureAndImporter> showList;
    private long totalMemSize;
    public TextureReviewWindow()
    {
        this.SetupSplitter();
    }

    void OnEnable()
    {
        this.titleContent = new GUIContent("图片浏览器");
        
    }

    protected virtual void SetupSplitter()
    {
        float[] array = new float[1];
        int[] array2 = new int[1];
        array[0] = 300f;
        array2[0] = 100;
        this.m_Splitter = new SplitterState(array, array2, null);
        
    }

    void OnGUI()
    {
        header = "OL title";
        background = "OL Box";
        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("采集", GUILayout.Height(height)))
            {
                Collect();
            }

            if (GUILayout.Button("清理", GUILayout.Height(height)))
            {
                Clear();
            }

        }
        GUILayout.EndHorizontal();
        GUILayout.BeginVertical();
        {
            
            DrawHeader();
            DrawTexture();
        }
        GUILayout.EndVertical();
    }

    void DrawHeader()
    {
        
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("Name" , header , GUILayout.Height(height));
            GUILayout.Label("Size", header, GUILayout.Height(height));
            GUILayout.Label("Format", header, GUILayout.Height(height));
            GUILayout.Label(string.Format("总大小为{0}", EditorUtility.FormatBytes(totalMemSize)));
            EditorGUI.BeginChangeCheck();
            type = (TextureImporterType)EditorGUILayout.EnumPopup("类型", type , GUILayout.Height(height));
            if (EditorGUI.EndChangeCheck())
            {

                FilterTexture();
            }
        }
        GUILayout.EndHorizontal();
    }

    void DrawTexture( )
    {
        if (showList == null)
        {
            GUILayout.Label("请先采集数据");
            return;
        }
        m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition, background);
        int first, last;

        GetViewIndex(position.height - 40 , out first , out last);
        for (int i = 0; i < showList.Count; i++)
        {
            var info = showList[i];
            if (i >= first && i <= last)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(info.name, GUILayout.Width(200), GUILayout.Height(height));
                GUILayout.Label(string.Format("{0}X{1}", info.width, info.height), GUILayout.Width(100), GUILayout.Height(height));
                GUILayout.Label(info.type.ToString(), GUILayout.Height(height));
                GUILayout.Label(info.format.ToString(), GUILayout.Height(height));
                GUILayout.Label(EditorUtility.FormatBytes(showList[i].memSize), GUILayout.Height(height));
                GUILayout.Label(showList[i].warn);
                if (GUILayout.Button("Select", GUILayout.Width(60), GUILayout.Height(height)))
                {
                    SelectTexture(info.path);
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Space(height);
            }
            
        }
        GUILayout.EndScrollView();
    }

    private void GetViewIndex(float allHeight , out int first , out int last)
    {
        float y = m_ScrollPosition.y;
        first = (int) Mathf.Floor(y / height);
        last = first + Mathf.CeilToInt(allHeight / height);
    }

    private void Collect()
    {
        texInfoList = new List<TextureAndImporter>();
        string[] allPaths =  AssetDatabase.GetAllAssetPaths();
        for (int i = 0; i < allPaths.Length; i++)
        {
            
            string path = allPaths[i];
            if (path.StartsWith("Assets/Art") ||
                path.StartsWith("Assets/RawResources") ||
                path.StartsWith("Assets/UIEditor"))
            {
                EditorUtility.DisplayProgressBar("检测资源中", path, (float)i / (float)allPaths.Length);
                if (CheckExt(path))
                {
                    Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(path);
                    if (texture != null)
                    {
                        var importer = TextureImporter.GetAtPath(path) as TextureImporter;
                        
                        TextureImporterPlatformSettings settings = importer.GetPlatformTextureSettings("Android");
                        texInfoList.Add(new TextureAndImporter()
                        {
                            memSize = EditorTools.GetTextureStorageMemorySize(texture),
                            height = texture.height,
                            width = texture.width,
                            path = path,
                            format = settings.format,
                            name = texture.name,
                            type = importer.textureType,
                            warn =  getWarn(importer),
                        });
                    }
                }
            }
            
        }
        
        texInfoList.Sort((l, r) => { return (int)(r.memSize - l.memSize); });
        FilterTexture();
        EditorUtility.ClearProgressBar();

    }

    private string getWarn(TextureImporter importer)
    {

        var method = importer.GetType().GetMethod("GetImportWarnings", BindingFlags.Instance | BindingFlags.NonPublic);
        object obj = method.Invoke(importer, new object[]{});
        if (obj == null)
        {
            return "";
        }
        else
        {
            return obj.ToString();
        }

    }

    private void FilterTexture()
    {
        totalMemSize = 0;
        showList = new List<TextureAndImporter>();
        for (int i = 0; i < texInfoList.Count; i++)
        {
            if (texInfoList[i].type == type)
            {
                showList.Add(texInfoList[i]);
                totalMemSize += texInfoList[i].memSize;
            }
        }
    }

    private void SelectTexture(string path)
    {
        EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath(path));
    }

    private bool CheckExt(string path)
    {
        string ext = Path.GetExtension(path);
        ext = ext.ToLower();
        return (ext.Equals(".jpg") || ext.Equals(".png") || ext.Equals(".psd") || ext.Equals(".exr"));
    }

    private void Clear()
    {
        pathList = null;
        texInfoList = null;
        showList = null;
    }


    [MenuItem("Tools/TextureReviewWindow")]
    public static void Open()
    {
        EditorWindow.GetWindow<TextureReviewWindow>();
    }

    public class TextureAndImporter
    {
        public long memSize;
        public TextureImporterType type;
        public string path;
        public TextureImporterFormat format;
        public int width;
        public int height;
        public string name;
        public string warn;
    }
    
    public class  TextureDetailDrawer
    {
        public string path;
        public string[] dependency;
        public Texture texture;
        private Vector2 m_scrollPos;
        public void Refresh(string newPath)
        {
            
            var newtexture = AssetDatabase.LoadAssetAtPath<Texture>(newPath);
            if (newtexture != null)
            {
                path = newPath;
                texture = newtexture;
                dependency = AssetDatabase.GetDependencies(path);
                Debug.Log(dependency.Length);
            }
        }

        public void Draw()
        {
            if (texture == null)
            {
                GUILayout.Label("无选中图片");
            }
            else
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Label(texture, GUILayout.Width(256), GUILayout.Height(256));
                }
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
                {
                    m_scrollPos = GUILayout.BeginScrollView(m_scrollPos);
                    for (int i = 0; i < dependency.Length; i++)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label(dependency[i]);

                            if (GUILayout.Button("选中"))
                            {
                                Selection.SetActiveObjectWithContext(texture, null);
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndScrollView();
                }
                GUILayout.EndVertical();
                
            }
        }
    }
}
