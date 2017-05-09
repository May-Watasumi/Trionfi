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

		public override void init (Dictionary<string,string> param)
		{
			this.param = param;
//ToDo:
			GameObject g = Resources.Load (/*GameSetting.PATH_SD_OBJECT +*/ "fbx/" + this.param ["storage"]) as GameObject;

			float x = float.Parse (this.param ["x"]);
			float y = float.Parse (this.param ["y"]);
			float z = float.Parse (this.param ["z"]);

			this.show_x_position = x;

			this.rootObject = (GameObject)Instantiate (g, new Vector3 (x, y, z), Quaternion.identity); 
			this.rootObject.name = this.name;

			this.image = g;

			this.rootObject.AddComponent<Renderer>();

			float scale = float.Parse (this.param ["scale"]);
			this.rootObject.transform.localScale = new Vector3 (scale, scale, scale);

			float rot_x = float.Parse (this.param ["rot_x"]);
			float rot_y = float.Parse (this.param ["rot_y"]);
			float rot_z = float.Parse (this.param ["rot_z"]);

			this.rootObject.transform.Rotate (new Vector3 (rot_x, rot_y, rot_z));

			/*
						this.spriteRenderImage = this.image.GetComponent<SpriteRenderer>();
						this.targetSprite = this.spriteRenderImage.sprite;

						Color tmp = this.spriteRenderImage.color;
						tmp.a = float.Parse (this.param ["a"]);
						this.spriteRenderImage.color = tmp;
			*/

			//透明度を設定できる


		}

		public override void set (Dictionary<string,string> param)
		{

			if (this.rootObject == null) {
				this.init (param);
			}


		}

		public override void setColider()
		{

			this.rootObject.AddComponent<BoxCollider2D>();
			BoxCollider2D b = this.rootObject.GetComponent<BoxCollider2D>();
			b.isTrigger = true;
			if (this.isShow == true) {
				b.enabled = true;
			} else {
				b.enabled = false;
			}

		}

		public override void playAnim (string state)
		{
					
			Animator a = this.rootObject.GetComponent<Animator>();
			a.SetBool (state, true);

		}

		public override void stopAnim (string state)
		{

			Animator a = this.rootObject.GetComponent<Animator>();
			a.SetBool (state, false);

		}

		public override void show (float time, string easeType)
		{

			this.isShow = true;
			this.enabled = true;
			Vector3 v = this.rootObject.transform.position;

			v.x = this.show_x_position;
			this.rootObject.transform.position = v;
				
		}

		public override void setScale (float scale_x, float scale_y, float scale_z)
		{

			this.image.transform.localScale = new Vector3 (scale_x, scale_y, 1);

		}

		public override void hide (float time, string easeType)
		{

			this.isShow = false;
			this.enabled = false;
						
			Vector3 v = this.rootObject.transform.position;
			v.x = -500f;
			this.rootObject.transform.position = v;

			//非表示にする
			//this.rootObject.GetComponent<Renderer>().enabled = true;


		}
		//アニメーションの終了をいじょうするための
		private void finishAnimation()
		{

			if (this.completeDeletgate != null) {
				this.completeDeletgate();
			}

		}
		// Use this for initialization
		void Start()
		{


		}
		// Update is called once per frame
		void Update()
		{

		}
	}
}