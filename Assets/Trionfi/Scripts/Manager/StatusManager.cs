using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;

namespace NovelEx {
	public enum TRState
    {
		InfiniteStop,
		MessageShow,
		MessageShowAll,
		MessageHide,
		PageWait,
		WaitClick,
		Wait,
//		Return,
//		OnSkip,
//		OnAuto,
		SkipOrder,
		NextOrder,
		EmptyOrder
	};

	public enum TRMessageState {
		Normal,
		SkipRun,
		SkipStop,
//		Auto,
	};

	public class StatusManager : SingletonMonoBehaviour<StatusManager>
	{
		public TRState currentState
		{
			get; private set;
		}

		public TRMessageState currentMessageState = TRMessageState.Normal;

		//これが真の場合は命令を無視する
		public bool skipOrder = false;

		//次に読み込むべきファイルがある場合。セーブデータスロット名が入る
//		public string nextLoad = ""; 
		public string nextFileName = ""; 
		public string nextTargetName = "";

		//ここに値が入っているあいだtag に自動的に値がはいる
		public string TagDefaultVal = ""; 

		public event Action onEndScenario;

		public float autoWaitTime = -1.0f;
		public float autoWaitCounter = -1.0f;

		// 現在スキップ中かどうかを判定する
		public bool onSkip
        {
			get {
				return (currentMessageState == TRMessageState.SkipRun) || (currentMessageState == TRMessageState.SkipStop);
			 }
		}

		//	オート中かどうかを判定する
		public bool onAuto
        {
			get {
				return autoWaitTime > 0.0f;
//				return currentMessageState == JokerMessageState.Auto;
			}
		}

		public bool UIClicked = false; //UIボタン系などが先に押されたので、同時に起こったscene_iniのその後のクリックは無視して欲しい
		public bool enableEventClick = true; //イベントをクリック検知できるかどうか event_stopの場合は下のやつを使う

		public bool isEventStop=false; //イベント停止中かどうかを保存
		public bool isEventButtonStop = false; //uGUIのボタンイベントが先に反応している場合

		public string currentPlayBgm = "";  //現在再生中のBGMがある場合
		public string currentMessage = ""; //現在表示中の文字列
		public int currentScenarioPosition = -1;

		public string currentScenario
		{
			get; set;
		}

		//SceneInit
		public string messageForSaveTitle = ""; //セーブのタイトル用に保持する文字列.

		public bool IsRunning
		{
			get
			{
				return this.currentState != TRState.EmptyOrder;
			}
		}

		//シーンが切り替わったタイミングでクリアする内容
		public void initScene()
		{
			this.currentPlayBgm  = "";
			this.UIClicked       = false;
			this.currentScenario = currentScenario;
			this.currentState    = TRState.EmptyOrder;
		}

		public void setSkipOrder()
        {
            currentState = TRState.SkipOrder;
//			skipOrder = true;
//			enableNextOrder = false;
		}

		public void releaseSkipOrder(){
//
			currentState = TRState.NextOrder;
//			skipOrder = false;
//			enableNextOrder = true;
		}

		public void EndScenario()
		{
//			JOKEREX.Instance.uiInstance.Release();
//			JOKEREX.Instance.uiInstance = null;
			this.currentState = TRState.EmptyOrder;

            if (onEndScenario != null)
			{
				onEndScenario();
			}

            Debug.Log("[NovelEx.StatusManagerEx]EndScenario");
		}

		public void NextOrder()
		{
			currentState = TRState.NextOrder;
		}

		public void Wait()
		{
			currentState = TRState.Wait;
		}

		public void MessageShow()
		{
			currentState = TRState.MessageShow;
		}

		public void WaitClick()
		{
			currentState = TRState.WaitClick;
		}

		public void PageWait()
		{
			autoWaitCounter = -1.0f;
			currentState = TRState.PageWait;
		}

		public void InfiniteStop()
		{
			currentState = TRState.InfiniteStop;
		}

		public void MessageHide()
		{
			currentState = TRState.MessageHide;
		}

		public void StartAuto(float time = -1.0f)
		{
			autoWaitTime = time <= 0.5f ? 0.5f : time;
			autoWaitCounter = -1.0f;

/*
			if(onSkip)
			{
				MessageShow();
			}
 */
		}

