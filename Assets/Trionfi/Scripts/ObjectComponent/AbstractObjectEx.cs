using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace NovelEx
{
	public abstract class AbstractObject : MonoBehaviour
    {
 		protected Dictionary<string,string> paramDic;

//        public string GetParam(string key) { return paramDic[key]; }

        public float ValidFloatParam(string key)
        {
            float res = 0.0f;
            if (paramDic.ContainsKey(key))
            {
                string v = paramDic[key];

                if (!float.TryParse(v, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out res))
                {
                    ErrorLogger.Log("InvalidParam: key=" + key + " Value=" + v);
                }
            }
            else
            {
                ErrorLogger.Log("NoParam: key=" + key);
            }            
            return res;
        }

        public int ValidIntParam(string key)
        {
            int res = 0;
            if (paramDic.ContainsKey(key))
            {
                string v = paramDic[key];

                if (!System.Int32.TryParse(v, out res))
                {
                    ErrorLogger.Log("InvalidParam: key=" + key + " Value=" + v);
                }
            }
            else
            {
                ErrorLogger.Log("NoParam: key=" + key);
            }
            return res;
        }

        public string ValidStringParam(string key)
        {
            if (paramDic.ContainsKey(key))
            {
                return paramDic[key];
            }
            else
            {
                ErrorLogger.Log("NoParam: key=" + key);
                return "";
            }
        }

        public T ValidParam<T>(string key)
        {
            if (typeof(T) == typeof(int))
            {
                return (T)(object)ValidIntParam(key);
            }
            else if (typeof(T) == typeof(float))
            {
                return (T)(object)ValidFloatParam(key);
            }
            else
            {
                return (T)(object)ValidStringParam(key);
            }
        }

        public virtual Dictionary<string, string> param
        {
            [SerializeField]
            get
            {
                return paramDic;
            }

            set
            {
                paramDic.Clear();
                gameObject.name = value["name"];
                gameObject.tag = value["tag"];
                paramDic = value;

                SetPosition(ValidFloatParam(paramDic["x"]), ValidFloatParam(paramDic["y"]), ValidFloatParam(paramDic["z"]));
                SetScale(ValidFloatParam(paramDic["scale_x"]), ValidFloatParam(paramDic["scale_y"]), ValidFloatParam(paramDic["scale_z"]));
                SetRotate(ValidFloatParam(paramDic["rot_x"]), ValidFloatParam(paramDic["rot_y"]), ValidFloatParam(paramDic["rot_z"]));
                /*
                                if (param["isShow"] == "true")
                                    Show(0, "linear");

                                                paramDic["tag"] = param["tag"];
                                                paramDic["storage"] = param["storage"];
                                                paramDic["isShow"] = "false";
                                                paramDic["imagePath"] = "";
                                                paramDic["className"] = "";
                                                paramDic["event"] = "false";
                                */
                //            paramDic = param;
            }
        }
/*
        public virtual void UpdateParam(Dictionary<string, string> param)
        {
            gameObject.name = param["name"];
            gameObject.tag = param["tag"];
//            paramDic = param;
        }
*/
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
        public virtual void Load(Dictionary<string, string> param) { }

		public virtual void Show(float time, string easeType){}
		public virtual void Hide(float time, string easeType){}
    }

}
