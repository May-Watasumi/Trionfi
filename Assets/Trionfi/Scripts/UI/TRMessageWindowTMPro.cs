using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using TRTask = Cysharp.Threading.Tasks.UniTask;
using TRTaskString = Cysharp.Threading.Tasks.UniTask<string>;

namespace Trionfi
{
    public class TRMessageWindowTMPro : TRMessageWindowBase
    {
        [SerializeField]
        public　RubyTextMeshProUGUI/*RubyTextMeshPro*/ currentMessage;

        [SerializeField]
        public TMP_FontAsset fontAsset;

        [SerializeField]
        public RubyTextMeshProUGUI currentName;
        public override void Start()
        {
            if (currentMessage != null)
            {
                currentMessage.fontSize = TRSystemConfig.Instance.fontSize;
                currentMessage.color = TRSystemConfig.Instance.fontColor;
            }
            
            base.Start();
        }
        
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
            string tempString = currentMessage.text;
            int index = 0;

            void AddString()
            {
                if(message[index] == '<')
                {
                    int endIndex = message.IndexOf('>', index);
                    if(endIndex != -1)
                    {
                        tempString += message.Substring(index, endIndex - index + 1);
                        index = endIndex + 1;
                    }
                    else
                    {
                        tempString += message[index];
                        index++;
                    }
                }
                else
                {
                    tempString += message[index];
                    index++;
                }
            }
            
            if(fontAsset != null)
                currentMessage.font = fontAsset;
            
            // TRSystemConfig.Instance.defaultFont;

            currentName.font = currentMessage.font;

            float mesWait = mesCurrentWait / speedRatio;

            if (!enableSkip && !TRSystemConfig.Instance.isNovelMode)
                currentMessage.text = string.Empty;         

            if (currentName != null)
                currentName.text = nameString;
            else if (!string.IsNullOrEmpty(nameString))
            {
                currentMessage.text += nameString + "\r\n";

//                if (TRSystemConfig.Instance.isNovelMode)
//                    currentMessage.VisibleLength = currentMessage.MaxIndex-1;
            }

            // ClearMessage()を呼び出すと１フレーム間何もないのを表示するので、
            // 名前とメッセージの初期化はここで実行する                        
            else if (currentName != null) currentName.text = string.Empty;

            nameString = ""; //ClearMessageを呼ばないと直前の名前が残っているのでここで初期化

            if (TRSystemConfig.Instance.isNovelMode)
            {
                if(!string.IsNullOrEmpty(currentName.text))
                   currentMessage.text += "【" + currentName.text + "】";
                //currentMessage.text +=  message;
            }
            //else
                //currentMessage.text = message;

            AudioClip currentVoice = null;

            if (Trionfi.Instance.audioInstance[TRAudioID.VOICE1].instance.isPlaying)
                currentVoice = Trionfi.Instance.audioInstance[TRAudioID.VOICE1].instance.clip;

            if (Trionfi.Instance.messageLogwindow != null)
                Trionfi.Instance.messageLogwindow.AddLogData(currentMessage.text, nameString, currentVoice);

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
