using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NovelEx
{
    public abstract class AbstractComponent
    {
        //デフォルトで定義しておくパラメータ初期値。継承先で定義する
        //public TagParam tagParam = new TagParam();
        //public ParamDictionary originalParamDic = new ParamDictionary();
        public TagParam tagParam;
        public ParamDictionary expressionedParams = new ParamDictionary();

#if UNITY_EDITOR || DEVELOPMENT_BUILD || TRIONFI_DEBUG
        public List<string> essentialParams = new List<string>();

        public void Validate(bool stopOnError = false)
        {
            //タグから必須項目が漏れていないか、デフォルト値が入ってない場合はエラーとして警告を返す
            foreach (string param in essentialParams)
            {
                if (!tagParam.ContainsKey(param))
                {
                    //エラーを追加
                    string message = "必須パラメータ「" + param + "」が不足しています";
                    ErrorLogger.addLog(message, "", tagParam.lineCount, false);
                }
            }
        }
#endif
        //引数なしの場合
        public AbstractComponent()
        {
            ErrorLogger.Log("Tag:" + GetType().Name);
        }

        public AbstractComponent(TagParam param)//, int line_num)
        {
            Init(param);
        }

        public void Init(TagParam param)//, int line_num)
        {
            ErrorLogger.Log("Tag:" + GetType().Name);
            this.tagParam = param;
            //DEBUG
            Validate();
        }
        public string tagName()
        {
            string _tag = this.GetType().Name.Replace("Component", "");
            return _tag;
        }

        abstract protected IEnumerator Start();

        public IEnumerator Exec()
        {
            expressionedParams = tagParam.Expression();
            yield return Start(); 
        }

        //Start()前にかならず実行されます。skip中でも実行されるのでチェックしたい項目があれば、継承先で実装してください
        public virtual void Before() { }

        //Start()後ににかならず実行されます。skip中でも実行されるのでチェックしたい項目があれば、継承先で実装してください
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
