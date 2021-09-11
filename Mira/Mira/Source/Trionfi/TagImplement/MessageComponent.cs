#if !TR_PARSEONLY
 using UnityEngine;
 using UnityEngine.UI;
 #if UNITY_2017_1_OR_NEWER
  using UnityEngine.U2D;
 #endif
using DG.Tweening;
#endif

using System;
using System.Collections;
using System.Collections.Generic;

namespace Trionfi
{
    [Serializable]
    public class TextdataComponent : AbstractComponent
    {
        public TextdataComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
                "storage",
            };
#endif
        }

        protected override void TagFunction()
        {
            hasSync = true;
        }

#if !TR_PARSEONLY
        public override IEnumerator TagSyncFunction()
        {
            string storage = tagParam["storage"].Literal();


            //            while (TRResourceLoader.Instance.isLoading)
            //                yield return new WaitForSeconds(1.0f);

            //            if (TRResourceLoader.Instance.isSuceeded)

            if (!string.IsNullOrEmpty(storage))
            {
                var coroutine = TRResourceLoader.Instance.LoadText(storage);
                yield return TRResourceLoader.Instance.StartCoroutine(coroutine);
                TRVirtualMachine.currentTagInstance.ReadTextData((string)coroutine.Current);
            }

            yield return null;
        }
#endif
    }

    [Serializable]
    public class SetlanguageComponent : AbstractComponent
    {
        public SetlanguageComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
                "lang",
            };
#endif
        }

        protected override void TagFunction()
        {
#if !TR_PARSEONLY
            string id = tagParam["lang"].Literal();

            switch (id.ToUpper())
            {
                case "JP":
                case "JPN":
                case "JAPAN":
                default:
                    TRSystemConfig.Instance.localizeID = LocalizeID.JAPAN;
                    break;
                case "EN":
                case "ENG":
                case "ENGLISH":
                    TRSystemConfig.Instance.localizeID = LocalizeID.ENGLISH;
                    break;
                case "CN":
                case "CHN":
                case "CHINA":
                    TRSystemConfig.Instance.localizeID = LocalizeID.CHINESE;
                    break;
                case "KR":
                case "KOR":
                case "KOREA":
                    TRSystemConfig.Instance.localizeID = LocalizeID.KOREAN;
                    break;
            }
#endif
        }
    }
    
    //[story val="メッセージ"]
    [Serializable]
    public class MessageComponent : AbstractComponent
    {
        public MessageComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialMoreOneParams = new List<string> {
                // val or id
                "val",
                "id"
            };
#endif
        }

        protected override void TagFunction()
        {
#if !TR_PARSEONLY
            Trionfi.Instance.SetStandLayerTone();

            string message = string.Empty;

            if (tagParam.ContainsKey("val"))
                message = tagParam["val"].Literal();

            if (tagParam.ContainsKey("id"))
            {
                message = TRVirtualMachine.currentTagInstance.textData[tagParam["id"].Int()].GetText(TRSystemConfig.Instance.localizeID);
                message = message.Replace("\\r", "\r");
                message = message.Replace("\\n", "\n");
            }

            if (!Trionfi.Instance.messageWindow.gameObject.activeSelf)
                Trionfi.Instance.messageWindow.gameObject.SetActive(true);

            Trionfi.Instance.messageWindow.ShowMessage(message, TRGameConfig.configData.textspeed);
#endif
		}

#if !TR_PARSEONLY
		public override IEnumerator TagSyncFunction()
        {
            yield return new WaitWhile(() => Trionfi.Instance.messageWindow.state != TRMessageWindow.MessageState.None);

            if (!TRSystemConfig.Instance.isNovelMode)
                Trionfi.Instance.messageWindow.ClearMessage();
        }
#endif
	}

    //[name val="なまえ" face="表情"]
    [Serializable]
    public class NameComponent : AbstractComponent
    {
        public NameComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
                "val"
            };
