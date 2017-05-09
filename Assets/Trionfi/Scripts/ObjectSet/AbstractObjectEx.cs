using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NovelEx {
	public abstract class AbstractObject:MonoBehaviour{
//		protected GameManager gameManager;
		public GameObject rootObject;
		public Dictionary<string,string> param;

		public string imagePath ="";

		protected CompleteDelegate completeDeletgate = null;
		public CompleteDelegate animCompleteDeletgate = null; //アニメーションのデリゲート指定

		//アニメーション完了時の処理渭城先

		public virtual void setPosition(float x,float y,float z){
			this.rootObject.transform.position = new Vector3(x,y,z);
		}

		public virtual void setScale(float scale_x, float scale_y, float scale_z){
			this.rootObject.transform.localScale = new Vector3 (scale_x, scale_y, 1);
		}

		public virtual Vector3 getPosition() {
			return this.rootObject.transform.position;
		}

		//position を アニメーションしながら変更します
		public void animPosition(Vector3 position , float scale, float time, string type){

			iTween.MoveTo(this.rootObject, iTween.Hash(
				"position", position,
				"time", time, 
				"oncomplete", "complete_anim", 
				"oncompletetarget", this.gameObject, 
				"easeType", type,
				"islocal",true
				//"space",Space.Self
				//"space", Space.worldでグローバル座標系で移動
			));

			iTween.ScaleTo(this.rootObject,iTween.Hash(
				"x",scale,
				"y",scale,
				"time",time
			));
		}

		//アニメ完了時
		public void complete_anim() {
			if (this.animCompleteDeletgate != null)
				this.animCompleteDeletgate();
		}

		public virtual void playAnim(string state){
			//Animationクリップの読み込み
//			AnimationClip ap = Resources.Load(GameSetting.PATH_ANIM_FILE + state) as AnimationClip;
			AnimationClip ap = JOKEREX.Instance.StorageManager.loadAnimation(state);

			this.rootObject.AddComponent<Animation>();
			Animation anim = this.rootObject.GetComponent<Animation>();
			anim.AddClip (ap, "onmouse");
			anim.Play ("onmouse");

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

		public virtual void stopAnim(string state) { }

//ToDo:Image系
		public virtual void remove() {
			Destroy (this.rootObject);
		}

		public void setFinishAnimationDelegate(CompleteDelegate completeDeletgate){
			this.completeDeletgate = completeDeletgate;
		}

		public virtual void setColider(){}
		public virtual void init(Dictionary<string,string> param){}

		public virtual void show(float time,string easeType){}
		public virtual void hide(float time,string easeType){}
		public virtual void set(Dictionary<string,string> param){}
	}

}
