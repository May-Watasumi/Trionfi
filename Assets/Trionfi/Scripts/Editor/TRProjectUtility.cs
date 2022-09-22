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
        string _input = "NewProject";
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
            string _newPath = "Assets/" + _input;
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

            if (trionfiInstandce != null)
            {
                trionfiInstandce.gameObject.GetComponent<TRStageEnviroment>().CharacterNameListCSV = AssetDatabase.LoadAssetAtPath<TextAsset>(_newPath + "/Resources/ActorList.csv");
                trionfiInstandce.gameObject.GetComponent<TRStageEnviroment>().CharacterEmotionPatternListCSV = AssetDatabase.LoadAssetAtPath<TextAsset>(_newPath + "/Resources/ActPatternList.csv");
                trionfiInstandce.gameObject.GetComponent<TRSystemConfig>().screenSize = new Vector2(width, height);
            }

            //これはひどい。
            GameObject.DestroyImmediate(trionfiInstandce.titleWindow.gameObject);
            GameObject.DestroyImmediate(trionfiInstandce.messageWindowList[0].gameObject);
            GameObject.DestroyImmediate(trionfiInstandce.messageWindowList[1].gameObject);
            GameObject.DestroyImmediate(trionfiInstandce.messageLogwindow.gameObject);
            GameObject.DestroyImmediate(trionfiInstandce.globalTap.gameObject);
            GameObject.DestroyImmediate(trionfiInstandce.selectWindow.gameObject);
            GameObject.DestroyImmediate(trionfiInstandce.configWindow.gameObject);
            GameObject.DestroyImmediate(trionfiInstandce.systemMenuWindow.gameObject);
            GameObject.DestroyImmediate(trionfiInstandce.dialogWindow.gameObject);
            GameObject.DestroyImmediate(trionfiInstandce.nowLoading.gameObject);

            prefabList.Add(AssetDatabase.LoadAssetAtPath(_newPath + "/Prefabs/TitleBase.prefab", typeof(GameObject)) as GameObject);                //0
            prefabList.Add(AssetDatabase.LoadAssetAtPath(_newPath + "/Prefabs/MessageWindowBase.prefab", typeof(GameObject)) as GameObject);        //1
            prefabList.Add(AssetDatabase.LoadAssetAtPath(_newPath + "/Prefabs/MessageFullScreenWindow.prefab", typeof(GameObject)) as GameObject);  //2
            prefabList.Add(AssetDatabase.LoadAssetAtPath(_newPath + "/Prefabs/MessageLogWindowBase.prefab", typeof(GameObject)) as GameObject);     //3
            prefabList.Add(AssetDatabase.LoadAssetAtPath(_newPath + "/Prefabs/GlobalTap.prefab", typeof(GameObject)) as GameObject);                //4
            prefabList.Add(AssetDatabase.LoadAssetAtPath(_newPath + "/Prefabs/SelectWindowBase.prefab", typeof(GameObject)) as GameObject);         //5
            prefabList.Add(AssetDatabase.LoadAssetAtPath(_newPath + "/Prefabs/GameConfigBase.prefab", typeof(GameObject)) as GameObject);           //6
            prefabList.Add(AssetDatabase.LoadAssetAtPath(_newPath + "/Prefabs/SystemMenuBase.prefab", typeof(GameObject)) as GameObject);           //7
            prefabList.Add(AssetDatabase.LoadAssetAtPath(_newPath + "/Prefabs/DialogBase.prefab", typeof(GameObject)) as GameObject);               //8
            prefabList.Add(AssetDatabase.LoadAssetAtPath(_newPath + "/Prefabs/NowLoadingBase.prefab", typeof(GameObject)) as GameObject);           //9

            int count = 0;
            foreach (GameObject _object in prefabList)
            {
                if (_object == null)
                    Debug.Log("Prefab" + count.ToString() + " is null");
                else
                {
                    GameObject _instance = PrefabUtility.InstantiatePrefab(_object, scene) as GameObject;

                    if (_instance == null)
                        Debug.Log("Instance" + count.ToString() + " is null");
                    else
                    {
                        _instance.GetComponent<RectTransform>().SetParent(uiCanvas.gameObject.transform);
                        _instance.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                        _instance.GetComponent<RectTransform>().localScale = Vector3.one;
                        instanceList.Add(_instance);
                    }
                }
            }

            trionfiInstandce.titleName = _input;
            trionfiInstandce.titleWindow = instanceList[0].GetComponent<TRTitle>() ?? prefabList[0].GetComponent<TRTitle>();
            trionfiInstandce.messageWindowList = new List<TRMessageWindow>();
            trionfiInstandce.messageWindowList.Add(instanceList[1].GetComponent<TRMessageWindow>() ?? prefabList[1].GetComponent<TRMessageWindow>());
            trionfiInstandce.messageWindowList.Add(instanceList[2].GetComponent<TRMessageWindow>() ?? prefabList[2].GetComponent<TRMessageWindow>());
            trionfiInstandce.messageWindow = trionfiInstandce.messageWindowList[0];

            trionfiInstandce.messageLogwindow = instanceList[3].GetComponent<TRMessageLogWindow>() ?? prefabList[3].GetComponent<TRMessageLogWindow>();
            trionfiInstandce.globalTap = instanceList[4] ?? prefabList[4];
            trionfiInstandce.selectWindow = instanceList[5].GetComponent<TRSelectWindow>() ?? prefabList[5].GetComponent<TRSelectWindow>();
            trionfiInstandce.configWindow = instanceList[6].GetComponent<TRGameConfigWindow>() ?? prefabList[6].GetComponent<TRGameConfigWindow>();
            trionfiInstandce.systemMenuWindow = instanceList[7].GetComponent<TRSystemMenuWindow>() ?? prefabList[7].GetComponent<TRSystemMenuWindow>();
            trionfiInstandce.dialogWindow = instanceList[8].GetComponent<TRCustomDialog>() ?? prefabList[8].GetComponent<TRCustomDialog>();
            trionfiInstandce.nowLoading = instanceList[9] ?? prefabList[9];

            /*
                        AssetDatabase.Refresh();
                        AssetDatabase.SaveAssets();
            */

            EditorSceneManager.SaveScene(scene);
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
