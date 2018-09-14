using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class TRAssetBuilder : EditorWindow
{
    string _projectRoot = "Asssets/Trionfi/Template/Example/Resources";
    string _outputPath = "Assets/AssetBundles/";

    bool target_iOS = false;
    bool target_Android = false;
//    bool target_Standalone = false;
    bool target_WebGL = true;

    string _bgPath = "bg";
    string _standPath = "fgimage";
    string _uiImagePath = "image";
    string _bgmPath = "bgm";
    string _ruleImagePath = "rule";
    string _voicePath = "voice";
    string _sePath = "sound";
    string _otherPath = "other";

    string[] _targetFiles;

    [MenuItem("Tools/Trionfi/Build AssetBundle")]
    private static void BuildAssetBundle()
    {
        GetWindow<TRAssetBuilder>("AssetBundle Builder");
    }

    void OnGUI()
    {
        GUILayout.Label("Trionfi Resources Path");

        _projectRoot = GUILayout.TextField(_projectRoot);
        GUILayout.Space(10f);

        GUILayout.Label("Resource Path(Advanced)");
        GUILayout.Space(5.0f);

        GUILayout.Label("BG");
        _bgPath = GUILayout.TextField(_bgPath);
        GUILayout.Label("FIGURE");
        _standPath = GUILayout.TextField(_standPath);
        GUILayout.Label("UI");
        _uiImagePath = GUILayout.TextField(_uiImagePath);
        GUILayout.Label("BGM");
        _bgmPath = GUILayout.TextField(_bgmPath);
        GUILayout.Label("RULE");
        _ruleImagePath = GUILayout.TextField(_ruleImagePath);
        GUILayout.Label("VOICE");
        _voicePath = GUILayout.TextField(_voicePath);
        GUILayout.Label("SE");
        _sePath = GUILayout.TextField(_sePath);
        GUILayout.Label("OTHER");
        _otherPath = GUILayout.TextField(_otherPath);

        GUILayout.BeginHorizontal();
        target_iOS = GUILayout.Toggle(target_iOS, "iOS");
        target_Android = GUILayout.Toggle(target_Android, "Android");
        //        target_Standalone = GUILayout.Toggle(target_iOS, "PC");
        target_WebGL = GUILayout.Toggle(target_WebGL, "WebGL");
        GUILayout.EndHorizontal();
        GUILayout.Space(5.0f);
        if (GUILayout.Button("SelectAll", GUILayout.Height(30f)))
        {
            target_iOS = true;
            target_Android = true;
            //            target_Standalone = true;
            target_WebGL = true;
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("SelectFoloder", GUILayout.Height(30f)))
        {
            _projectRoot = EditorUtility.OpenFolderPanel("対象プロジェクトフォルダ", "Assets", "Template");
        }

        if (GUILayout.Button("Build", GUILayout.Height(30f)))
        {
            BuildAllAsset();
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(10.0f);

        /*
                #region Horizontal
                GUILayout.BeginHorizontal(GUILayout.Width(this.position.width - 20));
                btnCntent.text = "新しく開き直す";
                btnCntent.tooltip = "このウインドウを開きなおす";


                if (GUILayout.Button(btnCntent, GUI.skin.button, GUILayout.Height(20)))
                {
                    this.Close();
                    Init();
                }

                GUILayout.EndHorizontal();
                #endregion Horizontal
          */

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

                    _targetFiles = DragAndDrop.paths;

                    foreach (string path in DragAndDrop.paths)
                    {
                        //GetFullPath(path);
                    }

                    foreach (var draggedObject in DragAndDrop.objectReferences)
                    {
                        Debug.Log(draggedObject);
                    }
                    DragAndDrop.activeControlID = 0;
                }
                Event.current.Use();
                break;
        }

        foreach (string path in _targetFiles)
        {
            EditorGUILayout.TextField("DragAndDrop.path", path);
//            EditorGUILayout.TextField("FullPath", GetFullPath(path));
            EditorGUILayout.Space();
        }
    }

    void BuildAllAsset()
    {
        BuildAssetBundles(_projectRoot + "/" + _bgPath);
        BuildAssetBundles(_projectRoot + "/" + _standPath);
        BuildAssetBundles(_projectRoot + "/" + _uiImagePath);
        BuildAssetBundles(_projectRoot + "/" + _bgmPath);
        BuildAssetBundles(_projectRoot + "/" + _ruleImagePath);
        BuildAssetBundles(_projectRoot + "/" + _voicePath);
        BuildAssetBundles(_projectRoot + "/" + _sePath);
        BuildAssetBundles(_projectRoot + "/" + _otherPath);
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
            AssetImporter importer = AssetImporter.GetAtPath(srcFilePath);
            if (importer != null)
            {
                importer.SetAssetBundleNameAndVariant(fileName, "");
            }
        }

//        AssetBundleManifest _manifest;

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
