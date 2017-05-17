using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ExpressionParser;
using System;

namespace NovelEx {

	/*	
--------------

[doc]
tag=save
group=データ関連
title=セーブ

[desc]
このタグの場所でセーブを作成します
セーブデータはsavesnapから保存されます。
savesnapは[savesnap]タグを明示して作成するか
[gamesleep]を実行したタイミングで生成されます。

[sample]

[save name=save_01] 

[param]
name=セーブファイル名を指定します


[_doc]
--------------------
 */

	public class SaveComponent : AbstractComponent {
		public SaveComponent() {
			//必須項目
			arrayVitalParam = new List<string> {
				"name"
			};

			originalParamDic = new Dictionary<string,string>() {
				{ "name","" }
			};
		}

		public override IEnumerator Start()
        {
			string name = paramDic ["name"];

			//セーブを実行する。指定された名前で
			Serializer.Serialize(name);
            //			this.gameManager.nextOrder();
            yield return null;
        }
    }

	/*	
--------------

[doc]
tag=load
group=データ関連
title=ロード

[desc]
セーブしてあったデータをロードします。

[sample]
;現在のゲーム状態をすべて破棄してセーブデータを読み込む
[load name="save_01"]
[param]
name=ロードするセーブファイル名を指定します


[_doc]
--------------------
 */

	public class LoadComponent : AbstractComponent {
		public LoadComponent() {
			//必須項目
			arrayVitalParam = new List<string> {
				"name"
			};

			originalParamDic = new Dictionary<string,string>() {
				{ "name","" }
			};
		}

		public override IEnumerator Start() {
			string name = paramDic ["name"];
//ToDo:これはひどい。
			Serializer.Deserialize(name);
            //Application.LoadLevel("NovelPlayer");
            //this.gameManager.nextOrder();
            yield return null;
        }
    }


	/*		
--------------

[doc]
tag=saveloop
group=データ関連
title=セーブデータの列挙

[desc]
これはセーブ画面の制作で使用する機能です。
セーブファイルの情報を順番に列挙してループ処理することができます。

必ず対になる[end_saveloop]タグが必要です。

configファイルで指定されている最大セーブスロット数まで、0から順に列挙していきます。
save_0 save_1 save_2 …  というセーブファイルを列挙していく

ループ毎にsave変数にデータが格納されます。
取得できる情報は

save.title : セーブタイトル
save.date ：セーブ日時
save.description：セーブ説明文字列
save.name：セーブファイル名（ロード時に指定する名前でもある）

save.max_num：セーブスロットの最大数
save.index：現在のセーブインデックス
save.max_index：このページの最大インデックス
save.loop_start_component_index：


[sample]


[saveloop]

;セーブデータを表示
[trace exp="save"]

;セーブデータを表示し、セーブボタンを配置
@image_new storage="save" layer=ui name="{save.name}" tag="save_button" x=4 y={tmp.image_y} scale=1.5
@text_new name="text_date_{save.name}" anchor="MiddleLeft" cut=20 val="{save.date}" tag="samune" x=0.27 y={tmp.text_date_y}
@text_new name="text_{save.name}" anchor="MiddleLeft" cut=20 val="{save.title}" tag="samune" x=0.27 y={tmp.text_y}

[end_saveloop]



[param]
num=ループ回数
page=ページ。つまり、numが5でpageが1なら５〜10までのセーブデータがループで検索されます 

[_doc]
--------------------
 */


	public class SaveloopComponent : AbstractComponent {
		public SaveloopComponent() {
			//必須項目
			arrayVitalParam = new List<string> {
				//	"name"
			};

			originalParamDic = new Dictionary<string,string>() {
				{ "num","5" }, //一度に表示する数
				{ "page","0" }, // num*page から num個分表示するという意味
			};
		}

