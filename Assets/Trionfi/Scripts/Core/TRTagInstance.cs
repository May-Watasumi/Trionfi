using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Trionfi
{
    [Serializable]
	public class TRTagInstance
    {
        public List<AbstractComponent> tagComponents;
        public Dictionary<string, int> labelInfo = new Dictionary<string, int>();

        public List<AbstractComponent> arrayComponents = new List<AbstractComponent>();
        public int currentComponentIndex = -1;      //-1は未初期化、0からスタート

		public bool CompileScriptString(string text) {
            ErrorLogger.Clear();

            //パーサーを動作させる
            tagComponents = TRScriptParser.Instance.Parse(text);

            int _index = 0;

            foreach (AbstractComponent _component in arrayComponents)
            {
                if (_component.tagName == "label")
                    AddLabel(_component.expressionedParams["name"], _index);

                _index++;
            }

            return ErrorLogger.ShowAll();
        }

		public bool CompileScriptFile(string storage)
		{
//				string fullpath = /*useStoragePath ? StorageManager.Instance.PATH_SD_SCENARIO :*/ "";
				TextAsset script_text = StorageManager.Instance.LoadObject(storage, TRDataType.TextAsset) as TextAsset;

				return CompileScriptString(script_text.text);
		}

        public void AddLabel(string label_name, int index)
        {
            labelInfo[label_name] = index;
        }

        public int GetLabelPosition(string label_name)
        {
            if (string.IsNullOrEmpty(label_name) || !labelInfo.ContainsKey(label_name))
            {
                ErrorLogger.StopError("にラベル「" + label_name + "」が見つかりません。");
                return -1;
            }

            return labelInfo[label_name];
        }

        //0=デフォルト1=componentのフラグが立ってない-1シナリオ最後に
        public IEnumerator Run(int index = 0)
		{
                currentComponentIndex = index;
                //			StatusManager.Instance.currentScenario = this;
                //			StatusManager.Instance.MessageShow();

            if (currentComponentIndex < arrayComponents.Count)
			{
				AbstractComponent _tagComponent = arrayComponents[currentComponentIndex];

                _tagComponent.Before();

				//タグ
//				if (StatusManager.Instance.currentState == JokerState.SkipOrder)
				{
					Debug.Log("SkipOrderされました");
				}
//				else
				{
//                    _tagComponent.CalcVariable();
//                    _tagComponent.Validate();

					string p = "";
					foreach (KeyValuePair<string, string> kvp in _tagComponent.expressionedParams)
					{
						p += kvp.Key + "=" + kvp.Value + " ";
					}

					if(TRSystemConfig.Instance.showTag)
					{
						Debug.Log("[" + _tagComponent.tagName + " " + p + " ]");
					}

					_tagComponent.Execute();
				}

                _tagComponent.After();

                yield return _tagComponent.TagAsyncWait();

                //ToDo:flag
                currentComponentIndex++;
			}
            /*
			//シナリオファイルの最後まで来た時。スタックが存在するならreturn する
			//スタックがあるならreturn する
			if(CountStack() > 0)
			{
                ReturnComponent _ret = new ReturnComponent();
                _ret.Execute();
//                TRScriptParser.Instance.StartTag("[return]");
			}
			else
			{
				StatusManager.Instance.EndScenario();
			}
            */
            yield return null;			
		}
	}
};
