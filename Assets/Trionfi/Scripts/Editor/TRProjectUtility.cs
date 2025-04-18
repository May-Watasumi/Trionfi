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
        private bool useTextMeshPro = true;
        private bool isNoveMode = false;

        [MenuItem("Tools/Trionfi/New Project")]
        private static void Open()
        {
            GetWindow<TRProjectUtility>("Input ProjectName");
        }

        private void SetupProject()
        {
            string _newPath = "Assets/" + _input;
            string _templatePath = _trionfiPath + "/Template";
            string _prefabPath = useTextMeshPro ? "TMPro/" : "Legacy/";


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
            Trionfi trionfiInstance = null;
            CanvasScaler uiCanvas = null;
            CanvasScaler layerCanvas = null;

            foreach (GameObject _object in rootObject)
            {
                if (_object.name == "Layer")
                    layerCanvas = _object.GetComponent<CanvasScaler>();
                else if (_object.name == "UI")
                    uiCanvas = _object.GetComponent<CanvasScaler>();
                else if (_object.name == "Trionfi")
                    trionfiInstance = _object.GetComponent<Trionfi>();
            }

            if (!int.TryParse(_x, out width))
                width = 1280;
            if (!int.TryParse(_y, out height))
                height = 720;

            PlayerSettings.defaultScreenWidth = width;
            PlayerSettings.defaultScreenHeight = height;

            PlayerSettings.productName = _input;

            if (uiCanvas != null)
                uiCanvas.referenceResolution = new Vector2(width, height);

            if (layerCanvas != null)
                layerCanvas.referenceResolution = new Vector2(width, height);

            if (trionfiInstance != null)
            {
                trionfiInstance.gameObject.GetComponent<TRStageEnviroment>().CharacterNameListCSV = AssetDatabase.LoadAssetAtPath<TextAsset>(_newPath + "/Resources/ActorList.csv");
                trionfiInstance.gameObject.GetComponent<TRStageEnviroment>().CharacterEmotionPatternListCSV = AssetDatabase.LoadAssetAtPath<TextAsset>(_newPath + "/Resources/ActPatternList.csv");

				trionfiInstance.gameObject.GetComponent<TRSystemConfig>().screenSize = new Vector2(width, height);
			}
			else
			{
				Debug.Log("Trionfi Instance is null");
			}

            //これはひどい。
            GameObject.DestroyImmediate(trionfiInstance.titleWindow.gameObject);           //0
            GameObject.DestroyImmediate(trionfiInstance.messageWindowList[0].gameObject);  //1
            GameObject.DestroyImmediate(trionfiInstance.messageWindowList[1].gameObject);  //2
            GameObject.DestroyImmediate(trionfiInstance.messageLogwindow.gameObject);      //3
            GameObject.DestroyImmediate(trionfiInstance.globalTap.gameObject);             //4
            GameObject.DestroyImmediate(trionfiInstance.selectWindow.gameObject);          //5
            GameObject.DestroyImmediate(trionfiInstance.configWindow.gameObject);          //6
            GameObject.DestroyImmediate(trionfiInstance.systemMenuWindow.gameObject);      //7
            GameObject.DestroyImmediate(trionfiInstance.serializer.gameObject); ;          //8
            GameObject.DestroyImmediate(trionfiInstance.dialogWindow.gameObject);          //9
            GameObject.DestroyImmediate(trionfiInstance.nowLoading.gameObject);            //10

            prefabList.Add(AssetDatabase.LoadAssetAtPath(_newPath + "/Prefabs/" + _prefabPath + "Title.prefab", typeof(GameObject)) as GameObject);                //0
            prefabList.Add(AssetDatabase.LoadAssetAtPath(_newPath + "/Prefabs/" + _prefabPath + "MessageWindow.prefab", typeof(GameObject)) as GameObject);        //1
            prefabList.Add(AssetDatabase.LoadAssetAtPath(_newPath + "/Prefabs/" + _prefabPath + "MessageFullScreenWindow.prefab", typeof(GameObject)) as GameObject);  //2
            prefabList.Add(AssetDatabase.LoadAssetAtPath(_newPath + "/Prefabs/" + _prefabPath + "MessageLogWindow.prefab", typeof(GameObject)) as GameObject);     //3
            prefabList.Add(AssetDatabase.LoadAssetAtPath(_newPath + "/Prefabs/GlobalTap.prefab", typeof(GameObject)) as GameObject);                               //4
            prefabList.Add(AssetDatabase.LoadAssetAtPath(_newPath + "/Prefabs/" + _prefabPath + "SelectWindow.prefab", typeof(GameObject)) as GameObject);         //5
            prefabList.Add(AssetDatabase.LoadAssetAtPath(_newPath + "/Prefabs/" + _prefabPath + "GameConfig.prefab", typeof(GameObject)) as GameObject);           //6
            prefabList.Add(AssetDatabase.LoadAssetAtPath(_newPath + "/Prefabs/" + _prefabPath + "SystemMenu.prefab", typeof(GameObject)) as GameObject);           //7
            prefabList.Add(AssetDatabase.LoadAssetAtPath(_newPath + "/Prefabs/" + _prefabPath + "Serializer.prefab", typeof(GameObject)) as GameObject);           //8
            prefabList.Add(AssetDatabase.LoadAssetAtPath(_newPath + "/Prefabs/" + _prefabPath + "Dialog.prefab", typeof(GameObject)) as GameObject);               //9
            prefabList.Add(AssetDatabase.LoadAssetAtPath(_newPath + "/Prefabs/" + _prefabPath + "NowLoading.prefab", typeof(GameObject)) as GameObject);           //10

			GameObject logData = (AssetDatabase.LoadAssetAtPath(_newPath + "/Prefabs/" + _prefabPath + "MessageLogData.prefab", typeof(GameObject)) as GameObject);       //11
			GameObject selectButton = (AssetDatabase.LoadAssetAtPath(_newPath + "/Prefabs/" + _prefabPath + "SelectButton.prefab", typeof(GameObject)) as GameObject);         //12

            prefabList[3].GetComponent<TRMessageLogWindowBase>().logContentPrefab = logData;
            prefabList[5].GetComponent<TRSelectWindow>().selectorPrefab = selectButton;

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


			trionfiInstance.titleName = _input;
            trionfiInstance.titleWindow = instanceList[0].GetComponent<TRTitleBase>() ?? prefabList[0].GetComponent<TRTitleBase>();
            trionfiInstance.messageWindowList = new List<TRMessageWindowBase>();
            trionfiInstance.messageWindowList.Add(instanceList[1].GetComponent<TRMessageWindowBase>() ?? prefabList[1].GetComponent<TRMessageWindowBase>());
            trionfiInstance.messageWindowList.Add(instanceList[2].GetComponent<TRMessageWindowBase>() ?? prefabList[2].GetComponent<TRMessageWindowBase>());
            trionfiInstance.currentMessageWindow = trionfiInstance.messageWindowList[0];

            trionfiInstance.messageLogwindow = instanceList[3].GetComponent<TRMessageLogWindowBase>() ?? prefabList[3].GetComponent<TRMessageLogWindowBase>();
//            trionfiInstance.messageLogwindow.logContentPrefab = logData;

			trionfiInstance.globalTap = instanceList[4] ?? prefabList[4];
            trionfiInstance.selectWindow = instanceList[5].GetComponent<TRSelectWindow>() ?? prefabList[5].GetComponent<TRSelectWindow>();
//            trionfiInstance.selectWindow.selectorPrefab = selectButton;

			trionfiInstance.configWindow = instanceList[6].GetComponent<TRGameConfigWindowBase>() ?? prefabList[6].GetComponent<TRGameConfigWindowBase>();
            trionfiInstance.systemMenuWindow = instanceList[7].GetComponent<TRSystemMenuWindowBase>() ?? prefabList[7].GetComponent<TRSystemMenuWindowBase>();
            trionfiInstance.serializer = instanceList[8].GetComponent<TRSerializerWindowBase>() ?? prefabList[7].GetComponent<TRSerializerWindowBase>();
            trionfiInstance.dialogWindow = instanceList[9].GetComponent<ICustomDialog>() ?? prefabList[8].GetComponent<ICustomDialog>();
            trionfiInstance.nowLoading = instanceList[10] ?? prefabList[9];

            /*
                        AssetDatabase.Refresh();
                        AssetDatabase.SaveAssets();

            */

            EditorSceneManager.SaveScene(scene);
        }

        void OnGUI()
        {
            GUILayout.Label("Project Name");
            GUI.SetNextControlName("ForcusField");

            _input = GUILayout.TextField(_input);
            GUILayout.Space(10f);

            GUILayout.Label("Trionfi Path");
            _trionfiPath = GUILayout.TextField(_trionfiPath);

            GUILayout.Label("Resolution");

            GUILayout.BeginHorizontal();

            GUILayout.Label("Width");
            _x = GUILayout.TextField(_x);
            GUILayout.Label("Height");
            _y = GUILayout.TextField(_y);

            GUILayout.EndHorizontal();

            GUILayout.Space(10f);

            GUILayout.Label("TextWindow Type");
            
            useTextMeshPro = GUILayout.Toggle(useTextMeshPro, "TextMeshPro");
            isNoveMode = GUILayout.Toggle(isNoveMode, "NovelMode");
            
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
