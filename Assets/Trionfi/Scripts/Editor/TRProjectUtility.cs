using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Trionfi
{
    // テキスト入力Window
    public class TRProjectUtility : EditorWindow
    {
        bool _isInit = false;

        int width = 1280;
        int height = 720;
        string _input = "NewProject";
        string _x = 1280.ToString();
        string _y = 720.ToString();

        string _bgPath = "bg";
        string _standPath = "fgimage";
        string _uiImagePath = "image";
        string _bgmPath = "bgm";
        string _ruleImagePath = "rule";
        string _voicePath = "voice";
        string _sePath = "sound";
        string _otherPath = "other";

        [MenuItem("Tools/Trionfi/New Project")]
        private static void Open()
        {
            GetWindow<TRProjectUtility>("Input ProjectName");
        }

        private void SetupProject(string name)
        {
            string _newPath = "Assets/" + name;// + "/" + TRAssetPathObject.assetName;
            const string _templatePath = "Assets/Trionfi/Template";

            AssetDatabase.CopyAsset("Assets/Trionfi/Template", "Assets/" + _newPath);
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            AssetDatabase.StartAssetEditing();

            EditorUtility.DisplayProgressBar("DeepDuplicate", "Start", 0);

            foreach (string filePath in Directory.GetFiles(_newPath, "*", SearchOption.AllDirectories))
            {
                string _ext = Path.GetExtension(filePath);

                if (_ext == ".meta" || _ext == ".unity")
                    continue;

                foreach (Object _object in AssetDatabase.LoadAllAssetsAtPath(filePath))
                {
                    if (_object == null)
                        continue;

                    if (PrefabUtility.GetPrefabType(_object) == PrefabType.Prefab)
                    {
                        Object oldAsset = AssetDatabase.LoadMainAssetAtPath(filePath);
                    }
                }
            }
        }

        void OnGUI()
        {
            GUILayout.Label("Project Setting");
            GUI.SetNextControlName("ForcusField");

            _input = GUILayout.TextField(_input);
            GUILayout.Space(10f);

            GUILayout.Label("解像度");

            GUILayout.BeginHorizontal();

            GUILayout.Label("Width");
            _x = GUILayout.TextField(_x);
            GUILayout.Label("Height");
            _y = GUILayout.TextField(_y);

            GUILayout.EndHorizontal();

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

            GUILayout.Space(10.0f);

            // 何かしら入力しないとOKボタンを押せないようにするDisableGroup
            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(_input) || !int.TryParse(_x, out width) || !int.TryParse(_y, out height));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("OK", GUILayout.Height(30f)))
            {
                SetupProject(_input);
                Close();
            }

            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button("CANCEL", GUILayout.Height(30f)))
            {
                Close();
            }
            GUILayout.EndHorizontal();

            if (_isInit == false)
            {
                EditorGUI.FocusTextInControl("ForcusField");
            }

            _isInit = true;
        }
    }
}
