using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

public class TRCustomDialogTMPro : ICustomDialog {
    [SerializeField]
    TextMeshProUGUI notice = null;
    [SerializeField]
    Button buttonYes = null;
    [SerializeField]
    Button buttonNo = null;
    [SerializeField]
    Button buttonOK = null;

    public override async UniTask Open(string text, TRDialogType type)
    {
        bool typeok = TRDialogType.OK == type ? true : false;

        notice.text = text;
        buttonYes.gameObject.SetActive(!typeok);
        buttonNo.gameObject.SetActive(!typeok);
        buttonOK.gameObject.SetActive(typeok);
        await  Wait();
	}
}
