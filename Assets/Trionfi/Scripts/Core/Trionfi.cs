﻿using System.IO;
using System;
using System.Diagnostics;
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
using TRDataType = Jace.DataType;

namespace Trionfi
{

    [Serializable]
    public enum MessageType { Error, Warning, Info };

    [Serializable]
    public enum LocalizeID
    {
        JAPAN = 0,
        ENGLISH = 1,
        CHINESE = 2,
        KOREAN = 3,
    }

    [Serializable]
    public enum TRStandPosition
    {
        CENTER = 0,
        LEFT = 1,
        RIGHT = 2,
        LEFT_CENTER = 3,
        RIGHT_CENTER
    }

    [Serializable]
    public enum TRAudioID
    {
        BGM = 0,
        SE1 = 1,
        SE2 = 2,
        VOICE1 = 11,
        UI = 99
    }

    [Serializable]
    public enum TRLayerID
    {
        BG = 0,
        STAND1 = 1,
        STAND2 = 2,
        STAND3 = 3,
        STAND4 = 4,
        STAND5 = 5,
        STAND6 = 6,
        STAND7 = 7,
        STAND8 = 8,
        STAND9 = 9,
        EVENT = 10,
        MESICON = 98,
        MOVIE = 99,
        UI = 100
    }

    [Serializable]
    public enum TRKeyboardShortCut
    {
        AutoMode,
        SkipMode,
        HideWindow,
        LogWindow,
        SystemWindow,
        AutoSave,
    }

    [Serializable]
    public enum TRResourceType
    {
        LocalStatic = 0,
        LocalStreaming = 1,
        WWW = 2,
        AssetBundle = 3,
    }

    [Serializable]
    public enum TRAssetType
    {
        Texture,
        Audio,
        Text,
        Movie,
        AssetBundle,
        Unknown
    };

#if !TR_PARSEONLY

    [Serializable]
    public class MediaInstanceKey<T> : SerializableDictionary<T, string> { };

    [Serializable]
    public class TRMediaInstance<T>
    {
        [SerializeField]
        public TRResourceType resourceType;
        [SerializeField]
        public string path;
        [SerializeField]
        public T instance;
    }

#if TR_USE_CRI
    [System.Serializable]
    public class TRAdx : TRMediaInstance<CriAtomExPlayer>
    {
        public static string basePath;
        public static void  LoadAcf(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }
            CriAtomEx.UnregisterAcf();
            CriAtomEx.RegisterAcf(null, path);
        }

        public CriAtomExAcb acb;
    }
