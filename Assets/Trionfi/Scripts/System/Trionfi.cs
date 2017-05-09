using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NovelEx
{
    [ExecuteInEditMode]
    public class Trinofi : MonoBehaviour
	{
		public static string characterName;

        public Camera _targetCamera = null;
		public Camera targetCamera
		{
			get
			{
				return _targetCamera;
			}
			set
			{
				_targetCamera = value;
			}
		}

		private static Trinofi jokerEx;
        public static NovelParser Parser;

		/// <summary>
		/// The GUI interface creator.
		/// defaultのインターフェイスを作成する
		/// </summary>

		//シナリオ終端で呼ばれる
		public static void terminateScenario() { }

		//実行すべき命令がないか、チェックする
		public void check() { }

		//moved from SaveDataManager(UserDataManager)
		public string getConfig(string key)
		{
			return scenarioManager.variable.get("config." + key);
		}

		//moved from SaveDataManager(UserDataManager)
		public void setConfig(string key, string val)
		{
			scenarioManager.variable.set("config." + key, val);
		}

		//文字列から即時タグを実行することができます。
		public static void startTag(string tag) {
			AbstractComponent cmp = Instance.ScenarioManager.NovelParser.makeTag(tag);
			cmp.start();
		}

		//Awake
		public void createObject()
		{
			jokerEx = this;

			systemConfig = this.GetComponent<SystemConfig>();
//			selectorManager = this.GetComponent<SelectorManager>();
			logManager = this.GetComponent<LogManager>();
			statusManager = this.GetComponent<StatusManager>();
			storageManager = this.GetComponent<StorageManager>();

			scenarioManager = new ScenarioManager();
			imageManager = new ImageManager();
			audioManager = new AudioManager();
			eventManager = new EventManager();

			if(systemConfig.useSerializer)
				serializer = new Serializer();
		}

		//public void Reset()
		/*
				public static void clearSingleton() {
					_selectorManager = null;
					systemConfig = null;
					imageManager = null;
					scenarioManager = null;
					statusManager = null;
					audioManager = null;
					eventManager = null;
					userdataManager = null;

		//			logManager = null;
				}

 		public void destroyObject()
		{
	
		}

		*/

		/// <summary>
		/// 一度走らせたScriptの依存関係を切る用の関数
		/// </summary>
		public void initScene()
		{
			//すべてクリアする。
//			clearSingleton();
			ImageManager.initScene();
			StatusManager.initScene();

			//SystemConfig
			GetComponent<NovelConfig>().initConfig();

			//グローバルコンフィグ読み込み
			//ToDo:
			//Serializer.LoadGlobalObject();

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
				AbstractComponent cmp = ScenarioManager.NovelParser.makeTag(tag_str);
				cmp.start();
			}

			//ToDo_Future:バックログもスクリプト管理
			/*
			GameObject g = JOKEREX.StorageManager.loadPrefab("CanvasLog");
			GameObject canvaslog = GameObject.Instantiate(g) as GameObject;
			canvaslog.name = "CanvasLog";
			*/

			targetCamera = Camera.main != null ? Camera.main : Camera.allCameras[0];
		}

		/// <summary>
		/// 初期化関数GUIのインターフェイスを渡す
		/// </summary>
		/// <param name="guiInterface">GUI interface.</param>
		public void Init()
		{
			Debug.Log("-Starting JOKEREX-");

//			LAppLive2DManager.Instance.ClearScene();

			initScene();

			//自分のシーンのみOnにしておく。prefabのデフォルトはfalse
			if (SystemConfig.autoBoot)
			{
				string storage = getConfig("first_scenario");

				storage =  StorageManager.PATH_SD_SCENARIO + storage;

				doScript(storage);
			}
			//			JOKEREX.StatusManager.currentState = JokerState.EmptyOrder;
		}

		void Awake()
		{
			createObject();
		}

		void Update()
		{
			if(!ScenarioManager.hasComponent && StatusManager.currentState != JokerState.EmptyOrder)
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
			ScenarioManager.decodeScenario();
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
			ScenarioManager.loadScenario(file, false);
			ScenarioManager.decodeScenario();
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
	}
};
