using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NovelEx {


	/*	
--------------

[doc]
tag=clickable
group=クリッカブル関連
title=クリッカブル表示

[desc]
画面にクリッカブル領域を配置することができます。
クリッカブルは画面をタッチすることでイベントを発生させることができる透明の領域のことです。
脱出ゲームなどで多用される技術です

開発中はパラメータの透明度を上げておいて、見えるようにして開発するのがよいでしょう。

[sample]

;クリッカブルを設定
[clickable name="click1" tag=clickable a=0 scale_x=2.37 scale_y =0.55 x=0.94 y=0.8 ]
[clickable name="click2" tag=clickable a=0 scale_x=0.4 scale_y=0.3 x=3.41 y=1.83 ]
[clickable name="click3" tag=clickable a=0 scale_x=1.31 scale_y=1.18 x=-6.08 y=1.52 ]

;クリックされた時にイベントを設定
[evt name="click1" target="*clickable1"]
[evt name="click2" target="*clickable2"]
[evt name="click3" target="*clickable3"]

[param]

name=表示するクリッカブルの名前を指定してください
tag=タグ名称を指定します
layer=クリッカブルを配置するレイヤを設定できます
sort=クリッカブルの表示順を指定できます。数値が大きいほど全面に配置されます
a=クリッカブルの透明度を指定できます 0〜1.0 で0が完全に透明を表します
x=表示位置のX座標を指定します
y=表示位置のY座標を指定します
scale_x=クリッカブルの横幅の拡大率を指定できます。
scale_y=クリッカブルの縦幅の拡大率を指定できます。

[_doc]
--------------------
 */

	public class ClickableComponent:AbstractComponent {
		public ClickableComponent()
        {
			//string imagePath = GameSetting.PATH_SYSTEM_IMAGE;

			//必須項目

			arrayVitalParam = new List<string>
            {
				"name"
			};

			originalParamDic = new Dictionary<string,string>() {
				{ "name", "" },
				{ "tag", "" },
				{ "layer", "Default" },
				{ "sort", "10" },
				{ "a", "0.3" },
				{ "x", "0" },
				{ "y", "0" },
				{ "z", "0" },
				{ "scale_x", "1" },
				{ "scale_y", "1" },
				{ "scale_z", "1" }
			};
		}

		public override IEnumerator Start() {
//			Debug.Log("start----------");
			//string name = paramDic ["name"];
			//string tag = paramDic ["tag"];
			paramDic ["storage"] = "black";
			paramDic["className"] ="Clickable";

            TRImageObjectBehaviour g = ImageObjectManager.Instance.Create(paramDic["name"], ObjectType.BG);
            g.Load(paramDic);
            yield return null;

            //			this.gameManager.nextOrder();
        }
    }


	//IComponentTextはテキストを流すための機能を保持するためのインターフェース
	public class Clickable_removeComponent : Image_removeComponent
    {
		public Clickable_removeComponent() : base() { }
        public override IEnumerator Start()
        {
            base.Start();
            yield return null;
        }
    }
}
