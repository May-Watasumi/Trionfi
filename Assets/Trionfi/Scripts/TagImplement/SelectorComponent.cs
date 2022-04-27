#if !TR_PARSEONLY
 using UnityEngine;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using TRVariable = Jace.Operations.VariableCalcurator;

namespace Trionfi
{
	[Serializable]
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

	[Serializable]
	public class SelectComponent : AbstractComponent
	{
		public SelectComponent() {	}

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

			if (tagParam.ContainsKey("var"))
			{
				TRVirtualMachine.Instance.globalVariableInstance[tagParam["var"].Literal()] = new TRVariable(TRSelectWindow.Instance.resutNum);
			}
			else if (!TRVirtualMachine.Instance.currentCallStack.LocalJump(TRSelectWindow.Instance.result))
			{
				ErrorLogger.Log("No Jump target:" + TRSelectWindow.Instance.result);
			}
        }
#endif
	}
}
