using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Jace.Operations;
using Newtonsoft.Json;

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
    public class Trionfi : SingletonMonoBehaviour<Trionfi>
    {
        public static readonly string assetPath = "Assets/Trionfi/";
        public static readonly string readFlagData = "ReadFlags.dat";

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
        public TRSerializeManager serializer;

        [SerializeField]
        public List<TRMessageWindow> messageWindowList = new List<TRMessageWindow>();

        [Serializable]
        public class TRAudioInstance : SerializableDictionary/*TRMediaInstanceDictionary*/<TRAudioID, TRAudio>
        {
            public void Reset()
            {
                foreach (KeyValuePair<TRAudioID, TRAudio> pair in this)
                {
                    pair.Value.instance.Stop();
                    //                    pair.Value.instance.volume;
                    pair.Value.instance.clip = null;
                    pair.Value.faderVolume = 1.0f;
                    //                    pair.Value.mainVolume = 1.0f;
                    pair.Value.tagParam.Clear();
                }
            }

            public SerializableDictionary<int, TRVariableDictionary> Serialize()
            {
                SerializableDictionary<int, TRVariableDictionary> tagParams = new SerializableDictionary<int, TRVariableDictionary>();
                {
                    foreach (KeyValuePair<TRAudioID, TRAudio> pair in this)
                    {
                        tagParams[(int)pair.Key] = pair.Value.tagParam;
                    }

                    return tagParams;
                }
            }

            public void Deserialize(SerializableDictionary<int, TRVariableDictionary> tagParams)
            {
                Reset();

                foreach (KeyValuePair<TRAudioID, TRAudio> pair in this)
                {
                    pair.Value.tagParam = tagParams[(int)pair.Key];
                }
            }
        }

        [Serializable]
        public class TRImageInstance : SerializableDictionary/*TRMediaInstanceDictionary*/<TRLayerID, TRLayer>
        {
            public void Reset()
            {
                foreach (KeyValuePair<TRLayerID, TRLayer> pair in this)
                {
                    pair.Value.instance.rectTransform.anchoredPosition = new Vector2(pair.Value.instance.rectTransform.anchoredPosition.x, 0.0f);
                    pair.Value.instance.texture = null;
                    pair.Value.actor = string.Empty;
                    pair.Value.instance.color = Color.white;
                    pair.Value.tagParam.Clear();
                    pair.Value.instance.enabled = false;
                }
            }

            public SerializableDictionary<int, TRVariableDictionary> Serialize()
            {
                SerializableDictionary<int, TRVariableDictionary> tagParams = new SerializableDictionary<int, TRVariableDictionary>();
   
                {
                    foreach (KeyValuePair<TRLayerID, TRLayer> pair in this)
                    {
                        tagParams[(int)pair.Key] = pair.Value.tagParam;
                    }
                }

                return tagParams;
            }

            public void Deserialize(SerializableDictionary<int, TRVariableDictionary> tagParams)
            {
                Reset();

                foreach (KeyValuePair<TRLayerID, TRLayer> pair in this)
                {
                    pair.Value.tagParam = tagParams[(int)pair.Key];
                }
            }
        }

        public class TRScriptInstance : SerializableDictionary/*TRMediaInstanceDictionary*/<string, TRScript>
        {
            /*
            public void Reset()
            {
                foreach (KeyValuePair<string, TRScript> pair in this)
                { }
            } 
            */
            public SerializableDictionary<string, TRVariableDictionary> Serialize()
            {
                SerializableDictionary<string, TRVariableDictionary> tagParams = new SerializableDictionary<string, TRVariableDictionary>();
                {
                    foreach (KeyValuePair<string, TRScript> pair in this)
                    {
                        tagParams[pair.Key] = pair.Value.tagParam;
                    }
                }

                return tagParams;
            }

            public void Deserialize(SerializableDictionary<string, TRVariableDictionary> tagParams)
            {
//                Reset();

                foreach (KeyValuePair<string, TRScript> pair in this)
                {
                    pair.Value.tagParam = tagParams[pair.Key];
                }
            }

        };

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

        protected TRCrypterBase crypter = null;

        //コールバック。
        public delegate void SystemEvent();
        public SystemEvent AwakeTrionfi;
        public SystemEvent SleepTrionfi;

        //public Dictionary<string, string> flagDatas = new Dictionary<string, string>();
        public Dictionary<string, List<bool>> flagDatas;

        public void SaveReadFlag()
        {
            string path = Application.persistentDataPath + "/" + readFlagData;
#if true
            string jsonData = JsonConvert.SerializeObject(flagDatas);//, Formatting.Indented);

            File.WriteAllText(path, jsonData);
#else            
            System.Runtime.Serialization.DataContractSerializer dataSerializer = new System.Runtime.Serialization.DataContractSerializer(typeof(Dictionary<string, List<bool>>));

            MemoryStream dataStream = new MemoryStream();
            using (var binWriter = System.Xml.XmlDictionaryWriter.CreateBinaryWriter(dataStream))
            {
                dataSerializer.WriteObject(binWriter, flagDatas);

                byte[] binArray = dataStream.ToArray();

                BinaryWriter writer = new BinaryWriter(File.OpenWrite(path));
                writer.Write(binArray);
            }
#endif
        }

        public void LoadReadFlag()
        {
            string path = Application.persistentDataPath + "/" + readFlagData;
#if true
            //            string jsonData =  PlayerPrefs.GetString(readFlagData);

            if (File.Exists(path))
            {
                string jsonData = File.ReadAllText(path);

                if(!string.IsNullOrEmpty(jsonData))
                    flagDatas = Newtonsoft.Json.JsonConvert.DeserializeObject< Dictionary<string, List<bool>> >(jsonData);
            }

            if (flagDatas == null)
                flagDatas = new Dictionary<string, List<bool>>();

            /*
            if(flagDatas != null)
                    foreach (KeyValuePair<string, string> script in flagDatas)
                    {
                        scriptInstance[script.Key].instance.SetReadFlagJsonData(script.Value);
                    }
            */
#else
#if UNITY_EDITOR
            string path = Directory.GetCurrentDirectory();
#else
            string path = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
#endif
            path += ("/" + readFlagData);

            //            BinaryReader reader = new BinaryReader(File.OpenRead(path));

            using (FileStream fileStream = new FileStream(path, FileMode.Open))//, FileAccess.Read);
            {
                //            MemoryStream dataStream = new MemoryStream();

                if (fileStream != null)
                {
                    using (var binReader = System.Xml.XmlDictionaryReader.CreateBinaryReader(fileStream, System.Xml.XmlDictionaryReaderQuotas.Max))
                    {
                        System.Runtime.Serialization.DataContractSerializer dataSerializer = new System.Runtime.Serialization.DataContractSerializer(typeof(Dictionary<string, List<bool>>));
                        flagDatas = (Dictionary<string, List<bool>>)dataSerializer.ReadObject(binReader);

                        if (flagDatas == null)
                            flagDatas = new Dictionary<string, List<bool>>();
                    }
                }
            }
#endif
        }

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

                if (!flagDatas.ContainsKey(storage))
                {
                    flagDatas[storage] = new List<bool>();

                    foreach (AbstractComponent tag in _instance.arrayComponents)
                    {
                        if (tag.GetType() == typeof(MessageComponent))
                            flagDatas[storage].Add(false);
                    }
                }

                _instance.isJMessageReadFlags = flagDatas[storage];

                TRScript _script = new TRScript();

                _script.tagParam = new TRVariableDictionary();
                _script.tagParam[storage] = new TRVariable(storage);
                _script.instance = _instance;
                _script.resourceType = type;

                scriptInstance[storage] = _script;
            }

            if (run)
                StartCoroutine(TRVirtualMachine.instance.Run(storage));

            yield return _coroutine.Current;
        }

        public IEnumerator LoadAudio(TRVariableDictionary tagParam, TRResourceType type )//int ch, string storage, TRResourceType type = TRResourceLoader.defaultResourceType)
        {
            TRAudioID id = (TRAudioID)tagParam["buf", 0];
            string storage = tagParam["storage", string.Empty];

            var _coroutine = TRResourceLoader.Instance.LoadAudio(storage, type);
            yield return StartCoroutine(_coroutine);

            if(_coroutine.Current != null)
            {
                audioInstance[id].instance.clip = (AudioClip)_coroutine.Current;
                audioInstance[id].tagParam = tagParam;
            }
            yield return _coroutine.Current;
        }

        public IEnumerator LoadImage(TRVariableDictionary tagParam, TRResourceType type)
        {
            TRLayerID id = (TRLayerID)tagParam["layer", 0];
            string storage = tagParam["storage", string.Empty];

            var _coroutine = TRResourceLoader.Instance.LoadTexture(storage, type);
            yield return StartCoroutine(_coroutine);

            if (_coroutine.Current != null)
            {
                layerInstance[id].instance.texture = (Texture2D)_coroutine.Current;
                layerInstance[id].tagParam = tagParam;
            }
 
            yield return _coroutine.Current;
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

            LoadReadFlag();

            UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);

            TRVirtualMachine.instance.functionRegistry.RegisterFunction("intrandom", (Func<VariableCalcurator, VariableCalcurator, VariableCalcurator>)((a, b) => new VariableCalcurator(UnityEngine.Random.Range(a.Int(), b.Int()))));
            TRVirtualMachine.instance.functionRegistry.RegisterFunction("floatrandom", (Func<VariableCalcurator, VariableCalcurator, VariableCalcurator>)((a, b) => new VariableCalcurator(UnityEngine.Random.Range(a.Float(), b.Float()))));
            TRVirtualMachine.instance.functionRegistry.RegisterFunction("random", (Func<VariableCalcurator>)(() => new VariableCalcurator(UnityEngine.Random.value)));

            TRStageEnviroment.instance.Initialize();

            if (TRTitle.instance != null)
            {
                TRTitle.instance.Initialize();
            }
            else if (!string.IsNullOrEmpty(bootScriptName))
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

        public void ResetInstance(int mesWindowID = 0)
        {
            messageWindow = messageWindowList[mesWindowID];
            messageWindow.ClearMessage();

            layerInstance.Reset();
            audioInstance.Reset();
        }

        public void ActivateAllCanvas(bool state)
        {
            layerCanvas.gameObject.GetComponent<CanvasGroup>().alpha = state ? 1.0f : 0.0f;
//            layerCanvas.gameObject.SetActive(state);
            uiCanvas.gameObject.SetActive(state);
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

//        public void Update() { }

        public void OnDestroy()
        {
            if (captureBuffer != null)
                captureBuffer.Release();
            if(movieBuffer != null)
                movieBuffer.Release();
            if (subRenderBuffer[0] != null)
                subRenderBuffer[0].Release();

            SaveReadFlag();
        }
    }
}
