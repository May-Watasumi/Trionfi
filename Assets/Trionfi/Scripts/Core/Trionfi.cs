using UnityEngine;
using UnityEngine.UI;
using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

namespace Trionfi
{
    //	[System.Serializable]
    public enum TRStorageType
    {
        LocalResources,
        LocalFile,
        URL,
        AssetBundle,
        Terminate
    };

    public class TRMediaInstance<T>
    {
        [SerializeField]
        public TRStorageType type;
        public T instance;
    }

    [System.Serializable]
    public class TRAudio : TRMediaInstance<AudioSource> { }
    [System.Serializable]
    public class TRLLayer : TRMediaInstance<Image> { }

    [ExecuteInEditMode]
    public class Trionfi : SingletonMonoBehaviour<Trionfi>
    {
        public static readonly string assetPath = "Assets/Trionfi/";

        [SerializeField]
        TextAsset bootScript;
        [SerializeField]
        public Camera targetCamera;
        [SerializeField]
        public Canvas targetCanvas;
        [SerializeField]
        public Canvas uiCanvas;

        [SerializeField]
        public UnityEngine.Video.VideoPlayer videoPlayer;

        [SerializeField]
        List<TRAudio> bgmInstance = new List<TRAudio>();
        [SerializeField]
        List<TRAudio> seInstance = new List<TRAudio>();
        [SerializeField]
        List<TRAudio> voiceInstance = new List<TRAudio>();
        [SerializeField]
        List<TRLLayer> layerInstance = new List<TRLLayer>();

        enum TRAudioType
        {
            BGM = 0,
            VOICE = 1,
            SE = 2,
        }

        enum LayerOrder
        {
            UI = 100,
            EVENT = 99,
            VIDEO = 99,
            STAND = 1,
            BG = 0,
        }

        enum StandOrder
        {
             CENTER = 0,
             LEFT = 1,
             RIGHT = 2
        }

        public TRTagInstance currentTagInstance = null;
        public Dictionary<string, TRTagInstance> tagInstance = new Dictionary<string, TRTagInstance>();

        public void Init(bool changeLayerOrder = false)
        {
            if(changeLayerOrder)
            {
                layerInstance[0].instance.gameObject.transform.SetAsFirstSibling();
//                referencedObjects.eventLayer.gameObject.transform.SetAsLastSibling();                
            }

            if(targetCamera == null)
                targetCamera = Camera.main;

            if (bootScript != null)
            {
                currentTagInstance = new TRTagInstance();
                if(currentTagInstance.CompileScriptFile(bootScript.name));
                StartCoroutine(currentTagInstance.Run());
            }
        }

        public void Start()
        {
            Init();
        }

        public delegate void OnClickEvent();
        public OnClickEvent ClickEvent;

        public void OnGlobalTapEvent() { }

        public AudioSource GetAudio(TRAssetType type, int ch = 0)
        {
            switch (type)
            {
                case TRAssetType.BGM:
                    return bgmInstance[ch].instance;
                case TRAssetType.SE:
                    return seInstance[ch].instance;
                case TRAssetType.Voice:
                    return  voiceInstance[ch].instance;
            }

            return null;
        }

        public Image GetLayer(TRAssetType type, int ch = 0)
        {
            switch (type)
            {
                case TRAssetType.BG:
                    return layerInstance[0].instance; 
                case TRAssetType.Character:
                    return layerInstance[ch+1].instance;
                case TRAssetType.Event:
                    return layerInstance[layerInstance.Count - 1].instance;
            }

            return null;
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
