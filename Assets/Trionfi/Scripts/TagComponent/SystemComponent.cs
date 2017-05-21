using System;
using System.Collections;
using System.Collections.Generic;
//using ExpressionParser;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using NovelEx;

namespace NovelEx {
	public class LabelComponent : AbstractComponent {
		public LabelComponent() {

			//必須項目
			arrayVitalParam = new List<string> {
				"name"
			};

			originalParamDic = new ParamDictionary() {
				{ "name","" }
			};

		}

		public override IEnumerator Start()
        {
            //ToDo:
            //			this.gameManager.nextOrder();
            yield return null;
		}
	}

	/*	
--------------

[doc]
tag=macro
group=シナリオ制御
title=マクロ定義

[desc]

マクロ記述を開始します。新しいタグを定義することが出来ます。
このタグから、endmacro タグまでにある文章やタグは
name 属性で指定されたタグとして登録され、以後使用できるようになります。

マクロには値を渡すことができます。渡された変数には mp という変数に格納され、アクセスすることが可能です。

[sample]



[macro name="newtag"]
	新しいタグです。[p]
	{mp.arg1}という値が渡されました。	
[endmacro]

[newtag arg1="テスト"]

[param]
name=ラベル名を指定してください


[_doc]
--------------------
 */

	//マクロを作成して管理する
	public class MacroComponent : AbstractComponent {
		public MacroComponent() {
			//必須項目
			arrayVitalParam = new List<string> {
				"name"
			};

			originalParamDic = new ParamDictionary() {
				{ "name","" },
			};

		}

		public override IEnumerator Start() {
			//macro 
			StatusManager.Instance.setSkipOrder();

			string name = paramDic ["name"];
//ToDo:
			ScriptDecoder.Instance.AddMacro(name, StatusManager.Instance.currentScenario, ScriptDecoder.Instance.currentComponentIndex);
            //			this.gameManager.nextOrder();
            yield return null;

        }

    }

	//マクロを実行するためのタグ
	public class _MacrostartComponent:AbstractComponent {
		public _MacrostartComponent() {

			//必須項目
			arrayVitalParam = new List<string> {
			//	"name"
			};

			originalParamDic = new ParamDictionary() {
			};
		}

		public override IEnumerator Start()
        {
			paramDic["name"] = tagName;

			ScriptDecoder.Macro macro = ScriptDecoder.Instance.GetMacro(paramDic ["name"]);

			if(macro == null) {
				ErrorLogger.stopError("マクロ「"+paramDic["name"]+"」は存在しません。");
                //ToDo:
                yield break;
			}

			paramDic["index"] = ""+macro.index ;
			paramDic["file"]  = macro.file_name;

			ScriptDecoder.Instance.macroNum++;
			//this.gameManager.scenarioManager.addMacroStack (macro.name, this.paramDic);
			AbstractComponent cmp = TRScriptParser.Instance.makeTag ("call", paramDic);
			yield return cmp.Start();
        }
    }

	/*	
--------------

[doc]
tag=endmacro
group=シナリオ制御
title=マクロの終端

[desc]

マクロ終了を表します

[sample]


[macro name="newtag"]
	新しいタグです。[p]
	{mp.arg1}という値が渡されました。	
[endmacro]


[_doc]
--------------------
 */


	//マクロを作成して管理する
	public class EndmacroComponent:AbstractComponent
	{
		public EndmacroComponent() { }

		public override void Before(){
			StatusManager.Instance.releaseSkipOrder();
		}

		public override IEnumerator Start() {
			if(ScriptDecoder.Instance.macroNum > 0) {
				ScriptDecoder.Instance.macroNum--;
				//ココに来た場合はreturn を実行する 
				AbstractComponent cmp = TRScriptParser.Instance.makeTag ("[return]");
				yield return cmp.Start();
				nextOrder = false;
			}
			else
            {
//ToDo:
//				this.gameManager.nextOrder();
			}
            yield return null;
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
			arrayVitalParam = new List<string> {
				//"target"
			};

			originalParamDic = new ParamDictionary() {
				{ "target","" },
				{ "file","" },
				{ "index",""},
				{ "scene",""}, //ここにnew が入っている場合はジャンプ後にシーンをイチから作り直す。
				{ "next","true"}, //next にfalse が入っている場合、ジャンプ先でnextOrderを行いません。
			};
		}

