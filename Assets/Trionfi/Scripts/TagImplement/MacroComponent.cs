#if !TR_PARSEONLY
 using UnityEngine;
 using UnityEngine.SceneManagement;
 using UnityEngine.UI;
#endif

using System;
using System.Collections;
using System.Collections.Generic;

namespace Trionfi
{
    //マクロを作成して管理する
    [Serializable]
    public class MacroComponent : AbstractComponent
    {
        public MacroComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
                "name"
            };
#endif
        }

        protected override void TagFunction()
        {
#if !TR_PARSEONLY
            string name = tagParam["name"].Literal();
            FunctionalObjectInstance _cuttentStack = TRVirtualMachine.Instance.currentCallStack;

            int beginPos = _cuttentStack.currentPos + 1;
            _cuttentStack.SkipTo<MacroendComponent>();
            int endPos = _cuttentStack.currentPos;

            FunctionalObjectInstance function = new FunctionalObjectInstance(FunctionalObjectType.Macro, _cuttentStack.scriptName, beginPos, endPos);
            TRVirtualMachine.Instance.functionalObjects[name] = function;
#endif
        }
    }

    //マクロ定義の終了宣言
    [Serializable]
    public class MacroendComponent : AbstractComponent
    {
        public MacroendComponent() { }

        protected override void TagFunction()
        {
#if !TR_PARSEONLY
            /*
			TRVirtualMachine.FunctionalObjectInstance _func = TRVirtualMachine.callStack.Peek();

            if (_func.type == TRVirtualMachine.FunctionalObjectType.Macro)
            {

                _func.currentPos += 999999;
            }
            */
#endif
		}
    }

    //マクロを作成して管理する
    [Serializable]
    public class MacroEraceComponent : AbstractComponent
    {
        public MacroEraceComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
                "name"
            };
#endif
        }

        protected override void TagFunction()
        {
#if !TR_PARSEONLY
			string name = tagParam["name"].Literal();
            TRVirtualMachine.Instance.functionalObjects.Remove(name);
#endif
		}
    }

    //マクロを作成して管理する
    [Serializable]
    public class ClearallmacroComponent : AbstractComponent
    {
        public ClearallmacroComponent()
        {
        }

        protected override void TagFunction()
        {
#if !TR_PARSEONLY
			TRVirtualMachine.Instance.functionalObjects.Clear();
#endif
		}
    }
}
