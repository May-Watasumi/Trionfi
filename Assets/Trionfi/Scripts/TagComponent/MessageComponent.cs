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
            arrayVitalParam = new List<string> {
                "val"
            };

            originalParamDic = new ParamDictionary() {
                { "val","" }
            };
        }

        public override IEnumerator Start()
        {
            string message = paramDic["val"];
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
            arrayVitalParam = new List<string> {
                "val"
            };

            originalParamDic = new ParamDictionary() {
                { "val","" },
                { "face","" }
            };
        }

        public override IEnumerator Start()
        {
            string name = paramDic["val"];
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

            public override IEnumerator Start()
            {
                TRUIManager.Instance.currentMessageWindow.currentMessage.text += "\n";
                yield return null;
            }
        }
    */
    //クリック待ち
    public class LComponent : AbstractComponent
    {
        public LComponent() { originalParamDic = new ParamDictionary() { }; }

        public override IEnumerator Start()
        {
            yield return TRUIManager.Instance.currentMessageWindow.Wait();
        }
    }

    //クリックを待ち＋メッセージクリア
    public class PComponent : AbstractComponent
    {
        public PComponent() { originalParamDic = new ParamDictionary() { }; }

        public override IEnumerator Start()
        {
            yield return TRUIManager.Instance.currentMessageWindow.Wait();
            TRUIManager.Instance.currentMessageWindow.ClearMessage();
            yield return null;
        }
    }

    public class CmComponent : AbstractComponent
    {
        public CmComponent() { originalParamDic = new ParamDictionary() { }; }

        public override IEnumerator Start()
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
			arrayVitalParam = new List<string> { };

			originalParamDic = new ParamDictionary() {
				{"size",""},
				{"color",""},
			};
		}

		public override IEnumerator Start() {
            int size = paramDic.Int("size");
            Color color = TRUtility.HexToRGB(paramDic.String("color"));

            TRUIManager.Instance.currentMessageWindow.currentMessage.fontSize = size;
            TRUIManager.Instance.currentMessageWindow.currentMessage.color = color;
            yield return null;
        }
	}

	public class FontresetComponent : AbstractComponent
	{
		public FontresetComponent() { }

		public override IEnumerator Start()
		{
//            TRUIManager.Instance.currentMessageWindow.Reset();
            TRUIManager.Instance.currentMessageWindow.currentMessage.fontSize = TRSystemConfig.Instance.fontSize;
            TRUIManager.Instance.currentMessageWindow.currentMessage.color = TRSystemConfig.Instance.fontColor;
            yield return null;
        }
    }
}
