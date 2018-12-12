using UnityEngine;
using System.Collections;
using Trionfi;
using DG.Tweening;
using UnityEngine.UI;

namespace Trionfi
{
    public class TRMessageWindow : MonoBehaviour
    {
        public enum MessageState { None, OnShow, /*OnSkip, OnAuto,*/ OnWait, OnClose }
        public enum WaitIcon { None, Alpha, Rotate }

        public bool enableLogWindow = true;

        public bool forceSkip = false;

        public bool onSkip = false;
        public bool onAuto = false;

        public bool enableSkip
        { get { return forceSkip || onSkip; } }

        public MessageState state = MessageState.None;

        public float speedRatio = 1.0f;

        public Tweener tweener = null;

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

        [SerializeField]
        public Image faceIcon;

        [SerializeField]
        public Image waitCursor;

        public string nameString = string.Empty;

        public void Start()
        {
            if (currentUguiMessage != null)
                currentUguiMessage.fontSize = TRSystemConfig.Instance.fontSize;

            if (currentMessage != null)
            {
                currentMessage.fontSize = TRSystemConfig.Instance.fontSize;
                currentMessage.color = TRSystemConfig.Instance.fontColor;
            }

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

        public void Close()
        {
            if (state == MessageState.OnShow)
                StopCoroutine(_waitCoroutine);

            Trionfi.Instance.messageWindow.gameObject.SetActive(false);
        }

        public void Open()
        {
            Trionfi.Instance.messageWindow.gameObject.SetActive(true);

            if (state == MessageState.OnShow)
            {
                StartCoroutine(_waitCoroutine);
            }
        }

        public void ClearMessage()
        {
            if (currentUguiMessage != null)
                currentUguiMessage.text = string.Empty;
            if (currentMessage != null)
                currentMessage.text = string.Empty;
            if (currentName != null)
                currentName.text = string.Empty;
            if (faceIcon != null)
                faceIcon.enabled = false;

            nameString = string.Empty;

            speedRatio = 1.0f;
        }

        public IEnumerator _waitCoroutine = null;

        public void ShowMessage(string text, float mesCurrentWait = 0)
        {
            state = MessageState.OnShow;

            if (useUguiText)
                _waitCoroutine = ShowMessageUguiSub(text, mesCurrentWait);
            else
                _waitCoroutine = ShowMessageSub(text, mesCurrentWait);

            StartCoroutine(_waitCoroutine);
        }

        private IEnumerator ShowMessageSub(string message, float mesCurrentWait)
        {
            float mesWait = mesCurrentWait / speedRatio;

            if (!enableSkip)
                currentMessage.VisibleLength = 0;

            currentMessage.text = message;

            if (currentName != null)
                currentName.text = nameString;

            Trionfi.Instance.messageLogwindow.AddLogData(currentMessage.text, currentName.text);

            if (!enableSkip && mesWait > 0.0f)
            {
                for (int i = 0; i < currentMessage.MaxIndex; i++)
                {
                    if (state == MessageState.OnShow && !enableSkip)
                        currentMessage.VisibleLength++;
                    else
                        break;

                    yield return new WaitForSeconds(mesWait);
                }
            }

            currentMessage.VisibleLength = -1;

            yield return Wait();
        }

        private IEnumerator ShowMessageUguiSub(string message, float mesCurrentWait)
        {
            float mesWait = mesCurrentWait / speedRatio;

            string tempString = string.Empty;

            if (!enableSkip)
            {
                for (int a = 0; a < message.Length; a++)
                {
                    if (message[a] == '\n' || message[a] == '\r')
                        tempString += message[a];
                    else
                        tempString += "@" + a.ToString();
                }
            }

            //            currentUguiMessage.text = tempString;

            if (currentName != null)
                currentName.text = nameString;

            Trionfi.Instance.messageLogwindow.AddLogData(message, nameString);

            if (!enableSkip && mesWait > 0.0f)
            {
                for (int i = 0; i < message.Length; i++)
                {
                    if (state == MessageState.OnShow && !enableSkip)
                        currentUguiMessage.text += message[i].ToString(); //.Replace("@" + i.ToString(), message[i].ToString());
                    else
                        break;

                    yield return new WaitForSeconds(mesWait);
                }
            }

            currentUguiMessage.text = message;

            yield return Wait();
        }


        Tweener _sequence = null;

        public IEnumerator Wait(WaitIcon icon = WaitIcon.Alpha, float autoWait = 1.0f)
        {
            if (tweener != null)
                tweener.Kill();

            if (!enableSkip)
            {
                state = MessageState.OnWait;

                WaitCursor(icon);

                if (onAuto)
                    yield return new WaitForSeconds(autoWait);
                else
                    yield return new WaitWhile(() => state == MessageState.OnWait && !enableSkip);
            }

            /*
            if(TRMessageLogWindow.Instance != null && enableLogWindow)
            {
                TRMessageLogWindow.Instance.AddLogData(currentName.text, currentMessage.text);
            }
            */

            state = MessageState.None;

            yield return new WaitForEndOfFrame();

            //if (!TRSystemConfig.Instance.isNovelMode)
            //    ClearMessage();

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

            yield return new WaitWhile(() => state == MessageState.OnWait && !enableSkip);

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
