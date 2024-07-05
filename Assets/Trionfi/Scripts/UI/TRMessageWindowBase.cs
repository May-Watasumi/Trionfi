using System;
using System.Text.RegularExpressions;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using TRTask = Cysharp.Threading.Tasks.UniTask;
using TRTaskString = Cysharp.Threading.Tasks.UniTask<string>;

namespace Trionfi
{
    public abstract class TRMessageWindowBase : MonoBehaviour
    {
        public abstract string currentText { get; set; }
        public enum MessageState { None, OnShow, /*OnSkip, OnAuto,*/ OnWait, OnClose }
        public enum WaitIcon { None, Alpha, Rotate }

        public bool forceSkip = false;

        public bool onSkip = false;
        public bool onAuto = false;

        public bool enableSkip { get { return forceSkip || onSkip; } }

        public string currentSpeaker = string.Empty;

        public MessageState state = MessageState.None;

        public float speedRatio = 1.0f;

        public Tweener tweener = null;
/*
        [SerializeField]
        public bool useUguiText = false;

        [SerializeField]
        public LetterWriter.Unity.Components.LetterWriterText currentMessage;

        [SerializeField]
        public Text currentUguiMessage;

        [SerializeField]
        public Text currentName;
        //    [SerializeField]
        //    private Image MessageFrameImage;
*/
        [SerializeField]
        public Image faceIcon;

        [SerializeField]
        public Image waitCursor;

        public GameObject systemWindow;

//        protected TRMessageLogWindowBase logWindow;

        public string nameString = string.Empty;

        public virtual void Start()
        {
//            if (currentUguiMessage != null)
//                currentUguiMessage.fontSize = TRSystemConfig.Instance.fontSize;

//            if (currentMessage != null)
//            {
//                currentMessage.fontSize = TRSystemConfig.Instance.fontSize;
//                currentMessage.color = TRSystemConfig.Instance.fontColor;
//            }

//            logWindow = Trionfi.Instance.messageLogwindow;

            Trionfi.Instance.ClickEvent += OnClick;
        }

        public void Reset()
        {
            //ToDo
        }

        private void OnClick()
        {
            if (!gameObject.activeSelf)
                return;
            else if (onSkip)
                onSkip = false;
            else if (onAuto)
                onAuto = false;
            else if (state == MessageState.OnShow)
                state = MessageState.OnWait;
            else if (state == MessageState.OnWait)
                state = MessageState.None;
        }

        public bool isSuspend = false;

        public void Pause()
        {
            if (state == MessageState.OnShow)
                isSuspend = true;
        }

        public void Restart()
        {
            if (state == MessageState.OnShow)
                isSuspend = false;
        }

        public virtual void ClearMessage()
        {
//            if (currentUguiMessage != null)
//                currentUguiMessage.text = string.Empty;
//            if (currentMessage != null)
//                currentMessage.text = string.Empty;
//            if (currentName != null)
//                currentName.text = string.Empty;
            if (faceIcon != null)
                faceIcon.enabled = false;

            nameString = string.Empty;
            currentSpeaker = string.Empty;

            speedRatio = 1.0f;
        }

        public void OpenWindow()
		{
            gameObject.SetActive(true);

            if (systemWindow != null)
                systemWindow.SetActive(true);
		}

        public void CloseWindow()
        {
            gameObject.SetActive(false);

            if (systemWindow != null)
                systemWindow.SetActive(false);
        }
        
        public static string MatchEvaluatorFunc(Match m)
        {
            // Match m にはパターンにマッチした結果が来るので、
            // その中のデータを取り出して、自由に加工できる。
            // 関数の戻り値の文字列が、Replaceの置換結果となる。

            // m.Groups[0].Valueはﾏｯﾁした全体の文字列
            var Exp = m.Groups[1].Value;

            string result = TRVirtualMachine.Instance.vstack.GetInstance(Exp).Literal();

            return result;
        }
        public void ShowMessage(string text, float mesCurrentWait = 0)
        {
            Trionfi.Instance.SetStandLayerTone();

//            string emb = "<emb exp=\"(.*)\">";
//            var regex = new Regex(emb);

            string emb2 = "#(.*)#";
            var regex2 = new Regex(emb2);

//            text = /*_subText*/ regex.Replace(text, MatchEvaluatorFunc);
            text = /*_subText*/ regex2.Replace(text, MatchEvaluatorFunc);

            state = MessageState.OnShow;
            ShowMessageSub(text, mesCurrentWait).Forget();
        }