		public override IEnumerator Start()
		{
			string target = this.paramDic["target"].Replace ("*", "").Trim();
			string file = this.paramDic["file"];

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
			if (this.paramDic["index"] != "")
				index = int.Parse(this.paramDic["index"]);
			else
				index = ScriptDecoder.Instance.GetIndex(file, target);

			if(index == -1)
				index = 0;

			//mp変数の中身を書き換える jumpのpmの内容で
			//NovelSingleton.GameManager.StatusManager.Instance.variable.replaceAll("mp",this.paramDic);;

			//ゲームマネージャーの現在の位置をそこに書き換えてnextOrderでどうだ。
//ToDo:
//			ScriptDecoder.Instance.currentComponentIndex = index;
			ScriptDecoder.Instance.StartScenario(file, index);

			//シーンをクリアして作りなおす
			if (this.paramDic ["scene"] == "new") {
				//new の場合はスタックをすべて削除する
				ScriptDecoder.Instance.RemoveAllStacks();
				StatusManager.Instance.nextFileName = file;
				StatusManager.Instance.nextTargetName = target;

				//jumpから来たことを通知するためのパラメータが必要
				SceneManager.LoadScene("NovelPlayer");
			}

			Debug.Log("JUMP:scn=\"" + StatusManager.Instance.currentScenario + "\" " + "index=\"" + ScriptDecoder.Instance.currentComponentIndex + "\"");
            // + " param=\"" + this.paramDic.ToStringFull());

            //			if (this.paramDic ["next"] != "false") {
            //				nextOrder = false;
            //				StatusManager.Instance.currentState = JokerState.NextOrder;
            //			}
            //			else {
            //				this.gameManager.nextOrder();
            //			}
            yield return null;

        }
        public override void After() {
			//SkipOrder中もafterが実行される（のが仕様としては正しいんだけども）
//			if(StatusManager.Instance.currentState != JokerState.SkipOrder)
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
			arrayVitalParam = new List<string> {
				//"target"
			};

			originalParamDic = new ParamDictionary() {
				{ "target","" },
				{ "file","" },
				//{ "index",""},
			};
		}

		public override IEnumerator Start()
		{
			string target = this.paramDic ["target"].Replace("*", "").Trim();
			string file = this.paramDic ["file"];

			string index = "";

			if (this.paramDic.ContainsKey("index"))
				index = this.paramDic ["index"];

			string tag_str ="[jump file='"+file+"' target='"+target+"' index="+ index +" ]";
//ToDo:
			Debug.Log("PUSH:scn=\"" + StatusManager.Instance.currentScenario + "\" " + "index=\"" + (ScriptDecoder.Instance.currentComponentIndex).ToString()+ "\"");

			ScriptDecoder.Instance.AddStack(StatusManager.Instance.currentScenario, ScriptDecoder.Instance.currentComponentIndex, this.paramDic);
			
			//タグを実行
			AbstractComponent cmp = TRScriptParser.Instance.makeTag(tag_str);
			yield return cmp.Start();

//nextOrder分
			ScriptDecoder.Instance.currentComponentIndex--;

            //			nextOrder = false;
            //			StatusManager.Instance.currentState = JokerState.NextOrder;

            //ゲームマネージャーの現在の位置をそこに書き換えてnextOrderでどうだ。

            //macro もひとつのcomponent_array みたいにしていいんじゃないかしら。ラベルじゃないけど
            //StackManager に　呼び出し状態を保持させる macro の中で別ファイルへのjumpは禁止したいね。
            //現在の位置をスタックとして保持させる
            //this.gameManager.nextOrder();
            yield return null;
        }
        public override void After(){
//			ScriptDecoder.Instance.currentComponentIndex--;
//			 base.after();
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
			arrayVitalParam = new List<string> {
				//"target"
			};

			originalParamDic = new ParamDictionary() {
				{"file",""},
				{"target",""},
			};
		}

