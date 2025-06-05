using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TRTask = Cysharp.Threading.Tasks.UniTask;

namespace Trionfi
{
    abstract public class TRSelectButtonBase : MonoBehaviour {
        //ジャンプ先ラベル@KAG仕様
        public string targetLabel;
        public int resultNum;

        public TRSelectWindow parent;

        [SerializeField]
        public AudioSource decisionSound;

        public void OnSelected()
        {
            parent.result = targetLabel;
            parent.resutNum = resultNum;

            AudioSource audio = null;
            if (decisionSound != null && decisionSound.clip != null)
                audio = decisionSound;
            else if (parent.decisionSound != null && parent.decisionSound.clip != null)
                audio = parent.decisionSound;

            EndSelector(audio).Forget();
        }

        async TRTask EndSelector(AudioSource audio)
        {
            float volume = TRGameConfig.configData.mastervolume * TRGameConfig.configData.sevolume;

            if (audio != null)
            {
                audio.volume = volume;
                audio.Play();
                await UniTask.WaitWhile(() => audio.isPlaying);
            }

            await UniTask.WaitForFixedUpdate();

            Trionfi.Instance.PopWindow();

            parent.onWait = false;
        }
        
        abstract protected void SetText(string text);

        public void Set(string content, string label, int result, TRSelectWindow _parent)
        {
            parent = _parent;

            SetText(content);

//            if(contentText != null)
//                contentText.text = content;

            targetLabel = label;

            resultNum = result;
        }
    }
}
