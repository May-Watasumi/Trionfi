using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NovelEx
{
    [ExecuteInEditMode]
    public class Trionfi : SingletonMonoBehaviour<Trionfi>
	{
        enum AudioChannel
        {
            BGM = 1,
            VOICE = 2,
            SE = 3,
        }

        [Serializable]
        public class ReferencedObject
        {
            public Camera targetCamera;
            public Canvas targetCanvas;

            public AudioClip[] audioBGM = new AudioClip[2];
            public AudioClip[] audioSE = new AudioClip[2];
            public AudioClip[] audioVoice = new AudioClip[2];          
        }

        [SerializeField]
        ReferencedObject referencedObjects = new ReferencedObject();

        ScriptDecoder scriptDecoder;
        Serializer serializer;

        //ToDo:UserConfigはprefsへ。命令系統を変える
		//文字列から即時タグを実行することができます。
		public IEnumerator StartTag(string tag)
        {
			AbstractComponent cmp = TRScriptParser.Instance.makeTag(tag);
			yield return StartCoroutine(cmp.Exec());
		}

        public void Init()
        {
            scriptDecoder = new ScriptDecoder();
            serializer = new Serializer();
            //            TRUIManager.Instance.Init();
            //TRUIManager.Instance.currentMessageWindow.
            TRUIManager.Instance.currentSelectWindow.Init(10);
        }

        public void Start()
        {
            Init();

            if(TRSystemConfig.Instance.initialScriptFile != null)
            {
                scriptDecoder.LoadScenariofromString(TRSystemConfig.Instance.initialScriptFile.text, TRSystemConfig.Instance.initialScriptFile.name);
                StartCoroutine(scriptDecoder.Run());
            }                
        }

        public delegate void OnClickEvent();
        public OnClickEvent ClickEvent;

        public void OnGlobalTapEvent()
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
			GameObject g = JOKEREX.StorageManager.loadPrefab("CanvasLog");
			GameObject canvaslog = GameObject.Instantiate(g) as GameObject;
			canvaslog.name = "CanvasLog";
			*/
		}

		public void Init()
		{
			Debug.Log("-Starting JOKEREX-");

//			LAppLive2DManager.Instance.ClearScene();

			initScene();

			//自分のシーンのみOnにしておく。prefabのデフォルトはfalse
			if(SystemConfig.Instance.autoBoot && initialScriptFile)
			{
//                doScript(initialScriptFile.text);
			}
			//			JOKEREX.StatusManager.currentState = JokerState.EmptyOrder;
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
				else if (!JOKEREX.Instance.uiInstance.onMessage
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