#endif

    [Serializable]
    public class TRAudio : TRMediaInstance<AudioSource>
    {
        public float mainVolume = 1.0f;
        public float faderVolume = 1.0f;

        public void UpdateVolume()
        {
            instance.volume = faderVolume * mainVolume * TRGameConfig.configData.mastervolume;
        }
    }

    [Serializable]
    public class TRLayer : TRMediaInstance<RawImage>
    {
        public string actor;
    }

    [ExecuteInEditMode]
    public class Trionfi : SingletonMonoBehaviour<Trionfi>
    {
        public static readonly string assetPath = "Assets/Trionfi/";

        [System.NonSerialized]
        public RenderTexture captureBuffer;
        [System.NonSerialized]
        public RenderTexture movieBuffer;

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

        public void Init(bool changeLayerOrder = false)
        {
            audioInstance[TRAudioID.BGM].mainVolume = TRGameConfig.configData.bgmvolume * TRGameConfig.configData.mastervolume;
            audioInstance[TRAudioID.SE1].mainVolume = TRGameConfig.configData.sevolume * TRGameConfig.configData.mastervolume;
            audioInstance[TRAudioID.VOICE1].mainVolume = TRGameConfig.configData.voicevolume * TRGameConfig.configData.mastervolume;

            //Create Screen Cahpure Buffer;
            captureBuffer = new RenderTexture(Screen.width, Screen.height, 32);
            movieBuffer = new RenderTexture(Screen.width, Screen.height, 32);

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
            HideObject(configWindow.gameObject);//   TRGameConfigWindow.Instance.gameObject);
            HideObject(messageWindow.gameObject);
            HideObject(systemMenuWindow.gameObject);// TRSystemMenuWindow.Instance.gameObject);
            HideObject(messageLogwindow.gameObject);

            ClickEvent += PopWindow;
        }

        public void OpenAllUI()
        {
            if (videoPlayer.isPlaying)
                videoPlayer.Stop();

            PopWindow();
            //messageWindow.Restart();
            //messageWindow.gameObject.SetActive(true);
            systemMenuWindow.gameObject.gameObject.SetActive(true);

            //ClickEvent -= PopWindow;
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
            uiCanvas.gameObject.GetComponent<CanvasGroup>().DOFade(0.0f, 1.0f).OnComplete
                (() =>
                    {
                        if (messageWindow.currentName != null)
                            messageWindow.currentName.text = string.Empty;
                        if (messageWindow.currentUguiMessage != null)
                            messageWindow.currentUguiMessage.text = string.Empty;
                        if (messageWindow.currentMessage != null)
                            messageWindow.currentMessage.text = string.Empty;

                        messageWindow.gameObject.SetActive(true);
                        systemMenuWindow.gameObject.SetActive(true);

                        if (titleWindow != null)
                            titleWindow.gameObject.SetActive(false);

                        uiCanvas.gameObject.GetComponent<CanvasGroup>().DOFade(1.0f, 1.0f).OnComplete
                        (() =>
                        {
                            if (!string.IsNullOrEmpty(scriptName))
                            {
                                StartCoroutine(TRVirtualMachine.Instance.LoadScenarioAsset(scriptName, type, true));
                            }
                        }
                            );
                    }
                );
        }

        public void InitKAGAlias()
        {
            Button _button = globalTap.GetComponent<Button>();
            _button.onClick.AddListener(OnGlobalTapEvent);

            TRVariableDictionary _temp = new TRVariableDictionary();
            _temp["buf"] = new TRVariable((int)TRAudioID.BGM);
            TRVirtualMachine.aliasTagInstance["playbgm"] = new AudioComponent();
            TRVirtualMachine.aliasTagInstance["playbgm"].tagParam = _temp;
            TRVirtualMachine.aliasTagInstance["pausebgm"] = new AudiopauseComponent();
            TRVirtualMachine.aliasTagInstance["pausebgm"].tagParam = _temp;
            TRVirtualMachine.aliasTagInstance["resumebgm"] = new AudioresumeComponent();
            TRVirtualMachine.aliasTagInstance["resumebgm"].tagParam = _temp;
            TRVirtualMachine.aliasTagInstance["stopbgm"] = new AudiostopComponent();
            TRVirtualMachine.aliasTagInstance["stopbgm"].tagParam = _temp;

            _temp["buf"] = new TRVariable((int)TRAudioID.SE1);
            TRVirtualMachine.aliasTagInstance["playse"] = new AudioComponent();
            TRVirtualMachine.aliasTagInstance["playse"].tagParam = _temp;
            TRVirtualMachine.aliasTagInstance["pausese"] = new AudiopauseComponent();
            TRVirtualMachine.aliasTagInstance["pausese"].tagParam = _temp;
            TRVirtualMachine.aliasTagInstance["stopse"] = new AudiostopComponent();
            TRVirtualMachine.aliasTagInstance["stopse"].tagParam = _temp;

            _temp["buf"] = new TRVariable((int)TRAudioID.VOICE1);
            TRVirtualMachine.aliasTagInstance["playvoice"] = new AudioComponent();
            TRVirtualMachine.aliasTagInstance["playvoice"].tagParam = _temp;
            TRVirtualMachine.aliasTagInstance["pausevoice"] = new AudiopauseComponent();
            TRVirtualMachine.aliasTagInstance["pausevoice"].tagParam = _temp;
            TRVirtualMachine.aliasTagInstance["stopvoice"] = new AudiostopComponent();
            TRVirtualMachine.aliasTagInstance["stopvoice"].tagParam = _temp;
             
            TRVirtualMachine.aliasTagInstance["playvideo"] = new VideoplayComponent();
            TRVirtualMachine.aliasTagInstance["pausevideo"] = new VideopauseComponent();
            TRVirtualMachine.aliasTagInstance["resumevideo"] = new VideoresumeComponent();
            TRVirtualMachine.aliasTagInstance["stopvideo"] = new VideostopComponent();

            TRVirtualMachine.aliasTagInstance["freeimage"] = new ImagefreeComponent();
            TRVirtualMachine.aliasTagInstance["copylay"] = new SnapshotComponent();
            TRVirtualMachine.aliasTagInstance["freeimage"] = new ImagefreeComponent();
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
            messageWindow.gameObject.SetActive(false);

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
                //レイヤー1～10が立ち絵として割り振ってある。
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
            Vector2 canvasSize = uiCanvas.GetComponent<CanvasScaler>().referenceResolution;

            if(globalTap != null)
                globalTap.GetComponent<RectTransform>().sizeDelta = canvasSize;
            if (messageLogwindow != null)
                messageLogwindow.gameObject.SetActive(false);
            if(messageWindow != null)
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
        }
    }
#endif
}
