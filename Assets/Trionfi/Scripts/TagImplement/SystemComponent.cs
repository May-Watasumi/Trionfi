using System;
using System.Collections;
using System.Collections.Generic;

#if !TR_PARSEONLY
 using UnityEngine;
 using UnityEngine.SceneManagement;
#endif

using Jace.Operations;

namespace Trionfi {

    //コメント。何もしない。
    [Serializable]
    public class CommentComponent : AbstractComponent
    {
        public CommentComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            essentialParams = new List<string> { "text" };
#endif
        }

        protected override void TagFunction()
        {
        }
    }

    //エイリアスを定義する。実行はUnknownTag任せでパーサーでは変換しない（独立性）
    [Serializable]
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
#if !TR_PARSEONLY
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
#endif
		}
    }

    [Serializable]
    public class InitsceneComponent : AbstractComponent
    {
        public InitsceneComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
#endif
        }

        protected override void TagFunction()
        {
#if !TR_PARSEONLY
			int mesID = tagParam["mes", 0];
            Trionfi.Instance.ResetCanvas(mesID);
#endif
		}
    }

    [Serializable]
    public class EndsceneComponent : AbstractComponent
    {
        public EndsceneComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
#endif
        }

        protected override void TagFunction()
        {
#if !TR_PARSEONLY
			Trionfi.Instance.HideCanvas();
#endif
		}
    }

    [Serializable]
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
    [Serializable]
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
#if !TR_PARSEONLY
			TRVirtualMachine.Serialize(tagParam["file"].Literal());
#endif
		}
    }

    //ジャンプ＝コールスタックを変えない。いわゆるgoto
    [Serializable]
    public class JumpComponent : AbstractComponent {
		public JumpComponent() {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialMoreOneParams = new List<string> {
				"target",
                "file"
			};
#endif
        }

        protected override void TagFunction() {  }

#if !TR_PARSEONLY
        public override IEnumerator TagSyncFunction()
        {
            string target = string.Empty;
            string file = string.Empty;

            if (tagParam.ContainsKey("target"))
                target = tagParam["target"].Literal();

//            if (tagParam.ContainsKey("storage"))
             file = tagParam["storage", TRVirtualMachine.currentCallStack.scriptName];

            //ファイルが異なるものになる場合、シナリオをロードする
            if (file != TRVirtualMachine.currentCallStack.scriptName)
            {
                TRResourceType type = GetResourceType();

                yield return Trionfi.Instance.LoadScript(file, type);

                //スタックをすべて削除する
//                TRVirtualMachine.RemoveAllStacks();
                TRVirtualMachine.FunctionalObjectInstance func = new TRVirtualMachine.FunctionalObjectInstance(TRVirtualMachine.FunctionalObjectType.Script, file, 0);

                if (tagParam.ContainsKey("target"))
                    func.LocalJump(tagParam["target"].Literal());

                TRVirtualMachine.currentCallStack.currentPos = TRVirtualMachine.currentCallStack.endPos + 1;
                TRVirtualMachine.callStack.Push(func);
            }
            //ローカルジャンプ
            else
            {
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
#endif
	}

    //コールスタックに保存されるジャンプ。いわゆるサブルーチン
    [Serializable]
    public class CallComponent : AbstractComponent
    {
        public CallComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialMoreOneParams = new List<string> {
                "target",
                "file"
            };
#endif
        }

        protected override void TagFunction() { }

        public override IEnumerator TagSyncFunction()
        {
#if !TR_PARSEONLY
            string target = string.Empty;
            string file = string.Empty;

            if (tagParam.ContainsKey("target"))
                target = tagParam["target"].Literal();
            if (tagParam.ContainsKey("storage"))
                file = tagParam["storage", TRVirtualMachine.currentCallStack.scriptName];

            //ファイルが異なるものになる場合、シナリオをロードする
            if (file != TRVirtualMachine.currentCallStack.scriptName)
            {
                TRResourceType type = GetResourceType();

                yield return Trionfi.Instance.LoadScript(file, type);

                //スタックをすべて削除する
                //                TRVirtualMachine.RemoveAllStacks();
                TRVirtualMachine.FunctionalObjectInstance func = new TRVirtualMachine.FunctionalObjectInstance(TRVirtualMachine.FunctionalObjectType.Script, file, 0);

                if (tagParam.ContainsKey("target"))
                    func.LocalJump(tagParam["target"].Literal());

                ErrorLogger.Log("call : file=\"" + TRVirtualMachine.currentCallStack.scriptName + "\" " + "index=\"" + TRVirtualMachine.currentCallStack.currentPos + "\"");

                yield return TRVirtualMachine.Instance.Call(func, tagParam);

            }
            //Local Call
            else
            {
                if (string.IsNullOrEmpty(file))
                    file = TRVirtualMachine.currentCallStack.scriptName;

                int index = TRVirtualMachine.currentTagInstance.arrayComponents.labelPos.ContainsKey(target) ? -1 : TRVirtualMachine.currentTagInstance.arrayComponents.labelPos[target];

                TRVirtualMachine.FunctionalObjectInstance func = new TRVirtualMachine.FunctionalObjectInstance(TRVirtualMachine.FunctionalObjectType.Script, file, 0);

                if (tagParam.ContainsKey("target"))
                    func.LocalJump(tagParam["target"].Literal());

                ErrorLogger.Log("call : file=\"" + TRVirtualMachine.currentCallStack.scriptName + "\" " + "index=\"" + TRVirtualMachine.currentCallStack.currentPos + "\"");

                yield return TRVirtualMachine.Instance.Call(func, tagParam);
#endif
            }

        }
    }

    //サブルーチン等の返値を伴うコールスタック復帰処理。
    [Serializable]
    public class ReturnComponent : AbstractComponent {
		public ReturnComponent() {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
				//"target"
			};
#endif
        }

		protected override void TagFunction()
		{
#if !TR_PARSEONLY
            if(TRVirtualMachine.callStack.Count <= 1)
                ErrorLogger.Log("callとreturnの不整合");
            else
                TRVirtualMachine.currentCallStack.currentPos = TRVirtualMachine.currentCallStack.endPos + 1;
#endif
		}
	}

    //Unityシーン追加呼び出し
    [Serializable]
    public class SceneComponent : AbstractComponent {
		public SceneComponent() {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
				"file"
			};
#endif
        }

