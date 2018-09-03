using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trionfi
{
    public class TRSystemMenuWindow : SingletonMonoBehaviour<TRSystemMenuWindow>
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
            Trionfi.Instance.messageWindow.onAuto = true;
        }

        public void OnSkipButton()
        {
            Trionfi.Instance.messageWindow.onSkip = true;
        }

        public void OnMessageLogButton()
        {
            Trionfi.Instance.messageWindow.gameObject.SetActive(false);
            Trionfi.Instance.messageLogwindow.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }

        public void OnWindowCloseButton()
        {
            Trionfi.Instance.messageWindow.gameObject.SetActive(false);
            Trionfi.Instance.messageLogwindow.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
