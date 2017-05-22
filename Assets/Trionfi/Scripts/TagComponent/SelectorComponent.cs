using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NovelEx
{
	public class SeladdComponent : AbstractComponent {
		public SeladdComponent() {
			//必須項目
			arrayVitalParam = new List<string> {
				"text",
//				"storage",
			};

			originalParamDic = new ParamDictionary() {
				{"text",""},
				{"storage",""}
			};
		}

		public override IEnumerator Start() {
            TRUIManager.Instance.currentSelectWindow.Add(paramDic["text"]);
            //ToDo:storage等飛び先を保存
            yield return null;
        }
    }

	public class SelectComponent : AbstractComponent
	{
		public SelectComponent() { }

		public override IEnumerator Start()
		{
            TRUIManager.Instance.currentSelectWindow.Begin();
            yield return new WaitWhile(() => TRUIManager.Instance.currentSelectWindow.state == SelectWindow.SelectState.Wait);
        }
    }
}
