using System;
using System.Collections;
using System.Collections.Generic;
//using ExpressionParser;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NovelEx {
    //データセーブ
    public class DatasaveComponent : AbstractComponent
    {
        public DatasaveComponent()
        {
            essentialParams = new List<string> { "file" };
        }

        protected override void TagFunction()
        {
            Serializer.Serialize(expressionedParams["file"], true);
        }
    }

    public class LabelComponent : AbstractComponent {
		public LabelComponent() {

			//必須項目
			essentialParams = new List<string> {
				"name"
			};

		}

		protected override void TagFunction()
        {
            //ToDo
		}
	}
    
 /*
[macro name="newtag"]
	新しいタグです。[p]
	{mp.arg1}という値が渡されました。	
[endmacro]

[newtag arg1="テスト"]
*/
	//マクロを作成して管理する
	public class MacroComponent : AbstractComponent {
		public MacroComponent() {
			//必須項目
			essentialParams = new List<string> {
				"name"
			};
		}

		protected override void TagFunction()
        {
			string name = expressionedParams ["name"];
//ToDo:
			ScriptDecoder.Instance.AddMacro(name, StatusManager.Instance.currentScenario, ScriptDecoder.Instance.currentComponentIndex);
        }
    }

	//マクロを実行するためのタグ
	public class _MacrostartComponent:AbstractComponent {
		public _MacrostartComponent() {

			//必須項目
			essentialParams = new List<string> {
				"name"
			};
		}

		protected override void TagFunction()
        {
			expressionedParams["name"] = tagName();

			ScriptDecoder.Macro macro = ScriptDecoder.Instance.GetMacro(expressionedParams["name"]);

            if (macro != null)
            {

                expressionedParams["index"] = "" + macro.index;
                expressionedParams["file"] = macro.file_name;

                ScriptDecoder.Instance.macroNum++;
                //this.gameManager.scenarioManager.addMacroStack (macro.name, this.expressionedParams);
                AbstractComponent cmp = TRScriptParser.Instance.makeTag("call", expressionedParams);
                cmp.Execute();
            }
            else
            {
                ErrorLogger.stopError("マクロ「" + expressionedParams["name"] + "」は存在しません。");
            }
        }
    }

	//マクロを作成して管理する
	public class EndmacroComponent : AbstractComponent
	{
		public EndmacroComponent() { }

		protected override void TagFunction() {
            //ToDo:あんまりきれいじゃない
			if(ScriptDecoder.Instance.macroNum > 0) {
				ScriptDecoder.Instance.macroNum--;

                AbstractComponent cmp = TRScriptParser.Instance.makeTag("[return]");
				cmp.Execute();
			}
			else
            {
//ToDo:
			}
        }
    }

	/*	
--------------

[doc]
tag=jump
group=シナリオ制御
title=別のシナリオ位置へジャンプ

[desc]
このタグの場所に到達するとfileとtargetで指定された場所へジャンプします

ジャンプ命令はcallスタックに残りません。つまり、return で指定位置に戻ることができません。
jump先では標準でcaller_index と caller_file という変数が格納されています。
これは、jumpした地点のファイルとindexを保持しています。
mp.caller_index と mp.caller_file を使うことで元の位置に戻ることが可能です

jumpには好きなパラメータを渡すことが可能です。
jump先ではmp.arg1 のような形で変数にアクセスすることが可能です。

scene=new とすることで、全く新しいシーンを新たに生成した上でジャンプすることができます。
まっさらな状態になるので、もう一度背景やキャラクター情報などを定義する必要があります。

場面の切り替わりなどではscene=newでjumpすることにより、不要なデータを一掃することで
健全な状態を保ってゲームを進めることができるできます。
ですので、定期的にscene=new でジャンプを行うことをオススメします。


[sample]

[jump taget=*test]
ここは無視される[p]

*test

ここにジャンプする。

[param]
file=移動するシナリオファイル名を指定します。省略された場合は現在のシナリオファイルと見なされます
target=ジャンプ先のラベル名を指定します。省略すると先頭から実行されます
index=内部的に保持しているゲーム進行状況の数値を指定することができます。
scene=new を指定すると、新しくシーンを作成した上でジャンプします。


[_doc]
--------------------
 */


	public class JumpComponent:AbstractComponent {
		public JumpComponent() {

			//必須項目
			essentialParams = new List<string> {
				//"target"
			};
/*
			originalParamDic = new ParamDictionary() {
				{ "target","" },
				{ "file","" },
				{ "index",""},
				{ "scene",""}, //ここにnew が入っている場合はジャンプ後にシーンをイチから作り直す。
				{ "next","true"}, //next にfalse が入っている場合、ジャンプ先でnextOrderを行いません。
			};
*/
		}

		protected override void TagFunction()
		{
			string target = this.expressionedParams["target"].Replace ("*", "").Trim();
			string file = this.expressionedParams["file"];

			if (file == "")
				file = StatusManager.Instance.currentScenario;

			int index = -1;

			//ファイルが異なるものになる場合、シナリオをロードする

			if (StatusManager.Instance.currentScenario != file)
			{
				//ToDo:
				ScriptDecoder.Instance.LoadScenario(file);
			}

			//index直指定の場合はそれに従う
			if (this.expressionedParams["index"] != "")
				index = int.Parse(this.expressionedParams["index"]);
			else
				index = ScriptDecoder.Instance.GetIndex(file, target);

			if(index == -1)
				index = 0;


//ToDo:
			ScriptDecoder.Instance.StartScenario(file, index);

			//シーンをクリアして作りなおす
			if (this.expressionedParams ["scene"] == "new") {
				//new の場合はスタックをすべて削除する
				ScriptDecoder.Instance.RemoveAllStacks();
				StatusManager.Instance.nextFileName = file;
				StatusManager.Instance.nextTargetName = target;

				//jumpから来たことを通知するためのパラメータが必要
				SceneManager.LoadScene("NovelPlayer");
			}

			Debug.Log("JUMP:scn=\"" + StatusManager.Instance.currentScenario + "\" " + "index=\"" + ScriptDecoder.Instance.currentComponentIndex + "\"");
            // + " param=\"" + this.expressionedParams.ToStringFull());

            //			if (this.expressionedParams ["next"] != "false") {
            //				nextOrder = false;
            //				StatusManager.Instance.currentState = JokerState.NextOrder;
            //			}
            //			else {
            //				this.gameManager.nextOrder();
            //			}

            //メインループ側で配列Indexが++されるので
            ScriptDecoder.Instance.currentComponentIndex--;
        }
	}

	/*	
--------------

[doc]
tag=call
group=シナリオ制御
title=サブルーチンの呼び出し

[desc]
指定されたシナリオファイルの指定されたラベルで示される サブルーチンを呼び出します。
呼び出されたサブルーチンは、 return タグで 呼び出し元や任意の場所に戻ることができます


[sample]

[call taget=*test]
サブルーチンが終わるとここに戻ってきます[p]

*test

ここにジャンプする。

[return]

[param]
file=呼び出したいサブルーチンのあるのシナリオファイルを 指定します。省略すると、現在 のシナリオファイル内であると見なされます
target=呼び出すサブルーチンのラベルを指定します。省略すると、ファイルの先頭から実行されます。


[_doc]
--------------------
 */

	//Call は Jumpと同様にストレージを移動する。ただし、呼び出しは スタックトレースに保存され、return で元の位置に戻ります
	public class CallComponent:AbstractComponent {
		public CallComponent() {
			//必須項目
			essentialParams = new List<string> {
				//"target"
			};
/*
			originalParamDic = new ParamDictionary() {
				{ "target","" },
				{ "file","" },
				//{ "index",""},
			};
*/
		}

		protected override void TagFunction()
		{
			string target = this.expressionedParams ["target"].Replace("*", "").Trim();
			string file = this.expressionedParams ["file"];

			string index = "";

			if (this.expressionedParams.ContainsKey("index"))
				index = this.expressionedParams ["index"];

			string tag_str ="[jump file='"+file+"' target='"+target+"' index="+ index +" ]";
//ToDo:
			Debug.Log("PUSH:scn=\"" + StatusManager.Instance.currentScenario + "\" " + "index=\"" + (ScriptDecoder.Instance.currentComponentIndex).ToString()+ "\"");

			ScriptDecoder.Instance.AddStack(StatusManager.Instance.currentScenario, ScriptDecoder.Instance.currentComponentIndex, this.expressionedParams);
			
			//タグを実行
			AbstractComponent cmp = TRScriptParser.Instance.makeTag(tag_str);
			cmp.Execute();

            //メインループ側で配列Indexが++されるので
			ScriptDecoder.Instance.currentComponentIndex--;

            //macro もひとつのcomponent_array みたいにしていいんじゃないかしら。ラベルじゃないけど
            //StackManager に　呼び出し状態を保持させる macro の中で別ファイルへのjumpは禁止したいね。
            //現在の位置をスタックとして保持させる
        }
	}


	/*	
--------------

[doc]
tag=return
group=シナリオ制御
title=サブルーチンから戻る

[desc]
サブルーチンから呼び出し元に戻ります。
return時にfileとtargetを指定することでスタックを消費した上で
任意の場所に戻ることもできます。

[sample]

[call taget=*test]
サブルーチンが終わるとここに戻ってきます[p]

*test

ここにジャンプする。

[return]

[param]
file=サブルーチンの呼び出し元に戻らずに、指定したファイルへ移動することできます。
target=サブルーチンの呼び出し元に戻らずに、指定したラベルへ移動することできます。


[_doc]
--------------------
 */

	public class ReturnComponent : AbstractComponent {
		public ReturnComponent() {
			//必須項目
			essentialParams = new List<string> {
				//"target"
			};
/*
			originalParamDic = new ParamDictionary() {
				{"file",""},
				{"target",""},
			};
*/
		}

		protected override void TagFunction() {
			ScriptDecoder.CallStack stack = ScriptDecoder.Instance.PopStack();

			string tag_str = "";

			//return 時の戻り場所を指定できます
			if (this.expressionedParams ["file"] != "" || this.expressionedParams ["target"] != "")
				tag_str = "[jump file='" + this.expressionedParams["file"] + "' target='" + this.expressionedParams["target"] + "' ]";
			else
				tag_str = "[jump file='" + stack.scenarioNname + "' index='" + stack.index + "' ]";

			Debug.Log("RETURN scn=\"" + stack.scenarioNname + "\" " + "index=\"" + stack.index.ToString()+ "\"");// + " param=\"" + this.expressionedParams.ToStringFull());

			//タグを実行
			AbstractComponent cmp = TRScriptParser.Instance.makeTag(tag_str);
			cmp.Execute();
        }
	}

