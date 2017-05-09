using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NovelEx {


	/*	
--------------

[doc]
tag=anim
group=アニメーション関連
title=イメージのアニメーション

[desc]
newしている画像をアニメーションさせることができます。
tagを指定することで複数ファイルを一斉にアニメーションさせることもできます。

[sample]

;画像の定義
[image_new name="test" storage="test"]

;イメージを表示
[image_show name="test" ]

;X=3 Y=3に５秒かけてアニメーションしながら移動します
[anim name="test" time=5 x=3 y=3]


[param]
name=識別するための名前を指定します
tag=タグ名を指定できます
time=秒数を指定します。指定した秒数をかけてアニメーションを行います
type=アニメーションの演出を指定できます。指定できるtypeについては http://www.robertpenner.com/easing/easing_demo.html が参考になります
wait=アニメーションの完了を待つかどうかを指定します。trueで完了まで待ちます。
x=アニメーション先のX座標を指定します。
y=アニメーション先のY座標を指定します。
scale=大きさをアニメーションできます。拡大率を指定します。例えば2.0 だと２倍の大きさになります。

[_doc]
--------------------
 */


	public class AnimComponent:AbstractComponent {
		public AnimComponent() {
			//string imagePath = GameSetting.PATH_ANIM_FILE;
			//必須項目
			this.arrayVitalParam = new List<string> { 	};

			this.originalParam = new Dictionary<string,string>() {
				{ "name",""},
				{ "tag",""},
				{ "time","1"},
				{ "type","linear"},
				{ "wait","true"},
				{ "x",""},
				{ "y",""},
				{ "z",""},
				{ "scale",""},
			};
		}

		public override void start() {
			string name = this.param ["name"];
			string tag = this.param ["tag"];

			float time = float.Parse(this.param ["time"]);

			List<string> images = new List<string>();

			if (tag != "")
				images = JOKEREX.Instance.ImageManager.getImageNameByTag(tag);	
			else
				images.Add (name);

			foreach (string image_name in images) {
				Image image = JOKEREX.Instance.ImageManager.getImage(image_name);

				float x = (this.param ["x"]!="") ? float.Parse(this.param["x"]) : float.Parse(image.getParam("x"));
				float y = (this.param ["y"] != "") ? float.Parse (this.param ["y"]) : float.Parse(image.getParam ("y"));
				float z = (this.param ["z"] != "") ? float.Parse (this.param ["z"]) : float.Parse(image.getParam ("z"));

				//Debug.Log ("anim--------------");
				//Debug.Log (x + ":" + y + ":" + z);

				float scale = (this.param["scale"]!="") ? float.Parse(this.param["scale"]) : float.Parse(image.getParam ("scale"));

				if (this.param ["wait"] == "true") {
					//クリック無効にする
					//ToDo
					//JOKEREX.Instance.StatusManager.enableClickOrder = false;
					JOKEREX.Instance.StatusManager.Wait();
					image.getObject().animCompleteDeletgate = this.finishAnimation;
				}
			
				image.animPosition (new Vector3(x,y,z),scale,time,this.param["type"]);

				nextOrder = false;
				if (this.param["wait"] != "true") {
					JOKEREX.Instance.StatusManager.NextOrder();
//					this.gameManager.nextOrder();
					nextOrder = true;
				}
			}
		}

		public override void finishAnimation() {
		if (this.param ["wait"] == "true") {
				JOKEREX.Instance.StatusManager.NextOrder();
//				nextOrder = true;
//				this.gameManager.nextOrder();
//				JOKEREX.Instance.StatusManager.enableClickOrder = true;
			}
		}
	}
}




