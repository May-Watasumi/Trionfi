using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NovelEx {

	/*	
--------------

[doc]
tag=skipstart
group=ゲーム操作関連
title=スキップ開始

[desc]
メッセージのスキップを開始します。
skipstopタグに到達するか
プレイヤーが画面をクリックするまで自動的に読み進みます。

[sample]

ここまでは通常通り表示[p]
[skipstart]
ここからスキップが開始されます


[_doc]
--------------------
 */


	//イベント登録用のコンポーネント
	public class SkipstartComponent:AbstractComponent{
		public SkipstartComponent() { }

		public override void start() {
			//skip start 
			JOKEREX.Instance.StatusManager.startSkip();
//			StatusManager.enableNextOrder = true;
//			this.gameManager.nextOrder();
//			this.gameManager.scene.startSkip();
		}


	}

	/*	
--------------

[doc]
tag=skipstop
group=ゲーム操作関連
title=スキップ停止

[desc]
メッセージのスキップ状態の場合
強制的にメッセージスキップを停止することができます。
スキップさせたくない場面の直前に配置しておくと良いかと思います。

[sample]

[skipstart]
ここはスキップ[p]
[skipstart]
ここでスキップが停止される。

[_doc]
--------------------
 */

	//このタグに到達した時点で、強制的にスキップを停止します
	public class SkipstopComponent:AbstractComponent {
		public SkipstopComponent() { }

		public override void start()
		{
			JOKEREX.Instance.StatusManager.stopSkip();
//			this.gameManager.scene.stopSkip();
		}


	}

	/*	
--------------

[doc]
tag=savesnap
group=データ関連
title=セーブスナップの作製

[desc]
このタグに到達した時点でゲームの状態を
savesnap.savというファイルに格納することができます。
後に[save]を実行した時にこのsavesnapの状態がセーブされます。

gamesleepタグを使った場合は暗黙的にsavesnapが実行されます。
デフォルトのセーブ機能ではgamesleep時に保存しておいたデータを遷移先のセーブ画面で保存するという実装になっています。
セーブ機能をカスタマイズする場合、このsavesnapをよく理解してください

[sample]

[savesnap]
セーブ状態を格納。

;上記のsavesnapタグ時点のデータがsave_0として保存される
[save name="save_0"]


[_doc]
--------------------
 */

	//セーブ用の状態を作る
	public class SavesnapComponent:AbstractComponent {
		public SavesnapComponent() { }

		public override void start() {
			JOKEREX.Instance.Serializer.Serialize("savesnap", true);
//			this.gameManager.nextOrder();
		}
	}

	/*	
--------------

[doc]
tag=sleepgame
group=データ関連
title=ゲームの一時停止

[desc]
このタグに到達した時点でゲームの状態を
gametmpとsavesnapに保存した上で、他のシナリオへ移動することができます。
そして遷移先で[awakegame]が実行されたらgametmpから状態を再現してゲームに復帰できます。

このタグは本流のゲームから一時的に画面を遷移したい場合に非常に強力に機能します。

例えば、ゲームの途中でコンフィグの設定を行いたい場合などは
sleepgameで進行状態を保持した上で、コンフィグ画面に移動します。[awakegame]タグでゲームに復帰します

[sample]

[sleepgame file="libs/save" target="*savestart"]

遷移先でawakegameが実行されるとここに戻ってくる

[param]
file=ゲームを中断して処理を始めるファイル名を記述します。省略された場合は、現在のファイル名と解釈されます
target=ジャンプする先のラベル名を指定できます。省略されている場合は先頭位置からと解釈されます
savesnap=trueを指定するとゲームを一時停止する直前の状態をsavesnapに保存します

[_doc]
--------------------
 */

	//ゲーム情報を一時退避させる
	public class SleepgameComponent:AbstractComponent {
		public SleepgameComponent() {
			//必須項目
			this.arrayVitalParam = new List<string>{ };

			this.originalParam = new Dictionary<string,string>() {
				{ "file","" },
				{ "target","" },
				{ "savesnap","true" }, //セーブ用に使うなら。。。使わないかな
			};
		}

		public override void start() {
//			GameManager gameManager = NovelSingleton.GameManager;
			JOKEREX.Instance.Serializer.Serialize("gametmp", true); //一時領域に退避させる。これはawakeのタイミングで戻ってくるために使う。
			JOKEREX.Instance.Serializer.Serialize("savesnap", true); //セーブ用のスナップも同じタイミングで作る

			Debug.Log ("Save success");

			//ジャンプする シーンをnew状態で
			string tag_str ="[jump file='"+this.param["file"]+"' target='"+ this.param["target"] +"' scene=new ]";

			Debug.Log (tag_str);

			//タグを実行
			AbstractComponent cmp = JOKEREX.Instance.ScenarioManager.NovelParser.makeTag (tag_str);
			cmp.start();
		}
	}

	/*	
--------------

[doc]
tag=awakegame
group=データ関連
title=一時停止から復帰

[desc]

[gamesleep]タグで一時していたゲームを再開します。

[sample]

;gamesleep状態から復帰
[awakegame]

[param]

[_doc]
--------------------
 */

	public class AwakegameComponent:AbstractComponent {
		public AwakegameComponent() {
		}

		public override void start() {
			//次にシナリオ読み込みます。nextOrder はしてほしくない。
			nextOrder = false;
			JOKEREX.Instance.StatusManager.NextOrder();
			JOKEREX.Instance.Serializer.Deserialize("gametmp");
			//Application.LoadLevel("NovelPlayer");
		}
	}


	/*	
--------------

[doc]
tag=autostart
group=データ関連
title=オートメッセージ

[desc]
一定間隔でストーリーを読み進める事ができます。
画面クリックでオートモードがキャンセルされます。

[sample]

[autostart]
ここからは[p]
一定間隔で[p]
自動的に読み進まれます[p]

[param]
time=何秒間隔で自動的にストーリーが進むかを設定することができます。

[_doc]
--------------------
 */


	//FlagSkiiping
	public class AutostartComponent:AbstractComponent {
		public AutostartComponent() {
			//必須項目
			this.arrayVitalParam = new List<string> {
//				"time"
			};

			this.originalParam = new Dictionary<string,string>() {
				{"time","-1"}
			};
		}

		public override void start() {
			float time = float.Parse(this.param["time"]);

			if(time < 0.0f)
				time = float.Parse(JOKEREX.Instance.ScenarioManager.variable.get("config.autoModeWait"));

			//string time = this.param ["time"];
			JOKEREX.Instance.StatusManager.StartAuto(time);
		}
	}
	public class AutostopComponent : AbstractComponent
	{
		public AutostopComponent()
		{
			//必須項目
			this.arrayVitalParam = new List<string> {
			};

			this.originalParam = new Dictionary<string, string>() {
			};
		}

		public override void start()
		{
			JOKEREX.Instance.StatusManager.StopAuto();
		}
	}


}
