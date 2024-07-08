using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TRTask = Cysharp.Threading.Tasks.UniTask;
using TRTaskString = Cysharp.Threading.Tasks.UniTask<string>;
using TMPro;
namespace Trionfi
{
    [Serializable]
    public class TRSerializerWindowTMPro : TRSerializerWindowBase
    {
        public override string pageString
        {
            get { return pageText.text; }
            set { pageText.text = value; }
        }
        public override string modeString
        {
            get { return modeText.text;}
            set { modeText.text = value; }
        }

        protected override void SetDateText(int a, string text)
        {
            dateText[a].text = text;
        }

        protected override void SetInfoText(int a, string text)
        {
            infoText[a].text = text;
        }
    [SerializeField] TextMeshProUGUI pageText = null; // = new Text[dataCount];
    [SerializeField] TextMeshProUGUI modeText = null; // = new Text[dataCount];
    [SerializeField] TextMeshProUGUI[] dateText = null; // = new Text[dataCount];
    [SerializeField] TextMeshProUGUI[] infoText = null; // = new Text[dataCount];
    }
}
