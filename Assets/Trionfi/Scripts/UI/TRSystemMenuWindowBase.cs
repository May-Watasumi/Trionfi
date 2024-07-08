using UnityEngine;
using UnityEngine.UI;

namespace Trionfi
{
    public class TRSystemMenuWindowBase : SingletonMonoBehaviour<TRSystemMenuWindowBase>
    {
        public void OnSaveButton()
        {
            Trionfi.Instance.currentMessageWindow.Pause();
            Trionfi.Instance.currentMessageWindow.gameObject.SetActive(false);
            Trionfi.Instance.serializer.gameObject.SetActive(true);
            Trionfi.Instance.serializer.Begin(TRSerializerWindowBase.Mode.Save);
            gameObject.SetActive(false);
        }

        public void OnLoadButton()
        {
            Trionfi.Instance.currentMessageWindow.Pause();
            Trionfi.Instance.currentMessageWindow.gameObject.SetActive(false);
            Trionfi.Instance.serializer.gameObject.SetActive(true);
            Trionfi.Instance.serializer.Begin(TRSerializerWindowBase.Mode.Load);
            gameObject.SetActive(false);
        }

        public void OnAutoButton()
        {
            Trionfi.Instance.currentMessageWindow.onAuto = !Trionfi.Instance.currentMessageWindow.onAuto;
        }

        public void OnSkipButton()
        {
            Trionfi.Instance.currentMessageWindow.onSkip = !Trionfi.Instance.currentMessageWindow.onSkip;

            // onSkipがtrueの場合、もしプレイ中のボイスがあれば停止する
            if (Trionfi.Instance.currentMessageWindow.onSkip && Trionfi.Instance.audioInstance[TRAudioID.VOICE1].instance.isPlaying)
                Trionfi.Instance.audioInstance[TRAudioID.VOICE1].instance.Stop();
        }

        public void OnMessageLogButton()
        {
            Trionfi.Instance.currentMessageWindow.Pause();
            Trionfi.Instance.currentMessageWindow.gameObject.SetActive(false);
            Trionfi.Instance.messageLogwindow.gameObject.SetActive(true);
            Trionfi.Instance.globalTap.SetActive(false);
            gameObject.SetActive(false);
        }

        public void OnOptionButton()
        {
            Trionfi.Instance.currentMessageWindow.Pause();
            Trionfi.Instance.HideObject(Trionfi.Instance.currentMessageWindow.gameObject);
            Trionfi.Instance.HideObject(gameObject);

            Trionfi.Instance.OpenUI(Trionfi.Instance.configWindow.gameObject);
        }

        public void OnWindowCloseButton()
        {
            Trionfi.Instance.CloseAllUI();
        }
    }
}
