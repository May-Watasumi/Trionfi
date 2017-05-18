
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NovelEx
{
		/*			
--------------

[doc]
tag=sd_new
group=3Dモデル関連
title=3Dモデル定義

[desc]

外部ツールで作成した3DモデルやMMDなどのデータをゲームに表示させます。
対応形式はfbx形式です

3Dモデルデータはsd/fbx フォルダ以下に配置してください

[sample]

;3Dモデルの定義
@sd_new name=miku storage="latmiku" x=5.37 y=-5.23 

;3Dモデルを表示
@sd_show name=miku


[param]

name=識別するための名前を指定します
storage=表示するfbxファイルを指定します。ファイルパスはsd/fbxフォルダ以下に配置してください
tag=タグ名を指定できます
layer=表示させるレイヤを指定します。画面の背面から順に、background,Default,character,message,front が指定できます。デフォルトはDefaultが指定されます
sort=同一レイヤ内の表示順を整数で指定してください
x=中心からのx位置を指定します
y=中心からのy位置を指定します
z=中心からのz位置を指定します
scale_x=X方向への3Dモデルの拡大率を指定します。
scale_y=Y方向への3Dモデルの拡大率を指定します。
scale_z=Z方向への3Dモデルの拡大率を指定します。
scale=3Dモデルの拡大率を一律していできます。つまり2を指定するとモデルが2倍の大きさになります。
rot_x=3DモデルのX軸角度を指定します。0〜360の間で指定します
rot_y=3DモデルのY軸角度を指定します。0〜360の間で指定します
rot_z=3DモデルのZ軸角度を指定します。0〜360の間で指定します

[_doc]
--------------------
 */

	public class Sd_newComponent : AbstractComponent {
		public Sd_newComponent() {
			//string imagePath = GameSetting.PATH_SD_OBJECT;

			//必須項目

			arrayVitalParam = new List<string> {
				"name",
				"storage"
			};

			originalParamDic = new Dictionary<string,string>() {
				{ "name",""},
				{ "storage",""},
				{ "tag",""},
				{ "layer","Default"},
				{ "sort","0"},
				//{ "a","0.3"},
				{ "x","0"},
				{ "y","0"},
				{ "z","6.66"},
				{ "scale","5"},
				{ "scale_x","1"},
				{ "scale_y","1"},
				{ "scale_z","1"},
				{ "rot_x","0"},
				{ "rot_y","180"},
				{ "rot_z","0"},
			};
		}

		public override IEnumerator Start() {
			//string name = paramDic ["name"];
			//string tag = paramDic ["tag"];
			paramDic["className"] ="Sd";

            TRImageObjectBehaviour g = ImageObjectManager.Instance.Create(paramDic["name"], ObjectType.BG);
            g.Load(paramDic);
            //			this.gameManager.nextOrder();
            yield return null;
        }
    }

	/*		
--------------

[doc]
tag=sd_show
group=3Dモデル関連
title=3Dモデル表示

[desc]
sd_newで定義した3Dモデルを画面に表示します

[sample]

;3Dモデルの定義
@sd_new name=miku storage="latmiku" x=5.37 y=-5.23 

;3Dモデルを表示
@sd_show name=miku


[param]
name=識別するための名前を指定します
tag=識別するためのタグを指定します
x=中心からのx位置を指定します
y=中心からのy位置を指定します
z=中心からのz位置を指定します

[_doc]
--------------------
 */

	//IComponentTextはテキストを流すための機能を保持するためのインターフェース
	public class Sd_showComponent:Image_showComponent
    {
		public Sd_showComponent() : base() { }
		public override IEnumerator Start()
        {
            base.Start();
            yield return null;
        }
    }


	/*	
--------------

[doc]
tag=sd_remove
group=3Dモデル関連
title=3Dモデル削除

[desc]
sd_newで定義した3Dモデルを削除します。
使用しなくなった3Dモデルはこのタグでこまめに削除することで
動作を軽くすることができます。

[sample]

;3Dモデルの定義
@sd_new name=miku storage="latmiku" x=5.37 y=-5.23 

;3Dモデルを表示
@sd_show name=miku

;3Dモデルの削除
@sd_remove name=miku


[param]
name=識別するための名前を指定します
tag=識別するためのタグを指定します

[_doc]
	  --------------------
	  */

	public class Sd_removeComponent : Image_removeComponent
    {
		public Sd_removeComponent() : base() { }
		public override IEnumerator Start()
        {
            base.Start();
            yield return null;
        }
    }


	/*	
--------------

[doc]
tag=sd_hide
group=3Dモデル関連
title=3Dモデル退場

[desc]
画面に表示している3Dモデルを非表示にします。
sd_hideタグで非表示にしたキャラクターは
いつでもsd_showで再登場させることができます。

[sample]

;3Dモデルの定義
@sd_new name=miku storage="latmiku" x=5.37 y=-5.23 

;3Dモデルを表示
@sd_show name=miku

;3Dモデルの非表示
@sd_hide name=miku

;3Dモデルを再度表示
@sd_show name=miku


[param]
name=識別するための名前を指定します
tag=識別するためのタグを指定します

[_doc]
--------------------
 */

	//IComponentTextはテキストを流すための機能を保持するためのインターフェース
	public class Sd_hideComponent:Image_hideComponent
    {
		public Sd_hideComponent() : base() { }
		public override IEnumerator Start()
        {
            base.Start();
            yield return null;
        }
    }


	/*	
--------------

[doc]
tag=sd_anim
group=3Dモデル関連
title=3Dモデルモーション再生

[desc]
画面に表示している3Dモデルにモーションを設定します。
モーションは予めMecanimで設定しておき、このタグからフラグを制御することで
３Dモデルに好きなタイミングでモーションを実行させることができます。

[sample]

;3Dモデルの定義
@sd_new name=miku storage="latmiku" x=5.37 y=-5.23 

;3Dモデルを表示
@sd_show name=miku

;3Dモデルにモーションを適応 NyanDanceをtrueにする
@sd_anim name=miku state="NyanDance"


[param]
name=識別するための名前を指定します
tag=識別するためのタグを指定します
state=Mecanimに設定されている変数にtrueを設定します。Mecahimではここで指定するステートのON OFFによって状態を変更するように設定します
condition=stateで指定するステートをtrue or falseで指定することができます。

[_doc]
--------------------
 */


	//IComponentTextはテキストを流すための機能を保持するためのインターフェース
	public class Sd_animComponent : AbstractComponent {
		public Sd_animComponent() {

			arrayVitalParam = new List<string> {
				"name","state"
			};

			originalParamDic = new Dictionary<string,string>() {
				{ "name",""},
				{ "tag",""},
				{ "stete",""},
				{ "condition","true"},
			};
		}

		public override IEnumerator Start() {
			string name = paramDic ["name"];
			string tag = paramDic ["tag"];
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
                /*
                if (paramDic ["condition"] == "true")
					image.PlayAnimation(paramDic["state"]);
				else
					image.StopAnimation(paramDic["state"]);
                */
			}
            //			this.gameManager.nextOrder();
            yield return null;
        }
    }
}
