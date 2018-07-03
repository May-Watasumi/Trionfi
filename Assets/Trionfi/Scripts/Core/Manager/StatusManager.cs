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
            get
            {
                return (currentMessageState == TRMessageState.SkipRun) || (currentMessageState == TRMessageState.SkipStop);
            }
        }

        //	オート中かどうかを判定する
        public bool onAuto
        {
            get
            {
                return autoWaitTime > 0.0f;
                //				return currentMessageState == JokerMessageState.Auto;
            }
        }

        public bool UIClicked = false; //UIボタン系などが先に押されたので、同時に起こったscene_iniのその後のクリックは無視して欲しい
        public bool enableEventClick = true; //イベントをクリック検知できるかどうか event_stopの場合は下のやつを使う

        public bool isEventStop = false; //イベント停止中かどうかを保存
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
            this.currentPlayBgm = "";
            this.UIClicked = false;
            this.currentScenario = currentScenario;
            this.currentState = TRState.EmptyOrder;
        }

        public void setSkipOrder()
        {
            currentState = TRState.SkipOrder;
            //			skipOrder = true;
            //			enableNextOrder = false;
        }

        public void releaseSkipOrder()
        {
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
            if (autoWaitCounter < 0.0f)
                autoWaitCounter = 0.0f;
            else
                autoWaitCounter += delta;

            return autoWaitCounter < autoWaitTime;
        }
    }
}
