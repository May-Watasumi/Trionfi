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
            TRVirtualMachine.FunctionalObject function = new TRVirtualMachine.FunctionalObject();
            function.scriptName = TRVirtualMachine.currentScriptName;
            function.pos = TRVirtualMachine.currentTagInstance.currentComponentIndex;
            TRVirtualMachine.functionalObject[name] = function;
        }
    }
    /*
        //マクロを実行するためのタグ
        public class _MacrostartComponent : AbstractComponent {
            public _MacrostartComponent() {

                //必須項目
                essentialParams = new List<string> {
                    "name"
                };
            }

            protected override void TagFunction()
            {
                expressionedParams["name"] = tagName;

                Macro macro = TRVirtualMachine.GetMacro(expressionedParams["name"]);

                if (macro != null)
                {

                    expressionedParams["index"] = "" + macro.index;
                    expressionedParams["file"] = macro.file_name;

                    TRVirtualMachine.macroNum++;
                    //this.gameManager.scenarioManager.addMacroStack (macro.name, this.expressionedParams);
                    AbstractComponent cmp = TRScriptParser.Instance.MakeTag("call", expressionedParams);
                    cmp.Execute();
                }
                else
                {
                    ErrorLogger.StopError("マクロ「" + expressionedParams["name"] + "」は存在しません。");
                }
            }
        }
    */

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
            TRVirtualMachine.functionalObject.Remove(name);
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
            TRVirtualMachine.functionalObject.Clear();
        }
    }
}
