using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Trionfi
{
    // テキスト入力Window
    public class TRProjectUtility : EditorWindow
    {
        bool _isInit = false;

        int width = 1280;
        int height = 720;
        string _input = "Assets/NewProject";
        string _trionfiPath = "Assets/Trionfi";
        string _x = 1280.ToString();
        string _y = 720.ToString();

        [MenuItem("Tools/Trionfi/New Project")]
        private static void Open()
        {
            GetWindow<TRProjectUtility>("Input ProjectName");
        }

        private void SetupProject()
        {
            string _newPath = _input;// "Assets/" + name;
            string _templatePath = _trionfiPath + "/Template";

            AssetDatabase.CopyAsset(_templatePath, _newPath);

            string scenepath = _newPath + "/Start.unity";

            var scenes = EditorBuildSettings.scenes;
            ArrayUtility.Add( ref scenes, new EditorBuildSettingsScene(scenepath, true));
            EditorBuildSettings.scenes = scenes;

            //シーンを開く
            Scene scene = EditorSceneManager.OpenScene(scenepath);//, OpenSceneMode.Additive);

            List<GameObject> prefabList = new List<GameObject>();
            List<GameObject> instanceList = new List<GameObject>();

            GameObject[] rootObject = scene.GetRootGameObjects();
            Trionfi trionfiInstandce = null;
            CanvasScaler uiCanvas = null;
            CanvasScaler layerCanvas = null;

            foreach (GameObject _object in rootObject)
            {
                if (_object.name == "Layer")
                    layerCanvas = _object.GetComponent<CanvasScaler>();
                else if (_object.name == "UI")
                    uiCanvas = _object.GetComponent<CanvasScaler>();
                else if (_object.name == "Trionfi")
                    trionfiInstandce = _object.GetComponent<Trionfi>();
            }

            if (!int.TryParse(_x, out width))
                width = 1280;
            if (!int.TryParse(_y, out height))
                height = 720;

            if (uiCanvas != null)
                uiCanvas.referenceResolution = new Vector2(width, height);

            if (layerCanvas != null)
                layerCanvas.referenceResolution = new Vector2(width, height);

            //これはひどい。
            GameObject.DestroyImmediate(trionfiInstandce.titleWindow.gameObject);
            GameObject.DestroyImmediate(trionfiInstandce.messageWindow.gameObject);
            GameObject.DestroyImmediate(trionfiInstandce.messageLogwindow.gameObject);
            GameObject.DestroyImmediate(trionfiInstandce.globalTap.gameObject);
            GameObject.DestroyImmediate(trionfiInstandce.selectWindow.gameObject);
            GameObject.DestroyImmediate(trionfiInstandce.systemMenuWindow.gameObject);
            GameObject.DestroyImmediate(trionfiInstandce.configWindow.gameObject);
            GameObject.DestroyImmediate(trionfiInstandce.dialogWindow.gameObject);
            GameObject.DestroyImmediate(trionfiInstandce.nowLoading.gameObject);

            prefabList.Add( AssetDatabase.LoadAssetAtPath(_newPath + "/Prefabs/TitleBase.prefab", typeof(GameObject)) as GameObject);
            prefabList.Add( AssetDatabase.LoadAssetAtPath(_newPath + "/Prefabs/MessageLogWindowBase.prefab", typeof(GameObject)) as GameObject);
            prefabList.Add( AssetDatabase.LoadAssetAtPath(_newPath + "/Prefabs/MessageWindowBase.prefab", typeof(GameObject)) as GameObject);
            prefabList.Add( AssetDatabase.LoadAssetAtPath(_newPath + "/Prefabs/GlobalTap.prefab", typeof(GameObject)) as GameObject);
            prefabList.Add( AssetDatabase.LoadAssetAtPath(_newPath + "/Prefabs/SelectWindowBase.prefab", typeof(GameObject)) as GameObject);
            prefabList.Add( AssetDatabase.LoadAssetAtPath(_newPath + "/Prefabs/SystemMenuBase.prefab", typeof(GameObject)) as GameObject);
            prefabList.Add( AssetDatabase.LoadAssetAtPath(_newPath + "/Prefabs/GameConfigBase.prefab", typeof(GameObject)) as GameObject);
            prefabList.Add( AssetDatabase.LoadAssetAtPath(_newPath + "/Prefabs/DialogBase.prefab", typeof(GameObject)) as GameObject);
            prefabList.Add( AssetDatabase.LoadAssetAtPath(_newPath + "/Prefabs/NowLoadingBase.prefab", typeof(GameObject)) as GameObject);

            foreach (GameObject _object in prefabList)
            {
                GameObject _instance = PrefabUtility.InstantiatePrefab(_object, scene) as GameObject;
                _instance.GetComponent<RectTransform>().SetParent(uiCanvas.gameObject.transform);
                _instance.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                _instance.GetComponent<RectTransform>().localScale = Vector3.one;
                instanceList.Add(_instance);
            }

            trionfiInstandce.titleWindow = instanceList[0].GetComponent<TRTitle>() ?? prefabList[0].GetComponent<TRTitle>();
            trionfiInstandce.messageLogwindow = instanceList[1].GetComponent<TRMessageLogWindow>() ?? prefabList[2].GetComponent<TRMessageLogWindow>();
            trionfiInstandce.messageWindow = instanceList[2].GetComponent<TRMessageWindow>() ?? prefabList[1].GetComponent<TRMessageWindow>();
            trionfiInstandce.globalTap = instanceList[3] ?? prefabList[3];
            trionfiInstandce.selectWindow = instanceList[4].GetComponent<TRSelectWindow>() ?? prefabList[4].GetComponent<TRSelectWindow>();
            trionfiInstandce.systemMenuWindow = instanceList[5].GetComponent<TRSystemMenuWindow>() ?? prefabList[5].GetComponent<TRSystemMenuWindow>();
            trionfiInstandce.configWindow = instanceList[6].GetComponent<TRGameConfigWindow>() ?? prefabList[6].GetComponent<TRGameConfigWindow>();
            trionfiInstandce.dialogWindow = instanceList[7].GetComponent<TRCustomDialog>() ?? prefabList[7].GetComponent<TRCustomDialog>();
            trionfiInstandce.nowLoading = instanceList[8] ?? prefabList[8];
/*
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            EditorSceneManager.SaveScene(scene);
*/
        }

        void OnGUI()
        {
            GUILayout.Label("Project Path");
            GUI.SetNextControlName("ForcusField");

            _input = GUILayout.TextField(_input);
            GUILayout.Space(10f);

            GUILayout.Label("Trionfi Path");
            _trionfiPath = GUILayout.TextField(_trionfiPath);

            GUILayout.Label("解像度");

            GUILayout.BeginHorizontal();

            GUILayout.Label("Width");
            _x = GUILayout.TextField(_x);
            GUILayout.Label("Height");
            _y = GUILayout.TextField(_y);

            GUILayout.EndHorizontal();

            GUILayout.Space(10f);

            // 何かしら入力しないとOKボタンを押せないようにするDisableGroup
            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(_input) || !int.TryParse(_x, out width) || !int.TryParse(_y, out height));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("OK", GUILayout.Height(30f)))
            {
                SetupProject();
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
