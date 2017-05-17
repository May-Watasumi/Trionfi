using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

namespace NovelEx {
	public class TextObject : AbstractObject
    {
		//private string name;

//		private Sprite targetSprite ;
//		private bool isShow = false;

		public string filename = "";

		public override void Load(Dictionary<string,string> param)
        {
//ToDo:
			paramDic = param;
//            gameObject.transform.parent = RootObject.transform;

            GameObject g = StorageManager.Instance.loadPrefab("Text") as GameObject;
//			gameObject = (GameObject)GameObject.Instantiate(g,new Vector3(0.0f, 0.0f, 0.0f),Quaternion.identity); 

			GameObject canvas = GameObject.Find ("Canvas") as GameObject;

			gameObject.name = param ["name"];
			gameObject.transform.parent = canvas.transform;

			UnityEngine.UI.Text guiText = gameObject.GetComponent<Text>();

			//Debug.Log (paramDic ["anchor"]);
			//Debug.Log (TextEnum.textAnchor (paramDic ["anchor"]));
//			guiText.alignment = TextEnum.textAnchor (paramDic ["anchor"]);

            string color = paramDic ["color"];

			Color objColor = TRUtility.HexToRGB(color);
			objColor.a = 0;
			guiText.color = objColor;
			guiText.fontSize = int.Parse(paramDic ["fontsize"]);
		}
        /*
		public override void SetParam(Dictionary<string,string> param)
        {
            base.SetParam(param);

			string text = paramDic["val"];

			if (paramDic ["cut"] != "") {
				int cut = int.Parse (paramDic ["cut"]);
				if (cut < text.Length) {
					text = text.Substring (0,cut);
			
					paramDic ["val"] = text;

				}
			}
			gameObject.GetComponent<Text>().text = text;
			//gameObject.GetComponent<Text>().resizeTextForBestFit = true;
		}
        */
		public override void SetColider()
        {
			/*
			gameObject.AddComponent<BoxCollider2D>();
			BoxCollider2D b = gameObject.GetComponent<BoxCollider2D>();
			b.isTrigger = true;
			if (this.isShow == true) {
				b.enabled = true;
			} else {
				b.enabled = false;
			}
			Vector2 size = new Vector2 (this.targetSprite.bounds.size.x, this.targetSprite.bounds.size.y);
			b.size = size;
			*/
		}

		public override void SetPosition(float x,float y,float z)
        {
			gameObject.transform.localPosition = new Vector3(x,y,z);
		}

		public override void Show(float time,string easeType)
        {
//			this.isShow = true;

			//通常の表示切り替えの場合
			iTween.ValueTo( gameObject,iTween.Hash(
				"from",0,
				"to",1,
				"time",time,
				"oncomplete","finishAnimation",
				"oncompletetarget", gameObject,
				"easeType",easeType,
				"onupdate","crossFade"
			));
		}

		public override void Hide(float time,string easeType)
        {
//			this.isShow = false;

			//BoxCollider2D b = gameObject.GetComponent<BoxCollider2D>();
			//b.enabled = false;

			//通常の表示切り替えの場合
			iTween.ValueTo( gameObject,iTween.Hash(
				"from",1,
				"to",0,
				"time",time,
				"oncomplete","finishAnimation",
				"oncompletetarget", gameObject,
				"easeType",easeType,
				"onupdate","crossFade"
			));
		}

		private void CrossFade(float val)
        {
			var color = gameObject.GetComponent<Text>().color;
			color.a = val;
			gameObject.GetComponent<Text>().color = color;
		}
	}
}
