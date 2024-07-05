using UnityEngine;
using UnityEngine.UI;

public class TRCustomDialogLegacy : ICustomDialog {
    [SerializeField]
    Text notice = null;
    [SerializeField]
    Button buttonYes = null;
    [SerializeField]
    Button buttonNo = null;
    [SerializeField]
    Button buttonOK = null;

    public override void Init(string text, TRDialogType type)
    {
        bool typeok = TRDialogType.OK == type ? true : false;

        notice.text = text;
        buttonYes.gameObject.SetActive(!typeok);
        buttonNo.gameObject.SetActive(!typeok);
        buttonOK.gameObject.SetActive(typeok);
        //        yield return Wait();
    }
}
