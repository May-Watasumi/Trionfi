using UnityEngine;
using TMPro;

namespace Trionfi
{
    public class TRSelectButtonTMPro : TRSelectButtonBase {
        [SerializeField]
        public TextMeshProUGUI contentText;

        protected override void SetText(string text)
        {
            contentText.text = text;
        }
    }
}
