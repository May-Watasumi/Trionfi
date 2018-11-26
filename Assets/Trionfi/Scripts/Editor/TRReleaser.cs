using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class TRManifestInfo
{
    [SerializeField]
    public string bundleName;
    [SerializeField]
    public Hash128 hash;
    [SerializeField]
    public uint crc;
}

public class TRReleaser : EditorWindow
{
    string _projectRoot = "Assets/Trionfi/Example/Resources";
    string _outputPath = "Assets/StreamingAssets/";

    bool enableAdvanced = false;

    bool target_iOS = false;
    bool target_Android = false;
    bool target_Standalone = true;
    bool target_WebGL = false;

    [System.Serializable]
    class TRAssetBundleInfo
    {
        [SerializeField]
        public int ID;
        [SerializeField]
        public string sourcePath;
        [SerializeField]
        public string bundleName;
    }

    List<TRAssetBundleInfo> bundleList = new List<TRAssetBundleInfo>();
    List<bool> bundleActivity = new List<bool>();

    [MenuItem("Tools/Trionfi/Releaser")]
    private static void BuildAssetBundle()
    {
        GetWindow<TRReleaser>("Releaser");
    }

    void OnGUI()
    {
        GUILayout.Label("Target Platform");
        GUILayout.Space(2.5f);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("SelectAll", GUILayout.Height(30.0f)))
        {
            target_iOS = true;
            target_Android = true;
            //target_Standalone = true;
            target_WebGL = true;
        }
        if (GUILayout.Button("Build", GUILayout.Height(30.0f)))
        {
            if(enableAdvanced)
                BuildAllAsset();
            else
                BuildSimple();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(2.5f);

        GUILayout.BeginHorizontal();
        target_iOS = GUILayout.Toggle(target_iOS, "iOS");
        target_Android = GUILayout.Toggle(target_Android, "Android");
        target_Standalone = GUILayout.Toggle(target_Standalone, "PC");
        target_WebGL = GUILayout.Toggle(target_WebGL, "WebGL");
        GUILayout.EndHorizontal();

        GUILayout.Space(5.0f);

        GUILayout.Label("Output Foloder");
        _outputPath = GUILayout.TextField(_outputPath);
        GUILayout.BeginHorizontal();
            if (GUILayout.Button("SelectFolder", GUILayout.Height(30.0f)))
            {
                _outputPath = EditorUtility.OpenFolderPanel("出力フォルダ", "Assets", "Template");
            }
        GUILayout.EndHorizontal();

        GUILayout.Space(7.5f);
        enableAdvanced = GUILayout.Toggle(enableAdvanced, "Advanced");
        GUILayout.Space(12.5f);

        if (enableAdvanced)
        {
            GUILayout.Label("【Advanced Mode】");
            GUILayout.Space(5.0f);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save", GUILayout.Height(25.0f)))
            {
                string _File  = EditorUtility.SaveFilePanel("リスト保存", _outputPath, "List", "json");
                if (!string.IsNullOrEmpty(_File))
                {
                    string _json = JsonUtility.ToJson(bundleList);
                    File.WriteAllText(_File, _json);
                }
            }

            if (GUILayout.Button("Load", GUILayout.Height(25.0f)))
            {
                string _File = EditorUtility.OpenFilePanel("リスト読み込み", "Lists", "json");
                if (!string.IsNullOrEmpty(_File))
                {
                    StreamReader _stream = File.OpenText(_File);
                    if (_stream != null)
                    {
                        string _json = _stream.ReadToEnd();
                        bundleList = JsonUtility.FromJson<List<TRAssetBundleInfo>>(_json);
                    }
                }
            }

            if (GUILayout.Button("Clear", GUILayout.Height(25.0f)))
            {
                bundleList.Clear();
                bundleActivity.Clear();
            }

            GUILayout.EndHorizontal();

            var evt = Event.current;

            var dropArea = GUILayoutUtility.GetRect(0.0f, 100.0f, GUILayout.ExpandWidth(true));
            GUI.Box(dropArea, "Drag & Drop");
            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition)) break;