		public override IEnumerator Start() {
			ScriptDecoder.CallStack stack = ScriptDecoder.Instance.PopStack();

			string tag_str = "";

			//return 時の戻り場所を指定できます
			if (this.paramDic ["file"] != "" || this.paramDic ["target"] != "")
				tag_str = "[jump file='" + this.paramDic["file"] + "' target='" + this.paramDic["target"] + "' ]";
			else
				tag_str = "[jump file='" + stack.scenarioNname + "' index='" + stack.index + "' ]";

			Debug.Log("RETURN scn=\"" + stack.scenarioNname + "\" " + "index=\"" + stack.index.ToString()+ "\"");// + " param=\"" + this.paramDic.ToStringFull());

			//タグを実行
			AbstractComponent cmp = TRScriptParser.Instance.makeTag(tag_str);
			yield return cmp.Start();
            //			nextOrder = false;
            //			StatusManager.Instance.currentState = JokerState.NextOrder;
            //this.gameManager.nextOrder();
        }
        public override void After(){
//			ScriptDecoder.Instance.currentComponentIndex--;
//			 base.after();
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

	//Call は Jumpと同様に　ストレージを移動する。ただし、呼び出しは スタックトレースに保存され、return で元の位置に戻ります
	public class SceneComponent : AbstractComponent {
		public SceneComponent() {
			//必須項目
			arrayVitalParam = new List<string> {
				//"target"
			};

			originalParamDic = new ParamDictionary() {
				{ "file","" },
				//{ "index",""},
			};
		}

		public override IEnumerator Start()
        {
			string file = paramDic ["file"];

            SceneManager.LoadScene(file);

            //処理終了
            yield return null;

            //this.gameManager.nextOrder();
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
			arrayVitalParam = new List<string> {
				"exp"
			};

			originalParamDic = new ParamDictionary() {
				{"exp",""}
			};
		}

		public override IEnumerator Start() {
			string exp = this.paramDic ["exp"];

			ExpObject eo = new ExpObject (exp);

			string result = ExpObject.calc (eo.exp);

			ScriptDecoder.Instance.variable.Set(eo.type + "." + eo.name, result);
            yield return null;

            //ToDo:
            //			this.gameManager.nextOrder();

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
			arrayVitalParam = new List<string> {
				"exp"
			};

			originalParamDic = new ParamDictionary() {
				{"exp",""}
			};
		}

		public override IEnumerator Start() {
			string exp = paramDic ["exp"];

			ExpObject eo = new ExpObject (exp);
			ScriptDecoder.Instance.variable.Set(eo.type + "." + eo.name, eo.exp);
            yield return null;

            //ToDo:
            //			this.gameManager.nextOrder();
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
			arrayVitalParam = new List<string> {
				"exp"
			};

			originalParamDic = new ParamDictionary() {
				{"exp",""}
			};
		}

		public override IEnumerator Start() {
			string exp = paramDic["exp"];
			string val = paramDic["exp"];

			nextOrder = false;

			//変数なら素直に代入
			if(val.IndexOf(".") != -1)
				ScriptDecoder.Instance.variable.Set(exp, val);

			string tag_str ="[story val='"+val+"' ]";

			AbstractComponent cmp = TRScriptParser.Instance.makeTag(tag_str);
			yield return cmp.Start();
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
			arrayVitalParam = new List<string> {
				"exp"
			};

			originalParamDic = new ParamDictionary() {
				{"exp",""}
			};
		}

		public override void Before() {
			//スキップ中ならここは通過しない
			ScriptDecoder.Instance.ifNum++;
		}

		public override IEnumerator Start() {
			ScriptDecoder.Instance.AddIfStack(true);

			string exp = paramDic ["exp"];
			if (this.paramDic.ContainsKey ("mobile")) {
			
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
            yield return null;

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
			arrayVitalParam = new List<string> {
				"exp"
			};

			originalParamDic = new ParamDictionary() {
				{"exp",""}
			};
		}

		public override void Before() {
			StatusManager.Instance.setSkipOrder();

			if (ScriptDecoder.Instance.CountIfStack() == ScriptDecoder.Instance.ifNum) {
				if (ScriptDecoder.Instance.CurrentIfStack() == true)
					StatusManager.Instance.releaseSkipOrder();
			}
		}

		public override IEnumerator Start() {
			string exp = paramDic ["exp"];
			string result = ExpObject.calc (exp);

			//条件に合致した場合はそのままifの中へ
			if (result == "true") {
				//ifスタックが完了している
				ScriptDecoder.Instance.ChangeIfStack(false);
//ToDo:
//				this.gameManager.nextOrder();
			}
			else
			{
				//elsif か　endif まで処理を進める
				StatusManager.Instance.setSkipOrder();
//ToDo:
//				this.gameManager.nextOrder();
			}

            yield return null;

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
			arrayVitalParam = new List<string> {
			};

			originalParamDic = new ParamDictionary() {
			};
		}

		public override void Before() {
			StatusManager.Instance.setSkipOrder();

			if (ScriptDecoder.Instance.CountIfStack() == ScriptDecoder.Instance.ifNum) {
				if (ScriptDecoder.Instance.CurrentIfStack() == true)
					StatusManager.Instance.releaseSkipOrder();
			}
		}

		public override IEnumerator Start() {
			ScriptDecoder.Instance.ChangeIfStack(false);
            yield return null;

            //ToDo:
            //			this.gameManager.nextOrder();
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
			arrayVitalParam = new List<string> {
			};

			originalParamDic = new ParamDictionary() {
			};
		}

		public override void Before() {
			//if文とスタックの数が同一の場合はスキップをやめて、endif を実行
			StatusManager.Instance.setSkipOrder();

			if (ScriptDecoder.Instance.CountIfStack() == ScriptDecoder.Instance.ifNum)
				StatusManager.Instance.releaseSkipOrder();		

			ScriptDecoder.Instance.ifNum--;
		}

		public override IEnumerator Start() {
			//ifスタックが取り除かれる
			ScriptDecoder.Instance.PopIfStack();
            //ToDo:
            //			this.gameManager.nextOrder();
            yield return null;
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

		public override IEnumerator Start() {
			StatusManager.Instance.InfiniteStop();
            //			StatusManager.Instance.enableNextOrder = false;
            //			StatusManager.Instance.enableClickOrder = false;
            yield return null;

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
			arrayVitalParam = new List<string> {
				"tag"
			};

			originalParamDic = new ParamDictionary() {
				{"tag",""}
			};

		}

		public override IEnumerator Start()
		{
			//ifスタックが取り除かれる
			StatusManager.Instance.TagDefaultVal = paramDic["tag"];
            yield return null;

            //ToDo:
            //			this.gameManager.nextOrder();
            //その他 enableNextOrder が来るまで進めない
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

		public override IEnumerator Start()
		{
			//ifスタックが取り除かれる
			StatusManager.Instance.TagDefaultVal = "";
            //ToDo:
            //			this.gameManager.nextOrder();
            yield return null;

        }
    }

	//使用停止　ウィンドウを閉じる。クリックで再度表示されるやつ
	public class ClosemessageComponent : AbstractComponent
	{
		public ClosemessageComponent()
		{
			//必須項目
			arrayVitalParam = new List<string> {
			};

			originalParamDic = new ParamDictionary() {
				{"time","0.5"}
			};
		}

		public override IEnumerator Start() {
            yield return null;

            //			nextOrder = false;
            //			StatusManager.Instance.currentState = JokerState.WaitClick;
            //			StatusManager.Instance.enableClickOrder = false;
            //			StatusManager.Instance.nextClickShowMessage = true;
            //ToDo:
            //            Trionfi.Instance.currentMessageWindow.hideFlags();
            //float time = float.Parse (this.paramDic ["time"]);
            //			NovelSingleton.GameView.hideMessage (time);
        }
    }

	/*		
--------------

[doc]
tag=hidemessage
group=システム関連
title=メッセージ非表示

[desc]

メッセージウィンドウを非表示にします。
[showmessage]を明示的に実行するまで表示されません。

[sample]

[hidemessage]

[wait time=5 ]

[showmessage]
シナリオ再開

[param]


[_doc]
--------------------
 */

	//メッセージを削除する showMessage を行わないと表示されない
	public class HidemessageComponent : AbstractComponent {
		public HidemessageComponent() {

			//必須項目
			arrayVitalParam = new List<string> {
			};

			originalParamDic = new ParamDictionary() {
				{"time","0.5"}
			};
		}

		public override IEnumerator Start()
		{
            yield return null;

            //ToDo:
            //			nextOrder = false;
            //			StatusManager.Instance.currentState = JokerState.MessageHide;
            //float time = float.Parse (this.paramDic ["time"]);
        }
    }

	/*		
--------------

[doc]
tag=showmessage
group=システム関連
title=メッセージ表示

[desc]

メッセージウィンドウを表示します。

[sample]

[hidemessage]

[wait time=5 ]

[showmessage]
シナリオ再開

[param]


[_doc]
--------------------
 */

	//メッセージを表示する
	public class ShowmessageComponent : AbstractComponent {
		public ShowmessageComponent() {
			originalParamDic = new ParamDictionary() {
				{"time","0.5"}
			};
		}

		public override IEnumerator Start() {
            yield return null;

            //ToDo:
            //			nextOrder = false;
            //			StatusManager.Instance.currentState = JokerState.MessageShow;
            //			float time = float.Parse(this.paramDic["time"]);
            //TODO:Magic用拡張なので、汎用化が必要(string nextStorage = "")
            //JOKEREX.Instance.MainMessage.Show();
        }
    }

	/*		
--------------

[doc]
tag=trace
group=システム関連
title=変数の内容確認

[desc]

変数の中身をデバックコンソールで確認することができます。

[sample]


[calc exp="tmp.val=2"]
[flag exp="tmp.name=シケモク"]

;tmpの中身を確認できる
[trace exp="tmp"]

;結果↓がデバッグコンソールに表示される
;[trace]tmp
;val=2
;name=シケモク

[param]
exp=確認したい変数名を指定します。

[_doc]
--------------------
 */

	public class TraceComponent : AbstractComponent {
		public TraceComponent() {
			originalParamDic = new ParamDictionary() {
				{"exp",""}
			};
		}

		public override IEnumerator Start() {
			string exp = paramDic ["exp"];
			ScriptDecoder.Instance.variable.Trace(exp);
            yield return null;

            //ToDo:
            //			this.gameManager.nextOrder();
        }
    }

	/*		
--------------

[doc]
tag=talk_name
group=システム関連
title=発言者欄の変更

[desc]

発言者欄の名前を変更します。
chara_newでjnameを定義している場合はその値が採用されます。

このタグは省略形が用意されています
以下の２つは同じ意味になります。

#yuko

[talk_name val=yuko ]


[sample]

@talk_name val=優子

以下のようにも書けます

#優子
優子がしゃべってます。

@talk_name val=""
消したい場合は空白を指定します

[param]
val=名前を表示します。キャラクター情報と絡めたい場合はchara_newした時のnameを指定してください。


[_doc]
--------------------
 */

	//メッセージを表示する
	public class Talk_nameComponent : AbstractComponent
	{
		public Talk_nameComponent()
		{
			originalParamDic = new ParamDictionary()
			{
				{"val",""}
			};
		}

		public override IEnumerator Start()
		{
			string name = this.paramDic ["val"];
			Trionfi.Instance.currentMessageWindow.currentName.text = name;
            yield return null;

        }
    }

	/*		
--------------

[doc]
tag=wait
group=システム関連
title=ウェイトを入れる

[desc]

ウェイトを入れます。time属性で指定した時間、操作できなくなります。

[sample]

;２.5秒間　処理を停止します
[wait time=2.5]

[param]
time=停止する時間を秒で指定します


[_doc]
--------------------
 */

	//メッセージを表示する
	public class WaitComponent : AbstractComponent
	{
		public WaitComponent()
		{
			//必須項目
			arrayVitalParam = new List<string> {
				"time"
			};

			originalParamDic = new ParamDictionary() {
				{"time",""}
			};
		}

        //時間を止める。
        public override IEnumerator Start()
        {
            float time = float.Parse(paramDic["time"]);
            yield return new WaitForSeconds(time);
		}
	}

 /*		
--------------

[doc]
tag=web
group=その他
title=Webページヘジャンプします。

[desc]

ゲーム中にブラウザを開いてサイトを開きます
スマホアプリの場合、一時的にゲームを中断してブラウザが起動します

[sample]

;やほーを訪問します
[web url="http://yahoo.co.jp"]

[param]
url=移動したいURLをhttpから指定します


[_doc]
--------------------
 */

	public class WebComponent : AbstractComponent {
		public WebComponent() {
			//必須項目
			arrayVitalParam = new List<string> {
				"url"
			};

			originalParamDic = new ParamDictionary() {
				{"url",""}
			};
		}

		public override IEnumerator Start()
        {
			string url = paramDic ["url"];
			Application.OpenURL(url);
            //ToDo:
            yield return null;
		}
	}

	/*		
--------------

[doc]
tag=clearvar
group=システム関連
title=変数を削除。

[desc]

変数を削除します。


[sample]

[calc exp="tmp.val=2"]
[flag exp="tmp.name=シケモク"]

;tmpの中身を確認できる
[trace exp="tmp"]

;tmp変数をすべて初期化します
[clearvar name="tmp"]

[param]
name=削除する変数名を指定してください。


[_doc]
--------------------
 */
	//変数クリア
	public class ClearvarComponent : AbstractComponent {
		public ClearvarComponent() {
			//必須項目
			arrayVitalParam = new List<string> {
				"name"
			};

			originalParamDic = new ParamDictionary() {
				{"name",""}
			};
		}

		public override IEnumerator Start() {
			//削除
			string name = paramDic["name"];
            ScriptDecoder.Instance.variable.Remove(name);
            yield return null;
        }
    }

	/*		
--------------

[doc]
tag=showlog
group=システム関連
title=バックログ表示

[desc]

バックログを表示します。

[sample]

[showlog]

[param]


[_doc]
--------------------
 */


	public class ShowlogComponent : AbstractComponent {
		public ShowlogComponent() { }

		public override IEnumerator Start() {
            //			StatusManager.Instance.Wait();

            //イベントを停止する
            //			StatusManager.Instance.enableEventClick = false;
            //ToDo:
            //            Trionfi.Instance.currentBackLogWindow;
            //			nextOrder = false;
            //nextorder しない。
            Trionfi.Instance.currentLogWindow.gameObject.SetActive(true);
//ToDo:
            yield return null;

        }
    }
}
