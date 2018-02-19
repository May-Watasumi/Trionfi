using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NovelEx
{
    //[story val="メッセージ"]
    public class StoryComponent : AbstractComponent
    {
        public StoryComponent()
        {
            //必須項目
            essentialParams = new List<string> {
                "val"
            };
        }

        protected override void TagFunction()
        {
            string message = expressionedParams["val"];
            TRUIManager.Instance.currentMessageWindow.ShowMessage(message, TRSystemConfig.Instance.messageShowWait);
        }
    }

    //[name val="なまえ" face="表情"]
    public class NameComponent : AbstractComponent
    {
        public NameComponent()
        {
            //必須項目
            essentialParams = new List<string> {
                "val"
            };
        }

        protected override void TagFunction()
        {
            string name = expressionedParams["val"];
            TRUIManager.Instance.currentMessageWindow.ShowName(name);
        }
    }

    /*
        //改行命令 [r]
        public class RComponent : AbstractComponent
        {
            public RComponent()
            {
                originalParamDic = new ParamDictionary() { };
            }

            protected override void TagFunction()
            {
                TRUIManager.Instance.currentMessageWindow.currentMessage.text += "\n";
                yield return null;
            }
        }
    */

    //クリック待ち
    public class LComponent : AbstractComponent
    {
        protected override void TagFunction()
        {
            return;
        }

        public override IEnumerator TagAsyncWait()
        {
            yield return TRUIManager.Instance.currentMessageWindow.Wait();
        }
    }

    //クリックを待ち＋メッセージクリア
    public class PComponent : AbstractComponent
    {
        protected override void TagFunction()
        {
            return;
        }

        public override IEnumerator TagAsyncWait()
        {
            yield return TRUIManager.Instance.currentMessageWindow.Wait();
        }
    }

    public class CmComponent : AbstractComponent
    {
        protected override void TagFunction()
        {
            TRUIManager.Instance.currentMessageWindow.ClearMessage();
        }
    }

    //フォント設定    
    //[font size=26 color=#FFFFFF80]
	public class FontComponent : AbstractComponent {
		public FontComponent() {
			//必須項目
			essentialParams = new List<string> { };
//			originalParamDic = new ParamDictionary() {
//				{"size",""},
//				{"color",""},
//			};
		}

		protected override void TagFunction() {
            int size = expressionedParams.Int("size");
            Color color = TRUtility.HexToRGB(expressionedParams.String("color"));

            TRUIManager.Instance.currentMessageWindow.currentMessage.fontSize = size;
            TRUIManager.Instance.currentMessageWindow.currentMessage.color = color;
        }
	}

	public class FontresetComponent : AbstractComponent
	{
		public FontresetComponent() { }

		protected override void TagFunction()
		{
//            TRUIManager.Instance.currentMessageWindow.Reset();
            TRUIManager.Instance.currentMessageWindow.currentMessage.fontSize = TRSystemConfig.Instance.fontSize;
            TRUIManager.Instance.currentMessageWindow.currentMessage.color = TRSystemConfig.Instance.fontColor;
        }
    }
}
