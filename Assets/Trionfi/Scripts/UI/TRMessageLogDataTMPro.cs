using UnityEngine;
using TMPro;
public class TRMessageLogDataTMPro : TRMessageLogDataBase
{
    protected override string nameString
    {
        get { return nameText.text;  }
        set { nameText.text = value; }
    }

    protected override string logString
    {
        get { return sentenceText.text; }
        set { sentenceText.text = value; }
    }

    [SerializeField]
    public RubyTextMeshProUGUI sentenceText;
    [SerializeField]
    public RubyTextMeshProUGUI nameText;
}
