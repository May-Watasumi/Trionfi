using System;
using System.Collections;
using System.Collections.Generic;
//using ExpressionParser;
using UnityEngine;
using UnityEngine.SceneManagement;
using Jace.Operations;

namespace Trionfi {
    //エイリアスを定義する。実行はUnknownTag任せでパーサーでは変換しない（独立性）
    public class AliasComponent : AbstractComponent
    {
        public AliasComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            essentialParams = new List<string> { "name", "tag" };
#endif
        }

        protected override void TagFunction()
        {
            string className = "Trionfi." + TRParserBase.tf.ToTitleCase(tagParam["tag"].Literal()) + "Component";

            // リフレクションで動的型付け
            Type masterType = Type.GetType(className);
            AbstractComponent _component = (AbstractComponent)Activator.CreateInstance(masterType);

            if (_component == null)
            {
                ErrorLogger.Log("Alias failed : Undefined Tag \"" + className + "\"");
            }
            else
            {
                _component.tagParam = tagParam;
                TRVirtualMachine.aliasTagInstance[tagParam["name"].Literal()] = _component;
            }
        }
    }

    public class InitsceneComponent : AbstractComponent
    {
        public InitsceneComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
#endif
        }

        protected override void TagFunction()
        {
            Trionfi.Instance.ResetCanvas();
        }
    }

    public class EndsceneComponent : AbstractComponent
    {
        public EndsceneComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
#endif
        }

        protected override void TagFunction()
        {
            Trionfi.Instance.ResetCanvas();
        }
    }

    public class LabelComponent : AbstractComponent
    {
        public LabelComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> { "name" };
#endif
        }

        protected override void TagFunction()
        {
            //ToDo
        }
    }

    //ユーザーデータセーブ。
    public class DatasaveComponent : AbstractComponent
    {
        public DatasaveComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            essentialParams = new List<string> { "file" };
#endif
        }

        protected override void TagFunction()
        {
            TRVirtualMachine.Serialize(tagParam["file"].Literal());
        }
    }

    //ジャンプ＝コールスタックを変えない。いわゆるgoto
	public class JumpComponent : AbstractComponent {
		public JumpComponent() {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
				"target"
			};
#endif
        }

        protected override void TagFunction() {  }

        public override IEnumerator TagSyncFunction()
        {
            string target = tagParam["target"].Literal();
            string file = tagParam["file", TRVirtualMachine.currentCallStack.scriptName];

			//ファイルが異なるものになる場合、シナリオをロードする
			if(file != TRVirtualMachine.currentCallStack.scriptName)
			{
                yield return TRVirtualMachine.Instance.LoadScenarioAsset(file);

                //ToDo:スタックをすべて削除する
//                TRVirtualMachine.RemoveAllStacks();
//                TRVirtualMachine.currentScriptName = file;
            }

            if (string.IsNullOrEmpty(file))
                file = TRVirtualMachine.currentCallStack.scriptName;

            int index = TRVirtualMachine.currentTagInstance.arrayComponents.labelPos.ContainsKey(target) ? -1 : TRVirtualMachine.currentTagInstance.arrayComponents.labelPos[target];

            if (tagParam.ContainsKey("target"))
                TRVirtualMachine.currentCallStack.LocalJump(tagParam["target"].Literal());
            else
                ErrorLogger.StopError("にラベル「" + target + "」が見つかりません。");

            ErrorLogger.Log("jump : file=\"" + TRVirtualMachine.currentCallStack.scriptName + "\" " + "index=\"" + TRVirtualMachine.currentCallStack.currentPos + "\"");
        }
	}

	//コールスタックに保存されるジャンプ。いわゆるサブルーチン
	public class CallComponent : AbstractComponent {
		public CallComponent() {
#if UNITY_EDITOR && TR_DEBUG
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
#endif
        }

		protected override void TagFunction()
		{
            string target = tagParam["target"].Literal();

            string file = tagParam["file", TRVirtualMachine.currentCallStack.scriptName];

            int index = string.IsNullOrEmpty(file) ? -1 : TRVirtualMachine.currentTagInstance.arrayComponents.labelPos[target];

			ErrorLogger.Log("Call : file=\"" + file + "\" " + "index = \"" + index.ToString()+ "\"");

//            TRVirtualMachine.callStack.Push(new CallStackObject(TRVirtualMachine.currentCallStack.currentPos , tagParam));
            //ToDo:ジャンプ
            //メインループ側で配列Indexが++されるので
//			Trionfi.Instance.currentTagInstance.currentComponentIndex--;
        }
	}

    //サブルーチン等の返値を伴うコールスタック復帰処理。
	public class ReturnComponent : AbstractComponent {
		public ReturnComponent() {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
				//"target"
			};
#endif
        }

		protected override void TagFunction() {
            TRVirtualMachine.FunctionalObjectInstance callStack = TRVirtualMachine.callStack.Pop();

			string tag_str = "";

			//return 時の戻り場所を指定できます
			if( string.IsNullOrEmpty(tagParam["file", string.Empty]) && string.IsNullOrEmpty(tagParam["target", string.Empty]) )
				tag_str = "[jump file='" + tagParam["file"] + "' target='" + tagParam["target"] + "' ]";
			else
				tag_str = "[jump file='" + callStack.scriptName + "' index='" + callStack.currentPos + "' ]";

			Debug.Log("RETURN scn=\"" + callStack.scriptName + "\" " + "index=\"" + callStack.currentPos.ToString()+ "\"");// + " param=\"" + this.expressionedParams.ToStringFull());

//ToDo:
			//タグを実行
//			AbstractComponent cmp = TRScriptParser.Instance.MakeTag(tag_str);
//			cmp.Execute();
        }
	}

    //Unityシーン追加呼び出し
    public class SceneComponent : AbstractComponent {
		public SceneComponent() {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
				"file"
			};
#endif
        }

        AsyncOperation syncState;

        protected override void TagFunction()
        {
//			string file = tagParam.Identifier("file");
//            SceneManager.LoadScene(file, LoadSceneMode.Additive);
        }

        public override IEnumerator TagSyncFunction()
        {
            string file = tagParam["file"].Literal();
            syncState = SceneManager.LoadSceneAsync(file, LoadSceneMode.Additive);
            yield return new WaitUntil(() => syncState.isDone);
        }
    }

    public class EvalComponent : AbstractComponent {
		public EvalComponent() {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
				"exp"
			};
#endif
        }

		protected override void TagFunction() {
            string exp = tagParam["exp"].Literal();
            VariableCalcurator result = TRVirtualMachine.Calc(exp, tagParam);
        }
    }

    //if
	public class IfComponent : AbstractComponent {
		public IfComponent() {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
				"exp"
			};
#endif
        }

		protected override void TagFunction() {
            string exp = tagParam["exp"].Literal();

            VariableCalcurator result = TRVirtualMachine.Calc(exp, tagParam);
            TRVirtualMachine.ifStack.Push(result.Bool());

            if (!result.Bool())
            {
                TRVirtualMachine.FunctionalObjectInstance _cuttentStack = TRVirtualMachine.currentCallStack;
                _cuttentStack.SkipTo<ElseComponent, ElseifComponent, EndifComponent>();
            }
        }
    }
    
    //elseif
	public class ElseifComponent : AbstractComponent {
		public ElseifComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
				"exp"
			};
