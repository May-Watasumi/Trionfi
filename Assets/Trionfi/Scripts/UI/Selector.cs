using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Trionfi
{
    public class Selector : MonoBehaviour {
        public static TRSelectWindow currentWindow;
        public static int result = -1;

        public int answer;

        public void OnSelected()
        {
            result = answer;
            currentWindow.state = TRSelectWindow.SelectState.None;
        }

        public Text ContentText;
        public Sprite ContentIcon;

        public void Set(string str, Sprite sprite)
        {
            if(ContentText != null)
                ContentText.text = str;

            if(ContentIcon != null)
                ContentIcon = sprite;
        }
/*
        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {
        }
*/
    }
}