		public void StopAuto()
        {
			autoWaitTime = -1.0f;
			autoWaitCounter = -1.0f;
/*
			if (onAuto)
			{
				MessageShow();
			}
*/
		}

		public bool onAutoWait(float delta)
        {
			if(autoWaitCounter < 0.0f)
				autoWaitCounter = 0.0f;
			else
				autoWaitCounter += delta;
			
			return autoWaitCounter < autoWaitTime;
		}
/*
		public void Update() {
			if(onAuto) {
				autoWaitCounter += Time.deltaTime;

				if(autoWaitCounter > autoWaitTime)
				{
					JOKEREX.Instance.uiInstance.Clear();
					JOKEREX.Instance.LogManager.ApplyLog();
					currentState = JokerState.NextOrder;
				}				
			}
		}
*/
		public void coroutineAnimation(Animation a, CompleteDelegate completeDeletgate)
        {
			object[] parameters = new object[2] { a, completeDeletgate };
//			enableNextOrder = false;
			StartCoroutine("animationWait", parameters);
		}

		private IEnumerator animationWait(object[] param)
        {
			Animation a = (Animation)param[0];
			CompleteDelegate completeDeletgate = (CompleteDelegate)param[1];

			//アニメーションの終了を待つ
			while(a.isPlaying) {
				// childのisComplete変数がtrueになるまで待機
				yield return new WaitForEndOfFrame();
			}
			completeDeletgate();
		}
/*
		//クリックされて次の命令に行くかをチェックする
		public void clickNextOrder() {

			//window非表示状態からの復帰の場合
			if (NovelSingletonEx.StatusManager.nextClickShowMessage == true)
			{
				NovelSingletonEx.StatusManager.nextClickShowMessage = false;
				//ToDo:
				//			NovelSingleton.GameView.showMessageWithoutNextOrder(0.5f); //nextorder はしない。
				return;
			}

			if (NovelSingletonEx.StatusManager.enableClickOrder == true && NovelSingletonEx.StatusManager.enableNextOrder == true)
			{
				NovelSingletonEx.ScenarioManager.nextOrder();
			}
		}
*/
		//スキップをスタートさせる
		public void startSkip() {
			//＠井筒修正：フラグが立ってるときは無視するように。
//			if (FlagSkiiping == false)
			if (!onSkip) {
				currentState = TRState.MessageShowAll;
				currentMessageState = TRMessageState.SkipRun;		
//				StartCoroutine("Loop", 0.01f);
			}
		}

		//文字速度とかも変更しないと
		public void stopSkip() {
			//＠井筒修正：フラグが立ってるときは無視するように。
			if(onSkip) {
				currentMessageState = TRMessageState.Normal;		
				currentState = TRState.MessageShowAll;
				//ToDo;
				//			this.MessageSpeed = float.Parse(this.gameManager.getConfig("messageSpeed"));
			}
		}

//ToDo:
/*
		//オートを開始させる
		public void startAuto(float time) {
//			FlagAuto = true;
			StartCoroutine("Loop", time);
		}

		//オートが停止される
		public void stopAuto() {
//			FlagAuto = false;
			StartCoroutine("Loop",3.0f);
		}

		public IEnumerator Loop(float time) {
			while (true) {
				yield return new WaitForSeconds(time);
//				clickNextOrder();
				if (JOKEREX.Instance.StatusManager.onSkip || JOKEREX.Instance.StatusManager.onAuto)
					break;
			}
			StopCoroutine("Loop");
		}
*/
		public void wait(float time) {
			//処理を止める
			StartCoroutine("startWait", time);
		}

		//一定時間処理を停止するためのコルーチン
		private IEnumerator startWait(float time) {
			yield return new WaitForSeconds(time);
			/*EmptyOrder
			NovelSingletonEx.StatusManager.enableEventClick = true;
			NovelSingletonEx.StatusManager.enableClickOrder = true;
			NovelSingletonEx.StatusManager.enableNextOrder = true;

			NovelSingletonEx.ScenarioManager.nextOrder();
*/
//			yield return null;
		}

		// Update is called once per frame
		public void ClickButton()
		{
			StartCoroutine(ClickButtonCoroutine());
		}

