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
            TRVitualMachine.Serialize(tagParam["file"]);
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
			string name = tagParam["name"];
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
			string target = tagParam.String("target").Replace ("*", "").Trim();
            string file = tagParam.String("file", TRVitualMachine.currentScriptName);

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
            string target = tagParam.String("target").Replace("*", "").Trim();
            string file = tagParam.String("file", TRVitualMachine.currentScriptName);

            int index = string.IsNullOrEmpty(file) ? -1 : Trionfi.Instance.tagInstance[file].GetLabelPosition(target);

//			string tag_str ="[jump file='"+file+"' target='"+target+"' index="+ index +" ]";
//ToDo:
			ErrorLogger.Log("Call : file=\"" + TRVitualMachine.currentScriptName + "\" " + "index=\"" + (Trionfi.Instance.currentTagInstance.currentComponentIndex).ToString()+ "\"");

            TRVitualMachine.callStack.Push(new CallStackObject(TRVitualMachine.currentScriptName, Trionfi.Instance.currentTagInstance.currentComponentIndex, tagParam));

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
			if(tagParam["file"] != "" || tagParam ["target"] != "")
				tag_str = "[jump file='" + tagParam["file"] + "' target='" + tagParam["target"] + "' ]";
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
			string file = tagParam["file"];

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

    //Jaceによってcalcとflagは差別化がなくなったので不要になる
    public class CalcComponent : AbstractComponent {
		public CalcComponent() {
			//必須項目
			essentialParams = new List<string> {
				"exp"
			};
		}

		protected override void TagFunction() {
            double result = TRVitualMachine.Calc(TRVitualMachine.variableInstance, tagParam["exp"]);
        }
    }
	public class FlagComponent : AbstractComponent {
		public FlagComponent() {
			//必須項目
			essentialParams = new List<string> {
				"exp"
			};
		}

		protected override void TagFunction() {
            double result = TRVitualMachine.Calc(TRVitualMachine.variableInstance, tagParam["exp"]);
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

            double result = TRVitualMachine.Calc(TRVitualMachine.variableInstance, tagParam["exp"]);
//            ToDo:
            /*
			//条件に合致した場合はそのままifの中へ
			if(result == "false" || result == "0")
                TRVitualMachine.ifStack.Push(false);
            else
                TRVitualMachine.ifStack.Push(true);
            */
        }
    }
    
    //elseif
	public class ElseifComponent : AbstractComponent {
		public ElseifComponent()
        {
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
                string exp = tagParam["exp"];
                double result = TRVitualMachine.Calc(TRVitualMachine.variableInstance, exp);
                //ToDo:
/*
                string result = ExpObject.calc(exp);

                //条件に合致した場合はそのままifの中へ
                if (result == "false" || result == "0")
                    TRVitualMachine.ifStack.Push(false);
                else
                    TRVitualMachine.ifStack.Push(true);
*/
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
            _time = tagParam.Float("time");
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
			string url = tagParam["url"];
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
