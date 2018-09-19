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


    int advanvedCountID = 0;

    class TRAssetBundleInfo
    {
        public int ID;
        public string sourcePath;
        public string destPath;
    }


    Dictionary<int, TRAssetBundleInfo> bundleDictionary = new Dictionary<int, TRAssetBundleInfo>();

    List<string> _targetFiles = new List<string>();

    [MenuItem("Tools/Trionfi/Releaser")]
    private static void BuildAssetBundle()
    {
        GetWindow<TRReleaser>("Releaser");
    }

    void OnGUI()
    {
        GUILayout.Label("Target Platform");
        GUILayout.Space(2.5f);

        if (GUILayout.Button("SelectAll", GUILayout.Height(30f)))
        {
            target_iOS = true;
            target_Android = true;
            //target_Standalone = true;
            target_WebGL = true;
        }

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
        GUILayout.Space(5.0f);

        if (enableAdvanced)
        {
            GUILayout.Label("【Advanced Mode】");
            GUILayout.Space(5.0f);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Build", GUILayout.Height(25.0f)))
            {
                BuildAllAsset();
            }
            if (GUILayout.Button("Clear", GUILayout.Height(25.0f)))
            {
                _targetFiles.Clear();
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
                            _info.ID = advanvedCountID;
                            _info.sourcePath = _path;
                            _info.destPath = _outputPath;
                            bundleDictionary[advanvedCountID] = _info;

                            advanvedCountID++;

                        }
                        DragAndDrop.activeControlID = 0;
                    }
                    Event.current.Use();
                    break;
            }

            foreach (KeyValuePair<int, TRAssetBundleInfo> _infoKey in bundleDictionary)
            {
                GUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("", _infoKey.Value.sourcePath);
                EditorGUILayout.TextField("", _infoKey.Value.destPath);
                if (GUILayout.Button("Select Folder", GUILayout.Height(20.0f)))
                {
                    _infoKey.Value.sourcePath = EditorUtility.OpenFolderPanel("出力先フォルダ", "Assets", "Template");
                }
                if (GUILayout.Button("Delete", GUILayout.Height(20.0f)))
                {
                    bundleDictionary.Remove(_infoKey.Key);
                }

                GUILayout.EndHorizontal();
            }
        }
        else
        {
            GUILayout.Label("【Simple Mode(Archive 1 file)】");
            GUILayout.Label("Resources Foloder");
            _projectRoot = GUILayout.TextField(_projectRoot);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Select Folder", GUILayout.Height(30.0f)))
            {
                _projectRoot = EditorUtility.OpenFolderPanel("リソースフォルダ", "Assets", "Template");
            }

            if (GUILayout.Button("Build", GUILayout.Height(30f)))
            {
                BuildSimple();
            }

            GUILayout.EndHorizontal();
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
            _info.ID = advanvedCountID;
            _info.sourcePath = srcFilePath;
            _info.destPath = _outputPath;
            bundleDictionary[advanvedCountID] = _info;

            advanvedCountID++;
        }

        if (!Directory.Exists(_outputPath + "Windows"))
            Directory.CreateDirectory(_outputPath + "Standalone");
        BuildPipeline.BuildAssetBundles(_outputPath + "Standalone", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows);

        if (!Directory.Exists(_outputPath + "MacOS"))
            Directory.CreateDirectory(_outputPath + "MacOS");
        BuildPipeline.BuildAssetBundles(_outputPath + "MacOS", BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneOSX);

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