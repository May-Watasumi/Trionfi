using System;
using System.Collections;
using System.Collections.Generic;
//using ExpressionParser;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Trionfi {
 
    //ユーザーデータセーブ。
    public class DatasaveComponent : AbstractComponent
    {
        public DatasaveComponent()
        {
            essentialParams = new List<string> { "file" };
        }

        protected override void TagFunction()
        {
            TRVitualMachine.Serialize(expressionedParams["file"]);
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
            TRVitualMachine.invovationInstance[name] = new InvocationObject(TRVitualMachine.currentScriptName, Trionfi.Instance.currentTagInstance.currentComponentIndex, TRSTACKTYPES.MACRO);
        }
    }
/*
	//マクロを実行するためのタグ
	public class _MacrostartComponent : AbstractComponent {
		public _MacrostartComponent() {

			//必須項目
			essentialParams = new List<string> {
				"name"
			};
		}

		protected override void TagFunction()
        {
			expressionedParams["name"] = tagName;

			Macro macro = TRVitualMachine.GetMacro(expressionedParams["name"]);

            if (macro != null)
            {

                expressionedParams["index"] = "" + macro.index;
                expressionedParams["file"] = macro.file_name;

                TRVitualMachine.macroNum++;
                //this.gameManager.scenarioManager.addMacroStack (macro.name, this.expressionedParams);
                AbstractComponent cmp = TRScriptParser.Instance.MakeTag("call", expressionedParams);
                cmp.Execute();
            }
            else
            {
                ErrorLogger.StopError("マクロ「" + expressionedParams["name"] + "」は存在しません。");
            }
        }
    }
*/

    //マクロ定義の終了宣言
	public class EndmacroComponent : AbstractComponent
	{
		public EndmacroComponent() { }

		protected override void TagFunction()
        {
            TRVitualMachine.callStack.Pop();
        }
    }

    //ジャンプ＝コールスタックを変えない。いわゆるgoto
	public class JumpComponent : AbstractComponent {
		public JumpComponent() {

			//必須項目
			essentialParams = new List<string> {
//				"target"
			};
		}

		protected override void TagFunction()
		{
			string target = expressionedParams.String("target").Replace ("*", "").Trim();
            string file = expressionedParams.String("file", TRVitualMachine.currentScriptName);

			//ファイルが異なるものになる場合、シナリオをロードする
			if(TRVitualMachine.currentScriptName != file)
			{
				Trionfi.Instance.currentTagInstance.CompileScriptFile(file);

                //ToDo:スタックをすべて削除する
                TRVitualMachine.RemoveAllStacks();
            }

            int index = string.IsNullOrEmpty(file) ? -1 : Trionfi.Instance.tagInstance[file].GetLabelPosition(target);

            //ToDo:ジャンプ処理

//            Trionfi.Instance.currentTagInstance.Run(index);

            ErrorLogger.Log("jump : file=\"" + TRVitualMachine.currentScriptName + "\" " + "index=\"" + Trionfi.Instance.currentTagInstance.currentComponentIndex + "\"");

            //ToDo:メインループ側で配列Indexが++されるので
            Trionfi.Instance.currentTagInstance.currentComponentIndex--;
        }
	}

	//コールスタックに保存されるジャンプ。いわゆるサブルーチン
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
            string target = expressionedParams.String("target").Replace("*", "").Trim();
            string file = expressionedParams.String("file", TRVitualMachine.currentScriptName);

            int index = string.IsNullOrEmpty(file) ? -1 : Trionfi.Instance.tagInstance[file].GetLabelPosition(target);

//			string tag_str ="[jump file='"+file+"' target='"+target+"' index="+ index +" ]";
//ToDo:
			ErrorLogger.Log("Call : file=\"" + TRVitualMachine.currentScriptName + "\" " + "index=\"" + (Trionfi.Instance.currentTagInstance.currentComponentIndex).ToString()+ "\"");

            TRVitualMachine.callStack.Push(new CallStackObject(TRVitualMachine.currentScriptName, Trionfi.Instance.currentTagInstance.currentComponentIndex, expressionedParams));

            //ToDo:ジャンプ
			
			//タグを実行
//			AbstractComponent cmp = TRScriptParser.Instance.MakeTag(tag_str);
//			cmp.Execute();

            //メインループ側で配列Indexが++されるので