/*	
--------------

[doc]
tag=scene
group=シナリオ制御
title=Unityのシーン呼び出し

[desc]
指定されたUnityシーンを呼び出します。
ジョーカーの会話シーンから、例えばアクションフェーズへの移動といったばあいに活用できます。
UnityのBuildSettingでScene in Buildに移動先のシーンを登録するのを忘れないようにしてください。

[sample]

[scene file=scene1 ]

[param]
file=呼び出したいシーン名


[_doc]
--------------------
 */
	public class SceneComponent : AbstractComponent {
		public SceneComponent() {
			//必須項目
			essentialParams = new List<string> {
				"file"
			};
		}

        AsyncOperation syncState;

        protected override void TagFunction()
        {
			string file = expressionedParams ["file"];

            if (syncWait)
                SceneManager.LoadScene(file);
            else
                syncState = SceneManager.LoadSceneAsync(file);
        }

        public override IEnumerator TagAsyncWait()
        {
            yield return new WaitUntil(() => syncState.isDone);
        }
    }


	/*	
--------------

[doc]
tag=calc
group=システム関連
title=数式の評価

[desc]
expで示された式を評価します。変数への値の代入などに使用されます。
文字列はこのタグでは扱うことはできません。文字列は[flag]タグを使用します

[sample]


[calc exp="f.test=500"]
;↑変数 test に数値を代入している

[calc exp="sf.test2=400"]
;↑システム変数 test に数値を代入している

[calc exp="f.test2={f.test}*3"]
;↑ゲーム変数 test2 に ゲーム変数 test の 3 倍の数値を代入している

{f.test2}[p]


[param]
exp=数式を指定します


[_doc]
--------------------
 */

	public class CalcComponent : AbstractComponent {
		public CalcComponent() {
			//必須項目
			essentialParams = new List<string> {
				"exp"
			};
		}

		protected override void TagFunction() {
			string exp = this.expressionedParams ["exp"];

			ExpObject eo = new ExpObject (exp);

			string result = ExpObject.calc (eo.exp);

			ScriptDecoder.Instance.variable.Set(eo.type + "." + eo.name, result);
        }
    }

	/*	
--------------

[doc]
tag=flag
group=システム関連
title=文字列の評価

[desc]
文字列を扱うことができます。

[sample]

[flag exp="f.test=ゆうこ"]
;↑変数 test に文字列を代入している

はじめまして{f.test}さん。[p]
;はじめましてゆうこさん。と表示される

;文字の連結
[flag exp="f.str = はじめまして{f.test}さん" ]
{f.str} 
;はじめましてゆうこさん。と表示される

[param]
exp=文字式を指定します


[_doc]
--------------------
 */

	public class FlagComponent : AbstractComponent {
		public FlagComponent() {
			//必須項目
			essentialParams = new List<string> {
				"exp"
			};
		}

		protected override void TagFunction() {
			string exp = expressionedParams ["exp"];

			ExpObject eo = new ExpObject (exp);
			ScriptDecoder.Instance.variable.Set(eo.type + "." + eo.name, eo.exp);
        }
    }


	/*	
--------------

[doc]
tag=emb
group=メッセージ関連
title=変数の展開

[desc]

メッセージ中に変数の中身を展開して表示することができます。
省略形として{ } で括る方法もあります。

[sample]


[flag exp="f.value1='変数の値だよ～ん'"]
とどこかで書いておいて、
[emb exp="f.value1"]
と書くと、この emb タグが 変数の値だよ～ん という内容に置き換わります。

[param]
exp=評価する変数を格納します。


[_doc]
--------------------
 */

	public class EmbComponent : AbstractComponent {
		public EmbComponent() {
			//必須項目
			essentialParams = new List<string> {
				"exp"
			};
		}

		protected override void TagFunction() {
			string exp = expressionedParams["exp"];
			string val = expressionedParams["exp"];

			//変数なら素直に代入
			if(val.IndexOf(".") != -1)
				ScriptDecoder.Instance.variable.Set(exp, val);

			string tag_str ="[story val='"+val+"' ]";

			AbstractComponent cmp = TRScriptParser.Instance.makeTag(tag_str);
			cmp.Execute();
        }
    }

	/*	
--------------

[doc]
tag=if
group=シナリオ関連
title=条件分岐

[desc]

式を評価し、その結果が true ( または 0 以外 ) ならば、 elsif・else・endif のいずれかまでにある
文章やタグを実行し、そうでない場合は無視します。

[sample]


; 例1
[if exp="false"]
ここは表示されない
[else]
ここは表示される
[endif]

; 例2
[if exp="false"]
ここは表示されない
[elsif exp="false"]
ここは表示されない
[else]
ここは表示される
[endif]

; 例3
[if exp="false"]
ここは表示されない
[elsif exp="true"]
ここは表示される
[else]
ここは表示されない
[endif]

; 例4
[if exp="true"]
ここは表示される
[elsif exp="true"]
ここは表示されない
[else]
ここは表示されない
[endif]


[param]
exp=評価する式を指定します。この式の結果が false ( または 0 な らば、elsif・else・endif タグまでの文章やタグが無視されます。

[_doc]
--------------------
 */

	public class IfComponent : AbstractComponent {
		public IfComponent() {
			//必須項目
			essentialParams = new List<string> {
				"exp"
			};
        }

		public override void Before() {
			//スキップ中ならここは通過しない
			ScriptDecoder.Instance.ifNum++;
		}

		protected override void TagFunction() {
			ScriptDecoder.Instance.AddIfStack(true);

			string exp = expressionedParams ["exp"];
			if (this.expressionedParams.ContainsKey ("mobile")) {
			
			}
			string result = ExpObject.calc (exp);

			//条件に合致した場合はそのままifの中へ
			if (result == "true") {
				//ifスタックが完了している
				ScriptDecoder.Instance.ChangeIfStack(false);
			}
			else {
				//elsif か　endif まで処理を進める
				StatusManager.Instance.setSkipOrder();
			}
        }
    }




	/*		
--------------

[doc]
tag=elsif
group=シナリオ関連
title=それまでの if の中身が実行されていなかったときに、条件付きで実行

[desc]

if タグと endif タグの間で用いられます。 それまでの if タグまたは elsif タグの中身がひとつも実行されていないときに 
式を評価し、その結果が真ならば elsif から次の elsif・else・endif までの間を実行します。
使い方の例については、if タグの項目を参照してください。

[sample]


[param]
exp=評価する変数を格納します。


[_doc]
--------------------
 */


