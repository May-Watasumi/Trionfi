using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;
using System;
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

    [NonSerialized]
    public TRDialogResult result = TRDialogResult.NONE;
    public virtual void OnClickYes() { result = TRDialogResult.YES; }
    public virtual void OnClickNo() { result = TRDialogResult.NO; }

    public abstract UniTask Open(string text, TRDialogType type);

    public async UniTask Wait()
    {
        await UniTask.WaitWhile(() => result == TRDialogResult.NONE);
		gameObject.SetActive(false);
	}
}
