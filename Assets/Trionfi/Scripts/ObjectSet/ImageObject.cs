using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.IO;

namespace NovelEx {
	public class ImageObject : AbstractObject {
        public ImageObject() { }
        public ImageObject(Dictionary<string, string> param) : base(param){ }

        Sprite currentSprite;

        public void LoadSprite(string storage)
        {
            if(instanceObject == null)
                instanceObject = GameObject.Instantiate(StorageManager.Instance.imageBasePrefab);

            currentSprite = StorageManager.Instance.LoadImage(storage);
            instanceObject.GetComponent<Image>().sprite = currentSprite;
        }

        public override void Init(Dictionary<string,string> param)
        {
			paramDic = param;
            LoadSprite(param["storage"]);

//          ToDo:
//            カレントのCanvasのルートツリーにぶらさげる
#if false
            //Layerの設定
            this.spriteRenderFore.sortingLayerName = paramDic ["layer"];
			this.spriteRenderFore.sortingOrder = int.Parse(paramDic ["sort"]);

			this.spriteRenderBack.sortingLayerName = paramDic ["layer"];
			this.spriteRenderBack.sortingOrder = int.Parse(paramDic ["sort"]);
#endif
        }

		public override void SetParam(Dictionary<string,string> param)
        {
			if(instanceObject == null)
				Init(param);

			if(paramDic.ContainsKey("path") && paramDic["path"] == "true")
            {
				if(paramDic["storage"] != "")
                {
                    LoadSprite(param["storage"]);
                }
                else
                {
					//画像がない場合はデフォルトの未設定のファイルを見せるか。。
				}
			}
			else
            {
				string filename = param["imagePath"] + param ["storage"];
                LoadSprite(filename);
            }

            //EX変更：Image系はSortingOrderのみ見るように
            if (paramDic["name"] == "background" || paramDic["strech"]=="true")
            {
//			if (paramDic ["layer"] == "background") {
				//背景の場合、サイズを画面いっぱいにする
//				float x = sr.sprite.bounds.size.x;
//				float y = sr.sprite.bounds.size.y;
//				float worldScreenHeight = Trionfi.Instance.targetCamera.orthographicSize * 2;
//				float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
			}
		}
/*
		public override void SetPosition(float x, float y, float z)
		{
			instanceObject.transform.position = new Vector3(x, y, z);
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
*/
		public override void Show(float time, string easeType)
        {
            instanceObject.SetActive(true);

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

		public override void Hide(float time, string easeType)
        {
//			isShow = false;
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
/*
        IEnumerable FadeOwn(bool isShow, float time = 1.0f)
        {
            float currenttime = 0.0f;

            instanceObject.GetComponent<Image>().color.a = 0.0;

            return null;
        }
*/
	}
}
