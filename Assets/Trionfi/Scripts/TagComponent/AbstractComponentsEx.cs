using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NovelEx
{
	//完了通知用のデリゲートメソッド
	public delegate void CompleteDelegate();

	public abstract class AbstractComponent
    {
        //デフォルトで定義しておくパラメータ初期値。継承先で定義する
        public ParamDictionary originalParamDic = new ParamDictionary();
        public ParamDictionary paramDic = new ParamDictionary();

		public List<string> arrayVitalParam = new List<string>();

		public string line;
		public int line_num;
		public string tagName;

        protected TagParam tag;

		public AbstractComponent()
        {
            ErrorLogger.Log("Tag:"+GetType().Name);
        }

		public void Init(TagParam tag, int line_num)
        {
			this.tag = tag;
			this.tagName = tag.Name;
			this.line_num = line_num;
		}

		public void CheckParam() {
			//タグから必須項目が漏れていないか、デフォルト値が入ってない場合はエラーとして警告を返す
			foreach (string vital in arrayVitalParam)
            {
                if(!tag.ContainsKey(vital))
                {
					//エラーを追加
					string message = "必須パラメータ「" + vital + "」が不足しています";
					ErrorLogger.addLog(message, "", line_num, false);
				}
			}
		}

        //継承先で渡されたパラメータについて正常かどうかをチェックします
        public virtual void Validate() { }

		//Start()前にかならず実行されます。skip中でも実行されるのでチェックしたい項目があれば、継承先で実装してください
		public virtual void Before() { }

        abstract public IEnumerator Start();

        //Start()後ににかならず実行されます。skip中でも実行されるのでチェックしたい項目があれば、継承先で実装してください
        public virtual void After() { }

		//実行前にパラメータを解析して変数を格納する
		public void CalcVariable()
        {
			ParamDictionary tmp_param = new ParamDictionary();

			//タグに入れる
			foreach (KeyValuePair<string, string> pair in originalParamDic)
            {
				tmp_param[pair.Key] = ExpObject.replaceVariable(pair.Value/*originalParamDic[pair.Key]*/);
			}

			//タグにデフォルト値を設定中かつ、tag が指定されていない場合
			if(StatusManager.Instance.TagDefaultVal != "")
            {
				if (tmp_param.ContainsKey("tag") && tmp_param["tag"] =="")
					tmp_param["tag"] = StatusManager.Instance.TagDefaultVal;
			}

            paramDic = tmp_param;
		}

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

	}
}
