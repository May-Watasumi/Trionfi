using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;
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

    public abstract void Init(string text, TRDialogType type);

    public IEnumerator Wait()
    {
        yield return new WaitWhile(() => result == TRDialogResult.NONE);
    }
}
