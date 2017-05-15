using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NovelEx {
	//完了通知用のデリゲートメソッド
	public delegate void CompleteDelegate();

	public abstract class AbstractComponent
    {
 		//デフォルトで定義しておくパラメータ初期値。継承先で定義する
		public Dictionary<string,string> originalParamDic = new Dictionary<string,string>();
		public Dictionary<string,string> paramDic = new Dictionary<string,string>();

		public List<string> arrayVitalParam = new List<string>();

		public string line;
		public int line_num;
		public string tagName;

		//命令の種類を保持する
		protected Tag tag;
		protected CompleteDelegate finishAnimationDeletgate;

		//EX:NextOrderが必要かどうか
		protected bool nextOrder = true;

		public AbstractComponent() { }

		public void Init(Tag tag, int line_num)
        {
			this.tag = tag;
			this.tagName = tag.Name;
			this.line_num = line_num;
            this.finishAnimationDeletgate = OnAnimationFinished;
		}

		public void CheckParam() {
			//タグから必須項目が漏れていないか、デフォルト値が入ってない場合はエラーとして警告を返す
			foreach (string vital in arrayVitalParam)
            {
				if(tag.getParam (vital) == null)
                {
					//エラーを追加
					string message = "必須パラメータ「" + vital + "」が不足しています";
					ErrorLogger.addLog(message, "", line_num, false);
				}
			}
		}

		//EX:自動で次へ進むかどうか。基本的にtrueが多いはず
		public virtual bool allowNextOrder() { return nextOrder; }

        //アニメーション完了時の処理委譲先
        public virtual void OnAnimationFinished() { }

        //継承先で渡されたパラメータについて正常かどうかをチェックします
        public virtual void Validate() { }

		//スタート前にかならず実行されます。skip中でも実行されるのでチェックしたい項目があれば、継承先で実装してください
		public virtual void Before() { }

		//EX変更点：スタート後ににかならず実行されます。skip中でも実行されるのでチェックしたい項目があれば、継承先で実装してください
		public virtual void After() { }

		//実行前にパラメータを解析して変数を格納する
		public void CalcVariable()
        {
			Dictionary<string,string> tmp_param = new Dictionary<string,string>();

			//タグに入れる
			foreach (KeyValuePair<string, string> pair in originalParamDic)
            {
				tmp_param[pair.Key] = ExpObject.replaceVariable(pair.Value/*originalParamDic[pair.Key]*/);
			}

			//タグにデフォルト値を設定中かつ、tag が指定されていない場合
			if(StatusManager.Instance.TagDefaultVal != "")
            {
				if (tmp_param.ContainsKey("tag") && tmp_param["tag"] =="")
					tmp_param["tag"] = StatusManager.Instance.TagDefaultVal;
			}

            paramDic = tmp_param;
		}

		//パラメータとの差分を確認して、ファイルを作成
		public void MergeDefaultParam()
        {
			Dictionary<string,string> param = this.tag.getParamByDictionary();

			//タグに入れる
			foreach(KeyValuePair<string, string> pair in param) {
				originalParamDic[pair.Key] = pair.Value;

				/*
				if (paramDic.ContainsKey (pair.Key)) {
					paramDic [pair.Key] = pair.Value;
				} else {

					string message = "パラメータ「" + pair.Key + "」は存在しません";
					gameManager.addMessage(MessageType.Warning,this.line_num, message);

				}
				*/
			}
		}

        public virtual string GetText() { return ""; }

        public void Show() { 
//			Debug.Log ("this is show:" + this.tag.Original);
		}
		//始まった時
		abstract public void Start();
	}

/*

[doc]
tag=story
title=テキスト表示
group=メッセージ関連

[desc]
テキストを表示するタグです。
通常シナリオはタグを使用せずに記述しますが
タグを使用することも可能です

[sample]

;下記２つは全く同一の動作
ストーリーを記述[p]
[story val="ストーリーを記述"]

[param]

val=表示するテキストを指定します

[_doc]

 */
	//IComponentTextはテキストを流すための機能を保持するためのインターフェース
	public class StoryComponent : AbstractComponent {
		public StoryComponent() {
			//必須項目
			arrayVitalParam = new List<string> {
				"val"
			};

			originalParamDic = new Dictionary<string,string>() {
				{ "val","" }
			};
		}

		public override void Start()
        {
//			if(	StatusManager.Instance.currentState == JokerState.NextOrder)
//				StatusManager.Instance.MessageShow();

			string message = paramDic["val"];
			//			StatusManager.Instance.enableNextOrder = false;
//ToDo:
			Trionfi.Instance.currentMessageWindow.ShowMessage(message);
//			string color = TRUtility.RGBToHex(JOKEREX.Instance.uiInstance.color);
//			JOKEREX.Instance.LogManager.AddLog(JOKEREX.characterName, color, message);
			//JOKEREX.Instance.MainMessage.coroutineShowMessage(message);
//			while (JOKEREX.Instance.MainMessage.isShow) {
//				StatusManager.Instance.wait(0.02f);
//			}
//			JOKEREX.Instance.MainMessage.showMessage(message);
		
//			if(StatusManager.Instance.currentState == JokerState.MessageShow)
//    		nextOrder = false;
			//this.gameManager.nextOrder();
			//Debug.Log(this.tag.getParam("val"));
		}
	}
		
/*
--------------

[doc]
tag=r
group=メッセージ関連
title=改行する

[desc]
改行をします。

[sample]

テキスト表示[r]
２行目テキスト表示[r]
３行目テキスト表示[r]

[param]

[_doc]
--------------------
 */

	//改行命令 [r]
	public class RComponent : AbstractComponent
    {
		public RComponent()
        {
			originalParamDic = new Dictionary<string,string>() { };
		}

		public override void Start()
        {
            Trionfi.Instance.currentMessageWindow.currentMessage.text += "\n";
		}
	}

	/*
--------------

[doc]
tag=l
group=メッセージ関連
title=クリック待ち

[desc]
このタグの位置でクリック待ちを行います

[sample]
いち[l]に[l]さん[l][r]


[param]

[_doc]
--------------------
 */

	public class LComponent : AbstractComponent
    {
		public LComponent()
        {
			//デフォルトのパラメータを指定
			originalParamDic = new Dictionary<string,string>() { };
		}

		public override void Start() {
			//一旦処理を止めてクリックを待つ
			nextOrder = false;
			StatusManager.Instance.WaitClick();
//			StatusManager.Instance.enableNextOrder = true;
		}
	}

	/*
--------------

[doc]
tag=p
group=メッセージ関連
title=改ページクリック待ち

[desc]
改ページをともなうクリック待ちを行います

[sample]
テキスト表示[p]
１度クリックを待って[l]
２度めのクリックで改ページ[p]

[param]

[_doc]
--------------------
 */

	//改ページをいれて、クリックを待つ [r]
	public class PComponent:AbstractComponent
    {
		public PComponent() {
			originalParamDic = new Dictionary<string,string>()
            {
				{ "name","" },
				{ "","" }
			};
		}

		public override void Start()
        {
				nextOrder = false;
				StatusManager.Instance.PageWait();
		}
	}

	/*
--------------

[doc]
tag=cm
group=メッセージ関連
title=改ページクリック無し

[desc]
このタグに到達した時点でメッセージをクリアします

[sample]

テキスト表示[p]
ここはクリック待たないで消える[cm]
ここはクリック待ち[p]

[param]

[_doc]
--------------------
 */

	public class CmComponent : AbstractComponent {
		public CmComponent() {
			originalParamDic = new Dictionary<string,string>() { };
		}

		public override void Start() {
            //画面に表示されている文字列も消す
            //			StatusManager.Instance.enableNextOrder = true;
            //ToDo:
            Trionfi.Instance.currentMessageWindow.ClearMessage();
//			JOKEREX.Instance.LogManager.ApplyLog();
		}
	}

	//IComponentTextはテキストを流すための機能を保持するためのインターフェース
	public class NoneComponent : AbstractComponent
	{
		public NoneComponent()
		{
			originalParamDic = new Dictionary<string,string>() {
				{ "name","" },
				{ "","" }
			};
		
		}

		public override void Start()
        {	
//			Debug.Log ("none component start");
		
			//タグのファイルを取得
//			Debug.Log (this.tag.Original);
		}
	}
}
