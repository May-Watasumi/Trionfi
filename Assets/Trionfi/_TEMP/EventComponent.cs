using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NovelEx
{
//    ToDo:
#if false
    /*	
    --------------

    [doc]
    tag=evt
    group=イベント関連
    title=イベントの登録

    [desc]
    画面上のイメージに対してイベントを登録することができます。
    イベントが発生した際は指定した場所へジャンプをおこないます。
    ジャンプ先は一方通行でスタックが残りません（return で戻れない）
    イベントが発生した場所に戻りたい場合はジャンプ先でevt変数に呼び出し元情報が格納されているのでそれを活用します
    evt.caller_index = イベントが発生した箇所のindexが格納されています　 
    evt.caller_file  = イベントが発生したファイル名が格納されています
    evt.caller_name  = イベントが発生したイメージのnameが格納されています。

    [sample]

    [button_new name="button" text="ボタンです" ]
    [button_show name="button" ]

    [evt name="event_button" target="*jump_start" ]

    *jump_start
    {evt.caller_name}がクリックされました


    [param]
    name=イベントを登録するnameを指定します
    tag=指定タグに対してまとめてイベントを登録することができます
    act=補足するイベントの種類をしていします。例えばクリックなどを指定できます。
    file=イベントが発生した際にジャンプするファイル名を記述します。省略された場合は、現在のファイル名と解釈されます
    target=イベントが発生した際にジャンプする先のラベル名を指定できます。省略されている場合は先頭位置からと解釈されます



    [_doc]
    --------------------
     */

    //イベント登録用のコンポーネント
    public class EvtComponent : AbstractComponent {
		protected string imagePath = "";

		public EvtComponent() {
			this.imagePath = StorageManager.Instance.PATH_IMAGE;

			//必須項目
			arrayVitalParam = new List<string> { };

			originalParamDic = new ParamDictionary() {
				{ "name",""},
				{ "tag",""},
				{ "act","click"},
				{ "file",""},
				{ "target",""},
			};
		}

		public override IEnumerator Start() {
			string name = paramDic ["name"];
			string tag = paramDic ["tag"];

			List<string> events = new List<string>();

			if (tag != "")
				events = ImageObjectManager.GetImageByTag(tag);	
			else
				events.Add(name);


			//ファイルが指定されていない場合は現在のシナリオを格納する
			if(paramDic["file"] == "")
				paramDic["file"] = StatusManager.Instance.currentScenario;

			foreach(string object_name in events)
            {
				EventManager.addEvent(object_name, paramDic);
			}

//			this.gameManager.nextOrder();
		}
	}

	/*	
--------------

[doc]
tag=evt_remove
group=イベント関連
title=イベントの削除

[desc]
登録しておいたイベントを削除し、無効化します。

[sample]


[param]
name=イベントを無効にするnameを指定します
tag=指定タグに対してまとめてイベントを無効にすることができます


[_doc]
--------------------
 */

	public class Evt_removeComponent:AbstractComponent {
		protected string imagePath = "";

		public Evt_removeComponent() {
			this.imagePath = StorageManager.Instance.PATH_IMAGE;

			//必須項目
			arrayVitalParam = new List<string> { 	};

			originalParamDic = new ParamDictionary() {
				{ "name",""},
				{ "tag",""},
				{ "act",""},
			};
		}

		public override IEnumerator Start() {
			string name = paramDic ["name"];
			string tag = paramDic ["tag"];

			List<string> events = new List<string>();
			if (tag != "")
				events = ImageObjectManager.GetImageByTag(tag);	
			else
				events.Add(name);

			foreach (string object_name in events)
				EventManager.removeEvent(object_name);
		}
	}

	/*	
--------------

[doc]
tag=evt_stop
group=イベント関連
title=イベントの一時無効化

[desc]
すべてのイベントを一時無効化します。
例えばボタンが押された後、処理が完了するまで他のボタンを押してほしくない場合などに指定することができます。

[sample]


[param]


[_doc]
--------------------
 */

	//イベントをストップします。他のイベントを受け付けなくなりま
	public class Evt_stopComponent:AbstractComponent {

		protected string imagePath = "";

		public Evt_stopComponent() {
			this.imagePath = StorageManager.Instance.PATH_IMAGE;

			//必須項目
			arrayVitalParam = new List<string> { };
			originalParamDic = new ParamDictionary() { };
		}

		public override IEnumerator Start() {
			//例外として許可する
			ScriptDecoder.Instance.variable.remove("_evt_name_permission");

			//StatusManager.enableEventClick = false;
			StatusManager.Instance.isEventStop = true;
//			this.gameManager.nextOrder();
		}
	}

	/*	
--------------

[doc]
tag=evt_resume
group=イベント関連
title=イベントの再開

[desc]
evt_stopで無効化していたイベントを再度有効にします。

[sample]


[param]
name=イベントを無効にするnameを指定します
tag=指定タグに対してまとめてイベントを無効にすることができます


[_doc]
--------------------
 */

	public class Evt_resumeComponent:AbstractComponent {
		protected string imagePath = "";

		public Evt_resumeComponent() {
			this.imagePath = StorageManager.Instance.PATH_IMAGE;

			//必須項目
			arrayVitalParam = new List<string> { };

			originalParamDic = new ParamDictionary() {
				{ "name",""},
				{ "tag",""},
			};
		}

		public override IEnumerator Start() {
			//例外として許可するイベントを登録
			string name = paramDic ["name"];
			string tag = paramDic ["tag"];

			///タグが指定されている場合
			if (tag != "") {
				var events = ImageObjectManager.GetImageByTag(tag);	
			
				foreach (string object_name in events) {
					ScriptDecoder.Instance.variable.set("_evt_name_permission." + object_name, "1");
				}
			}
			else if (name != "") {
				ScriptDecoder.Instance.variable.set("_evt_name_permission." + name, "1");
			}
			else {
				ScriptDecoder.Instance.variable.remove("_evt_name_permission");

				//StatusManager.enableEventClick = true;
				StatusManager.Instance.isEventStop = false;

			}
//			this.gameManager.nextOrder();
		}
	}
#endif
}
