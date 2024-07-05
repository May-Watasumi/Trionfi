using UnityEngine;
using UnityEngine.UI;
using LetterWriter.Unity.Components;
public class TRMessageLogDataLegacy : TRMessageLogDataBase
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
    public LetterWriterText sentenceText;
    [SerializeField]
    public Text nameText;
}