#endif
        }

		protected override void TagFunction() {

            bool _stack = TRVirtualMachine.ifStack.Pop();

            //直前が真の場合はelseifは実行されない
            if (_stack)
            {
                TRVirtualMachine.FunctionalObjectInstance _cuttentStack = TRVirtualMachine.currentCallStack;
                _cuttentStack.SkipTo<ElseComponent, ElseifComponent, EndifComponent>();
            }
            else
            {
                string exp = tagParam["exp"].Literal();
                VariableCalcurator result = TRVirtualMachine.Calc(exp, tagParam);
                TRVirtualMachine.ifStack.Push(result.Bool());

                if (!result.Bool())
                {
                    TRVirtualMachine.FunctionalObjectInstance _cuttentStack = TRVirtualMachine.currentCallStack;
                    _cuttentStack.SkipTo<ElseComponent, ElseifComponent, EndifComponent>();
                }
            }
        }
    }

    //else
	public class ElseComponent : AbstractComponent {
		public ElseComponent() {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
			};
#endif
        }

		protected override void TagFunction() {
            bool _stack = TRVirtualMachine.ifStack.Peek();

            //直前が真の場合はelseifは実行されない
            if (_stack)
            {
                TRVirtualMachine.FunctionalObjectInstance _cuttentStack = TRVirtualMachine.currentCallStack;
                _cuttentStack.SkipTo<EndifComponent>();
            }
        }
    }

    //endif
    public class EndifComponent : AbstractComponent {
		public EndifComponent() {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
			};
#endif
        }

		protected override void TagFunction() {
            //ToDo:コールスタックチェック
            TRVirtualMachine.ifStack.Pop();
        }
    }

    //通行止めタグ。基本的にテスト用。
    //ToDo:デバッグ時のみタップに反応するとか。
	public class SComponent : AbstractComponent {
		public SComponent() { }

		protected override void TagFunction() {
			//StatusManager.Instance.InfiniteStop();
            //ToDo:SyncWait
            //その他 enableNextOrder が来るまで進めない
        }
    }
    
    //ToDo:外部コンソールウィンドウへ
    //変数の中身をデバックコンソールで確認することができます。
	public class TraceComponent : AbstractComponent {
		public TraceComponent() {
#if UNITY_EDITOR && TR_DEBUG
            essentialParams = new List<string>
            {
                   "exp"
            };
#endif
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
		public WaitComponent()
		{
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
				"time"
			};
#endif
        }

        //時間を止める。
        protected override void TagFunction()
        {
		}

        public override IEnumerator TagSyncFunction()
        {
            int timeMsec = tagParam["time", 0];

            float _time = (float)timeMsec / 1000.0f;

            yield return new WaitForSeconds(_time);
        }
    }
    
    //title=Webページヘジャンプします。
	public class WebComponent : AbstractComponent {
		public WebComponent() {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
				"url"
			};
#endif
        }

		protected override void TagFunction()
        {
			string url = tagParam["url"].Literal();
			Application.OpenURL(url);
            //ToDo:
//            yield return null;
		}
	}

	//変数削除
	public class ClearvarComponent : AbstractComponent {
		public ClearvarComponent() {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
				"name"
			};
#endif
        }

		protected override void TagFunction() {
			//削除
            //ToDo
//			string name = expressionedParams["name"];
//            Trionfi.Instance.currentTagInstance.variable.Remove(name);
        }
    }   
}
