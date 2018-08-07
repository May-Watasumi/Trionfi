using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Trionfi
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

		protected override void TagFunction() {
            TRUIInstance.Instance.selectWindow.Add(tagParam.Identifier("text"));
            //ToDo:storage等飛び先を保存
        }
    }

	public class SelectComponent : AbstractComponent
	{
		public SelectComponent() { }

		protected override void TagFunction()
		{
            TRUIInstance.Instance.selectWindow.Begin();
        }

        protected override IEnumerator TagSyncFunction()
        {
            yield return new WaitWhile(() => TRUIInstance.Instance.selectWindow.state == TRSelectWindow.SelectState.Wait);
        }
    }
}
