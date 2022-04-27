using System;
using System.Collections;
using System.Collections.Generic;
using Jace.Operations;

#if !TR_PARSEONLY
 using UnityEngine;
 using UnityEngine.UI;
 using UnityEngine.EventSystems;
 using DG.Tweening;
#endif

using TRVariable = Jace.Operations.VariableCalcurator;
//using TRDataType = Jace.DataType;

namespace Trionfi
{
    [ExecuteInEditMode]
    public class Trionfi : SingletonMonoBehaviour<Trionfi>
    {
        public static readonly string assetPath = "Assets/Trionfi/";

        [System.NonSerialized]
        public RenderTexture captureBuffer;
        [System.NonSerialized]
        public RenderTexture movieBuffer;
        [System.NonSerialized]
        public RenderTexture[] subRenderBuffer = new RenderTexture[1];

        [SerializeField]
        public string titleName = "Example";

        [SerializeField]
        string bootScriptName;
        [SerializeField]
        public UnityEngine.Video.VideoPlayer videoPlayer;
        [SerializeField]
        public UnityEngine.Audio.AudioMixer audioMixer;
        [SerializeField]
        public GameObject otherComponent;

        [SerializeField]
        public RawImage rawImage;
        [SerializeField]
        public Text layerText;
        [SerializeField]
        public Camera targetCamera;
        [SerializeField]
        public Camera subCamera;
        [SerializeField]
        public Canvas layerCanvas;
        [SerializeField]
        public Canvas uiCanvas;
        [SerializeField]
        public RectMask2D layerMask;

        [SerializeField]
        public TRTitle titleWindow;
        [SerializeField]
        public TRMessageLogWindow messageLogwindow;
        [SerializeField]
        public TRMessageWindow messageWindow;
        [SerializeField]
        public GameObject globalTap;
        [SerializeField]
        public TRSelectWindow selectWindow;
        [SerializeField]
        public TRSystemMenuWindow systemMenuWindow;
        [SerializeField]
        public TRGameConfigWindow configWindow;
        [SerializeField]
        public TRCustomDialog dialogWindow;
        [SerializeField]
        public GameObject nowLoading;

        [SerializeField]
        public List<TRMessageWindow> messageWindowList = new List<TRMessageWindow>();

        [Serializable]
        public class TRAudioInstance : SerializableDictionary/*TRMediaInstanceDictionary*/<TRAudioID, TRAudio> { };
        [Serializable]
        public class TRImageInstance : SerializableDictionary/*TRMediaInstanceDictionary*/<TRLayerID, TRLayer> { };

        public class TRScriptInstance : SerializableDictionary/*TRMediaInstanceDictionary*/<string, TRScript> { };

#if TR_USE_CRI
        [Serializable]
        public class TRAdxInstance : SerializableDictionary<int, TRAdx> { }

        [SerializeField]
        public TRAdxInstance adxInstance = new TRAdxInstance()
        {
            { audioID["bgm"] , null },
            { audioID["se"] , null },
            { audioID["voice"] , null },

        };
#endif
        [SerializeField]
        public TRAudioInstance audioInstance = new TRAudioInstance();
        [SerializeField]
        public TRImageInstance layerInstance = new TRImageInstance();
        [SerializeField]
        public TRScriptInstance scriptInstance = new TRScriptInstance();
        [SerializeField]
        public bool enableEndCallback = true;

        protected TRTagParser tagParser = new TRTagParser(string.Empty);

        //コールバック。
        public delegate void SystemEvent();
        public SystemEvent AwakeTrionfi;
        public SystemEvent SleepTrionfi;

        public void DefaultEndScriptCallBack()
        {
            messageWindow.CloseWindow();               
        }

        public void DefaultBeginScriptCallBack()
        {
            messageWindow.OpenWindow();
        }

        public AbstractComponent GetTagComponent(string tagString)
        {
            return tagParser.Parse(tagString);
        }

        //スタック等を使わない簡易実行。
        public IEnumerator ExecuteTagArray(AbstractComponent[] tagComponent)
        {
            foreach(AbstractComponent tag in  tagComponent)
			{
                yield return tag.Execute();
			}
        }

        public IEnumerator LoadScript(string storage, TRResourceType type = TRResourceLoader.defaultResourceType, bool run = false)
        {
            var _coroutine = TRResourceLoader.Instance.LoadText(storage);
            yield return StartCoroutine(_coroutine);

            if (!string.IsNullOrEmpty((string)_coroutine.Current))
            {
                TRTagInstance _instance = new TRTagInstance();
                _instance.CompileScriptString((string)_coroutine.Current);

                TRScript _script = new TRScript();
                _script.instance = _instance;
                _script.path = storage;
                _script.resourceType = type;

                scriptInstance[storage] = _script;
            }

            if (run)
                StartCoroutine(TRVirtualMachine.instance.Run(storage));

            yield return _coroutine.Current;
        }

