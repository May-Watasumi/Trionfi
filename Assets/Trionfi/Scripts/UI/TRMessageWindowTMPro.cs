using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using TRTask = Cysharp.Threading.Tasks.UniTask;
using TRTaskString = Cysharp.Threading.Tasks.UniTask<string>;
using System.Text.RegularExpressions;

namespace Trionfi
{
    public class TRMessageWindowTMPro : TRMessageWindowBase
    {
        public override string currentText
        {
            get { return currentMessage  .text; } set{ currentMessage.text = value;  }
        }

        [SerializeField]
        public　RubyTextMeshProUGUI currentMessage;

        [SerializeField]
        public TMP_FontAsset fontAsset;

        [SerializeField]
        public RubyTextMeshProUGUI currentName;
        
        public override void ClearMessage()
        {
            if (currentName != null)
                currentName.text = string.Empty;

            if (currentMessage != null)
                currentMessage.text = string.Empty;

            base.ClearMessage();
        }
        
        protected override async TRTask ShowMessageSub(string message, float mesCurrentWait)
        {
            int index = 0;
            string tempString = string.Empty;

            void AddString()
            {
                if(message[index] == '<')
                {
                    int endIndex = message.IndexOf('>', index);
                    if(endIndex != -1)
                    {
						currentMessage.uneditedText += message.Substring(index, endIndex - index + 1);
                        index = endIndex + 1;
                    }
                    else
                    {
						currentMessage.uneditedText += message[index++];
                    }
                }
                else
                {
					currentMessage.uneditedText += message[index++];
                }

            }
            
            currentMessage.fontSize = fontSize;

			if (fontAsset != null)
                currentMessage.font = fontAsset;
            
            // TRSystemConfig.Instance.defaultFont;

            currentName.font = currentMessage.font;

            //Actor情報で色変え
            Color32 color32 = fontDefaultColor;

            if (string.IsNullOrEmpty(nameString))
            {
                color32 = fontDefaultColor;
            }
            else if (TRStageEnviroment.Instance.actorInfoes.ContainsKey(nameString))
            {
                Color color;

                if (ColorUtility.TryParseHtmlString("#" + TRStageEnviroment.Instance.actorInfoes[nameString].textColor, out color))
                {
                    color32 = TRUtility.ColorExtensions.FromHex(TRStageEnviroment.Instance.actorInfoes[nameString].textColor);
                }

            }
            else
            {
                color32 = fontMobColor;
            }

			currentMessage.faceColor = color32;
            currentName.faceColor = color32;

			//変数は##でくくる
			//string emb2 = "#([a-zA-z0-9-_]+)#";
			//var regex2 = new Regex(emb2);

			nameString = TRUtility.GetVariableString(nameString);//  regex2.Replace(text, TRUtility.MatchEvaluatorFunc);

			//
			float mesWait = mesCurrentWait / speedRatio;

            if (!enableSkip && !TRSystemConfig.Instance.isNovelMode)
				currentMessage.uneditedText = string.Empty;         

            if (currentName != null)
                currentName.uneditedText = nameString;
            else if (!string.IsNullOrEmpty(nameString))
            {
				currentMessage.uneditedText += nameString + "\r\n";

//                if (TRSystemConfig.Instance.isNovelMode)
//                    currentMessage.VisibleLength = currentMessage.MaxIndex-1;
            }

            // ClearMessage()を呼び出すと１フレーム間何もないのを表示するので、
            // 名前とメッセージの初期化はここで実行する                        
            else if (currentName != null) currentName.uneditedText = string.Empty;

            nameString = ""; //ClearMessageを呼ばないと直前の名前が残っているのでここで初期化

            if (TRSystemConfig.Instance.isNovelMode)
            {
                if(!string.IsNullOrEmpty(currentName.uneditedText))
					currentMessage.uneditedText += "【" + currentName.uneditedText + "】";
                //currentMessage.text +=  message;
            }
            //else
                //currentMessage.text = message;

            AudioClip currentVoice = null;

            if (Trionfi.Instance.audioInstance[TRAudioID.VOICE1].instance.isPlaying)
                currentVoice = Trionfi.Instance.audioInstance[TRAudioID.VOICE1].instance.clip;

            if (Trionfi.Instance.messageLogwindow != null)
                Trionfi.Instance.messageLogwindow.AddLogData(message, currentName.uneditedText, currentVoice);

            if (!enableSkip && mesWait > 0.0f)
            {
                //                int currentMessagePos = TRSystemConfig.Instance.isNovelMode ? currentMessage.text.Length - message.Length : 0;
                while(index < message.Length)
//                for (int i = currentMessage.VisibleLength; i < currentMessage.MaxIndex; i++)
                {
                    while (isSuspend)
                    {
                        await UniTask.DelayFrame(1);
                    }

                    //リロード等
                    if (TRVirtualMachine.Instance.state != TRVirtualMachine.State.Run)
                        return;

                    if (state == MessageState.OnShow && !enableSkip)
                        AddString();//currentMessage.VisibleLength++;
                    else
                        break;

                    await UniTask.Delay((int)(mesWait * 1000.0f));
//                    yield return new WaitForSeconds(mesWait);
                }
            }
            else
              state = MessageState.None;
            
            while(index < message.Length)
                AddString();
            //currentMessage.VisibleLength = TRSystemConfig.Instance.isNovelMode ? currentMessage.text.Length : -1;

            await Wait();
        }
    }
}
