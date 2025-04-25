using System.Text.RegularExpressions;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using TRTask = Cysharp.Threading.Tasks.UniTask;
using TRTaskString = Cysharp.Threading.Tasks.UniTask<string>;

namespace Trionfi
{
    public class TRMessageWindowLegacy : TRMessageWindowBase
    {
        public override string currentText { get { return currentMessage.text; } set { currentMessage.text = value; } }

        [SerializeField]
        public Font defaultFont = null;

        [SerializeField]
        public bool useUguiText = false;

        [SerializeField]
        public LetterWriter.Unity.Components.LetterWriterText currentMessage;

        [SerializeField]
        public Text currentUguiMessage;

        [SerializeField]
        public Text currentName;

        protected override void Start()
        {
            defaultFont = defaultFont ?? Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            base.Start();
        }

        override public void ClearMessage()
        {
            if (currentUguiMessage != null)
                currentUguiMessage.text = string.Empty;
            if (currentMessage != null)
                currentMessage.text = string.Empty;
            if (currentName != null)
                currentName.text = string.Empty;
            base.ClearMessage();
        }

        override protected async TRTask ShowMessageSub(string message, float mesCurrentWait)
        {
            //currentMessage.Font;
            currentMessage.fontSize = fontSize;

            float mesWait = mesCurrentWait / speedRatio;

            if (!enableSkip && !TRSystemConfig.Instance.isNovelMode)
                currentMessage.VisibleLength = 0;

            if (currentName != null)
                currentName.text = nameString;
            else if (!string.IsNullOrEmpty(nameString))
            {
                currentMessage.text += nameString + "\r\n";

                if (TRSystemConfig.Instance.isNovelMode)
                    currentMessage.VisibleLength = currentMessage.MaxIndex - 1;
            }

            // ClearMessage()を呼び出すと１フレーム間何もないのを表示するので、
            // 名前とメッセージの初期化はここで実行する                        
            else
                currentName.text = string.Empty;

            nameString = ""; //ClearMessageを呼ばないと直前の名前が残っているのでここで初期化

            if (TRSystemConfig.Instance.isNovelMode)
            {
                if (!string.IsNullOrEmpty(currentName.text))
                    currentMessage.text += "【" + currentName.text + "】";
                currentMessage.text += message;

            }
            else
                currentMessage.text = message;

            AudioClip currentVoice = null;

            if (Trionfi.Instance.audioInstance[TRAudioID.VOICE1].instance.isPlaying)
                currentVoice = Trionfi.Instance.audioInstance[TRAudioID.VOICE1].instance.clip;

            if (Trionfi.Instance.messageLogwindow != null)
                Trionfi.Instance.messageLogwindow.AddLogData(currentMessage.text, currentName.text, currentVoice);

            if (!enableSkip && mesWait > 0.0f)
            {
                //                int currentMessagePos = TRSystemConfig.Instance.isNovelMode ? currentMessage.text.Length - message.Length : 0;

                for (int i = currentMessage.VisibleLength; i < currentMessage.MaxIndex; i++)
                {
                    while (isSuspend)
                    {
                        await UniTask.DelayFrame(1);
                    }

                    //リロード等
                    if (TRVirtualMachine.Instance.state != TRVirtualMachine.State.Run)
                        return;

                    if (state == MessageState.OnShow && !enableSkip)
                        currentMessage.VisibleLength++;
                    else
                        break;

                    await UniTask.Delay((int)(mesWait * 1000.0f));
                    //                    yield return new WaitForSeconds(mesWait);
                }
            }
            else
                state = MessageState.None;

            currentMessage.VisibleLength = TRSystemConfig.Instance.isNovelMode ? currentMessage.text.Length : -1;

            await Wait();
        }
    }
}
