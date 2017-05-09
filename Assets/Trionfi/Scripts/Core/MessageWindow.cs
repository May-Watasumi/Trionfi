using UnityEngine;
using System.Collections;
using Novel;
using UnityEngine.UI;

public class MessageWindow : MonoBehaviour
{
    private Text currentMessage;
    private Image MessageFrameImage;
    public void Start()
    {
        currentMessage = GetComponentInChildren<Text>();
        MessageFrameImage = GetComponent<Image>();
    }

    public void Show(string text)
    {
        StartCoroutine("showMessage", text);
    }

    private IEnumerator showMessage(string message)
    {
        // this.messageForSaveTitle = message;
        float mesWait = 0.03f;//float.Parse(JOKEREX.Instance.getConfig("messageSpeed"));// 0.02f;
        string tempMessage = "";

        if(mesWait > 0.0f)//&& !JOKEREX.Instance.StatusManager.onSkip)
        {
            currentMessage.text = "";
            //スキップモードの場合は速度アップ
            for (int i = 0; i < message.Length; i++)
            {
                //スキップモードの場合は一度に複数の文字列を表示する
//                if (JOKEREX.Instance.StatusManager.currentState != JokerState.MessageShow)
//                    break;
//                else
                    tempMessage += message[i];

                //Debug.Log(CurrentMessage);
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
