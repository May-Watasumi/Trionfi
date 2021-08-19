#if !TR_PARSEONLY
 using UnityEngine;
#endif

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
				"target",
			};
#endif
        }

		protected override void TagFunction()
		{
#if !TR_PARSEONLY
			Trionfi.Instance.selectWindow.Add(tagParam["text"].Literal(), tagParam["target"].Literal());
#endif
		}
    }

	public class SelectComponent : AbstractComponent
	{
		public SelectComponent() { }

		protected override void TagFunction()
		{
#if !TR_PARSEONLY
			Trionfi.Instance.selectWindow.Begin();
#endif
		}

#if !TR_PARSEONLY
		public override IEnumerator TagSyncFunction()
        {
            yield return new WaitWhile(() => Trionfi.Instance.selectWindow.onWait);

            if (!TRVirtualMachine.currentCallStack.LocalJump(TRSelectWindow.Instance.result))
                ErrorLogger.Log("No Jump target:" + TRSelectWindow.Instance.result);
        }
#endif
	}
}
