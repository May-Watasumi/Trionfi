using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.IO;

namespace NovelEx {
	public class ImageObject : AbstractObject {
		//foreとbackを持つ
		//private string name;

		private GameObject spriteFore;
		private GameObject spriteBack;

		private SpriteRenderer spriteRenderFore;
		private SpriteRenderer spriteRenderBack;

		private Sprite targetSprite ;

		private bool isShow = false;
		public bool visible {
			get {
				return isShow;
			}
		}

		public string filename = "";

        public ImageObject(Dictionary<string, string> param) { Init(param); }

        public override void Init(Dictionary<string,string> param) {
			paramDic = param;

//			GameObject g = StorageManager.Instance.loadPrefab("Image");
			GameObject g = StorageManager.Instance.loadPrefab("ImageEx");
			instanceObject.transform.parent = RootObject.transform;
//			instanceObject.name = this.name;

			this.spriteFore = (GameObject)GameObject.Instantiate(g, new Vector3(0, 0f, -3.2f), Quaternion.identity);
			this.spriteBack = (GameObject)GameObject.Instantiate(g, new Vector3(0, 0f, -3.2f), Quaternion.identity);
			this.spriteFore.transform.parent = ImageObjectManager.frontRoot.transform;
			this.spriteBack.transform.parent = ImageObjectManager.backRoot.transform;
			this.spriteFore.name = paramDic["name"];
			this.spriteBack.name = paramDic["name"];
            instanceObject = g;//this.spriteFore;
//			instanceObject = (GameObject)Instantiate(g,new Vector3(0,0f,-3.2f),Quaternion.identity); 
//			instanceObject.name = this.name;
//			this.spriteFore = instanceObject.transform.FindChild("fore").gameObject;
//			this.spriteBack = instanceObject.transform.FindChild("back").gameObject;

			//サイズを指定できる
			this.spriteRenderFore = this.spriteFore.GetComponent<SpriteRenderer>();
			this.spriteRenderBack = this.spriteBack.GetComponent<SpriteRenderer>();

			//Layerの設定
			this.spriteRenderFore.sortingLayerName = paramDic ["layer"];
			this.spriteRenderFore.sortingOrder = int.Parse(paramDic ["sort"]);

			this.spriteRenderBack.sortingLayerName = paramDic ["layer"];
			this.spriteRenderBack.sortingOrder = int.Parse(paramDic ["sort"]);
		}

		public override void SetParam(Dictionary<string,string> param) {
			if(instanceObject == null)
				Init(param);

			if (paramDic.ContainsKey ("path") && paramDic ["path"] == "true")
            {
				if (paramDic ["storage"] != "") {

					#if(!UNITY_WEBPLAYER)

					byte[] bytes = File.ReadAllBytes(paramDic["storage"]);

					Texture2D texture = new Texture2D (0, 0);
					texture.LoadImage (bytes);
					this.targetSprite = Sprite.Create (texture, new Rect (0, 0, Screen.width, Screen.height), new Vector2 (1f, 1f));

					#else

					#endif

				}
                else
                {
					//画像がない場合はデフォルトの未設定のファイルを見せるか。。
				}
			}
			else {
				string filename = param["imagePath"] + param ["storage"];
				this.filename = filename;
				this.targetSprite = StorageManager.Instance.loadSprite(filename); //Resources.Load<Sprite>(filename);
			}

			SpriteRenderer sr_back = this.spriteBack.GetComponent<SpriteRenderer>();
			sr_back.sprite = this.targetSprite;

//EX変更：Image系はSortingOrderのみ見るように
			if(paramDic["name"] == "background" || paramDic["strech"]=="true") {
//			if (paramDic ["layer"] == "background") {
				//背景の場合、サイズを画面いっぱいにする
				SpriteRenderer[] srs = new SpriteRenderer[2];
				srs[0] = this.spriteRenderBack;
				srs[1] = this.spriteRenderFore;

				SpriteRenderer sr = this.spriteRenderBack;
				float x = sr.sprite.bounds.size.x;
				float y = sr.sprite.bounds.size.y;

				float worldScreenHeight = Trionfi.Instance.targetCamera.orthographicSize * 2;
				float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

				for (int a = 0; a < srs.Length ; a++) {
					srs[a].transform.localScale = new Vector3 (
						worldScreenWidth / x,
						worldScreenHeight / y, 1);
				}
//					sr.sortingOrder = 0;
					//this.spriteRenderFore.transform.localScale = new Vector3 (
					//	worldScreenWidth / x,
					//	worldScreenHeight / y, 1);
				this.spriteRenderFore.sortingOrder = 0;
			}
		}

