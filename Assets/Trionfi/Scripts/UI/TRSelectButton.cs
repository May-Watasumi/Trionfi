using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Trionfi
{
    public class TRSelectButton : MonoBehaviour {
        public string targetLabel;           //ジャンプ先ラベル@KAG仕様

        [SerializeField]
        public AudioSource decisionSound;

        public void OnSelected()
        {
            TRSelectWindow.Instance.result = targetLabel;

            if (decisionSound != null && decisionSound.clip != null)
                decisionSound.Play();
            else if(TRSelectWindow.Instance.decisionSound.clip != null)
                TRSelectWindow.Instance.decisionSound.Play();

            Trionfi.Instance.PopWindow();

            TRSelectWindow.Instance.gameObject.SetActive(false);

            TRSelectWindow.Instance.onWait = false;
        }

        public Text contentText;

        public void Set(string content, string result)
        {
            if(contentText != null)
                contentText.text = content;

            targetLabel = result;
        }
/*
        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {
        }
*/
    }
}