#endif
        }

        protected override void TagFunction()
        {
#if !TR_PARSEONLY
			string name = tagParam["val"].Literal();

            if (name.Contains("/"))
            {
                string[] nameInfo = name.Split('/');
                Trionfi.Instance.messageWindow.ShowName(nameInfo[0]);
                TRLayer.currentSpeaker = nameInfo[1];
            }
            else
            {
                Trionfi.Instance.messageWindow.ShowName(name);
                TRLayer.currentSpeaker = name;
            }
#endif
		}
    }

    [Serializable]
    public class MesspeedComponent : AbstractComponent
    {
        public MesspeedComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> {
                "ratio"
            };
#endif
        }

        protected override void TagFunction()
        {
#if !TR_PARSEONLY
			float ratio = tagParam["ratio", 1.0f];
            Trionfi.Instance.messageWindow.speedRatio = ratio;
#endif
		}
    }

    [Serializable]
    public class MesshakeComponent : AbstractComponent
    {
        public MesshakeComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string>
            {
//                "layer",
            };
#endif
        }

        protected override void TagFunction()
        {
#if !TR_PARSEONLY
			//振幅
			int strength = tagParam["strength", 5];
            //振動頻度
            int vibratio = tagParam["vibrato", 20];

            RectTransform _rect = Trionfi.Instance.messageWindow.gameObject.GetComponent<RectTransform>();

            Trionfi.Instance.messageWindow.tweener = _rect.GetComponent<RectTransform>().DOShakePosition(1.0f, strength, vibratio, 90.0f, false, false).SetLoops(-1);
#endif
		}
    }

    [Serializable]
    public class MeswindowComponent : AbstractComponent
    {
        public MeswindowComponent()
        {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string>
            {
                "id"
                //                "layer",
            };
#endif
        }

        protected override void TagFunction()
        {
#if !TR_PARSEONLY
			Trionfi.Instance.messageWindow.gameObject.SetActive(false);
            Trionfi.Instance.messageWindow = Trionfi.Instance.messageWindowList[tagParam["id"].Int()];
            Trionfi.Instance.messageWindow.ClearMessage();
            Trionfi.Instance.messageWindow.gameObject.SetActive(true);
#endif
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

    public class LComponent : AbstractComponent
    {
        protected override void TagFunction()
        {
        }

        public override IEnumerator TagAsyncWait()
        {
            yield return TRUIInstance.Instance.messageWindow.Wait();
        }
    }
    */

    //クリック待ち。novelmodeの時はメッセージクリアをしない（のでcmタグを手動で入れなければならない）
    [Serializable]
    public class PComponent : AbstractComponent
    {
        protected override void TagFunction()
        {
        }

#if !TR_PARSEONLY
		public override IEnumerator TagSyncFunction()
        {
            yield return Trionfi.Instance.messageWindow.Wait();
        }
#endif
	}

    //メッセージクリア
    [Serializable]
    public class CmComponent : AbstractComponent
    {
        protected override void TagFunction()
        {
#if !TR_PARSEONLY
			Trionfi.Instance.messageWindow.ClearMessage();
#endif
		}
    }

    //フォント設定    
    //[font size=26 color=#FFFFFF80]
    [Serializable]
    public class FontComponent : AbstractComponent {
		public FontComponent() {
#if UNITY_EDITOR && TR_DEBUG
            //必須項目
            essentialParams = new List<string> { };
            //			originalParamDic = new ParamDictionary() {
            //				{"size",""},
            //				{"color",""},
            //			};
#endif
        }

		protected override void TagFunction()
		{
#if !TR_PARSEONLY
			int size = tagParam["size", TRSystemConfig.Instance.fontSize];
            uint colorValue = tagParam["color", 0xFFFFFFFF];

            Color color = TRVariableDictionary.ToRGB(colorValue);
            Trionfi.Instance.messageWindow.currentMessage.fontSize = size;
            Trionfi.Instance.messageWindow.currentMessage.color = color;
#endif
		}
	}
}
