using System;
using System.Collections;
using System.Collections.Generic;
//using ExpressionParser;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
            string className = "Trionfi." + TRParserBase.tf.ToTitleCase(tagParam.Identifier("tag")) + "Component";

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
                TRVirtualMachine.aliasTagInstance[tagParam.Identifier("name")] = _component;
            }
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
            TRVirtualMachine.Serialize(tagParam.Identifier("file"));
        }
    }

    //ジャンプ＝コールスタックを変えない。いわゆるgoto
	public class JumpComponent : AbstractComponent {
		public JumpComponent() {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
//				"target"
			};
#endif
        }

        protected override void TagFunction() {  }

        public override IEnumerator TagSyncFunction()
        {
            string target = tagParam.Label("target");
            string file = tagParam.Identifier("file", TRVirtualMachine.currentScriptName);

			//ファイルが異なるものになる場合、シナリオをロードする
			if(TRVirtualMachine.currentScriptName != file)
			{
                yield return TRVirtualMachine.Instance.LoadScenarioAsset(file);

                //ToDo:スタックをすべて削除する
                TRVirtualMachine.RemoveAllStacks();

                TRVirtualMachine.currentScriptName = file;
            }

            if (string.IsNullOrEmpty(file))
                file = TRVirtualMachine.currentScriptName;

            int index = TRVirtualMachine.tagInstance[file].arrayComponents.labelPos.ContainsKey(target) ? -1 : TRVirtualMachine.tagInstance[file].arrayComponents.labelPos[target];

            if (index < 0)
                ErrorLogger.StopError("にラベル「" + target + "」が見つかりません。");
            else
                ErrorLogger.Log("jump : file=\"" + TRVirtualMachine.currentScriptName + "\" " + "index=\"" + TRVirtualMachine.currentTagInstance.currentComponentIndex + "\"");

            //ToDo:メインループ側で配列Indexが++されるのであまり美しくない。
            TRVirtualMachine.currentTagInstance.currentComponentIndex = index;
            TRVirtualMachine.currentTagInstance.currentComponentIndex--;
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
            string target = tagParam.Label("target");

            string file = tagParam.Identifier("file", TRVirtualMachine.currentScriptName);

            int index = string.IsNullOrEmpty(file) ? -1 : TRVirtualMachine.tagInstance[file].arrayComponents.labelPos[target];

			ErrorLogger.Log("Call : file=\"" + file + "\" " + "index = \"" + index.ToString()+ "\"");

            TRVirtualMachine.callStack.Push(new CallStackObject(TRVirtualMachine.currentScriptName, TRVirtualMachine.currentTagInstance.currentComponentIndex, tagParam));
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
            CallStackObject stack = TRVirtualMachine.callStack.Pop();

			string tag_str = "";

			//return 時の戻り場所を指定できます
			if( string.IsNullOrEmpty(tagParam.Identifier("file")) && string.IsNullOrEmpty(tagParam.Label("target")) )
				tag_str = "[jump file='" + tagParam["file"] + "' target='" + tagParam["target"] + "' ]";
			else
				tag_str = "[jump file='" + stack.scenarioNname + "' index='" + stack.index + "' ]";

			Debug.Log("RETURN scn=\"" + stack.scenarioNname + "\" " + "index=\"" + stack.index.ToString()+ "\"");// + " param=\"" + this.expressionedParams.ToStringFull());

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
            string file = tagParam.Identifier("file");
            syncState = SceneManager.LoadSceneAsync(file, LoadSceneMode.Additive);
            yield return new WaitUntil(() => syncState.isDone);
        }
    }

    //Jaceによってcalcとflagは差別化がなくなったので不要になる
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
            string _exp = tagParam.Literal("exp");
            string[] _exp2 = _exp.Split('=');
            double result = TRVirtualMachine.Calc(TRVirtualMachine.variableInstance, _exp2[1]);

            TRVirtualMachine.variableInstance[_exp2[0]] = new KeyValuePair<string, TRDataType>(result.ToString(), TRDataType.Float);
        }
    }
	public class FlagComponent : AbstractComponent {
		public FlagComponent() {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
				"exp"
			};
#endif
        }

		protected override void TagFunction() {
            double result = TRVirtualMachine.Calc(TRVirtualMachine.variableInstance, tagParam.Literal("exp"));
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

            double result = TRVirtualMachine.Calc(TRVirtualMachine.variableInstance, tagParam.Literal("exp"));
//            ToDo:
            /*
			//条件に合致した場合はそのままifの中へ
			if(result == "false" || result == "0")
                TRVirtualMachine.ifStack.Push(false);
            else
                TRVirtualMachine.ifStack.Push(true);
            */
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
                TRVirtualMachine.ifStack.Push(false);
            else
            {
                string exp = tagParam.Literal("exp");
                double result = TRVirtualMachine.Calc(TRVirtualMachine.variableInstance, exp);
                //ToDo:
/*
                string result = ExpObject.calc(exp);

                //条件に合致した場合はそのままifの中へ
                if (result == "false" || result == "0")
                    TRVirtualMachine.ifStack.Push(false);
                else
                    TRVirtualMachine.ifStack.Push(true);
*/
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
            bool _stack = TRVirtualMachine.ifStack.Pop();

            //直前が真の場合はelseifは実行されない
            TRVirtualMachine.ifStack.Push(!_stack);
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
        float _time = 0.0f;
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
            _time = tagParam.Float("time");
		}

        public override IEnumerator TagSyncFunction()
        {
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
			string url = tagParam.Identifier("url");
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
