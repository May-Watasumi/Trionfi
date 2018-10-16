using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Trionfi
{
    //マクロを作成して管理する
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
            string name = tagParam.Literal("name");
            TRVirtualMachine.FunctionalObjectInstance _cuttentStack = TRVirtualMachine.currentCallStack;

            TRVirtualMachine.FunctionalObjectInstance function = new TRVirtualMachine.FunctionalObjectInstance(TRVirtualMachine.FunctionalObjectType.Macro, _cuttentStack.scriptName, _cuttentStack.currentPos);
            TRVirtualMachine.functionalObjects[name] = function;
            _cuttentStack.SkipTo<MacroendComponent>();
        }
    }


    //マクロ定義の終了宣言
    public class MacroendComponent : AbstractComponent
    {
        public MacroendComponent() { }

        protected override void TagFunction()
        {
            TRVirtualMachine.callStack.Pop();
        }
    }

    //マクロを作成して管理する
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
            string name = tagParam.Literal("name");
            TRVirtualMachine.functionalObjects.Remove(name);
        }
    }

    //マクロを作成して管理する
    public class ClearallmacroComponent : AbstractComponent
    {
        public ClearallmacroComponent()
        {
        }

        protected override void TagFunction()
        {
            TRVirtualMachine.functionalObjects.Clear();
        }
    }
}
