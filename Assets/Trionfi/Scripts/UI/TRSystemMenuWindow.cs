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

        public void OnSaveButton()
        {
            Trionfi.Instance.Save("SaveData");
        }

        public void OnLoadButton()
        {
            Trionfi.Instance.Load("SaveData");
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
            Trionfi.Instance.messageWindow.Pause();
            Trionfi.Instance.HideObject(Trionfi.Instance.messageWindow.gameObject);
            Trionfi.Instance.HideObject(gameObject);

            Trionfi.Instance.OpenUI(Trionfi.Instance.messageLogwindow.gameObject);
        }

        public void OnOptionButton()
        {
            Trionfi.Instance.messageWindow.Pause();
            Trionfi.Instance.HideObject(Trionfi.Instance.messageWindow.gameObject);
            Trionfi.Instance.HideObject(gameObject);

            Trionfi.Instance.OpenUI(Trionfi.Instance.configWindow.gameObject);
        }

        public void OnWindowCloseButton()
        {
            Trionfi.Instance.CloseAllUI();
        }
    }
}