        public IEnumerator LoadAudio(int ch, string storage, TRResourceType type = TRResourceLoader.defaultResourceType)
        {
            var _coroutine = TRResourceLoader.Instance.LoadAudio(storage, type);
            yield return StartCoroutine(_coroutine);

            if(_coroutine.Current != null)
            {
                audioInstance[(TRAudioID)ch].instance.clip = (AudioClip)_coroutine.Current;
                audioInstance[(TRAudioID)ch].path = storage;
                audioInstance[(TRAudioID)ch].resourceType = type;
            }
            yield return _coroutine.Current;
        }

        public IEnumerator LoadImage(int ch, string storage, TRResourceType type = TRResourceLoader.defaultResourceType)
        {
            var _coroutine = TRResourceLoader.Instance.LoadTexture(storage, type);
            yield return StartCoroutine(_coroutine);

            if (_coroutine.Current != null)
            {
                layerInstance[(TRLayerID)ch].instance.texture = (Texture2D)_coroutine.Current;
                layerInstance[(TRLayerID)ch].path = storage;
                layerInstance[(TRLayerID)ch].resourceType = type;
            }
            yield return _coroutine.Current;
        }

        public void Save(string name)
        {
            SerializeData info = new SerializeData();
            info.Serialize();

            string data = JsonUtility.ToJson(info);

            PlayerPrefs.SetString(name, data);
        }

        public void Load(string name)
        {
            SerializeData info = new SerializeData();
            string data =  PlayerPrefs.GetString(name);
            info = JsonUtility.FromJson<SerializeData>(data);
            info.Deserialize();
        }

        public void Init(int subRenderCount = 0, bool changeLayerOrder = false)
        {
            audioInstance[TRAudioID.BGM].mainVolume = TRGameConfig.configData.bgmvolume * TRGameConfig.configData.mastervolume;
            audioInstance[TRAudioID.SE1].mainVolume = TRGameConfig.configData.sevolume * TRGameConfig.configData.mastervolume;
            audioInstance[TRAudioID.VOICE1].mainVolume = TRGameConfig.configData.voicevolume * TRGameConfig.configData.mastervolume;

            //Create Screen Cahpure Buffer;
            captureBuffer = new RenderTexture(Screen.width, Screen.height, 32);
            movieBuffer = new RenderTexture(Screen.width, Screen.height, 32);

            if (subCamera != null)
            {
                subRenderBuffer[0] = new RenderTexture(Screen.width, Screen.height, 32);
                subCamera.targetTexture = subRenderBuffer[0];
            }

            rawImage.texture = captureBuffer;

            videoPlayer.targetTexture = movieBuffer;
            layerInstance[TRLayerID.MOVIE].instance.texture = movieBuffer;

            float screenAspect = (float)Screen.width / (float)Screen.height;
            float canvasAspect = TRSystemConfig.Instance.screenSize.x / TRSystemConfig.Instance.screenSize.y;

            if (screenAspect >= canvasAspect)
            {
                rawImage.GetComponent<RectTransform>().sizeDelta = new Vector2(TRSystemConfig.Instance.screenSize.y * Screen.width / Screen.height, TRSystemConfig.Instance.screenSize.y);
                uiCanvas.gameObject.GetComponent<CanvasScaler>().matchWidthOrHeight = 1.0f;
            }
            else
            {
                rawImage.GetComponent<RectTransform>().sizeDelta = new Vector2(TRSystemConfig.Instance.screenSize.x, TRSystemConfig.Instance.screenSize.x * Screen.height / Screen.width);
                uiCanvas.gameObject.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.0f;
            }

            if (layerMask != null)
                layerMask.GetComponent<RectTransform>().sizeDelta = TRSystemConfig.Instance.screenSize;

            //Init Screen Size
            layerCanvas.gameObject.GetComponent<CanvasScaler>().referenceResolution = TRSystemConfig.Instance.screenSize;

            if (changeLayerOrder)
            {
                layerInstance[TRLayerID.BG].instance.gameObject.transform.SetAsFirstSibling();
                //                referencedObjects.eventLayer.gameObject.transform.SetAsLastSibling();                
            }

            if (targetCamera == null)
                targetCamera = Camera.main;

            layerCanvas.worldCamera = targetCamera;

            if (TRSystemConfig.instance.KAGCompatibility)
                InitKAGAlias();

            if(!string.IsNullOrEmpty(bootScriptName))
            {
                Begin(bootScriptName);
            }

            return;
        }

