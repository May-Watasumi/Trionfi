using UnityEngine;
using System.Collections;
using Trionfi;
using DG.Tweening;
using UnityEngine.UI;

namespace Trionfi
{

    public class TRMessageWindow : SingletonMonoBehaviour<TRMessageWindow>
    {
        public bool enableLogWindow = true;

        public bool forceSkip = false;

        public bool _onSkip = false;
        public bool _onAuto = false;

        public bool onSkip
        {
            get { return _onSkip; }
            set{
                _onSkip = value;

                if (_onSkip && state == MessageState.OnShow)
                {
                    state = MessageState.None;

//                    if (!onAuto)
//                        Trionfi.Instance.ClickEvent += ResetMessageMode;
                }
            }
        }

        public bool enableSkip
        { get { return forceSkip || onSkip; } }

        public bool onAuto
        {
            get { return _onAuto; }
            set
            {
                _onAuto = value;

                if (_onAuto)
                {
//                    state = MessageState.None;

//                    if (!onSkip)
//                        Trionfi.Instance.ClickEvent += ResetMessageMode;
                }
            }
        }

        public enum MessageState { None, OnShow, /*OnSkip, OnAuto,*/ OnWait }
        public enum WaitIcon { None, Alpha, Rotate }

        public MessageState state = MessageState.None;

        [SerializeField]
        public LetterWriter.Unity.Components.LetterWriterText currentMessage;
        [SerializeField]
        public Text currentName;
        //    [SerializeField]
        //    private Image MessageFrameImage;
        [SerializeField]
        public Image waitCursor;

        public string nameString = "";

        public void Start()
        {
            currentMessage.fontSize = TRSystemConfig.Instance.fontSize;
            currentMessage.color = TRSystemConfig.Instance.fontColor;

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
            else if (state == MessageState.OnShow)
                state = MessageState.OnWait;
            else if (state == MessageState.OnWait)
                state = MessageState.None;
        }

        public void ClearMessage()
        {
            currentMessage.text = "";
            currentName.text = "";
        }

        public void ShowMessage(string text, float mesCurrentWait = 0)
        {
            state = MessageState.OnShow;

            StartCoroutine(ShowMessageSub(text, mesCurrentWait));
        }

        private IEnumerator ShowMessageSub(string message, float mesCurrentWait)
        {
            float mesWait = mesCurrentWait;

            if(!enableSkip)
                currentMessage.VisibleLength = 0;

            currentMessage.text = message;
            currentName.text = nameString;

            if (!enableSkip && mesCurrentWait > 0.0f)
            {
                for (int i = 0; i < currentMessage.MaxIndex; i++)
                {
                    if (state == MessageState.OnShow)
                        currentMessage.VisibleLength++;
                    else
                        break;

                    yield return new WaitForSeconds(mesWait);
                }
            }

            currentMessage.VisibleLength = -1;

            yield return Wait();
        }

        Tweener _sequence = null;

        public IEnumerator Wait(WaitIcon icon = WaitIcon.Alpha, float autoWait = 1.0f)
        {
            if(!enableSkip)
            {
                state = MessageState.OnWait;
                WaitCursor(icon);

                if (onAuto)
                    yield return new WaitForSeconds(autoWait);
                else
                    yield return new WaitWhile(() => state == MessageState.OnWait);
            }

            /*
            if(TRMessageLogWindow.Instance != null && enableLogWindow)
            {
                TRMessageLogWindow.Instance.AddLogData(currentName.text, currentMessage.text);
            }
            */

            state = MessageState.None;

            yield return new WaitForEndOfFrame();

            if (!TRSystemConfig.Instance.isNovelMode)
                ClearMessage();

            //            if (!onSkip && !onAuto)
            //            Trionfi.Instance.ClickEvent -= onClickEvent;

            yield return null;
        }

        public void WaitCursor(WaitIcon icon)
        {
            StartCoroutine(WaitCusorSub(icon));
        }

        public IEnumerator WaitCusorSub(WaitIcon icon, float loopTime = 1.2f)
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

            yield return new WaitWhile(() => state == MessageState.OnWait);

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

        public void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl))
                forceSkip = true;
            else
                forceSkip = false;
        }

    }
}