		private IEnumerator ClickButtonCoroutine() {
			yield return new WaitForSeconds(0.01f);

			Vector3 aTapPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Collider2D aCollider2d = Physics2D.OverlapPoint(aTapPoint);

			//Debug.Log ("====EVENT");
			//Debug.Log (aCollider2d);
//
			if(UIClicked) {
//				NovelSingletonEx.StatusManager.UIClicked = false;
				yield return new WaitForSeconds(0.01f);
			}

			switch (currentMessageState) {
			case TRMessageState.SkipRun:
				currentMessageState	= TRMessageState.SkipStop;
				yield break;
//ToDo:Autoはスキップより優先？
//			case JokerMessageState.Auto:
//				currentMessageState = JokerMessageState.Normal;
//				yield break;
			}

			switch(currentState){
			case TRState.MessageShow:
			case TRState.MessageShowAll:
//				if (NovelSingletonEx.StatusManager.onSkip || NovelSingletonEx.StatusManager.onAuto) {
//					NovelSingletonEx.StatusManager.onSkip = false;
//					NovelSingletonEx.StatusManager.onAuto = false;
//				}
				currentState = TRState.MessageShowAll;
				yield break;
			case TRState.PageWait:
                    //ToDo:
//                uiInstance.Clear();
//				LogManager.ApplyLog();
				currentState = TRState.NextOrder;
				break;
//			case JokerState.OnAuto:
//			case JokerState.OnSkip:
//				currentState = JokerState.MessageShow;

//				if( currentState == JokerState.OnSkip) {
/*	
					//＠井筒追加：
					//イベントの名前はクリックされたオブジェクトの名前
					string name = NovelSingletonEx.UserDataManager.variable.get("evt.caller_name");
					//＠井筒修正。かなりその場つなぎ感があるのでα２以降の大改造物語でリファクタリングします。
					string act = NovelSingletonEx.UserDataManager.variable.get("evt.call_action");
					if (NovelSingletonEx.EventManager.dicEvent.ContainsKey(name) && NovelSingletonEx.EventManager.dicEvent[name].act == act)
						NovelSingletonEx.EventManager.dicEvent[name].onCalling = false;

//				}
//				yield break;
*/
			case TRState.WaitClick:
				currentState = TRState.NextOrder;
				yield break;
			case TRState.Wait:
				yield break;
			}

			if(isEventButtonStop == true)
            {
				isEventButtonStop = false;
				yield break;
			}
            
            if (isEventStop == false && aCollider2d) {
				GameObject obj = aCollider2d.transform.gameObject;
                //ToDo:
                //				EventManager. checkEvent(obj.name, "click");
            }
            /*
                        else
                        {
                            if (NovelSingletonEx.StatusManager.inUiClick == true) {
                                NovelSingletonEx.StatusManager.inUiClick = false;
                                yield break;
                            }

                            //skip中にクリックされた場合、Skipを止める
                            if (NovelSingletonEx.StatusManager.FlagSkiiping == true) {
                                NovelSingletonEx.StatusManager.FlagSkiiping = false;
                                //＠井筒追加：
                                //イベントの名前はクリックされたオブジェクトの名前
                                string name = NovelSingletonEx.UserDataManager.variable.get("evt.caller_name");
                                //＠井筒修正。かなりその場つなぎ感があるのでα２以降の大改造物語でリファクタリングします。
                                string act = NovelSingletonEx.UserDataManager.variable.get("evt.call_action");
                                if (NovelSingletonEx.EventManager.dicEvent.ContainsKey(name) && NovelSingletonEx.EventManager.dicEvent[name].act == act)
                                    NovelSingletonEx.EventManager.dicEvent[name].onCalling = false;
            EmptyOrder
                                yield break;
                            }

                            //ステータスマネージャみたいなの持たせてもいいよね
                            if (NovelSingletonEx.StatusManager.isMessageShowing == true) {
                                NovelSingletonEx.StatusManager.isMessageShowing = false;
                                //速度を上げる
                                //ToDo:
                                //gameManager.scene.MessageSpeed = 0.001f;
                                yield break;
                            }

                            //Auto中にクリックされた場合、Autoを止める
                            if (NovelSingletonEx.StatusManager.FlagAuto == true)
                            {
                                NovelSingletonEx.StatusManager.FlagAuto = false;
                                yield break;
                            }
                            if (NovelSingletonEx.StatusManager.enableClickOrder == true) {
                                clickNextOrder();
                            }
                        }
            */
        }
	}
}
