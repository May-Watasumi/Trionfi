#if !TR_PARSEONLY
using UnityEngine;
using Cysharp.Threading.Tasks;
using TRTask = Cysharp.Threading.Tasks.UniTask;
using TRTaskString = Cysharp.Threading.Tasks.UniTask<string>;
#else
using TRTask = System.Threading.Tasks.Task;
using TRTaskString = System.Threading.Tasks.Task<string>;
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

		protected override async TRTaskString TagFunction()
		{
#if !TR_PARSEONLY
			Trionfi.Instance.selectWindow.Add(tagParam["text"].Literal(), tagParam["target"].Literal());
#endif
			return string.Empty;
		}
    }

	[Serializable]
	public class SelectComponent : AbstractComponent
	{
		public SelectComponent() {	}

		protected override async TRTaskString TagFunction()
		{
#if !TR_PARSEONLY
			Trionfi.Instance.selectWindow.Begin();

			await UniTask.WaitWhile(() => Trionfi.Instance.selectWindow.onWait && TRVirtualMachine.Instance.state == TRVirtualMachine.State.Run);

			if (TRVirtualMachine.Instance.state != TRVirtualMachine.State.Run)
			{
				Trionfi.Instance.selectWindow.gameObject.SetActive(false);
				Trionfi.Instance.PopWindow(); 
			}
			else if (tagParam.ContainsKey("var"))
			{
				TRVirtualMachine.Instance.globalVariableInstance[tagParam["var"].Literal()] = new TRVariable(Trionfi.Instance.selectWindow.resutNum);
			}
			else if (!TRVirtualMachine.Instance.currentCallStack.LocalJump(Trionfi.Instance.selectWindow.result))
			{
				ErrorLogger.Log("No Jump target:" + Trionfi.Instance.selectWindow.result);
			}
#endif
			return string.Empty;
		}
	}
}
