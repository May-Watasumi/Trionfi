using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace NovelEx
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
		public override void Init(Dictionary<string,string> param)
		{
			paramDic = param;
//ToDo:
			GameObject g = Resources.Load (/*GameSetting.PATH_SD_OBJECT +*/ "fbx/" + paramDic ["storage"]) as GameObject;

			float x = float.Parse (paramDic ["x"]);
			float y = float.Parse (paramDic ["y"]);
			float z = float.Parse (paramDic ["z"]);

			this.show_x_position = x;

			instanceObject = (GameObject)GameObject.Instantiate(g, new Vector3 (x, y, z), Quaternion.identity); 
			instanceObject.name = param["name"];

			this.image = g;

			instanceObject.AddComponent<Renderer>();

			float scale = float.Parse (paramDic ["scale"]);
			instanceObject.transform.localScale = new Vector3 (scale, scale, scale);

			float rot_x = float.Parse (paramDic ["rot_x"]);
			float rot_y = float.Parse (paramDic ["rot_y"]);
			float rot_z = float.Parse (paramDic ["rot_z"]);

			instanceObject.transform.Rotate (new Vector3 (rot_x, rot_y, rot_z));

			/*
						this.spriteRenderImage = this.image.GetComponent<SpriteRenderer>();
						this.targetSprite = this.spriteRenderImage.sprite;

						Color tmp = this.spriteRenderImage.color;
						tmp.a = float.Parse (paramDic ["a"]);
						this.spriteRenderImage.color = tmp;
			*/

			//透明度を設定できる


		}
        
		public override void SetColider()
		{
			instanceObject.AddComponent<BoxCollider2D>();
			BoxCollider2D b = instanceObject.GetComponent<BoxCollider2D>();
			b.isTrigger = true;
            b.enabled = isShow;
		}

		public override void  PlayAnimation(string state)
		{					
			Animator a = instanceObject.GetComponent<Animator>();
			a.SetBool (state, true);
		}

		public override void StopAnimation(string state)
		{
			Animator a = instanceObject.GetComponent<Animator>();
			a.SetBool (state, false);
		}

		public override void Show(float time, string easeType)
		{
			isShow = true;
            instanceObject.SetActive(true);
            Vector3 v = instanceObject.transform.position;

			v.x = show_x_position;
			instanceObject.transform.position = v;
				
		}

		public override void SetScale (float scale_x, float scale_y, float scale_z)
		{
			this.image.transform.localScale = new Vector3 (scale_x, scale_y, 1);
		}

		public override void Hide(float time, string easeType)
		{
			isShow = false;
			instanceObject.SetActive(false);
						
			Vector3 v = instanceObject.transform.position;
			v.x = -500f;
			instanceObject.transform.position = v;

			//非表示にする
			//this.rootObject.GetComponent<Renderer>().enabled = true
		}
    }
}
