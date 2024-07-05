using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace Trionfi
{
    public class TRSystemMenuWindowTMPro : TRSystemMenuWindowBase
    {
        [SerializeField]
        TextMeshProUGUI closeButtonText = null;
        [SerializeField]
        TextMeshProUGUI saveButtonText = null;
        [SerializeField]
        TextMeshProUGUI loadButtonText = null;
        [SerializeField]
        TextMeshProUGUI skipButtonText = null;
        [SerializeField]
        TextMeshProUGUI autoButtonText = null;
        [SerializeField]
        TextMeshProUGUI logButtonText = null;
        [SerializeField]
        TextMeshProUGUI configButtonText = null;
        
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