#if !TR_PARSEONLY
        AsyncOperation syncState;
#endif
        protected override void TagFunction()
        {
//			string file = tagParam.Identifier("file");
//            SceneManager.LoadScene(file, LoadSceneMode.Additive);
        }

#if !TR_PARSEONLY
		public override IEnumerator TagSyncFunction()
        {
            string file = tagParam["file"].Literal();
            syncState = SceneManager.LoadSceneAsync(file, LoadSceneMode.Additive);
            yield return new WaitUntil(() => syncState.isDone);
        }
#endif
	}

    [Serializable]
    public class EvalComponent : AbstractComponent {
		public EvalComponent() {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
				"exp"
			};
#endif
        }

		protected override void TagFunction()
		{
#if !TR_PARSEONLY
			string exp = tagParam["exp"].Literal();
            VariableCalcurator result = TRVirtualMachine.Calc(exp, tagParam);
#endif
		}
    }

    //if
    [Serializable]
    public class IfComponent : AbstractComponent {
		public IfComponent() {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
				"exp"
			};
#endif
        }

		protected override void TagFunction()
		{
#if !TR_PARSEONLY
			string exp = tagParam["exp"].Literal();

            VariableCalcurator result = TRVirtualMachine.Calc(exp, tagParam);
            TRVirtualMachine.ifStack.Push(result.Bool());

            if (!result.Bool())
            {
                TRVirtualMachine.FunctionalObjectInstance _cuttentStack = TRVirtualMachine.currentCallStack;
                _cuttentStack.SkipTo<ElseComponent, ElseifComponent, EndifComponent>();
            }
#endif
		}
    }

    //elseif
    [Serializable]
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

		protected override void TagFunction()
		{
#if !TR_PARSEONLY
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
#endif
		}
    }

    //else
    [Serializable]
    public class ElseComponent : AbstractComponent {
		public ElseComponent() {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
			};
#endif
        }

		protected override void TagFunction()
		{
#if !TR_PARSEONLY
			bool _stack = TRVirtualMachine.ifStack.Peek();

            //直前が真の場合はelseifは実行されない
            if (_stack)
            {
                TRVirtualMachine.FunctionalObjectInstance _cuttentStack = TRVirtualMachine.currentCallStack;
                _cuttentStack.SkipTo<EndifComponent>();
            }
#endif
		}
    }

    //endif
    [Serializable]
    public class EndifComponent : AbstractComponent {
		public EndifComponent() {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
			};
#endif
        }

		protected override void TagFunction()
		{
#if !TR_PARSEONLY
			//ToDo:コールスタックチェック
			TRVirtualMachine.ifStack.Pop();
#endif
		}
    }

    //通行止めタグ。基本的にテスト用。
    //ToDo:デバッグ時のみタップに反応するとか。
    [Serializable]
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
    [Serializable]
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
    [Serializable]
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

#if !TR_PARSEONLY
		public override IEnumerator TagSyncFunction()
        {
            int timeMsec = tagParam["time", 0];

            float _time = (float)timeMsec / 1000.0f;

            yield return new WaitForSeconds(_time);
        }
#endif
	}

    //title=Webページヘジャンプします。
    [Serializable]
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
#if !TR_PARSEONLY
			string url = tagParam["url"].Literal();
			Application.OpenURL(url);
			//ToDo:
			//            yield return null;
#endif
		}
	}

    //変数削除
    [Serializable]
    public class ClearvarComponent : AbstractComponent {
		public ClearvarComponent() {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
				"name"
			};
#endif
        }

		protected override void TagFunction()
		{
			//削除
            //ToDo
//			string name = expressionedParams["name"];
//            Trionfi.Instance.currentTagInstance.variable.Remove(name);
        }
    }   
}
