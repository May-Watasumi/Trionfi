using UnityEngine;
using UnityEngine.UI;

namespace Trionfi
{
    public class TRSystemMenuWindowLegacy : TRSystemMenuWindowBase
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
    }
}
