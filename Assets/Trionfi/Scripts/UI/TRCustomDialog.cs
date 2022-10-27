using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public enum TRDialogType
{
    OK,
    YESNO,
}

public enum TRDialogResult
{
    NONE = 0,
    OK = 1,
    YES = 1,
    NO = 2,
}

public abstract class ICustomDialog : MonoBehaviour
{
    delegate void DialogCallBack();

    public TRDialogResult result = TRDialogResult.NONE;
    public virtual void OnClickYes() { result = TRDialogResult.YES; }
    public virtual void OnClickNo() { result = TRDialogResult.NO; }

    //    void Start() { }
    //    void Update() { }
    public abstract void Init(string text, TRDialogType type);

    public IEnumerator Wait()
    {
        yield return new WaitWhile(() => result == TRDialogResult.NONE);
    }
}

public class TRCustomDialog : ICustomDialog {
    [SerializeField]
    Text notice = null;
    [SerializeField]
    Button buttonYes = null;
    [SerializeField]
    Button buttonNo = null;

    public override void Init(string text, TRDialogType type)
    {
        notice.text = text;
        buttonYes.gameObject.SetActive(true);
        buttonNo.gameObject.SetActive(true);
//        yield return Wait();
	}
}
