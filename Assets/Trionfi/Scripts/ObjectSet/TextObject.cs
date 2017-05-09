using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

namespace NovelEx {
	public class TextObject : AbstractObject {
		//private string name;

//		private Sprite targetSprite ;
//		private bool isShow = false;

		public string filename = "";

		public override void init(Dictionary<string,string> param){
//ToDo:
			this.param = param;
			this.transform.parent = JOKEREX.Instance.transform; ;
			GameObject g = JOKEREX.Instance.StorageManager.loadPrefab("Text") as GameObject;
			this.rootObject = (GameObject)Instantiate(g,new Vector3(0,0.5f,-3.2f),Quaternion.identity); 

			GameObject canvas = GameObject.Find ("Canvas") as GameObject;

			this.rootObject.name = param ["name"];
			this.rootObject.transform.parent = canvas.transform;

			UnityEngine.UI.Text guiText = this.rootObject.GetComponent<Text>();

			//Debug.Log (this.param ["anchor"]);
			//Debug.Log (TextEnum.textAnchor (this.param ["anchor"]));
			guiText.alignment = TextEnum.textAnchor (this.param ["anchor"]);

			string color = this.param ["color"];

			Color objColor =  ColorX.HexToRGB(color);
			objColor.a = 0;
			guiText.color = objColor;
			guiText.fontSize = int.Parse(this.param ["fontsize"]);

			this.rootObject.name = this.name;
		}

		public override void set(Dictionary<string,string> param){
			if (this.rootObject == null) {
				this.init (param);
			}

			string text = this.param["val"];

			if (this.param ["cut"] != "") {
				int cut = int.Parse (this.param ["cut"]);
				if (cut < text.Length) {
					text = text.Substring (0,cut);
			
					this.param ["val"] = text;

				}
			}
			this.rootObject.GetComponent<Text>().text = text;
			//this.rootObject.GetComponent<Text>().resizeTextForBestFit = true;
		}

		public override void setColider(){
			/*
			this.rootObject.AddComponent<BoxCollider2D>();
			BoxCollider2D b = this.rootObject.GetComponent<BoxCollider2D>();
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

		public override void setPosition(float x,float y,float z){
			this.rootObject.transform.localPosition = new Vector3(x,y,z);
		}

		public override void show(float time,string easeType){
//			this.isShow = true;

			//通常の表示切り替えの場合
			iTween.ValueTo(this.gameObject,iTween.Hash(
				"from",0,
				"to",1,
				"time",time,
				"oncomplete","finishAnimation",
				"oncompletetarget",this.gameObject,
				"easeType",easeType,
				"onupdate","crossFade"
			));
		}

		public override void hide(float time,string easeType){
//			this.isShow = false;

			//BoxCollider2D b = this.rootObject.GetComponent<BoxCollider2D>();
			//b.enabled = false;

			//通常の表示切り替えの場合
			iTween.ValueTo(this.gameObject,iTween.Hash(
				"from",1,
				"to",0,
				"time",time,
				"oncomplete","finishAnimation",
				"oncompletetarget",this.gameObject,
				"easeType",easeType,
				"onupdate","crossFade"
			));
		}

		private void crossFade(float val){
			var color = this.rootObject.GetComponent<Text>().color;
			color.a = val;
			this.rootObject.GetComponent<Text>().color = color;
		}

		//アニメーションの終了をいじょうするための
		private void finishAnimation() {
			if (this.completeDeletgate != null)
				this.completeDeletgate();
		}

		// Use this for initialization
		void Start() { }

		// Update is called once per frame
		void Update() { }
	}
}
