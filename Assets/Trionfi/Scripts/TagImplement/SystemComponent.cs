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
            System.Diagnostics.Debug.WriteLine(tagParam["text"].Literal());
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
                TRVirtualMachine.Instance.aliasTagInstance[tagParam["name"].Literal()] = _component;
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
            Trionfi.Instance.ActivateAllCanvas(true);
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
			Trionfi.Instance.ActivateAllCanvas(false);
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
            ErrorLogger.Log("Label:"+tagParam["name"].Literal());
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
	        TRSerializeManager.Instance.SaveData(tagParam["file"].Int());
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
				"storage",
                "target"
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
            file = tagParam["storage", TRVirtualMachine.Instance.currentCallStack.scriptName];

            //ファイルが異なるものになる場合、シナリオをロードする
            if (file != TRVirtualMachine.Instance.currentCallStack.scriptName)
            {
                TRResourceType type = GetResourceType();

                yield return Trionfi.Instance.LoadScript(file, type);

                //スタックをすべて削除する
//                TRVirtualMachine.RemoveAllStacks();
                FunctionalObjectInstance func = new FunctionalObjectInstance(FunctionalObjectType.Script, file, 0, 0);

                if (tagParam.ContainsKey("target"))
                    func.LocalJump(tagParam["target"].Literal());

                TRVirtualMachine.Instance.currentCallStack.currentPos = TRVirtualMachine.Instance.currentCallStack.endPos + 1;
                ResumeTask nextTask = new ResumeTask();
                nextTask.type = ResumeTaskType.JUMP;
                nextTask.instance = func;
 
                TRVirtualMachine.Instance.nextTempFunc.Enqueue(nextTask);
            }
            //ローカルジャンプ
            else
            {
                if (string.IsNullOrEmpty(file))
                    file = TRVirtualMachine.Instance.currentCallStack.scriptName;

                int index = TRVirtualMachine.Instance.currentTagInstance.arrayComponents.labelPos.ContainsKey(target) ? -1 : TRVirtualMachine.Instance.currentTagInstance.arrayComponents.labelPos[target];

                if (tagParam.ContainsKey("target"))
                    TRVirtualMachine.Instance.currentCallStack.LocalJump(tagParam["target"].Literal());
                else
                    ErrorLogger.StopError("にラベル「" + target + "」が見つかりません。");

                ErrorLogger.Log("jump : file=\"" + TRVirtualMachine.Instance.currentCallStack.scriptName + "\" " + "index=\"" + TRVirtualMachine.Instance.currentCallStack.currentPos + "\"");
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
                file = tagParam["storage", TRVirtualMachine.Instance.currentCallStack.scriptName];

            //ファイルが異なるものになる場合、シナリオをロードする
            if (file != TRVirtualMachine.Instance.currentCallStack.scriptName)
            {
                TRResourceType type = GetResourceType();

                yield return Trionfi.Instance.LoadScript(file, type);

                FunctionalObjectInstance func = new FunctionalObjectInstance(FunctionalObjectType.Script, file, 0, 0);

                if (tagParam.ContainsKey("target"))
                    func.LocalJump(tagParam["target"].Literal());

                ErrorLogger.Log("call : file=\"" + TRVirtualMachine.Instance.currentCallStack.scriptName + "\" " + "index=\"" + TRVirtualMachine.Instance.currentCallStack.currentPos + "\"");

                yield return TRVirtualMachine.Instance.Execute(func, tagParam);

            }
            //Local Call
            else
            {
                if (string.IsNullOrEmpty(file))
                    file = TRVirtualMachine.Instance.currentCallStack.scriptName;

                int index = TRVirtualMachine.Instance.currentTagInstance.arrayComponents.labelPos.ContainsKey(target) ? -1 : TRVirtualMachine.Instance.currentTagInstance.arrayComponents.labelPos[target];

                FunctionalObjectInstance func = new FunctionalObjectInstance(FunctionalObjectType.Script, file, 0, 0);

                if (tagParam.ContainsKey("target"))
                    func.LocalJump(tagParam["target"].Literal());

                ErrorLogger.Log("call : file=\"" + TRVirtualMachine.Instance.currentCallStack.scriptName + "\" " + "index=\"" + TRVirtualMachine.Instance.currentCallStack.currentPos + "\"");

                yield return TRVirtualMachine.Instance.Execute(func, tagParam);
            }
#else
            yield return null;
#endif
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
            if(TRVirtualMachine.Instance.callStack.Count <= 1)
                ErrorLogger.Log("callとreturnの不整合");
            else
                TRVirtualMachine.Instance.currentCallStack.currentPos = TRVirtualMachine.Instance.currentCallStack.endPos + 1;
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
            VariableCalcurator result = TRVirtualMachine.Instance.Evaluation(exp);
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

            VariableCalcurator result = TRVirtualMachine.Instance.Evaluation(exp);

            TRVirtualMachine.Instance.ifStack.Push(result.Bool());

            if (!result.Bool())
            {
                FunctionalObjectInstance _cuttentStack = TRVirtualMachine.Instance.currentCallStack;
                _cuttentStack.SkipTo<ElseComponent, ElseifComponent, EndifComponent>();
                _cuttentStack.currentPos--;
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
			bool _stack = TRVirtualMachine.Instance.ifStack.Pop();

            //直前が真の場合はelseifは実行されない
            if (_stack)
            {
                //スタック数合わせ
                TRVirtualMachine.Instance.ifStack.Push(true);

                FunctionalObjectInstance _cuttentStack = TRVirtualMachine.Instance.currentCallStack;
                _cuttentStack.SkipTo</*ElseComponent, ElseifComponent,*/ EndifComponent>();
                _cuttentStack.currentPos--;
            }
            else
            {
                string exp = tagParam["exp"].Literal();
                VariableCalcurator result = TRVirtualMachine.Instance.Evaluation(exp);
                TRVirtualMachine.Instance.ifStack.Push(result.Bool());

                if (!result.Bool())
                {
                    FunctionalObjectInstance _cuttentStack = TRVirtualMachine.Instance.currentCallStack;
                    _cuttentStack.currentPos++;
                    _cuttentStack.SkipTo<ElseComponent, ElseifComponent, EndifComponent>();
                    _cuttentStack.currentPos--;
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
			bool _stack = TRVirtualMachine.Instance.ifStack.Peek();

            //直前が真の場合はelseは実行されない
            if (_stack)
            {
                FunctionalObjectInstance _cuttentStack = TRVirtualMachine.Instance.currentCallStack;
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
            if (TRVirtualMachine.Instance.ifStack.Count == 0)
                ErrorLogger.Log("Invalid endif");
            else
    			TRVirtualMachine.Instance.ifStack.Pop();
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

        protected override void TagFunction()
        {
#if UNITY_EDITOR && TR_DEBUG
            System.Diagnostics.Debug.WriteLine(TRVirtualMachine.Instance.globalVariableInstance[tagParam["exp"].Literal()]);
#endif
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
#if UNITY_EDITOR && TR_DEBUG
            TRVirtualMachine.Instance.globalVariableInstance.Clear();
#endif
        }
    }   
}
