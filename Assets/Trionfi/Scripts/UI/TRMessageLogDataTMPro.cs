using UnityEngine;
using TMPro;
public class TRMessageLogDataTMPro : TRMessageLogDataBase
{
    protected override string nameString
    {
        get { return nameText.uneditedText;  }
        set { nameText.uneditedText = value; }
    }

    protected override string logString
    {
        get { return sentenceText.uneditedText; }
        set { sentenceText.uneditedText = value; }
    }

    [SerializeField]
    public RubyTextMeshProUGUI sentenceText;
    [SerializeField]
    public RubyTextMeshProUGUI nameText;
}
