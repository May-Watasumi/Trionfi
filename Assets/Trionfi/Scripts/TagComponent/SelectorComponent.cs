using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NovelEx
{
	public class SeladdComponent : AbstractComponent {
		public SeladdComponent() {
			//必須項目
			essentialParams = new List<string> {
				"text",
//				"storage",
			};

//			originalParamDic = new ParamDictionary() {
//				{"text",""},
//				{"storage",""}
//			};
		}

		protected override IEnumerator Start() {
            TRUIManager.Instance.currentSelectWindow.Add(expressionedParams["text"]);
            //ToDo:storage等飛び先を保存
            yield return null;
        }
    }

	public class SelectComponent : AbstractComponent
	{
		public SelectComponent() { }

		protected override IEnumerator Start()
		{
            TRUIManager.Instance.currentSelectWindow.Begin();
            yield return new WaitWhile(() => TRUIManager.Instance.currentSelectWindow.state == SelectWindow.SelectState.Wait);
        }
    }
}
