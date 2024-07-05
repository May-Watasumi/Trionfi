using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Trionfi
{
    public class TRTitleLegacy : TRTitleBase
    {
        protected override string buttonString
        {
            get { return buttonText.text; }
            set { buttonText.text = value; }
        }

        protected override string logoString {
            get { return logoText.text; }
            set { logoText.text = value; }
        }

        [SerializeField]
        Text logoText = null;
        [SerializeField]
        Text buttonText = null;

		public override void Initialize()
        {
            buttonText.text = TRStageEnviroment.instance.GetUIText(TR_UITEXTID.TITLE_TAPSCREEN);
            logoText.text = TRStageEnviroment.instance.GetUIText(TR_UITEXTID.TITLE_TEXT);

            Trionfi.Instance.ClickEvent += TapEvent;

            DOTween.ToAlpha(
            () => buttonText.color,
            color => buttonText.color = color,
            0.0f,                               // 最終的なalpha値
            1.0f                                // time
            )
            .SetLoops(-1, LoopType.Yoyo);
        }
    }
}
