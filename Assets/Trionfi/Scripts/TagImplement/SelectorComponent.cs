using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Trionfi
{
	public class SeladdComponent : AbstractComponent {
		public SeladdComponent() {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
				"text",
//				"storage",
			};

            //			originalParamDic = new ParamDictionary() {
            //				{"text",""},
            //				{"storage",""}
            //			};
#endif
        }

		protected override void TagFunction() {
            Trionfi.Instance.selectWindow.Add(tagParam.Identifier("text"));
            //ToDo:storage等飛び先を保存
        }
    }

	public class SelectComponent : AbstractComponent
	{
		public SelectComponent() { }

		protected override void TagFunction()
		{
            Trionfi.Instance.selectWindow.Begin();
        }

        public override IEnumerator TagSyncFunction()
        {
            yield return new WaitWhile(() => Trionfi.Instance.selectWindow.state == TRSelectWindow.SelectState.Wait);
        }
    }
}
