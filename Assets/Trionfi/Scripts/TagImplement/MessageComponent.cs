
#if !TR_PARSEONLY
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using DG.Tweening;
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
using System.Text.RegularExpressions;

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

        protected override async TRTaskString TagFunction()
        {
            hasSync = true;
#if !TR_PARSEONLY
            string storage = tagParam["storage"].Literal();

            if (!string.IsNullOrEmpty(storage))
            {
                string mes = await TRResourceLoader.LoadText(storage, GetResourceType());

                if (string.IsNullOrEmpty(mes))
                    return "リソースエラー:" + storage; 
                
                TRVirtualMachine.Instance.currentTagInstance.ReadTextData(mes);

                return string.Empty;
            }

            return "エラー：必須パラメータstorageがない";
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

        protected override async TRTaskString TagFunction()
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
            return string.Empty;
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

        protected override async TRTaskString TagFunction()
        {
#if !TR_PARSEONLY
            Trionfi.Instance.SetStandLayerTone();

            string message = string.Empty;

            if (tagParam.ContainsKey("val"))
                message = tagParam["val"].Literal();

            if (tagParam.ContainsKey("id"))
            {
                message = TRVirtualMachine.Instance.currentTagInstance.textData[tagParam["id"].Int()].GetText(TRSystemConfig.Instance.localizeID);
                message = message.Replace("\\r", "\r");
                message = message.Replace("\\n", "\n");
            }

            //既読フラグ
            if (tagParam.ContainsKey("flagid"))
            {
                int flagid = tagParam["flagid"].Int();

                //未読スキップをしない＆未読のときはスキップ解除
                if (Trionfi.Instance.currentMessageWindow.onSkip  && !TRGameConfig.configData.readtextSkip && !TRVirtualMachine.Instance.currentTagInstance.isJMessageReadFlags[flagid])
                {
                    Trionfi.Instance.currentMessageWindow.onSkip = false;
                }

                if (TRVirtualMachine.Instance.tokenSource == null || TRVirtualMachine.Instance.tokenSource.IsCancellationRequested)
                    return string.Empty;
                else
                    TRVirtualMachine.Instance.currentTagInstance.isJMessageReadFlags[flagid] = true;
            }

            Trionfi.Instance.ActivateAllCanvas(true);

            if (!Trionfi.Instance.currentMessageWindow.gameObject.activeSelf)
            {
                Trionfi.Instance.currentMessageWindow.OpenWindow();
            }
            
            Trionfi.Instance.currentMessageWindow.ShowMessage(message, TRGameConfig.configData.textspeed);


            var token = Trionfi.Instance.GetCancellationTokenOnDestroy();

			await UniTask.WaitWhile(() => (Trionfi.Instance.currentMessageWindow.state != TRMessageWindowBase.MessageState.None) && (TRVirtualMachine.Instance.state == TRVirtualMachine.State.Run), PlayerLoopTiming.Update, token);

            if (TRSystemConfig.Instance.isNovelMode)
            {
                Trionfi.Instance.currentMessageWindow.nameString = string.Empty;
                Trionfi.Instance.currentMessageWindow.currentText += "\r";
            }
            // メッセージの初期化は新しいメッセージを表示する直前にするのでここではしない\\
            //            else
            //                Trionfi.Instance.currentMessageWindow.ClearMessage();
#endif
            return string.Empty;
		}
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

        protected override async TRTaskString TagFunction()
        {
#if !TR_PARSEONLY
            string name = tagParam["val"].Literal();

            string emb2 = "#(.*)#";
            var regex2 = new Regex(emb2);

            if (name.Contains("/"))
            {
                string[] nameInfo = name.Split('/');

                //            text = /*_subText*/ regex.Replace(text, MatchEvaluatorFunc);
                nameInfo[0] = regex2.Replace(nameInfo[0], TRMessageWindowBase.MatchEvaluatorFunc);

                Trionfi.Instance.currentMessageWindow.ShowName(nameInfo[0]);
                Trionfi.Instance.currentMessageWindow.currentSpeaker = nameInfo[1];
            }
            else
            {
                name = regex2.Replace(name, TRMessageWindowBase.MatchEvaluatorFunc);

                Trionfi.Instance.currentMessageWindow.ShowName(name);
                Trionfi.Instance.currentMessageWindow.currentSpeaker = name;
            }
#endif
            return string.Empty;
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

        protected override async TRTaskString TagFunction()
        {
#if !TR_PARSEONLY
            float ratio = tagParam["ratio", 1.0f];
            Trionfi.Instance.currentMessageWindow.speedRatio = ratio;
#endif
            return string.Empty;
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

        protected override async TRTaskString TagFunction()
        {
#if !TR_PARSEONLY
            //振幅
            int strength = tagParam["strength", 5];
            //振動頻度
            int vibratio = tagParam["vibrato", 20];

            RectTransform _rect = Trionfi.Instance.currentMessageWindow.gameObject.GetComponent<RectTransform>();

            Trionfi.Instance.currentMessageWindow.tweener = _rect.GetComponent<RectTransform>().DOShakePosition(1.0f, strength, vibratio, 90.0f, false, false).SetLoops(-1);
#endif
            return string.Empty;
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

        protected override async TRTaskString TagFunction()
        {
#if !TR_PARSEONLY
            Trionfi.Instance.currentMessageWindow.gameObject.SetActive(false);
            Trionfi.Instance.currentMessageWindow = Trionfi.Instance.messageWindowList[tagParam["id"].Int()];
            Trionfi.Instance.currentMessageWindow.ClearMessage();
            Trionfi.Instance.currentMessageWindow.gameObject.SetActive(true);
#endif
            return string.Empty;
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
            yield return TRUIInstance.Instance.currentMessageWindow.Wait();
        }
    }
    */

    //クリック待ち。novelmodeの時はメッセージクリアをしない（のでcmタグを手動で入れなければならない）
    [Serializable]
    public class PComponent : AbstractComponent
    {

        protected override async TRTaskString TagFunction()
        {
#if !TR_PARSEONLY
            await Trionfi.Instance.currentMessageWindow.Wait();
#endif
            return string.Empty;
        }
    }

    //メッセージクリア
    [Serializable]
    public class CmComponent : AbstractComponent
    {
        protected override async TRTaskString TagFunction()
        {
#if !TR_PARSEONLY
            Trionfi.Instance.currentMessageWindow.ClearMessage();
#endif
            return string.Empty;
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

        protected override async TRTaskString TagFunction()
        {
#if !TR_PARSEONLY
            int size = tagParam["size", 32];
            uint colorValue = tagParam["color", 0xFFFFFFFF];
            Color color = TRVariableDictionary.ToRGB(colorValue);

            Trionfi.Instance.currentMessageWindow.fontSize = size;
            Trionfi.Instance.currentMessageWindow.fontDefaultColor = color;
#endif
            return string.Empty;
		}
	}
}
