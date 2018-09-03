using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

namespace Trionfi
{
    public enum MessageType { Error, Warning, Info };

    enum TRStandPosition
    {
        CENTER = 0,
        LEFT = 1,
        RIGHT = 2
    }

    [System.Serializable]
    public class TRMediaInstance<T>
    {
        [SerializeField]
        public string path;
        [SerializeField]
        public T instance;
    }

    [System.Serializable]
    public class TRAudio : TRMediaInstance<AudioSource> { }

    [System.Serializable]
    public class TRLayer : TRMediaInstance<RawImage>
    {
        public string actor;
    }

    [ExecuteInEditMode]
    public class Trionfi : SingletonMonoBehaviour<Trionfi>
    {
        public static readonly string assetPath = "Assets/Trionfi/";

        [SerializeField]
        TextAsset bootScript;
        [SerializeField]
        public UnityEngine.Video.VideoPlayer videoPlayer;

        [SerializeField]
        public RenderTexture captureBuffer;
        [SerializeField]
        public RawImage rawImage;
        [SerializeField]
        public Camera targetCamera;
        [SerializeField]
        public Canvas layerCanvas;
        [SerializeField]
        public Canvas uiCanvas;
        [SerializeField]
        public TRMessageWindow messageWindow;
        [SerializeField]
        public TRSystemMenuWindow systemMenuWindow;
        [SerializeField]
        public TRMessageLogWindow messageLogwindow;
        [SerializeField]
        public TRSelectWindow selectWindow;

        static readonly SerializableDictionary<string, int> audioID = new SerializableDictionary<string, int>()
        {
            { "bgm", 0 },
            { "se", 1 },
            { "voice", 10 },
        };

        //SortOrderと等価
        static readonly SerializableDictionary<string, int> layerID = new SerializableDictionary<string, int>()
        {
            { "bg", 0 },
            { "stand", 1 },
            { "movie", 9 },
            { "event", 10 },
        };

        [Serializable]
        public class TRAudioInstance : SerializableDictionary<int, TRAudio> { }
        [Serializable]
        public class TRImageInstance : SerializableDictionary<int, TRLayer> { }

        [SerializeField]
        public TRAudioInstance audioInstance = new TRAudioInstance()
        {
            { audioID["bgm"] , null },
            { audioID["se"] , null },
            { audioID["voice"] , null },

        };

        [SerializeField]
        public TRImageInstance layerInstance = new TRImageInstance()
        {

            { layerID["bg"], null },
            { layerID["stand"], null },
            { layerID["event"], null },
        };

        public void Init(bool changeLayerOrder = false)
        {
            //Create Screen Cahpure Buffer;
            captureBuffer = new RenderTexture(Screen.width, Screen.height, 32);
            rawImage.texture = captureBuffer;

            //Init Screen Size
            layerCanvas.gameObject.GetComponent<CanvasScaler>().referenceResolution = TRSystemConfig.Instance.screenSize;

            if (changeLayerOrder)
            {
                layerInstance[0].instance.gameObject.transform.SetAsFirstSibling();
                //                referencedObjects.eventLayer.gameObject.transform.SetAsLastSibling();                
            }

            if (targetCamera == null)
                targetCamera = Camera.main;

            layerCanvas.worldCamera = targetCamera;

            if (TRSystemConfig.instance.KAGCompatibility)
                InitKAGAlias();

            if (bootScript != null)
            {
                TRTagInstance _tagInstance = new TRTagInstance();
                _tagInstance.CompileScriptString(bootScript.text);
                TRVirtualMachine.tagInstance[bootScript.name] = _tagInstance;
                StartCoroutine(TRVirtualMachine.Instance.Run("boot"));
            }
        }

        public delegate void OnClickEvent();
        public OnClickEvent ClickEvent;

        public void OnGlobalTapEvent()
        {
            ClickEvent();
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

        public void Begin(string scriptName)
        {
            uiCanvas.gameObject.GetComponent<CanvasGroup>().DOFade(0.0f, 1.0f).OnComplete
                (() =>
                    {
                        messageWindow.gameObject.SetActive(true);
                        systemMenuWindow.gameObject.SetActive(true);
                        TRTitle.Instance.gameObject.SetActive(false);
                        uiCanvas.gameObject.GetComponent<CanvasGroup>().DOFade(1.0f, 1.0f);

                        if (!string.IsNullOrEmpty(scriptName))
                        {
                            TRVirtualMachine.Instance.CompileScriptFile(scriptName);
                            TRVirtualMachine.Instance.Run(scriptName);
                        }
                    }
                );
        }

        public void InitKAGAlias()
        {
            TRVariable _temp = new TRVariable();
            _temp.Set("buf", audioID["bgm"]);
            TRVirtualMachine.aliasTagInstance["playbgm"] = new AudioComponent();
            TRVirtualMachine.aliasTagInstance["playbgm"].tagParam = _temp;
            TRVirtualMachine.aliasTagInstance["pausebgm"] = new AudiopauseComponent();
            TRVirtualMachine.aliasTagInstance["pausebgm"].tagParam = _temp;
            TRVirtualMachine.aliasTagInstance["resumebgm"] = new AudioresumeComponent();
            TRVirtualMachine.aliasTagInstance["resumebgm"].tagParam = _temp;
            TRVirtualMachine.aliasTagInstance["stopbgm"] = new AudiostopComponent();
            TRVirtualMachine.aliasTagInstance["stopbgm"].tagParam = _temp;

            _temp.Set("buf", audioID["se"]);
            TRVirtualMachine.aliasTagInstance["playse"] = new AudioComponent();
            TRVirtualMachine.aliasTagInstance["playse"].tagParam = _temp;
            TRVirtualMachine.aliasTagInstance["pausese"] = new AudiopauseComponent();
            TRVirtualMachine.aliasTagInstance["pausese"].tagParam = _temp;
            TRVirtualMachine.aliasTagInstance["stopse"] = new AudiostopComponent();
            TRVirtualMachine.aliasTagInstance["stopse"].tagParam = _temp;

            _temp.Set("buf", audioID["voice"]);
            TRVirtualMachine.aliasTagInstance["playvoice"] = new AudioComponent();
            TRVirtualMachine.aliasTagInstance["playvoice"].tagParam = _temp;
            TRVirtualMachine.aliasTagInstance["pausevoice"] = new AudiopauseComponent();
            TRVirtualMachine.aliasTagInstance["pausevoice"].tagParam = _temp;
            TRVirtualMachine.aliasTagInstance["stopvoice"] = new AudiostopComponent();
            TRVirtualMachine.aliasTagInstance["stopvoice"].tagParam = _temp;

            TRVirtualMachine.aliasTagInstance["freeimage"] = new ImagefreeComponent();

            TRVirtualMachine.aliasTagInstance["playvideo"] = new VideoplayComponent();
            TRVirtualMachine.aliasTagInstance["pausevideo"] = new VideopauseComponent();
            TRVirtualMachine.aliasTagInstance["resumevideo"] = new VideoresumeComponent();
            TRVirtualMachine.aliasTagInstance["stopvideo"] = new VideostopComponent();
        }

        public void Start()
        {
            Init();
        }
        public void Update()
        {
        }

    }
}
