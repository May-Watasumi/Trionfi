using UnityEngine;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

namespace Trionfi
{
    public abstract class AbstractComponent
    {
        //デフォルトで定義しておくパラメータ初期値。継承先で定義する
        public TagParam tagParam;
        public ParamDictionary expressionedParams = new ParamDictionary();

//#if UNITY_EDITOR || DEVELOPMENT_BUILD || TRIONFI_DEBUG
        public List<string> essentialParams = new List<string>();

        [Conditional("UNITY_EDITOR"), Conditional("TRIONFI_DEBUG"), Conditional("DEVELOPMENT_BUILD")]
        public void Validate(bool stopOnError = false)
        {
            //タグから必須項目が漏れていないか、デフォルト値が入ってない場合はエラーとして警告を返す
            foreach (string param in essentialParams)
            {
                if (!tagParam.ContainsKey(param))
                {
                    //エラーを追加
                    string message = "必須パラメータ「" + param + "」が不足しています";
                    ErrorLogger.AddLog(message, "", tagParam.lineCount, false);
                }
            }
        }
//#endif

        //同期フラグ。タグの引数側で設定される。
        public bool SyncWait
        {
            get { return tagParam != null ? tagParam.syncWait : false; }
        }       

//コンストラクタ
        //引数なしの場合
        public AbstractComponent()
        {
            ErrorLogger.Log("Tag:" + GetType().Name);
        }
        //引数あり
        public AbstractComponent(TagParam param)//, int line_num)
        {
            Init(param);
        }

        public void Init(TagParam param)//, int line_num)
        {
            ErrorLogger.Log("Tag:" + GetType().Name);
            this.tagParam = param;

            Validate();
        }

        //タグ名を取得（デバッグ用？）
        public string tagName
        {
            get
            {
                string _tag = this.GetType().Name.Replace("Component", "");
                return _tag;
            }
        }

        //タグ実行本体
        abstract protected void TagFunction();

        //非同期の時に待つための関数（各種フェードインアウト等）
        public virtual IEnumerator TagAsyncWait()
        {
            yield return null;
        }

        //タグの実行
        public void Execute()
        {
            expressionedParams = tagParam.Expression();
            TagFunction();
        }

        //Before→Execute→TagAsyncWait()→Afterの順番。Afterはどうするかは疑問

        public virtual void Before() { }

        public virtual void After() { }

#if false
        //パラメータとの差分を確認して、ファイルを作成
        public void MergeDefaultParam()
        {
            ParamDictionary param = tag;

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
#endif
	}
}
