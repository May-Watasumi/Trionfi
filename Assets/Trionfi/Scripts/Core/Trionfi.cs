using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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
        public Canvas targetCanvas;
        [SerializeField]
        public Canvas uiCanvas;
        [SerializeField]
        public TRMessageWindow messageWindow;
        [SerializeField]
        public TRMessageLogWindow messageLogwindow;
        [SerializeField]
        public TRSelectWindow selectWindow;

        static readonly SerializableDictionary<string, int> audioID = new SerializableDictionary<string, int>()
        {
            { "bgm", 0 },
            { "se", 10 },
            { "voice", 20 },
        };

        //SortOrderと等価
        static readonly SerializableDictionary<string, int> layerID = new SerializableDictionary<string, int>()
        {
            { "bg", 0 },
            { "stand", 1 },
            { "event", 99 },
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
            captureBuffer = new RenderTexture(Screen.width, Screen.height, 32);
            rawImage.texture = captureBuffer;

            if (changeLayerOrder)
            {
                layerInstance[0].instance.gameObject.transform.SetAsFirstSibling();
//                referencedObjects.eventLayer.gameObject.transform.SetAsLastSibling();                
            }

            if(targetCamera == null)
                targetCamera = Camera.main;

            if (bootScript != null)
            {
                TRTagInstance _tagInstance = new TRTagInstance();
                _tagInstance.CompileScriptString(bootScript.text);
                TRVirtualMachine.tagInstance["boot"] = _tagInstance;
                StartCoroutine(TRVirtualMachine.Instance.Run("boot"));
            }
        }

        public void Start()
        {
            Init();
        }

        public delegate void OnClickEvent();
        public OnClickEvent ClickEvent;

        public void OnGlobalTapEvent() { }

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

        public void Update()
        {
            if (IsPointerOverGameObject())
            {
                if (messageWindow.state == TRMessageWindow.MessageState.OnWait)
                    messageWindow.state = TRMessageWindow.MessageState.None;
            }
        }

        public void Run(TRTagList tagList)
        {

        }
        //ToDo
#if false

        /// <summary>
        /// 一度走らせたScriptの依存関係を切る用の関数
        /// </summary>
        public void initScene()
		{
			//ToDo:jump系でscene=newさせる意味がないような……
			if(StatusManager.nextFileName != "") {
				//scene new でジャンプしてきた後。variable は引き継がないとだめ。
				string file = StatusManager.nextFileName;
				string target = StatusManager.nextTargetName;

				StatusManager.nextFileName = "";
				StatusManager.nextTargetName = "";

				//この２つを元にその位置へジャンプした上で実行
				string tag_str = "[jump file='" + file + "' target='" + target + "' ]";

				//タグを実行
				AbstractComponent cmp = ScriptDecoder.Instance.TRScriptParser.makeTag(tag_str);
				cmp.start();
			}

			//ToDo_Future:バックログもスクリプト管理
			/*
			GameObject g = Trionfi.StorageManager.loadPrefab("CanvasLog");
			GameObject canvaslog = GameObject.Instantiate(g) as GameObject;
			canvaslog.name = "CanvasLog";
			*/
		}

		public void Init()
		{
			Debug.Log("-Starting Trionfi-");

//			LAppLive2DManager.Instance.ClearScene();

			initScene();

			//自分のシーンのみOnにしておく。prefabのデフォルトはfalse
			if(SystemConfig.Instance.autoBoot && initialScriptFile)
			{
//                doScript(initialScriptFile.text);
			}
			//			Trionfi.StatusManager.currentState = JokerState.EmptyOrder;
		}
        /*
                void Awake()
                {
                    createObject();
                }
        */
        void Update()
		{
			if(!ScriptDecoder.Instance.hasComponent && StatusManager.currentState != JokerState.EmptyOrder)
			{
				//オードモードの設定時間が過ぎた
				if(StatusManager.currentState == JokerState.PageWait && StatusManager.onAuto && !StatusManager.onAutoWait(Time.deltaTime))
				{
					uiInstance.Clear();
					LogManager.ApplyLog();
					StatusManager.NextOrder();
				}
				else if (!Trionfi.Instance.uiInstance.onMessage
					&& ( StatusManager.currentState == JokerState.MessageShowAll
						|| StatusManager.currentState == JokerState.MessageShow))
				{
					StartCoroutine("decodeScenario");
				}
				else if(StatusManager.currentState == JokerState.NextOrder
					|| StatusManager.currentState == JokerState.SkipOrder
					|| StatusManager.onSkip)
					//|| StatusManager.onAuto)
				{
					if(StatusManager.currentState == JokerState.PageWait)
					{
						LogManager.ApplyLog();
					}

					StartCoroutine("decodeScenario");
				}
			}
		}

		private IEnumerator decodeScenario()
		{
			ScriptDecoder.Instance.decodeScenario();
			yield return null;
		}

		/// <summary>
		/// Scenarioの再生
		/// </summary>
		/// <param name="file">File.</param>
		public void doScript(string file)
		{
			StartCoroutine(CallScenario(file));
		}

		/// <summary>
		/// StorageManagerから名前を保管してScenarioの再生
		/// </summary>
		/// <param name="file">File.</param>
		public void doScriptFromShortName(string file)
		{
			StartCoroutine(CallScenario(StorageManager.PATH_SD_SCENARIO + file));
		}

		private IEnumerator CallScenario(string file)
		{
			ScriptDecoder.Instance.loadScenario(file, false);
			ScriptDecoder.Instance.decodeScenario();
			yield return null;
		}
		
//	void OnGUI(){ }

		//アプリ終了前
		void OnApplicationQuit() { }
		//gameManager.saveManager.saveFromSnap("autosave");

		private void OnDestroy()
		{
			if (uiInstance != null)
			{
				uiInstance.Release();
				uiInstance = null;
			}

			if(ImageManager != null)
			{
				ImageManager.destroyScene();
			}

			LAppLive2DManager.Instance.ClearScene();
		}
#endif
    }
    };