		public override void SetPosition(float x, float y, float z)
		{
			this.spriteRenderBack.transform.position = new Vector3(x, y, z);
			this.spriteRenderFore.transform.position = new Vector3(x, y, z);
		}

		public override void SetScale(float scale_x, float scale_y, float scale_z)
		{
			this.spriteRenderFore.transform.localScale = new Vector3(scale_x, scale_y, 1);
			this.spriteRenderBack.transform.localScale = new Vector3(scale_x, scale_y, 1);
		}

		public override Vector3 GetPosition()
		{
			return this.spriteRenderBack.transform.position;
		}

		public override void SetColider() {
			if (this.targetSprite != null)
            {			
				instanceObject.AddComponent<BoxCollider2D>();
				BoxCollider2D b = instanceObject.GetComponent<BoxCollider2D>();
				b.isTrigger = true;
                b.enabled = isShow;
				Vector2 size = new Vector2(targetSprite.bounds.size.x, targetSprite.bounds.size.y);
				b.size = size;
			}
		}

		public override void Show(float time, string easeType)
        {
			isShow = true;

			if(time <= 0.0f)
                OnAnimationFinished();
			else {		
				//通常の表示切り替えの場合
				iTween.ValueTo(instanceObject, iTween.Hash(
					"from",0,
					"to",1,
					"time",time,
					"oncomplete","finishAnimation",
					"oncompletetarget", instanceObject,
					"easeType",easeType,
					"onupdate","crossFadeShow"
				));
			}

			//アニメーション開始
			/*
			Animation ani_fore = this.spriteFore.GetComponent<Animation>();
			Animation ani_back = this.spriteBack.GetComponent<Animation>();

			ani_fore.Play();
			ani_back.Play ("TransShow");

			//メソッドの登録
			CompleteDelegate completeDeletgate = this.finishAnimation;
			this.gameManager.scene.coroutineAnimation (ani_back, completeDeletgate);	
			*/

		}

		public override void Hide(float time, string easeType) {
			isShow = false;
/*
			BoxCollider2D b = instanceObject.GetComponent<BoxCollider2D>();
			if (b != null) {
				b.enabled = false;
			}
*/
			if (time <= 0.0f)
                OnAnimationFinished();
			else {
				//通常の表示切り替えの場合
				iTween.ValueTo(instanceObject, iTween.Hash(
					"from",1,
					"to",0,
					"time",time,
					"oncomplete","finishAnimation",
					"oncompletetarget", instanceObject,
                    "easeType",easeType,
					"onupdate","crossFadeHide"
				));
			}
		}

		private void crossFadeShow(float val){
			var color_back = this.spriteRenderBack.color;
			color_back.a = val;
			this.spriteRenderBack.color = color_back;

			var color_fore = this.spriteRenderFore.color;
			color_fore.a = 1-val;
			this.spriteRenderFore.color = color_fore;
		}

		private void crossFadeHide(float val) {
			var color_fore = this.spriteRenderFore.color;
			color_fore.a = val;
			this.spriteRenderFore.color = color_fore;
		}

        //アニメーションの終了をいじょうするための
        public override void OnAnimationFinished()
        {
            SpriteRenderer sr_fore = this.spriteFore.GetComponent<SpriteRenderer>();
			SpriteRenderer sr_back = this.spriteBack.GetComponent<SpriteRenderer>();

			//全面に今回適応した背景の画像を配置する
			BoxCollider2D b = instanceObject.GetComponent<BoxCollider2D>();

			if (this.isShow == true) {
				//表示の時
				sr_fore.sprite = this.targetSprite;
				sr_fore.color = new Color (1, 1, 1, 1);

				if (b != null)
					b.enabled = true;
			}
			else {
				//sr_fore.sprite = this.targetSprite;
				sr_fore.sprite = null;
				sr_fore.color = new Color (1, 1, 1, 0);
				if (b != null)
					b.enabled = false;
			}

			sr_back.color = new Color (1, 1, 1, 0);

			//sr_back.sprite = null;

			if (this.completeDeletgate != null)
				this.completeDeletgate();
		}        
	}
}
