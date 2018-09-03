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
        Text buttonText;
        [SerializeField]
        string scriptName;

        // Use this for initialization
        void Start()
        {
            Trionfi.Instance.ClickEvent += TapEvent;

            DOTween.ToAlpha(
            () => buttonText.color,
            color => buttonText.color = color,
            0.0f,                                // 最終的なalpha値
            1.0f
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
            Trionfi.Instance.Begin(scriptName);
        }
    }
}
