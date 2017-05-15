using UnityEngine;
using System.Collections;
using NovelEx;
using UnityEngine.UI;

public class MessageWindow : MonoBehaviour
{
    public float textwaitTime = 0.03f;
    public bool onSkip = false;
    public bool onAuto = false;

    [SerializeField]
    public Text currentMessage;
    [SerializeField]
    public Text currentName;
    [SerializeField]
    private Image MessageFrameImage;

    public void Start()
    {
//        currentMessage = GetComponentInChildren<Text>();
//        MessageFrameImage = GetComponent<Image>();
    }

    public void Reset()
    {
        //ToDo
    }

    public void ClearMessage()
    {
        currentMessage.text = "";
    }

    public void ShowMessage(string text)
    {
        StartCoroutine(ShowMessageSub(text));
    }

    private IEnumerator ShowMessageSub(string message)
    {
        // this.messageForSaveTitle = message;
        float mesWait = textwaitTime;

        string tempMessage = "";

        if(mesWait > 0.0f)//&& !StatusManager.Instance.onSkip)
        {
            currentMessage.text = "";

            //スキップモードの場合は速度アップ
            for(int i = 0; i < message.Length; i++)
            {
                //スキップモードの場合は一度に複数の文字列を表示する
                if(onSkip)
                    break;
                else
                    tempMessage += message[i];

                yield return new WaitForSeconds(mesWait);
            }
        }
        
        currentMessage.text = message;
        yield return null;
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
