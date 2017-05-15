using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NovelEx
{
	public abstract class AbstractObject
    {
        public static GameObject RootObject;

		public GameObject instanceObject;
		public Dictionary<string,string> paramDic;

		public string imagePath ="";
        public string name;

        protected CompleteDelegate completeDeletgate = null;
		public CompleteDelegate animCompleteDeletgate = null; //アニメーションのデリゲート指定

		//アニメーション完了時の処理委譲先

		public virtual void SetPosition(float x,float y,float z)
        {
			instanceObject.transform.position = new Vector3(x,y,z);
		}
        public virtual void SetPosition(Vector3 v)
        {
            instanceObject.transform.position = new Vector3(v.x, v.y, v.z);
        }

        public virtual void SetScale(float scale_x, float scale_y, float scale_z)
        {
			instanceObject.transform.localScale = new Vector3(scale_x, scale_y, 1);
		}

		public virtual Vector3 GetPosition()
        {
			return instanceObject.transform.position;
		}

		//position を アニメーションしながら変更します
		public void AnimationPosition(Vector3 position , float scale, float time, string type)
        {
			iTween.MoveTo(instanceObject, iTween.Hash(
				"position", position,
				"time", time, 
				"oncomplete", "complete_anim", 
				"oncompletetarget", instanceObject, 
				"easeType", type,
				"islocal",true
				//"space",Space.Self
				//"space", Space.worldでグローバル座標系で移動
			));

			iTween.ScaleTo(instanceObject,iTween.Hash(
				"x",scale,
				"y",scale,
				"time",time
			));
		}

		//アニメ完了時
		public virtual void OnAnimationFinished()
        {
			if (this.animCompleteDeletgate != null)
				this.animCompleteDeletgate();
		}

        //ToDo:分離
		public virtual void PlayAnimation(string state)
        {
			//Animationクリップの読み込み
//			AnimationClip ap = Resources.Load(GameSetting.PATH_ANIM_FILE + state) as AnimationClip;
			AnimationClip ap = StorageManager.Instance.loadAnimation(state);

			instanceObject.AddComponent<Animation>();
			Animation anim = instanceObject.GetComponent<Animation>();
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

		public virtual void StopAnimation(string state) { }

		public void SetFinishAnimationDelegate(CompleteDelegate completeDeletgate)
        {
			this.completeDeletgate = completeDeletgate;
		}

		public virtual void SetColider(){}
		public virtual void Init(Dictionary<string,string> param){}
   
		public virtual void Show(float time,string easeType){}
		public virtual void Hide(float time,string easeType){}
		public virtual void SetParam(Dictionary<string,string> param)
        {
            if (instanceObject == null)
            {
                Init(param);
            }
        }

        //ToDo:Image系
        public virtual void Remove()
        {
            GameObject.Destroy(instanceObject);
        }

    }

}