        protected virtual TRTask ShowMessageSub(string message, float mesCurrentWait)
        {
            return TRTask.CompletedTask;
        }

        protected Tweener _sequence = null;

        // autoWaitの入力がない場合はTRGameConfigの数値を使う
        // これのためautoWaitの基本値を1.0fから-1.0fのマイナス数値に修正
        public async TRTask Wait(WaitIcon icon = WaitIcon.Alpha, float autoWait = -1.0f)
        {
            if (tweener != null)
                tweener.Kill();

            if (autoWait < 0.0f)
                autoWait = TRGameConfig.configData.autotextWait;

            if (!enableSkip)
            {
                state = MessageState.OnWait;

                WaitCursor(icon);

                // オート待機中オートを解除した場合の処理追加
                if (onAuto)
                    await UniTask.Delay((int)(autoWait*1000.0f));
//                    yield return new WaitForSeconds(autoWait);
                else
                    await UniTask.WaitWhile(() => state == MessageState.OnWait && !enableSkip);

                {
                    // yield return new WaitForSeconds(autoWait);

                    while (autoWait > 0.0f && onAuto)
                    {
                        if (TRVirtualMachine.Instance.state != TRVirtualMachine.State.Run)
                            return;

                        // テキスト表示、ボイス再生共に終わった後からautoWaitの減算実施
                        if (!Trionfi.Instance.audioInstance[TRAudioID.VOICE1].instance.isPlaying)
                            autoWait -= Time.deltaTime;
                    }
                }

                // else
                //    yield return new WaitWhile(() => state == MessageState.OnWait && !enableSkip);

                // オートじゃない時と、オート待機途中オートを解除した時両方ユーザー入力を待つ必要があるので、条件修正
                if (!onAuto)
                    await UniTask.WaitWhile(() => state == MessageState.OnWait && !enableSkip && !onAuto);
            }
            else
                state = MessageState.None;
        }

        public void WaitCursor(WaitIcon icon)
        {
            WaitCusorSub(icon).Forget();
        }

        protected async TRTask WaitCusorSub(WaitIcon icon, float loopTime = 1.2f)
        {
            waitCursor.color = new Color(waitCursor.color.r, waitCursor.color.g, waitCursor.color.b, 1.0f);

            waitCursor.gameObject.SetActive(true);

            switch (icon)
            {
                case WaitIcon.Alpha:

                    waitCursor.gameObject.SetActive(true);

                    _sequence = DOTween.ToAlpha(
                    () => waitCursor.color,
                    color => waitCursor.color = color,
                    0.0f,                                // 最終的なalpha値
                    loopTime
                    )
                    .SetLoops(-1, LoopType.Yoyo);
                    break;
                case WaitIcon.Rotate:
                    //                waitCursor.GetComponent<RectTransform>().rotation = Vector3.zero;
                    _sequence = waitCursor.GetComponent<RectTransform>().DORotate(new Vector3(0, 0, 359), 1.0f).SetRelative(true).SetLoops(-1);
                    break;
            }

            await UniTask.WaitWhile(() => state == MessageState.OnWait && !enableSkip);

            _sequence.Kill();
            _sequence = null;

            waitCursor.gameObject.SetActive(false);
        }

        public void ShowName(string _name, Sprite face = null)
        {
            nameString = _name;
        }

        public void ResetMessageMode()
        {
            Trionfi.Instance.ClickEvent -= ResetMessageMode;
            onAuto = false;
            onSkip = false;
        }

        // LCTRLでスキップ中画面移動などが発生した時、スキップを無効化しても次のセリフはスキップされる問題に対応
         private void OnEnable()
        {
            forceSkip = false;
        }

        public void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl))
                forceSkip = true;
            else
                forceSkip = false;

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                OnClick();
        }
    }
}