		public override IEnumerator Start()
        {
			int num = int.Parse (paramDic ["num"]);
			int page = int.Parse (paramDic ["page"]);

			//セーブを実行する。指定された名前で
			//ループに入る。
			int current_index = num * page;
            int max_num = SystemConfig.Instance.saveSlotCount;
			int max_index = current_index + num;

			//セーブ変数の初期化
			//ジャンプを実行する時に呼び出した位置情報を保持する
			ScriptDecoder.Instance.variable.set("save.max_num", "" + max_num);
			ScriptDecoder.Instance.variable.set("save.index", "" + current_index);
			ScriptDecoder.Instance.variable.set("save.max_index", "" + max_index);
			ScriptDecoder.Instance.variable.set("save.loop_start_component_index", "" + ScriptDecoder.Instance.currentComponentIndex);

			Serializer.applySaveVariable("save_" + current_index, ScriptDecoder.Instance.variable);

            //オートセーブのデータが欲しいですね
            //			this.gameManager.nextOrder();
            yield return null;

        }
    }


	/*		
--------------

[doc]
tag=end_saveloop
group=データ関連
title=セーブデータの列挙終了

[desc]

[saveloop]タグの終端を表します

[sample]



[_doc]
--------------------
 */


	public class End_saveloopComponent  :AbstractComponent {
		public End_saveloopComponent() {
			//必須項目
			arrayVitalParam = new List<string> {
				//	"name"
			};

			originalParamDic = new Dictionary<string,string>() {
			};
		}

		public override IEnumerator Start() {
			int index = int.Parse(ScriptDecoder.Instance.variable.get("save.index"));
			int max_num = int.Parse(ScriptDecoder.Instance.variable.get("save.max_num"));
			//int max_index = int.Parse (StatusManager.variable.get("save.max_index"));

			index++;

			nextOrder = false;
//ToDo:この辺整理
			//上限に来た場合は表示を止める
			if(max_num <= index) {
				nextOrder = true;
                //NovelSingleton.GameManager.nextOrder();
                yield break;
			}

			ScriptDecoder.Instance.variable.set("save.index", "" + index);
			Serializer.applySaveVariable("save_" + index, ScriptDecoder.Instance.variable);

			//ジャンプする。[saveloop]タグの次のところへ
			string loop_back_index = ScriptDecoder.Instance.variable.get("save.loop_start_component_index");

			string tag_str = "[jump index='" + loop_back_index + "' ]";
		
			//タグを実行
			AbstractComponent cmp = NovelParser.Instance.makeTag (tag_str);
			yield return cmp.Start();
        }
    }


/*		
--------------

[doc]
tag=autosave
group=データ関連
title=オートセーブ

[desc]

オートセーブを実行します
これにより次回起動時にプレイヤーは
この地点からゲームを再開することができるようになります。

スマートフォンなどの場合、中断と再開が多いので
定期的にautosaveを挟んでおいて、再開できるようにしておく設計が望ましいでしょう。

セーブデータはautosaveという名前で保存されます
ロード時はnameにautosaveと指定することで状態を復元することが可能です

[sample]

[autosave]

[_doc]
---------

*/

	//自動セーブ。スナップを作成します
	public class AutosaveComponent : AbstractComponent {
		public AutosaveComponent() {
			//必須項目
			arrayVitalParam = new List<string> {
				//	"name"
			};

			originalParamDic = new Dictionary<string,string>() {
			};
		}

		public override IEnumerator Start() {
			Serializer.Serialize("autosave");
            //			this.gameManager.nextOrder();
            yield return null;
        }
    }

	/*		
--------------

[doc]
tag=get_autosave
group=データ関連
title=オートセーブの取得

[desc]

オートセーブで保存されているデータを
変数に格納して使用することができます。

[sample]

;autoという変数にオートセーブのデータを代入
@get_autosave var=auto

;オートセーブされたデータが存在するなら
[if exp="{auto.date}!=''"]

オートセーブファイル：{auto.name}[r]
オートセーブ日時：{auto.date}[r]
オートセーブタイトル：{auto.title}[r]

[param]

var=オートセーブのデータを格納する変数名を指定

[_doc]
---------

*/

	public class Get_autosaveComponent : AbstractComponent {
		public Get_autosaveComponent() {

			//必須項目
			arrayVitalParam = new List<string> {
				"var"
			};

			originalParamDic = new Dictionary<string,string>() {
				{"var","auto"}
			};
		}

		public override IEnumerator Start() {
			string var_name = paramDic ["var"];
			Serializer.applySaveVariable("autosave", ScriptDecoder.Instance.variable, var_name);
            //this.gameManager.nextOrder();
            yield return null;
        }
    }
}
