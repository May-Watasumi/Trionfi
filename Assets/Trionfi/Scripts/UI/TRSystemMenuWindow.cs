using UnityEngine;
using UnityEngine.UI;

namespace Trionfi
{
    public class TRSystemMenuWindow : SingletonMonoBehaviour<TRSystemMenuWindow>
    {
        [SerializeField]
        Text closeButtonText = null;
        [SerializeField]
        Text saveButtonText = null;
        [SerializeField]
        Text loadButtonText = null;
        [SerializeField]
        Text skipButtonText = null;
        [SerializeField]
        Text autoButtonText = null;
        [SerializeField]
        Text logButtonText = null;
        [SerializeField]
        Text configButtonText = null;

        // Use this for initialization
        void Start()
        {
            if (closeButtonText != null)
                closeButtonText.text = TRStageEnviroment.instance.GetUIText(TR_UITEXTID.MESSAGEWINDOW_CLOSE);
            if (saveButtonText != null)
                saveButtonText.text = TRStageEnviroment.instance.GetUIText(TR_UITEXTID.MESSAGEWINDOW_SAVE);
            if (loadButtonText != null)
                loadButtonText.text = TRStageEnviroment.instance.GetUIText(TR_UITEXTID.MESSAGEWINDOW_LOAD);
            if (skipButtonText != null)
                skipButtonText.text = TRStageEnviroment.instance.GetUIText(TR_UITEXTID.MESSAGEWINDOW_SKIP);
            if (autoButtonText != null)
                autoButtonText.text = TRStageEnviroment.instance.GetUIText(TR_UITEXTID.MESSAGEWINDOW_AUTO);
            if (logButtonText != null)
                logButtonText.text = TRStageEnviroment.instance.GetUIText(TR_UITEXTID.MESSAGEWINDOW_LOG);
            if (configButtonText != null)
                configButtonText.text = TRStageEnviroment.instance.GetUIText(TR_UITEXTID.MESSAGEWINDOW_CONFIG);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnSaveButton()
        {
            Trionfi.Instance.messageWindow.Pause();
            Trionfi.Instance.messageWindow.gameObject.SetActive(false);
            Trionfi.instance.serializer.gameObject.SetActive(true);
            TRSerializeManager.instance.Begin(TRSerializeManager.Mode.Save);
            gameObject.SetActive(false);
        }

        public void OnLoadButton()
        {
            Trionfi.Instance.messageWindow.Pause();
            Trionfi.Instance.messageWindow.gameObject.SetActive(false);
            Trionfi.instance.serializer.gameObject.SetActive(true);
            TRSerializeManager.instance.Begin(TRSerializeManager.Mode.Load);
            gameObject.SetActive(false);
        }

        public void OnAutoButton()
        {
            Trionfi.Instance.messageWindow.onAuto = !Trionfi.Instance.messageWindow.onAuto;
        }

        public void OnSkipButton()
        {
            Trionfi.Instance.messageWindow.onSkip = !Trionfi.Instance.messageWindow.onSkip;

            // onSkipがtrueの場合、もしプレイ中のボイスがあれば停止する
            if (Trionfi.Instance.messageWindow.onSkip && Trionfi.Instance.audioInstance[TRAudioID.VOICE1].instance.isPlaying)
                Trionfi.Instance.audioInstance[TRAudioID.VOICE1].instance.Stop();
        }

        public void OnMessageLogButton()
        {
            Trionfi.Instance.messageWindow.Pause();
            Trionfi.Instance.messageWindow.gameObject.SetActive(false);
            Trionfi.Instance.messageLogwindow.gameObject.SetActive(true);
            Trionfi.Instance.globalTap.SetActive(false);
            gameObject.SetActive(false);
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
