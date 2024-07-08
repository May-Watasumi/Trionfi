using UnityEngine;
using UnityEngine.UI;

namespace Trionfi
{
    public class TRSelectButtonLegacy : TRSelectButtonBase {
        [SerializeField]
        public Text contentText;

        protected override void SetText(string text)
        {
            contentText.text = text;
        }
    }
}
