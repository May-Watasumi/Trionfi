using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Trionfi
{
    public class TRTitle : SingletonMonoBehaviour<TRTitle>
    {
        [SerializeField]
        Text logoText;
        [SerializeField]
        Text buttonText;
        [SerializeField]
        string scriptName;
        [SerializeField]
        Image fader;

        // Use this for initialization
        void Start()
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

        // Update is called once per frame
        void Update()
        {

        }

        public void TapEvent()
        {
            Trionfi.Instance.ClickEvent -= TapEvent;

            DOTween.ToAlpha(
            () => fader.color,
            color => fader.color = color,
            1.0f,                                // 最終的なalpha値
            1.0f
            )
            .OnComplete
             (() =>
                {
                    Trionfi.Instance.Begin(scriptName);
                });
        }
    }
}
