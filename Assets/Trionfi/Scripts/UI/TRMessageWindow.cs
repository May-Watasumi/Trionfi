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

    MessageState state = MessageState.None;

    [SerializeField]
    public Text currentMessage;
    [SerializeField]
    public Text currentName;
    [SerializeField]
    private Image MessageFrameImage;
    [SerializeField]
    public Image waitCursor;

    public void Start()
    {
        //        currentMessage = GetComponentInChildren<Text>();
        //        MessageFrameImage = GetComponent<Image>();
        //TRUIInstance.Instance.OnClick += this.OnClick;
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
        StartCoroutine(ShowMessageSub(text, mesCurrentWait));
    }

    private IEnumerator ShowMessageSub(string message, float mesCurrentWait)
    {
        float mesWait = mesCurrentWait;

        string tempMessage = "";

        if(!onSkip && mesCurrentWait > 0.0f)
        {
            state = MessageState.OnShow;
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
    }

    public void ShowName(string name, Sprite face = null)
    {
        currentName.text = name;
    }
}

#if false
public void hideMessage(float time){

		//通常の表示切り替えの場合
		iTween.ValueTo(this.gameObject,iTween.Hash(
			"from",1,
			"to",0,
			"time",time,
			"oncomplete","finishAnimation",
			"oncompletetarget",this.gameObject,
			"easeType","linear",
			"onupdate","crossFade"
		));

		this.hideUiObject ();
	}

	public void showMessage(float time){

		//通常の表示切り替えの場合
		iTween.ValueTo(this.gameObject,iTween.Hash(
			"from",0,
			"to",1,
			"time",time,
			"oncomplete","finishAnimation",
			"oncompletetarget",this.gameObject,
			"easeType","linear",
			"onupdate","crossFade"
		));

		this.showUiObject ();
	}

	public void showMessageWithoutNextOrder(float time){

		//通常の表示切り替えの場合
		iTween.ValueTo(this.gameObject,iTween.Hash(
			"from",0,
			"to",1,
			"time",time,
			"oncomplete","finishAnimationWithoutNextOrder",
			"oncompletetarget",this.gameObject,
			"easeType","linear",
			"onupdate","crossFade"
		));

		this.showUiObject ();

	}


	public void hideMessageWithoutNextOrder(float time){

		//通常の表示切り替えの場合
		iTween.ValueTo(this.gameObject,iTween.Hash(
			"from",1,
			"to",0,
			"time",time,
			"oncomplete","finishAnimationWithoutNextOrder",
			"oncompletetarget",this.gameObject,
			"easeType","linear",
			"onupdate","crossFade"
		));

		this.hideUiObject ();

	}

	private void showUiObject(){


		GameObject canvas = GameObject.Find ("Canvas");
		canvas.GetComponent<Canvas>().enabled = true;

		/*
		//uiタグが付いているものを消去する
		GameObject[] obj = GameObject.FindGameObjectsWithTag ("ui");

		for (var i = 0; i < obj.Length; i++) {

			GameObject g_obj = (GameObject)obj[i];

			if (g_obj.GetComponent<GUIText> () != null) {
				g_obj.GetComponent<GUIText> ().enabled = true;
			} else {
				GameObject g_fore = g_obj.transform.FindChild ("fore").gameObject;
			
				g_fore.GetComponent<SpriteRenderer> ().enabled = true;
			}
		}
		*/

	}

	private void hideUiObject(){


		GameObject canvas = GameObject.Find ("Canvas");
		canvas.GetComponent<Canvas>().enabled = false;


		//uiタグが付いているものを消去する
		/*
		GameObject[] obj = GameObject.FindGameObjectsWithTag ("ui");

		for (var i = 0; i < obj.Length; i++) {

			GameObject g_obj = (GameObject)obj[i];

			if (g_obj.GetComponent<GUIText> () != null) {
				g_obj.GetComponent<GUIText> ().enabled = false;
			} else {
				GameObject g_fore = g_obj.transform.FindChild ("fore").gameObject;

				g_fore.GetComponent<SpriteRenderer> ().enabled = false;
			}
		}
		*/

	
	}

	public void crossFade(float val){
		//var test = this.gameObject.GetComponent<Image> ();

		var spriteRender = this.gameObject.GetComponent<UnityEngine.UI.Image> ();
		var color_fore = spriteRender.color;
		color_fore.a = val;
		spriteRender.color = color_fore;

	}

	public void finishAnimation(){

		StatusManager.enableClickOrder = true;
		NovelSingleton.GameManager.nextOrder ();
	}

	public void finishAnimationWithoutNextOrder(){

		StatusManager.enableClickOrder = true;

	}

#endif