        public delegate void OnClickEvent();
        public OnClickEvent ClickEvent;

        public void OnGlobalTapEvent()
        {
            if (ClickEvent != null)
                ClickEvent();
        }

        public Queue<GameObject> closedWindow = new Queue<GameObject>();

        GameObject currentUI;

        public void HideObject(GameObject window)
        {
            if(window.activeSelf)
                closedWindow.Enqueue(window);

            window.SetActive(false);
        }

        public void OpenUI(GameObject ui)
        {
            currentUI = ui;

            if(currentUI != null)
                currentUI.SetActive(true);

            ClickEvent += PopWindow;
        }

        public void PopWindow()
        {
            while (closedWindow.Count > 0)
            {
                GameObject window =  closedWindow.Dequeue();
                window.SetActive(true);
            }

            if (currentUI != null)
                currentUI.SetActive(false);

            if(messageWindow.gameObject.activeSelf)
                messageWindow.Restart();

            ClickEvent -= PopWindow;
        }

        public void CloseAllUI(GameObject ui = null)
        {
            messageWindow.Pause();

            HideObject(configWindow.gameObject);
            HideObject(messageWindow.gameObject);
            HideObject(systemMenuWindow.gameObject);
            HideObject(messageLogwindow.gameObject);

            ClickEvent += PopWindow;
        }

        public void OpenAllUI()
        {
            PopWindow();
            ClickEvent -= PopWindow;
        }