                    //マウスの形状
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (string _path in DragAndDrop.paths)
                        {
                            //                            _targetFiles.Add(_path);
                            //ファイルの名前を取得し、AssetBundleの名前に設定する
                            //string fileName = Path.GetFileNameWithoutExtension(srcFilePath);

                            TRAssetBundleInfo _info = new TRAssetBundleInfo();
                            //                            _info.ID = advanvedCountID;
                            _info.sourcePath = _path;
                            _info.bundleName = Path.GetFileNameWithoutExtension(_info.sourcePath);

                            bundleList.Add(_info);
                            bundleActivity.Add(true);
                        }
                        DragAndDrop.activeControlID = 0;
                    }
                    Event.current.Use();
                    break;
            }

            for(int a = 0; a < bundleActivity.Count;a++)
            {
                if (bundleActivity[a])
                {
                    GUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("", bundleList[a].sourcePath);
                    EditorGUILayout.TextField("", bundleList[a].bundleName);

                    if (GUILayout.Button("Delete", GUILayout.Height(20.0f)))
                    {
                        bundleActivity[a] = false;
                    }

                    GUILayout.EndHorizontal();
                }
            }
        }
        else
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("【Simple Mode(Archive 1 file)】");
            if (GUILayout.Button("Select Folder", GUILayout.Height(30.0f)))
            {
                _projectRoot = EditorUtility.OpenFolderPanel("リソースフォルダ", "Assets", "Template");
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("Resources Foloder");
            _projectRoot = GUILayout.TextField(_projectRoot);
        }
    }

    void SaveInfo(string path, AssetBundleManifest manifest)
    {
        
    }

    void BuildSimpleSub(string path, BuildTarget target)
    {
        TRManifestInfo _info = new TRManifestInfo();
        string _bundleFullpath;

        // AssetBundle一つを作成する場合
        AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
        buildMap[0].assetBundleName = Path.GetFileName(_projectRoot);
        buildMap[0].assetNames = new string[1] { _projectRoot };

        string _temp = Path.Combine(_outputPath, path);

        if (!Directory.Exists(_temp))
            Directory.CreateDirectory(_temp);

        AssetBundleManifest _manifest = BuildPipeline.BuildAssetBundles(_temp, buildMap, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows);

        _info.bundleName = buildMap[0].assetBundleName;
        _bundleFullpath = Path.Combine(_temp, buildMap[0].assetBundleName);

        if (BuildPipeline.GetCRCForAssetBundle(_bundleFullpath, out _info.crc) &&
            BuildPipeline.GetHashForAssetBundle(_bundleFullpath, out _info.hash))
        {
            string _json = JsonUtility.ToJson(_info);
        }
    }

    void BuildSimple()
    {
        if (target_Standalone)
        {
#if UNITY_EDITOR_WIN
            BuildSimpleSub("Windows", BuildTarget.StandaloneWindows64);
#elif UNITY_EDITOR_OSX
        BuildSimpleSub("MacOS", BuildTarget.StandaloneOSX);
#endif
        }
        if (target_iOS)
        {
            BuildSimpleSub("iOS", BuildTarget.iOS);
        }
        if (target_Android)
        {
            BuildSimpleSub("Android", BuildTarget.Android);
        }
        if (target_WebGL)
        {
            BuildSimpleSub("WebGL", BuildTarget.WebGL);
        }
    }

    void BuildAllAsset()
    {
    }

    void BuildAssetBundles(string rootPath)
    {
        //sourceディレクトリの全てのファイル・フォルダを取得(サブフォルダの内部までは見ない)
        //sourceフォルダに入れたものは全てAssetBundle化対象
        string[] srcFilesPath = Directory.GetFileSystemEntries(rootPath);

        foreach (string srcFilePath in srcFilesPath)
        {
            //ファイルの名前を取得し、AssetBundleの名前に設定する
            string fileName = Path.GetFileNameWithoutExtension(srcFilePath);

            TRAssetBundleInfo _info = new TRAssetBundleInfo();
//            _info.ID = advanvedCountID;
            _info.sourcePath = srcFilePath;
            _info.bundleName = _outputPath;
            //            bundleDictionary[advanvedCountID] = _info;
//            advanvedCountID++;
        }

        if (!Directory.Exists(_outputPath + "Windows"))
            Directory.CreateDirectory(_outputPath + "Standalone");
        BuildPipeline.BuildAssetBundles(_outputPath + "Standalone", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows);

        if (!Directory.Exists(_outputPath + "MacOS"))
            Directory.CreateDirectory(_outputPath + "MacOS");
#if UNITY_2017_2_OR_NEWER
        BuildPipeline.BuildAssetBundles(_outputPath + "MacOS", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneOSX);
#else
        BuildPipeline.BuildAssetBundles(_outputPath + "MacOS", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneOSXUniversal);
#endif
        if (target_iOS)
        {
            if (!Directory.Exists(_outputPath + "iOS"))
                Directory.CreateDirectory(_outputPath + "iOS");
            BuildPipeline.BuildAssetBundles(_outputPath + "iOS", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.iOS);
        }

        if (target_Android)
        {
            if (!Directory.Exists(_outputPath + "Android"))
                Directory.CreateDirectory(_outputPath + "Android");
            BuildPipeline.BuildAssetBundles(_outputPath + "Android", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);
        }

        if (target_WebGL)
        {
            if (!Directory.Exists(_outputPath + "WebGL"))
                Directory.CreateDirectory(_outputPath + "WebGL");
            BuildPipeline.BuildAssetBundles(_outputPath + "WebGL", BuildAssetBundleOptions.None, BuildTarget.WebGL);
        }

        //sourceディレクトリのAssetBundle対象化を解除
        foreach (string srcPath in srcFilesPath)
        {
            AssetImporter importer = AssetImporter.GetAtPath(srcPath);
            if (importer != null)
            {
                importer.SetAssetBundleNameAndVariant("", "");
            }
        }
    }
}