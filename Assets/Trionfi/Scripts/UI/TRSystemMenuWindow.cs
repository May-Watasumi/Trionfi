using UnityEngine;
using UnityEngine.UI;

namespace Trionfi
{
    public class TRSystemMenuWindow : SingletonMonoBehaviour<TRSystemMenuWindow>
    {
        [SerializeField]
        Text closeButtonText;
        [SerializeField]
        Text saveButtonText;
        [SerializeField]
        Text loadButtonText;
        [SerializeField]
        Text skipButtonText;
        [SerializeField]
        Text autoButtonText;
        [SerializeField]
        Text logButtonText;
        [SerializeField]
        Text configButtonText;

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
            Trionfi.Instance.SerializeToFile(0);
        }

        public void OnLoadButton()
        {
            Trionfi.Instance.BeginDeserialize(0);// .DeserializeFromFile(0);
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