        public static bool IsPointerOverGameObject()
        {
            if (EventSystem.current == null)
                return false;

            if (EventSystem.current.IsPointerOverGameObject())
                return true;

            if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
            {
                if (EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
                    return true;
            }

            return false;
        }

        public void Begin(string scriptName, TRResourceType type = TRResourceLoader.defaultResourceType)
        {
            if (titleWindow != null)
                titleWindow.gameObject.SetActive(false);

            if (!string.IsNullOrEmpty(scriptName))
                StartCoroutine(LoadScript(scriptName, type, true));
        }

        public void InitKAGAlias()
        {
            Button _button = globalTap.GetComponent<Button>();
            _button.onClick.AddListener(OnGlobalTapEvent);

            TRVariableDictionary _temp = new TRVariableDictionary();
            _temp["buf"] = new TRVariable((int)TRAudioID.BGM);
            TRVirtualMachine.Instance.aliasTagInstance["playbgm"] = new AudioComponent();
            TRVirtualMachine.Instance.aliasTagInstance["playbgm"].tagParam = _temp;
            TRVirtualMachine.Instance.aliasTagInstance["pausebgm"] = new AudiopauseComponent();
            TRVirtualMachine.Instance.aliasTagInstance["pausebgm"].tagParam = _temp;
            TRVirtualMachine.Instance.aliasTagInstance["resumebgm"] = new AudioresumeComponent();
            TRVirtualMachine.Instance.aliasTagInstance["resumebgm"].tagParam = _temp;
            TRVirtualMachine.Instance.aliasTagInstance["stopbgm"] = new AudiostopComponent();
            TRVirtualMachine.Instance.aliasTagInstance["stopbgm"].tagParam = _temp;

            _temp["buf"] = new TRVariable((int)TRAudioID.SE1);
            TRVirtualMachine.Instance.aliasTagInstance["playse"] = new AudioComponent();
            TRVirtualMachine.Instance.aliasTagInstance["playse"].tagParam = _temp;
            TRVirtualMachine.Instance.aliasTagInstance["pausese"] = new AudiopauseComponent();
            TRVirtualMachine.Instance.aliasTagInstance["pausese"].tagParam = _temp;
            TRVirtualMachine.Instance.aliasTagInstance["stopse"] = new AudiostopComponent();
            TRVirtualMachine.Instance.aliasTagInstance["stopse"].tagParam = _temp;

            _temp["buf"] = new TRVariable((int)TRAudioID.VOICE1);
            TRVirtualMachine.Instance.aliasTagInstance["playvoice"] = new AudioComponent();
            TRVirtualMachine.Instance.aliasTagInstance["playvoice"].tagParam = _temp;
            TRVirtualMachine.Instance.aliasTagInstance["pausevoice"] = new AudiopauseComponent();
            TRVirtualMachine.Instance.aliasTagInstance["pausevoice"].tagParam = _temp;
            TRVirtualMachine.Instance.aliasTagInstance["stopvoice"] = new AudiostopComponent();
            TRVirtualMachine.Instance.aliasTagInstance["stopvoice"].tagParam = _temp;
             
            TRVirtualMachine.Instance.aliasTagInstance["playvideo"] = new VideoplayComponent();
            TRVirtualMachine.Instance.aliasTagInstance["pausevideo"] = new VideopauseComponent();
            TRVirtualMachine.Instance.aliasTagInstance["resumevideo"] = new VideoresumeComponent();
            TRVirtualMachine.Instance.aliasTagInstance["stopvideo"] = new VideostopComponent();

            TRVirtualMachine.Instance.aliasTagInstance["freeimage"] = new ImagefreeComponent();
            TRVirtualMachine.Instance.aliasTagInstance["copylay"] = new SnapshotComponent();
            TRVirtualMachine.Instance.aliasTagInstance["freeimage"] = new ImagefreeComponent();
        }

        public void ResetCanvas(int mesWindowID = 0)
        {
            messageWindow = messageWindowList[mesWindowID];
            messageWindow.ClearMessage();

            foreach (KeyValuePair<TRLayerID ,TRLayer> instance in layerInstance)
            {
                instance.Value.instance.enabled = false;
                instance.Value.instance.texture = null;
                instance.Value.path = string.Empty;
                instance.Value.actor = string.Empty;
            }

            foreach (KeyValuePair<TRAudioID, TRAudio> instance in audioInstance)
            {
                instance.Value.instance.Stop();
                instance.Value.path = string.Empty;
            }

            layerCanvas.gameObject.SetActive(true);
            uiCanvas.gameObject.SetActive(true);

        }

        public void HideCanvas()
        {
            layerCanvas.gameObject.SetActive(false);
            uiCanvas.gameObject.SetActive(false);

            foreach (KeyValuePair<TRAudioID, TRAudio> instance in audioInstance)
            {
                instance.Value.instance.Stop();
                instance.Value.path = string.Empty;
            }
        }

        public void SetStandLayerTone()
        {
            if (!TRSystemConfig.Instance.layerFocus)
                return;

            foreach (KeyValuePair<TRLayerID ,TRLayer> instance in layerInstance)
            {
                //レイヤー1～9が立ち絵として割り振ってある。
                if (instance.Value.instance == null || instance.Key < TRLayerID.STAND1 || instance.Key > (TRLayerID)10)
                    continue;

                if (instance.Value.actor != messageWindow.currentSpeaker )
                    instance.Value.instance.color = Color.gray;
                else
                    instance.Value.instance.color = Color.white;
            }
        }

        public void Start()
        {
#if TR_USE_CRI
            TRAdx.basePath = Application.streamingAssetsPath;
            /*
            if (otherComponent.GetComponent< CriWareInitializer>())
                otherComponent.AddComponent<CriWareInitializer>();
            if(otherComponent.GetComponent<CriWareErrorHandler>())
                otherComponent.AddComponent<CriWareInitializer>();
            */
            foreach (KeyValuePair<int, TRAdx> _pair in adxInstance )
            {
                _pair.Value.instance = new CriAtomExPlayer();
            }
#endif
            AwakeTrionfi += DefaultBeginScriptCallBack;
            SleepTrionfi += DefaultEndScriptCallBack;

            Vector2 canvasSize = uiCanvas.GetComponent<CanvasScaler>().referenceResolution;

            if(globalTap != null)
                globalTap.GetComponent<RectTransform>().sizeDelta = canvasSize;
            if (messageLogwindow != null)
                messageLogwindow.gameObject.SetActive(false);
            if (messageWindow != null)
                messageWindow.gameObject.SetActive(false);
            if(selectWindow != null)
                selectWindow.gameObject.SetActive(false);
            if(systemMenuWindow != null)
                systemMenuWindow.gameObject.SetActive(false);
            if(configWindow != null)
                configWindow.gameObject.SetActive(false);
            if(dialogWindow != null)
                dialogWindow.gameObject.SetActive(false);
            if(nowLoading != null)
                nowLoading.gameObject.SetActive(false);

            if (!TRSystemConfig.Instance.isNovelMode)
                messageWindow.systemWindow = systemMenuWindow.gameObject;

            Init();
        }

        public void Update()
        {
        }

        public void OnDestroy()
        {
            if (captureBuffer != null)
                captureBuffer.Release();
            if(movieBuffer != null)
                movieBuffer.Release();
            if (subRenderBuffer[0] != null)
                subRenderBuffer[0].Release();
        }
    }
}
