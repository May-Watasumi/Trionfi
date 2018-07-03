using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NovelEx
{
    public abstract class AbstractObject : MonoBehaviour
    {
 		protected ParamDictionary paramDic = new ParamDictionary();

        public virtual ParamDictionary param
        {
            [SerializeField]
            get
            {
                return paramDic;
            }

            set
            {
                paramDic.Clear();
                paramDic = value;

                gameObject.name = value["name"];
                gameObject.tag = value["tag"];
                SetPosition(paramDic.Float("x"), paramDic.Float("y"), paramDic.Float("z"));
                SetScale(paramDic.Float("scale_x"), paramDic.Float("scale_y"), paramDic.Float("scale_z"));
                SetRotate(paramDic.Float("rot_x"), paramDic.Float("rot_y"), paramDic.Float("rot_z"));
            }
        }

        public virtual void SetPosition(float x,float y,float z)
        {
			gameObject.transform.position = new Vector3(x,y,z);
		}

        public virtual void SetScale(float scale_x, float scale_y, float scale_z)
        {
            gameObject.transform.localScale = new Vector3(scale_x, scale_y, 1);
		}

        public virtual void SetRotate(float rot_x, float rot_y, float rot_z)
        {
            gameObject.transform.rotation = Quaternion.Euler(rot_x, rot_y, rot_x);
        }

#if false
		//position を アニメーションしながら変更します
		public void AnimationPosition(Vector3 position , float scale, float time, string type)
        {
			iTween.MoveTo(gameObject, iTween.Hash(
				"position", position,
				"time", time, 
				"oncomplete", "complete_anim", 
				"oncompletetarget", gameObject, 
				"easeType", type,
				"islocal",true
				//"space",Space.Self
				//"space", Space.worldでグローバル座標系で移動
			));

			iTween.ScaleTo(gameObject, iTween.Hash(
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

			gameObject.AddComponent<Animation>();
			Animation anim = gameObject.GetComponent<Animation>();
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
#endif
        public virtual bool Load(string storage) { return true; }
        public virtual void Load(ParamDictionary param) { }

		public virtual void Show(float time, string easeType){}
		public virtual void Hide(float time, string easeType){}
    }

}
