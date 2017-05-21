using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NovelEx
{

/*	
--------------

[doc]
tag=image_new
group=イメージ関連
title=イメージの定義

[desc]
イメージを新しく定義します

[sample]
[image_new name="logo" tag=system ]

[param]
name=識別するための名前を指定します
storage=画像ファイルを指定します
tag=タグ名を指定できます
layer=表示させるレイヤを指定します。画面の背面から順に、background,Default,character,message,front が指定できます。デフォルトはDefaultが指定されます
sort=同一レイヤ内の表示順を整数で指定してください
x=中心からのx位置を指定します
y=中心からのy位置を指定します
z=中心からのz位置を指定します
scale_x=X方向へのイメージの拡大率を指定します。
scale_y=Y方向へのイメージの拡大率を指定します。
scale_z=Z方向へのイメージの拡大率を指定します。
scale=イメージの拡大率を指定します。つまり2と指定すると大きさが２倍になります

[_doc]
--------------------
 */

	//IComponentTextはテキストを流すための機能を保持するためのインターフェース
	public class Image_newComponent : AbstractComponent {
		protected string imagePath = "";

		public Image_newComponent() {
			//必須項目
			arrayVitalParam = new List<string> {
//				"name",
				"storage",
                "type"
			};

			originalParamDic = new ParamDictionary() {
				{ "name",""},
				{ "storage",""},
				{ "tag",""},
				{ "layer","Default"},
				{ "sort","3"},
//				{ "imagePath", StorageManager.Instance.PATH_IMAGE},
				{ "x","0"},
				{ "y","0"},
				{ "z","0"},
//				{ "scale",""},
				{ "scale_x","1"},
				{ "scale_y","1"},
				{ "scale_z","1"},
				{ "strech", "false"},
//				{ "path","false"}, //trueにすると、pathを補完しない
			};
		}

		public override IEnumerator Start()
        {
            /*
			if (paramDic ["scale"] != "")
            {
				paramDic ["scale_x"] = paramDic ["scale"];
				paramDic ["scale_y"] = paramDic ["scale"];
				paramDic ["scale_z"] = paramDic ["scale"];
		
			}
			else
				paramDic ["scale"] = "1";
           */
			TRImageObjectBehaviour g = ImageObjectManager.Instance.Create(paramDic["name"], TRObjectType.BG);
            g.Load(paramDic);
            yield return null;
        }
    }

	/*	
--------------

[doc]
tag=image_pos
group=イメージ関連
title=イメージの表示位置設定

[desc]
image_newで定義した画像の表示位置を指定することができます。

[sample]
[image_pos name="logo" x=2.5 y=1.5 ]

[param]
name=識別するための名前を指定します
x=中心からのx位置を指定します
y=中心からのy位置を指定します
scale_x=X方向へのイメージの拡大率を指定します。
scale_y=Y方向へのイメージの拡大率を指定します。
scale=イメージの拡大率を指定します。つまり2と指定すると大きさが２倍になります

[_doc]
--------------------
 */

	//キャラのポジションを変更する
	public class Image_posComponent : AbstractComponent {
		public Image_posComponent() {

			//必須項目
			arrayVitalParam = new List<string> {
				"name" 
			};

			originalParamDic = new ParamDictionary() {
				{ "name",""},
				{ "x",""},
				{ "y",""},
				{ "z",""},
				{ "scale_x",""},
				{ "scale_y",""},
				{ "scale_z",""},
				{ "scale",""},
			};
		}

		public override IEnumerator Start() {
			string name = paramDic["name"];
            TRImageObjectBehaviour image = ImageObjectManager.Instance.Find(paramDic["name"]);
            image.param = paramDic;
            /*
                        TRImageObjectBehaviour image = ImageObjectManager.Instance.Find(paramDic["name"]);

                        float x = (paramDic["x"]!="") ? float.Parse(paramDic["x"]) : float.Parse(image.GetParam("x"));
                        float y = (paramDic ["y"] != "") ? float.Parse (paramDic ["y"]) : float.Parse(image.GetParam ("y"));
                        float z = (paramDic["z"]!="") ? float.Parse(paramDic["z"]) : float.Parse(image.GetParam ("z"));

                        image.SetPosition (x, y, z);

                        //scaleが指定されている場合はそっちを優先
                        if (paramDic ["scale"] != "") {
                            paramDic ["scale_x"] = paramDic ["scale"]; 
                            paramDic ["scale_y"] = paramDic ["scale"];
                            paramDic ["scale_z"] = paramDic ["scale"];
                        }

                        float scale_x = (paramDic["scale_x"]!="") ? float.Parse(paramDic["scale_x"]) : float.Parse(image.GetParam ("scale_x"));
                        float scale_y = (paramDic["scale_y"]!="") ? float.Parse(paramDic["scale_y"]) : float.Parse(image.GetParam ("scale_y"));
                        float scale_z = (paramDic["scale_z"]!="") ? float.Parse(paramDic["scale_z"]) : float.Parse(image.GetParam ("scale_z"));

                        image.SetScale(scale_x,scale_y,scale_z);
            */
            yield return null;

            //アニメーション中にクリックして次に進めるかどうか。
            //			JOKEREX.Instance.ImageManager.nextOrder();
        }
    }

	/*	
--------------

[doc]
tag=image_show
group=イメージ関連
title=イメージを表示します。

[desc]
image_newで定義した画像を表示します

[sample]
;name ロゴを表示
[image_show name=logo ]

;tagを指定して複数画像を一斉に表示することも可能
[image_show tag=logo time=3 wait=false]


[param]
name=識別するための名前を指定します
tag=識別するためのタグを指定します
x=中心からのx位置を指定します
y=中心からのy位置を指定します
z=中心からのz位置を指定します
time=表示にかかる時間を秒で指定します。デフォルトは１（秒）です
wait=表示の完了を待つかどうかを true false で指定します。デフォルトは true です。
type=表示のされ方を指定できます。デフォルトはlinear です。

[_doc]
--------------------
 */

	//IComponentTextはテキストを流すための機能を保持するためのインターフェース
	public class Image_showComponent : AbstractComponent {
		private List<string> images;
		private bool isWait = false;

		public Image_showComponent() {
			//必須項目
			arrayVitalParam = new List<string> { };		//	"name" 

		    originalParamDic = new ParamDictionary() {
				{ "name",""},
				{ "tag",""},
				{ "x",""},
				{ "y",""},
				{ "z",""},
				{ "time","1"},
				{ "wait","true"},
				{ "type","linear"}
			};
		}

		public override IEnumerator Start() {
			string name = paramDic ["name"];
			string tag = paramDic ["tag"];
			string type = paramDic["type"];
			float time = float.Parse(paramDic["time"]);
			bool flag_delegate = true;

			List<TRImageObjectBehaviour> images;

            if (tag != "")
                images = ImageObjectManager.Instance.GetImageByTag(tag);
            else
            {
                images = new List<TRImageObjectBehaviour>();
                images.Add(ImageObjectManager.Instance.Find(name));
            }

            if (StatusManager.Instance.onSkip)
				time = 0.0f;

			//アニメーション中にクリックして次に進めるかどうか。
			if(time > 0.0f && paramDic["wait"] != "false")
            {
				nextOrder = false;
				StatusManager.Instance.Wait();
				isWait = true;
			}

			foreach(AbstractObject image in images)
            {
			//ToDo:
//				image.SetPosition (x, y, z);

				if(isWait) {
					//設定するのは一つだけ
					if (flag_delegate == true) {
						flag_delegate = false;
//						image.SetFinishAnimationDelegate(this.finishAnimationDeletgate);
					}
				}
				image.Show(time, type);	
			}
            yield return null;

        }

        public override void OnAnimationFinished()
        {
//			if(paramDic ["wait"] == "true") {
			if(isWait) {
				StatusManager.Instance.NextOrder();
//				StatusManager.Instance.enableNextOrder = true;
//				StatusManager.Instance.clickNextOrder();
			}
		}

	}

	/*	
--------------

[doc]
tag=image_hide
group=イメージ関連
title=イメージを非表示にします

[desc]
image_newで定義した画像を非表示にします。

[sample]
;name ロゴを表示
[image_show name=logo ]

;tagを指定して複数画像を一斉に表示することも可能
[image_hide name=logo time=3 wait=false]


[param]
name=識別するための名前を指定します
tag=識別するためのタグを指定します
time=非表示にかかる時間を秒で指定します。デフォルトは１（秒）です
wait=非表示の完了を待つかどうかを true false で指定します。デフォルトは true です。
type=非表示のされ方をしていできます。デフォルトはlinear です。

[_doc]
--------------------
 */
	//IComponentTextはテキストを流すための機能を保持するためのインターフェース
	public class Image_hideComponent : AbstractComponent {
		bool isWait = false;

		public Image_hideComponent() {
			//必須項目
			arrayVitalParam = new List<string> { }; //	"name",

			originalParamDic = new ParamDictionary() {
				{ "name",""},
				{ "tag",""},
				{ "time","1"},
				{ "type","linear"},
				{ "wait","true"},
			};
		}

		public override IEnumerator Start() {
			string name = paramDic["name"];
			string type = paramDic["type"];
			string tag = paramDic ["tag"];
			float time = float.Parse(paramDic["time"]);

			bool flag_delegate = true;

            List<TRImageObjectBehaviour> images;

            if (tag != "")
                images = ImageObjectManager.Instance.GetImageByTag(tag);
            else
            {
                images = new List<TRImageObjectBehaviour>();
                images.Add(ImageObjectManager.Instance.Find(name));
            }

            if (StatusManager.Instance.onSkip)
				time = 0.0f;

			//アニメーション中にクリックして次に進めるかどうか。
			if (time > 0.0f && paramDic["wait"] != "false")
			{
				nextOrder = false;
				StatusManager.Instance.Wait();
				isWait = true;
			}

			foreach(TRImageObjectBehaviour image in images) {
				if(isWait) {
					//設定するのは一つだけ
					if (flag_delegate == true) {
						flag_delegate = false;
//						image.SetFinishAnimationDelegate(finishAnimationDeletgate);
					}
				}
				image.Hide(time, type);
			}
            yield return null;

        }

        public override void OnAnimationFinished()
        {
			//アニメーション完了後にここにくる
			if(isWait) {
				StatusManager.Instance.NextOrder();
//				StatusManager.Instance.enableNextOrder = true;
//				StatusManager.Instance.clickNextOrder();
			}
		}
	}
	/*	
--------------

[doc]
tag=image_face
group=イメージ関連
title=イメージの表情を変更できます

[desc]
image_faceを登録しておくことで、以後は表情名を指定するだけで画像を切替える事ができるようになります。

[sample]
;name ロゴを表示
[image_show name=logo ]

;画像の表情を登録
[image_face face=logo2 storage="other_logo"]

;faceを指定するだけで画像の切り替えが可能になる
[image_mod name=logo face=logo2]

;chara_new で指定した画像はdefault という名前で指定できます
[image_mod name=logo face=default ]



[param]
name=識別するための名前を指定します
face=image_face で指定した表情名を指定できます
storage=画像ファイル名を直接していできます。フォルダはdata/images/image 以下を参照します。

[_doc]
--------------------
 */

	//キャラの表情登録用
	public class Image_faceComponent : AbstractComponent {
		protected string imagePath = "";

		public Image_faceComponent()
        {
//			this.imagePath = StorageManager.Instance.PATH_IMAGE;

			//必須項目
			arrayVitalParam = new List<string> {
				"name",
				"face",
				"storage"
			};

			originalParamDic = new ParamDictionary() {
				{ "name",""},
				{ "face",""},
				{ "storage",""},
			};
		}

		public override IEnumerator Start() {
			string name = paramDic ["name"];
			string face = paramDic ["face"];
			string storage = paramDic["storage"];

            TRImageObjectBehaviour image = ImageObjectManager.Instance.Find(name);

			ImageObjectManager.Instance.Find(name);
            //this.gameManager.nextOrder();
            //this.gameManager.scene.MessageSpeed = 0.02f;
            //this.gameManager.scene.coroutineShowMessage (message);
            yield return null;

        }

        public override void Validate()
		{
//			ToDo:
			Debug.Log("ToDo:Validate");
			//string storage = this.imagePath + paramDic ["storage"];
			//ToDo
			//this.gameManager.addMessage(MessageType.Error,this.line_num,Validate.checkStorage(storage));
		}

	}

	/*	
--------------

[doc]
tag=image_mod
group=イメージ関連
title=イメージを変更します

[desc]
image_newで定義した画像の情報を変更します。

[sample]
;name ロゴを表示
[image_show name=logo ]

;tagを指定して複数画像を一斉に表示することも可能
[image_mod name=logo storage="other_logo"]


[param]
name=識別するための名前を指定します
face=image_face で指定した表情名を指定できます
storage=画像ファイル名を直接していできます。フォルダはdata/images/image 以下を参照します。
time=変更にかかる時間を秒で指定します。デフォルトは１（秒）です
wait=変更を待つかどうかを true false で指定します。デフォルトは true です。
type=変更のされ方を指定できます。デフォルトはlinear です。

[_doc]
--------------------
 */

	public class Image_modComponent : AbstractComponent{ 
		public Image_modComponent() {

			//必須項目
			arrayVitalParam = new List<string> {
				"name"
			};

			originalParamDic = new ParamDictionary() {
				{ "name",""},
				{ "face",""},
				{ "storage",""},
				{ "time","1"},
				{ "wait","true"},
				{ "type","linear"}
			};
		}

		public override IEnumerator Start() {
			StatusManager.Instance.Wait();
//			StatusManager.Instance.enableNextOrder = false;

			string name = paramDic ["name"];
			string face = paramDic ["face"];
			string storage = paramDic ["storage"];

			float time = float.Parse (paramDic["time"]);
			string type = paramDic ["type"];

            TRImageObjectBehaviour image = ImageObjectManager.Instance.Find(name);

            //storage指定が優先される
            if (storage != "")
                image.param = paramDic;
            else
                image.Load(paramDic);

			if (StatusManager.Instance.onSkip || time <= 0.02f)
			{
//				image.OnAnimationFinished();
                yield break;
			}

			nextOrder = false;

			//処理を待たないなら
			if (paramDic ["wait"] == "false") {
				StatusManager.Instance.NextOrder();
//				StatusManager.Instance.enableNextOrder = true;
//				this.gameManager.nextOrder();
			}
			else
//				image.SetFinishAnimationDelegate(this.finishAnimationDeletgate);
            yield return null;

        }

        public override void OnAnimationFinished()
        {
			//アニメーション完了後にここにくる

			if (paramDic ["wait"] == "true") {
				StatusManager.Instance.NextOrder();
//				StatusManager.Instance.enableNextOrder = true;
//				StatusManager.Instance.clickNextOrder();
			}
		}
	}

	/*	
--------------

[doc]
tag=image_remove
group=イメージ関連
title=イメージの削除

[desc]
イメージ定義を削除します。
使用しなくなったイメージを削除することにより
メモリを節約することでゲームが快適に動作することができます。
このタグでキャラクターを削除した場合、再度表示するときは
新たに[image_new]する必要があります。

[sample]
[image_remove name="logo"]

[param]
name=削除するimageをimage_new の時に設定したnameを指定します。all と入力することですべてのイメージを削除することができます。
tag=削除するimageをimage_new の時に設定したtagを指定します。指定したtagが振られているイメージを一括削除できます



[_doc]
--------------------
 */
	//IComponentTextはテキストを流すための機能を保持するためのインターフェース
	public class Image_removeComponent : AbstractComponent {
		public Image_removeComponent() {
			//必須項目
			arrayVitalParam = new List<string> { };	//"name"

			originalParamDic = new ParamDictionary() {
				{ "name",""},
				{ "tag",""},
			};
		}

		public override IEnumerator Start() {
			string tag = paramDic ["tag"];
			string name = paramDic ["name"];

            List<TRImageObjectBehaviour> images;
            if (tag != "")
                images = ImageObjectManager.Instance.GetImageByTag(tag);
            else
            {
                images = new List<TRImageObjectBehaviour>();
                images.Add(ImageObjectManager.Instance.Find(name));
            }
			foreach(TRImageObjectBehaviour image in images)
            {
				//Image image = this.gameManager.imageManager.getImage (image_name);
//				ImageObjectManager.Instance.Remove(image.GetParam("name"));
			}
            yield return null;

            //JOKEREX.Instance.ImageManager.gameManager.nextOrder();
            //this.gameManager.scene.MessageSpeed = 0.02f;
            //this.gameManager.scene.coroutineShowMessage (message);
        }
    }


	/*	
	--------------

	[doc]
	tag=show
	group=イメージ関連
	title=画面上のオブジェクトの表示

	[desc]
	image_show chara_show text_show の短縮形です。
	すべての要素に適応できます。

	[sample]
	[image_new name="logo" tag=system ]
	@show name="logo"

	[param]
	name=識別するための名前を指定します
	tag=識別するためのタグを指定します
	x=中心からのx位置を指定します
	y=中心からのy位置を指定します
	z=中心からのz位置を指定します
	time=表示にかかる時間を秒で指定します。デフォルトは１（秒）です
	wait=表示の完了を待つかどうかを true false で指定します。デフォルトは true です。
	type=表示のされ方を指定できます。デフォルトはlinear です。


	[_doc]
	--------------------
	 */

	public class ShowComponent : Image_showComponent
    {
		public ShowComponent() : base() { }
		public override IEnumerator Start()
        {
            base.Start();
            yield return null;
        }
    }

	/*	
--------------

[doc]
tag=hide
group=イメージ関連
title=オブジェクトを非表示にします

[desc]
image_hide text_hide chara_hide の短縮形です
すべての要素について非表示に出来ます。

[sample]
;name ロゴを表示
[show name=logo ]

;tagを指定して複数画像を一斉に表示することも可能
[hide name=logo time=3 wait=false]


[param]
name=識別するための名前を指定します
tag=識別するためのタグを指定します
time=非表示にかかる時間を秒で指定します。デフォルトは１（秒）です
wait=非表示の完了を待つかどうかを true false で指定します。デフォルトは true です。
type=非表示のされ方をしていできます。デフォルトはlinear です。

[_doc]
--------------------
 */


	public class HideComponent : Image_hideComponent {
		public HideComponent() : base() { }
		public override IEnumerator Start()
        {
            base.Start();
            yield return null;
        }
    }

	/*	
--------------

[doc]
tag=remove
group=イメージ関連
title=オブジェクトの削除

[desc]
image_remove chara_remove text_remove の短縮形です
オブジェクトをゲーム上から削除します

[sample]
[remove name="logo"]

[param]
name=削除するimageをimage_new の時に設定したnameを指定します。all と入力することですべてのイメージを削除することができます。
tag=削除するimageをimage_new の時に設定したtagを指定します。指定したtagが振られているイメージを一括削除できます


[_doc]
--------------------
 */

	public class RemoveComponent : Image_removeComponent {
		public RemoveComponent() : base() { }
		public override IEnumerator Start()
        {
            base.Start();
            yield return null;
        }
    }
}
