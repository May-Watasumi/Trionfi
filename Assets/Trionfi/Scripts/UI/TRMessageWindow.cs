using UnityEngine;
using System.Collections;
using Trionfi;
using DG.Tweening;
using UnityEngine.UI;

public class TRMessageWindow : SingletonMonoBehaviour<TRMessageWindow>
{
    public bool enableLogWindow = true;

    public bool onSkip = false;
    public bool onAuto = false;

    public enum MessageState { None, OnShow, /*OnSkip, OnAuto,*/ OnWait }
    public enum WaitIcon { None, Alpha, Rotate }

    public MessageState state = MessageState.None;

    [SerializeField]
    public LetterWriter.Unity.Components.LetterWriterText currentMessage;
    [SerializeField]
    public Text currentName;
    [SerializeField]
    private Image MessageFrameImage;
    [SerializeField]
    public Image waitCursor;

    public void Start()
    {
    }

    public void Reset()
    {
        //ToDo
    }

    public void OnClick()
    {
        if(state == MessageState.OnShow)
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

    public void onClickEvent()
    {
        if (state == MessageState.OnShow)
            state = MessageState.OnWait;
        else if (state == MessageState.OnWait)
            state = MessageState.None;
    }

    private IEnumerator ShowMessageSub(string message, float mesCurrentWait)
    {
        float mesWait = mesCurrentWait;

        string tempMessage = "";

        Trionfi.Trionfi.Instance.ClickEvent += onClickEvent;

        if(!onSkip && mesCurrentWait > 0.0f)
        {
            currentMessage.text = "";

            for(int i = 0; i < message.Length; i++)
            {
                if (state == MessageState.OnShow)
                    tempMessage += message[i];
                else
                    break;

                currentMessage.text = tempMessage;

                yield return new WaitForSeconds(mesWait);
            }
        }

        currentMessage.text = message;

        yield return Wait();
    }

    public IEnumerator Wait(WaitIcon icon = WaitIcon.Alpha, float autoWait = 1.0f)
    {
        state = MessageState.OnWait;

        waitCursor.gameObject.SetActive(true);

        WaitCursor(icon);

        if (onAuto)
            yield return new WaitForSeconds(autoWait);
        else if (onSkip)
            yield return null;
        else
        {
            yield return new WaitWhile(() => state == MessageState.OnWait);
        }

        waitCursor.gameObject.SetActive(false);

        if(TRMessageLogWindow.Instance != null && enableLogWindow)
        {
            TRMessageLogWindow.Instance.AddLogData(currentName.text, currentMessage.text);
        }

        if(!Trionfi.TRSystemConfig.Instance.isNovelMode)
            ClearMessage();

        yield return null;
    }

    public void WaitCursor(WaitIcon icon)
    {
        StartCoroutine(WaitCusorSub(icon));
    }

    public IEnumerator WaitCusorSub(WaitIcon icon)
    {
        switch (icon)
        {
            case WaitIcon.Alpha:
                waitCursor.color = new Color(waitCursor.color.r, waitCursor.color.g, waitCursor.color.b, 0.0f);
                DOTween.ToAlpha(
                () => waitCursor.color,
                color => waitCursor.color = color,
                1.0f,                                // 最終的なalpha値
                1.5f
                )
                .SetLoops(-1, LoopType.Yoyo);
                break;
            case WaitIcon.Rotate:
//                waitCursor.GetComponent<RectTransform>().rotation = Vector3.zero;
                waitCursor.GetComponent<RectTransform>().DORotate(new Vector3(0, 0, 359), 1.0f).SetRelative(true).SetLoops(-1);
                break;
        }

        yield return new WaitWhile(() => state == MessageState.OnWait);

        Trionfi.Trionfi.Instance.ClickEvent -= onClickEvent;    }

    public void ShowName(string name, Sprite face = null)
    {
        currentName.text = name;
    }
}

