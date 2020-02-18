using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Trionfi
{
    public class TRSelectButton : MonoBehaviour {
        //ジャンプ先ラベル@KAG仕様
        public string targetLabel; 

        [SerializeField]
        public AudioSource decisionSound;

        public void OnSelected()
        {
            TRSelectWindow.Instance.result = targetLabel;

            AudioSource audio = null;
            if (decisionSound != null && decisionSound.clip != null)
                audio = decisionSound;
            else if (TRSelectWindow.Instance.decisionSound.clip != null)
                audio = TRSelectWindow.Instance.decisionSound;

            StartCoroutine(EndSelector(audio));
        }

        IEnumerator EndSelector(AudioSource audio)
        {
            float volume = TRGameConfig.configData.mastervolume * TRGameConfig.configData.sevolume;

            if (audio != null)
            {
                audio.volume = volume;
                audio.Play();
                yield return new WaitWhile(() => audio.isPlaying);
            }

            yield return new WaitForEndOfFrame();

            Trionfi.Instance.PopWindow();

            TRSelectWindow.Instance.onWait = false;
        }

        public Text contentText;

        public void Set(string content, string result)
        {
            if(contentText != null)
                contentText.text = content;

            targetLabel = result;
        }
    }
}