//			Trionfi.Instance.currentTagInstance.currentComponentIndex--;
        }
	}

    //サブルーチン等の返値を伴うコールスタック復帰処理。
	public class ReturnComponent : AbstractComponent {
		public ReturnComponent() {
			//必須項目
			essentialParams = new List<string> {
				//"target"
			};
		}

		protected override void TagFunction() {
            CallStackObject stack = TRVitualMachine.callStack.Pop();

			string tag_str = "";

			//return 時の戻り場所を指定できます
			if (this.expressionedParams ["file"] != "" || this.expressionedParams ["target"] != "")
				tag_str = "[jump file='" + this.expressionedParams["file"] + "' target='" + this.expressionedParams["target"] + "' ]";
			else
				tag_str = "[jump file='" + stack.scenarioNname + "' index='" + stack.index + "' ]";

			Debug.Log("RETURN scn=\"" + stack.scenarioNname + "\" " + "index=\"" + stack.index.ToString()+ "\"");// + " param=\"" + this.expressionedParams.ToStringFull());

			//タグを実行
			AbstractComponent cmp = TRScriptParser.Instance.MakeTag(tag_str);
			cmp.Execute();
        }
	}

    //Unityシーン追加呼び出し
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
			string file = expressionedParams["file"];

            if(SyncWait)
                SceneManager.LoadScene(file, LoadSceneMode.Additive);
            else
                syncState = SceneManager.LoadSceneAsync(file, LoadSceneMode.Additive);
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

			ExpObject eo = new ExpObject(exp);

			string result = ExpObject.calc (eo.exp);
            //ToDo
//			Trionfi.Instance.currentTagInstance.variable.Set(eo.type + "." + eo.name, result);
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
            //ToDo:
//			Trionfi.Instance.currentTagInstance.variable.Set(eo.type + "." + eo.name, eo.exp);
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
            //ToDo:
			//変数なら素直に代入
//			if(val.IndexOf(".") != -1)
//				Trionfi.Instance.currentTagInstance.variable.Set(exp, val);

			string tag_str ="[story val='"+val+"' ]";

			AbstractComponent cmp = TRScriptParser.Instance.MakeTag(tag_str);
			cmp.Execute();
        }
    }
    
    //if
	public class IfComponent : AbstractComponent {
		public IfComponent() {
			//必須項目
			essentialParams = new List<string> {
				"exp"
			};
        }

		protected override void TagFunction() {

			string exp = expressionedParams ["exp"];
			if(expressionedParams.ContainsKey ("mobile"))
            {
			
			}

            string result = ExpObject.calc(exp);

			//条件に合致した場合はそのままifの中へ
			if(result == "false" || result == "0")
                TRVitualMachine.ifStack.Push(false);
            else
                TRVitualMachine.ifStack.Push(true);

        }
    }
    
    //elseif
	public class ElseifComponent : AbstractComponent {
		public ElseifComponent() {
			//必須項目
			essentialParams = new List<string> {
				"exp"
			};
		}

		protected override void TagFunction() {

            bool _stack = TRVitualMachine.ifStack.Pop();

            //直前が真の場合はelseifは実行されない
            if (_stack)
                TRVitualMachine.ifStack.Push(false);
            else
            {
                string exp = expressionedParams["exp"];
                string result = ExpObject.calc(exp);

                //条件に合致した場合はそのままifの中へ
                if (result == "false" || result == "0")
                    TRVitualMachine.ifStack.Push(false);
                else
                    TRVitualMachine.ifStack.Push(true);
            }
        }
    }

    //else
	public class ElseComponent : AbstractComponent {
		public ElseComponent() {
			//必須項目
			essentialParams = new List<string> {
			};
		}

		protected override void TagFunction() {
            bool _stack = TRVitualMachine.ifStack.Pop();

            //直前が真の場合はelseifは実行されない
            TRVitualMachine.ifStack.Push(!_stack);
        }
    }

    //endif
    public class EndifComponent : AbstractComponent {
		public EndifComponent() {
			//必須項目
			essentialParams = new List<string> {
			};
		}

		protected override void TagFunction() {
            //ToDo:コールスタックチェック
            TRVitualMachine.ifStack.Pop();
        }
    }

    //通行止めタグ。基本的にテスト用。
    //ToDo:デバッグ時のみタップに反応するとか。
	public class SComponent : AbstractComponent {
		public SComponent() { }

		protected override void TagFunction() {
			//StatusManager.Instance.InfiniteStop();
            //			StatusManager.Instance.enableNextOrder = false;
            //			StatusManager.Instance.enableClickOrder = false;
            //ToDo:SyncWait
            //その他 enableNextOrder が来るまで進めない
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
            //ToDo:
//            string exp = expressionedParams ["exp"];
//			Trionfi.Instance.currentTagInstance.variable.Trace(exp);
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
            //ToDo
//			string name = expressionedParams["name"];
//            Trionfi.Instance.currentTagInstance.variable.Remove(name);
        }
    }   
}
