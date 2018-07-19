using UnityEngine;
using UnityEngine.UI;
using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

namespace Trionfi
{
    [ExecuteInEditMode]
    public class Trionfi : SingletonMonoBehaviour<Trionfi>
	{
        enum AudioChannel
        {
            BGM = 0,
            VOICE = 1,
            SE = 2,
        }

        enum LayerOrder
        {
            UI = -100,
            EVENT = -1,
            STAND = 0,
            BG = 1,
        }

        enum StandOrder
        {
             CENTER = 0,
             LEFT = 1,
             RIGHT = 2
        }

        [Serializable]
        public class ReferencedObject
        {
            public Camera targetCamera;
            public Canvas targetCanvas;

            public AudioSource[] audioBGM = new AudioSource[2];
            public AudioSource[] audioSE = new AudioSource[2];
            public AudioSource[] audioVoice = new AudioSource[2];            public Image[] standLayer = new Image[5];
            public Image eventLayer;
            public Image bgLayer;
            public Canvas uiCanvas;
            public UnityEngine.Video.VideoPlayer videoPlayer;
        }

        [SerializeField]
        TextAsset bootScript;

        [SerializeField]
        ReferencedObject referencedObjects = new ReferencedObject();

        public TRTagInstance currentTagInstance = null;

        public Dictionary<string, TRTagInstance> tagInstance = new Dictionary<string, TRTagInstance>();
        /*
        //単体タグ実行。他のタグから呼び出すことは禁止
        [Conditional("UNITY_EDITOR"), Conditional("TRIONFI_DEBUG"), Conditional("DEVELOPMENT_BUILD")]
        public void StartTag(string tag)
        {
			AbstractComponent cmp = TRScriptParser.Instance.MakeTag(tag);
            cmp.Execute();
            StartCoroutine(cmp.TagAsyncWait());
		}
        */

        public void Init(bool changeLayerOrder = false)
        {
            if (changeLayerOrder)
            {
                referencedObjects.bgLayer.gameObject.transform.SetAsFirstSibling();
                referencedObjects.eventLayer.gameObject.transform.SetAsLastSibling();                
            }

            if (referencedObjects.targetCamera == null)
                referencedObjects.targetCamera = Camera.main;

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

        public void OnGlobalTapEvent()
        {
        }

        public AudioSource GetAudio(TRAssetType type, int ch = 0)
        {
            switch (type)
            {
                case TRAssetType.BGM:
                    return referencedObjects.audioBGM[ch];
                case TRAssetType.SE:
                    return referencedObjects.audioSE[ch];
                case TRAssetType.Voice:
                    return referencedObjects.audioVoice[ch];
            }

            return null;
        }

        public Image GetLayer(TRAssetType type, int ch = 0)
        {
            switch (type)
            {
                case TRAssetType.BG:
                    return referencedObjects.bgLayer;
                case TRAssetType.Character:
                    return referencedObjects.standLayer[ch];
                case TRAssetType.Event:
                    return referencedObjects.eventLayer;
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
