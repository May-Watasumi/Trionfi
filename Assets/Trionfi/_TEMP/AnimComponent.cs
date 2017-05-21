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

    public class AnimComponent : AbstractComponent
    {
        public AnimComponent()
        {
            //string imagePath = GameSetting.PATH_ANIM_FILE;
            //必須項目
            arrayVitalParam = new List<string> { };

            originalParamDic = new ParamDictionary() {
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

        public override IEnumerator Start()
        {
            string name = paramDic["name"];
            string tag = paramDic["tag"];
            yield return null;
        }
    }
}
/*
        float time = float.Parse(paramDic ["time"]);

			List<string> images = new List<string>();

			if (tag != "")
				images = JOKEREX.Instance.ImageManager.getImageNameByTag(tag);	
			else
				images.Add (name);

			foreach (string image_name in images) {
				ImageObject image = JOKEREX.Instance.ImageManager.getImage(image_name);

				float x = (paramDic ["x"]!="") ? float.Parse(paramDic["x"]) : float.Parse(image.getParam("x"));
				float y = (paramDic ["y"] != "") ? float.Parse (paramDic ["y"]) : float.Parse(image.getParam ("y"));
				float z = (paramDic ["z"] != "") ? float.Parse (paramDic ["z"]) : float.Parse(image.getParam ("z"));

				//Debug.Log ("anim--------------");
				//Debug.Log (x + ":" + y + ":" + z);

				float scale = (paramDic["scale"]!="") ? float.Parse(paramDic["scale"]) : float.Parse(image.getParam ("scale"));

				if (paramDic ["wait"] == "true") {
					//クリック無効にする
					//ToDo
					//StatusManager.Instance.enableClickOrder = false;
					StatusManager.Instance.Wait();
					image.getObject().animCompleteDeletgate = this.finishAnimation;
				}
			
				image.animPosition (new Vector3(x,y,z),scale,time,paramDic["type"]);

				nextOrder = false;
				if (paramDic["wait"] != "true") {
					StatusManager.Instance.NextOrder();
//					this.gameManager.nextOrder();
					nextOrder = true;
				}
			}
		}

        public override void OnAnimationFinished()
        {
		    if (paramDic ["wait"] == "true") {
				    StatusManager.Instance.NextOrder();
    //				nextOrder = true;
    //				this.gameManager.nextOrder();
    //				StatusManager.Instance.enableClickOrder = true;
			    }
		    }
	}
}
*/