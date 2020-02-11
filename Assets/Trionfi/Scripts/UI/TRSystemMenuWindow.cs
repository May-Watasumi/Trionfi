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
            Trionfi.Instance.messageWindow.onAuto = !Trionfi.Instance.messageWindow.onAuto;
        }

        public void OnSkipButton()
        {
            Trionfi.Instance.messageWindow.onSkip = !Trionfi.Instance.messageWindow.onSkip;
        }

        public void OnMessageLogButton()
        {
            Trionfi.Instance.messageWindow.gameObject.SetActive(false);
            Trionfi.Instance.messageLogwindow.gameObject.SetActive(true);
            Trionfi.Instance.ClickEvent += Trionfi.Instance.messageLogwindow.OnClick;
            gameObject.SetActive(false);
        }

        public void OnOptionButton()
        {
            Trionfi.Instance.messageWindow.gameObject.SetActive(false);
            Trionfi.Instance.configWindow.gameObject.SetActive(true);
            Trionfi.Instance.ClickEvent += Trionfi.Instance.messageLogwindow.OnClick;
            gameObject.SetActive(false);
        }

        public void OnWindowCloseButton()
        {
            Trionfi.Instance.CloseWindow();
        }
    }
}
