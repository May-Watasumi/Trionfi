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

namespace Trionfi
{
    [Serializable]
    public class TRSerializerWindowLegacy : TRSerializerWindowBase
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
        
        [SerializeField]
        Text pageText = null;// = new Text[dataCount];
        [SerializeField]
        Text modeText = null;// = new Text[dataCount];
        [SerializeField]
        Text[] dateText = null;// = new Text[dataCount];
        [SerializeField]
        Text[] infoText = null;// = new Text[dataCount];
    }
}
