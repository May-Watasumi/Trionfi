using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Trionfi
{
    //[story val="メッセージ"]
    public class MessageComponent : AbstractComponent
    {
        public MessageComponent()
        {
            //必須項目
            essentialParams = new List<string> {
                "val"
            };
        }

        protected override void TagFunction()
        {
            string message = tagParam.Identifier("val");
            TRUIManager.Instance.currentMessageWindow.ShowMessage(message, TRGameConfig.Instance.textSpeed);
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
            string name = tagParam.Identifier("val");
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
        }

        public override IEnumerator TagAsyncWait()
        {
            yield return TRUIManager.Instance.currentMessageWindow.Wait();
            TRUIManager.Instance.currentMessageWindow.ClearMessage();
            yield return null;
        }
    }

    //メッセージクリア
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
            int size = tagParam.Int("size");
            uint colorValue = tagParam.Uint("color", 0xFFFFFFFF);

            Color color = TRVariable.ToRGB(colorValue);
            TRUIManager.Instance.currentMessageWindow.currentMessage.fontSize = size;
            TRUIManager.Instance.currentMessageWindow.currentMessage.color = color;
        }
	}
}