//正直elseifに変更したい

	public class ElsifComponent : AbstractComponent {
		public ElsifComponent() {
			//必須項目
			essentialParams = new List<string> {
				"exp"
			};
		}

		public override void Before() {
			StatusManager.Instance.setSkipOrder();

			if (ScriptDecoder.Instance.CountIfStack() == ScriptDecoder.Instance.ifNum) {
				if (ScriptDecoder.Instance.CurrentIfStack() == true)
					StatusManager.Instance.releaseSkipOrder();
			}
		}

		protected override void TagFunction() {
			string exp = expressionedParams ["exp"];
			string result = ExpObject.calc (exp);

			//条件に合致した場合はそのままifの中へ
			if (result == "true") {
				//ifスタックが完了している
				ScriptDecoder.Instance.ChangeIfStack(false);
			}
			else
			{
				//elsif か　endif まで処理を進める
				StatusManager.Instance.setSkipOrder();
			}
        }
    }

	/*		
--------------

[doc]
tag=else
group=シナリオ関連
title=if の中身が実行されなかったときに実行

[desc]

if タグもしくは elsif タグ と endif タグの間で用いられます。 if または elsif ブロックの中身がひとつも実行されていないとき、 else から endif までの間を実行します。
使い方の例については、if タグの項目を参照してください。

[sample]

[param]


[_doc]
--------------------
 */

	public class ElseComponent : AbstractComponent {
		public ElseComponent() {
			//必須項目
			essentialParams = new List<string> {
			};
		}

		public override void Before() {
			StatusManager.Instance.setSkipOrder();

			if (ScriptDecoder.Instance.CountIfStack() == ScriptDecoder.Instance.ifNum) {
				if (ScriptDecoder.Instance.CurrentIfStack() == true)
					StatusManager.Instance.releaseSkipOrder();
			}
		}

		protected override void TagFunction() {
			ScriptDecoder.Instance.ChangeIfStack(false);
        }
    }

	/*		
--------------

[doc]
tag=endif
group=シナリオ関連
title=if文を終了します

[desc]

if文を終了します。必ずif文の終わりに記述する必要があります

[sample]


[param]


[_doc]
--------------------
 */

	public class EndifComponent : AbstractComponent {
		public EndifComponent() {
			//必須項目
			essentialParams = new List<string> {
			};
		}

		public override void Before() {
			//if文とスタックの数が同一の場合はスキップをやめて、endif を実行
			StatusManager.Instance.setSkipOrder();

			if (ScriptDecoder.Instance.CountIfStack() == ScriptDecoder.Instance.ifNum)
				StatusManager.Instance.releaseSkipOrder();		

			ScriptDecoder.Instance.ifNum--;
		}

		protected override void TagFunction() {
			//ifスタックが取り除かれる
			ScriptDecoder.Instance.PopIfStack();
        }
    }

	/*		
--------------

[doc]
tag=s
group=シナリオ関連
title=ゲームを停止する

[desc]

シナリオファイルの実行を停止します。
選択肢表示した直後などに配置して利用する方法があります。

[sample]

テストこの後はジャンプなどでsを飛び越える処理を記述して追う必要があります[p]
@jump target=label1

[s]

*label1
ジャンプで[s]を飛び越える

[param]


[_doc]
--------------------
 */

	public class SComponent : AbstractComponent {
		public SComponent() { }

		protected override void TagFunction() {
			StatusManager.Instance.InfiniteStop();
            //			StatusManager.Instance.enableNextOrder = false;
            //			StatusManager.Instance.enableClickOrder = false;
            //ToDo:SyncWait
            //その他 enableNextOrder が来るまで進めない
        }
    }


	/*		
--------------

[doc]
tag=tag_default
group=シナリオ関連
title=デフォルトでタグ名を付与する

[desc]

以降のタグにデフォルトでtagパラメータを付与することができます。
[reset_tag_default]を行うまでtagが付与されるので注意してください。

[sample]

;以後の命令にcharaというtag属性を付与する

@tag_default tag="chara"
[chara_new name="hiro1" storage="mayuri_clothes_earnest" scale="1.2"]
[chara_new name="hiro2" storage="mayuri_clothes_lonely"]
[chara_new name="hiro3" storage="senior01_looking_away"]
@reset_tag_default

;タグを付与したキャラクターを一度に表示する
@chara_show tag="chara"


[param]
tag=付与するタグ名を指定します

[_doc]
--------------------
 */

	public class Tag_defaultComponent : AbstractComponent
	{
		public Tag_defaultComponent()
		{
			//必須項目
			essentialParams = new List<string> {
				"tag"
			};
		}

		protected override void TagFunction()
		{
			//ifスタックが取り除かれる
			StatusManager.Instance.TagDefaultVal = expressionedParams["tag"];
        }
    }

	/*		
--------------

[doc]
tag=reset_tag_default
group=シナリオ関連
title=デフォルトタグ設定を解除する

[desc]

[default_tag]の効果を無効にします

[sample]


[param]


[_doc]
--------------------
 */

	public class Reset_tag_defaultComponent : AbstractComponent
	{
		public Reset_tag_defaultComponent() { }

		protected override void TagFunction()
		{
			//ifスタックが取り除かれる
			StatusManager.Instance.TagDefaultVal = "";
        }
    }

	//使用停止　ウィンドウを閉じる。クリックで再度表示されるやつ
	public class ClosemessageComponent : AbstractComponent
	{
		public ClosemessageComponent()
		{
			//必須項目
			essentialParams = new List<string> {
			};
		}

		protected override void TagFunction() {
        }
    }
    
    //メッセージウィンドウを非表示にします。
	public class HidemessageComponent : AbstractComponent {
		public HidemessageComponent() {
		}

		protected override void TagFunction()
		{
        }
    }
    
    //メッセージウィンドウを表示します。
    public class ShowmessageComponent : AbstractComponent {
		public ShowmessageComponent() {
		}

		protected override void TagFunction()
        {
        }
    }
    
    //ToDo:外部コンソールウィンドウへ
    //変数の中身をデバックコンソールで確認することができます。
	public class TraceComponent : AbstractComponent {
		public TraceComponent() {
            essentialParams = new List<string>
            {
                   "exp"
            };
        }

        protected override void TagFunction() {
			string exp = expressionedParams ["exp"];
			ScriptDecoder.Instance.variable.Trace(exp);
        }
    }
    
	//話者名変更
	public class Talk_nameComponent : AbstractComponent
	{
        public Talk_nameComponent()
        {
            essentialParams = new List<string>
            {
                   "val"
            };
        }

        protected override void TagFunction()
		{
			string name = this.expressionedParams ["val"];
            TRUIManager.Instance.currentMessageWindow.currentName.text = name;
        }
    }

	//ディレイ
	public class WaitComponent : AbstractComponent
	{
        float _time = 0.0f;
		public WaitComponent()
		{
			//必須項目
			essentialParams = new List<string> {
				"time"
			};
		}

        //時間を止める。
        protected override void TagFunction()
        {
            _time = float.Parse(expressionedParams["time"]);
		}

        public override IEnumerator TagAsyncWait()
        {
            yield return new WaitForSeconds(_time);
        }
    }
    
    //title=Webページヘジャンプします。
	public class WebComponent : AbstractComponent {
		public WebComponent() {
			//必須項目
			essentialParams = new List<string> {
				"url"
			};
		}

		protected override void TagFunction()
        {
			string url = expressionedParams ["url"];
			Application.OpenURL(url);
            //ToDo:
//            yield return null;
		}
	}

	//変数削除
	public class ClearvarComponent : AbstractComponent {
		public ClearvarComponent() {
			//必須項目
			essentialParams = new List<string> {
				"name"
			};
		}

		protected override void TagFunction() {
			//削除
			string name = expressionedParams["name"];
            ScriptDecoder.Instance.variable.Remove(name);
        }
    }
    
    //バックログを表示します。
	public class ShowlogComponent : AbstractComponent {
		public ShowlogComponent() { }

		protected override void TagFunction() {
            //			StatusManager.Instance.Wait();

            //イベントを停止する
            //			StatusManager.Instance.enableEventClick = false;
            //ToDo:
            //            Trionfi.Instance.currentBackLogWindow;
            //			nextOrder = false;
            //nextorder しない。
            TRUIManager.Instance.currentLogWindow.gameObject.SetActive(true);
//ToDo:
        }
    }
}
