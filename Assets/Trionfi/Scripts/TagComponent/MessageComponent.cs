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

//            originalParamDic = new ParamDictionary() {
//                { "val","" }
//            };
        }

        protected override IEnumerator Start()
        {
            string message = expressionedParams["val"];
            TRUIManager.Instance.currentMessageWindow.ShowMessage(message, TRSystemConfig.Instance.messageShowWait);
            yield return null;
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

//            originalParamDic = new ParamDictionary() {
//                { "val","" },
//                { "face","" }
//            };
        }

        protected override IEnumerator Start()
        {
            string name = expressionedParams["val"];
            TRUIManager.Instance.currentMessageWindow.ShowName(name);
            yield return null;
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

            protected override IEnumerator Start()
            {
                TRUIManager.Instance.currentMessageWindow.currentMessage.text += "\n";
                yield return null;
            }
        }
    */
    //クリック待ち
    public class LComponent : AbstractComponent
    {
 //       public LComponent() { }

        protected override IEnumerator Start()
        {
            yield return TRUIManager.Instance.currentMessageWindow.Wait();
        }
    }

    //クリックを待ち＋メッセージクリア
    public class PComponent : AbstractComponent
    {
//        public PComponent() { }

        protected override IEnumerator Start()
        {
            yield return TRUIManager.Instance.currentMessageWindow.Wait();
            TRUIManager.Instance.currentMessageWindow.ClearMessage();
            yield return null;
        }
    }

    public class CmComponent : AbstractComponent
    {
//        public CmComponent() { originalParamDic = new ParamDictionary() { }; }

        protected override IEnumerator Start()
        {
            TRUIManager.Instance.currentMessageWindow.ClearMessage();
            yield return null;
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

		protected override IEnumerator Start() {
            int size = expressionedParams.Int("size");
            Color color = TRUtility.HexToRGB(expressionedParams.String("color"));

            TRUIManager.Instance.currentMessageWindow.currentMessage.fontSize = size;
            TRUIManager.Instance.currentMessageWindow.currentMessage.color = color;
            yield return null;
        }
	}

	public class FontresetComponent : AbstractComponent
	{
		public FontresetComponent() { }

		protected override IEnumerator Start()
		{
//            TRUIManager.Instance.currentMessageWindow.Reset();
            TRUIManager.Instance.currentMessageWindow.currentMessage.fontSize = TRSystemConfig.Instance.fontSize;
            TRUIManager.Instance.currentMessageWindow.currentMessage.color = TRSystemConfig.Instance.fontColor;
            yield return null;
        }
    }
}
