using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Trionfi
{
	//3Dモデルデータを取り扱います
	public class SdObject : AbstractObject
	{
		//foreとbackを持つ
		//private string name;
		private GameObject image;
		private SpriteRenderer spriteRenderImage;
		private Sprite targetSprite;
		private bool isShow = true;
		public string filename = "";
		private float show_x_position = 0; 

		//イメージオブジェクト新規作成
		public override void Load(ParamDictionary param)
		{
			paramDic = param;
            gameObject.tag = param["tag"];
            gameObject.name = param["name"];

            //ToDo:
            GameObject g = Resources.Load (/*GameSetting.PATH_SD_OBJECT +*/ "fbx/" + paramDic ["storage"]) as GameObject;

			float x = float.Parse (paramDic ["x"]);
			float y = float.Parse (paramDic ["y"]);
			float z = float.Parse (paramDic ["z"]);

			show_x_position = x;

			gameObject.name = param["name"];

			this.image = g;

			gameObject.AddComponent<Renderer>();

			float scale = float.Parse (paramDic ["scale"]);
			gameObject.transform.localScale = new Vector3 (scale, scale, scale);

			float rot_x = float.Parse (paramDic ["rot_x"]);
			float rot_y = float.Parse (paramDic ["rot_y"]);
			float rot_z = float.Parse (paramDic ["rot_z"]);

			gameObject.transform.Rotate (new Vector3 (rot_x, rot_y, rot_z));

			/*
						this.spriteRenderImage = this.image.GetComponent<SpriteRenderer>();
						this.targetSprite = this.spriteRenderImage.sprite;

						Color tmp = this.spriteRenderImage.color;
						tmp.a = float.Parse (paramDic ["a"]);
						this.spriteRenderImage.color = tmp;
			*/

			//透明度を設定できる


		}
/*        
		public override void SetColider()
		{
			gameObject.AddComponent<BoxCollider2D>();
			BoxCollider2D b = gameObject.GetComponent<BoxCollider2D>();
			b.isTrigger = true;
            b.enabled = isShow;
		}

		public override void  PlayAnimation(string state)
		{					
			Animator a = gameObject.GetComponent<Animator>();
			a.SetBool (state, true);
		}

		public override void StopAnimation(string state)
		{
			Animator a = gameObject.GetComponent<Animator>();
			a.SetBool (state, false);
		}
*/
		public override void Show(float time, string easeType)
		{
			isShow = true;
            gameObject.SetActive(true);
            Vector3 v = gameObject.transform.position;

			v.x = show_x_position;
			gameObject.transform.position = v;
				
		}

		public override void SetScale (float scale_x, float scale_y, float scale_z)
		{
			this.image.transform.localScale = new Vector3 (scale_x, scale_y, 1);
		}

		public override void Hide(float time, string easeType)
		{
			isShow = false;
			gameObject.SetActive(false);
						
			Vector3 v = gameObject.transform.position;
			v.x = -500f;
			gameObject.transform.position = v;

			//非表示にする
			//this.rootObject.GetComponent<Renderer>().enabled = true
		}
    }
}
