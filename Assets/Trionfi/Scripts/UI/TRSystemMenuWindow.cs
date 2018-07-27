using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trionfi
{
    public class TRSystemMenuWindow : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnAutoButton()
        {
            TRUIInstance.Instance.messageWindow.onAuto = true;
        }

        public void OnSkipButton()
        {
            TRUIInstance.Instance.messageWindow.onSkip = true;
        }

        public void OnMessageLogButton()
        {
            TRUIInstance.Instance.messageWindow.gameObject.SetActive(false);
            TRUIInstance.Instance.logWindow.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }

        public void OnWindowCloseButton()
        {
            TRUIInstance.Instance.messageWindow.gameObject.SetActive(false);
            TRUIInstance.Instance.logWindow.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
