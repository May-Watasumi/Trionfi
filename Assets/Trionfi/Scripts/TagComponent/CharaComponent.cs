using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NovelEx {



	/*		
--------------

[doc]
tag=chara_new
group=キャラクター関連
title=キャラクター定義

[desc]
キャラクターを新しく定義します。

[sample]
[chara_new name="yuko" tag="scene1" jname="優子" ]

#yuko
上記のように書くことでキャラクター名前欄に優子を表示することができる

[param]
name=識別するための名前を指定します
storage=画像ファイルを指定します
tag=タグ名を指定できます。
layer=表示させるレイヤを指定します。デフォルトはcharacterが採用されます
sort=同一レイヤ内の表示順を整数で指定してください 数値が小さいほど背面に表示されます
x=表示位置について中心からのx位置を指定します
y=表示位置について中心からのy位置を指定します
scale=キャライメージの拡大率を指定します。つまり2と指定すると大きさが２倍になります
jname=キャラクター名前欄に表示する名前を設定しておくことができます。
jcolor=jnameの表示色を指定することができます。

[_doc]
--------------------
 */

	public class Chara_newComponent : Image_newComponent
    {
		public Chara_newComponent() : base() {
			//画像のルートパスが異なってくる
			base.imagePath = StorageManager.Instance.PATH_CHARA_IMAGE;
		}

		public override IEnumerator Start() {
			paramDic["className"] = "Chara";

			paramDic ["layer"] ="character";
			paramDic["imagePath"] = StorageManager.Instance.PATH_CHARA_IMAGE;

			//jname jcolor  名前表示のときに色と名前を指定できます
			if (paramDic.ContainsKey ("jname"))
				ScriptDecoder.Instance.variable.set("_chara_jname." + paramDic["name"], paramDic["jname"]);

			if (paramDic.ContainsKey ("jcolor"))
				ScriptDecoder.Instance.variable.set("_chara_jcolor." + paramDic["name"], paramDic["jcolor"]);

			base.Start();
            yield return null;
        }
    }


	/*	
--------------

[doc]
tag=chara_pos
group=キャラクター関連
title=キャラクター位置設定

[desc]
キャラクター画像の表示位置を指定することができます。

[sample]
[chara_pos name="name" x=2.5 y=1.5 ]

[param]
name=位置変更する対象の名前を指定します
x=中心からのx位置を指定します
y=中心からのy位置を指定します
scale_x=X方向へのイメージの拡大率を指定します。
scale_y=Y方向へのイメージの拡大率を指定します。
scale=イメージの拡大率を指定します。つまり2と指定すると大きさが２倍になります

[_doc]
--------------------
 */

	public class Chara_posComponent : Image_posComponent
    {
		public Chara_posComponent() : base() { }
		public override IEnumerator Start()
        {
            base.Start();
            yield return null;
        }
    }

	/*	
--------------

[doc]
tag=chara_show
group=キャラクター関連
title=キャラクター登場。

[desc]
chara_newで定義したキャラクターを登場させます。

[sample]

[chara_new name="yuko" storage="yuko"]
;キャラクターを表示
[chara_show name=yuko ]

;tagを指定して複数キャラを一斉に表示することも可能
[chara_show tag=chara time=3 wait=false]


[param]
name=表示させる対象の名前を指定します
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
	public class Chara_showComponent:Image_showComponent
    {
		public Chara_showComponent() : base() { }
		public override IEnumerator Start()
        {
            base.Start();
            yield return null;
        }
    }

	/*	
--------------

[doc]
tag=chara_hide
group=キャラクター関連
title=キャラクター退場

[desc]
画面に表示しているキャラクターを非表示にします。
chara_hideタグで非表示にしたキャラクターは
いつでもchara_showで再登場させることができます。

[sample]

;キャラクターを表示する
[chara_show name=yuko ]

;キャラクターを退場させる
[chara_hide name=yuko time=3 wait=false]

;キャラクターをもう一度表示
[chara_show name=yuko ]


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
	public class Chara_hideComponent : Image_hideComponent
    {
		public Chara_hideComponent():base() {  }
        public override IEnumerator Start()
        {
            base.Start();
            yield return null;
        }
    }

	/*	
--------------

[doc]
tag=chara_face
group=キャラクター関連
title=キャラクター表情登録

[desc]
chara_faceで表情を登録しておくことで
それ以後は表情名を指定するだけで画像を切替える事ができるようになります。

[sample]

;キャラクター定義
[chara_new name="hiro" storage="chara" scale="1.2"]

;キャラクターの表情登録
[chara_face name="hiro" face="no1" storage=angry ]
[chara_face name="hiro" face="no2" storage=sad ]

;キャラクターの表示
[chara_show name="hiro" ]

;キャラクターの表情変更
[chara_mod name="hiro" face="no1"]

;最初に登録した表情にはdefaultと指定することで変更できる
[chara_mod name="hiro" face="default" ]


[param]
name=識別するための名前を指定します
face=登録する表情名を指定します
storage=画像ファイル名を指定します。ファイルはcharacterフォルダ以下に配置してください

[_doc]
--------------------
 */

	//キャラの表情登録用
	public class Chara_faceComponent : Image_faceComponent
    {
		public Chara_faceComponent():base() { base.imagePath = StorageManager.Instance.PATH_CHARA_IMAGE; }
		public override IEnumerator Start()
        {
            base.Start();
            yield return null;
        }
    }


	/*	
--------------

[doc]
tag=chara_mod
group=キャラクター関連
title=キャラクター変更

[desc]
定義済みのキャラクター情報を変更します。

[sample]

;キャラクター定義
[chara_new name="hiro" storage="chara" scale="1.2"]

;キャラクターの表情登録
[chara_face name="hiro" face="no1" storage=angry ]
[chara_face name="hiro" face="no2" storage=sad ]

;キャラクターの表示
[chara_show name="hiro" ]

;キャラクターの表情変更
[chara_mod name="hiro" face="no1"]

;直接画像ファイルを指定して変更することも可能
[chara_mod name="hiro" storage="test"]


[param]
name=識別するための名前を指定します
face=image_face で指定した表情名を指定できます
storage=画像ファイル名を直接指定できます。フォルダはcharacter 以下を参照します。
time=変更にかかる時間を秒で指定します。デフォルトは１（秒）です
wait=変更を待つかどうかを true false で指定します。デフォルトは true です。
type=変更のされ方を指定できます。デフォルトはlinear です。

[_doc]
--------------------
 */


	//IComponentTextはテキストを流すための機能を保持するためのインターフェース
	public class Chara_modComponent:Image_modComponent
    {
		public Chara_modComponent() : base() { }
		public override IEnumerator Start()
        {
            base.Start();
            yield return null;
        }
    }

	/*
--------------

[doc]
tag=chara_remove
group=キャラクター関連
title=キャラクター削除

[desc]
キャラクター定義を削除します。
使用しなくなったキャラクターを削除することにより
メモリを節約することでゲームが快適に動作することができます。
このタグでキャラクターを削除した場合、再度表示するときは
新たに[chara_new]する必要があります。

[sample]
[chara_remove name="yuko"]

[param]
name=削除するキャラクターをchara_new の時に設定したnameを指定します。all と入力することですべてのキャラクターを削除することができます。


[_doc]
--------------------
 */
	//IComponentTextはテキストを流すための機能を保持するためのインターフェース
	public class Chara_removeComponent : Image_removeComponent
    {
		public Chara_removeComponent() : base() { }
		public override IEnumerator Start()
        {
            base.Start();
            yield return null;
        }
    }
}
