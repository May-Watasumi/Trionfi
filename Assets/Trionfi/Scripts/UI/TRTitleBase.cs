using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Trionfi
{
    public abstract class TRTitleBase : SingletonMonoBehaviour<TRTitleBase>
    {
        protected abstract string buttonString{ get; set; }
        protected abstract string logoString{ get; set; }
        
        [SerializeField]
        protected string scriptName = null;
        [SerializeField]
        Image fader = null;

        public abstract void Initialize();
        
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
