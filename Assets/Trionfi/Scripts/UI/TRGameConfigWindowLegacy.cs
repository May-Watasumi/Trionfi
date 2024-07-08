using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Trionfi
{
    public class TRGameConfigWindowLegacy : TRGameConfigWindowBase
    {
        protected override string mastervolumeSliderString
        {
            get { return mastervolumeSliderText.text; }
            set { mastervolumeSliderText.text = value; }
        }

        protected override string bgmvolumeSliderString
        {
            get { return bgmvolumeSliderText.text;}
            set { bgmvolumeSliderText.text = value; }
        }
        protected override string sevolumeSliderString
        {
            get { return sevolumeSliderText.text; }
            set { sevolumeSliderText.text = value; }
        }
        protected override string voicevolumeSliderString
        {
            get { return voicevolumeSliderText.text;}
            set { voicevolumeSliderText.text = value; }
        }

        protected override string textspeedSliderString
        {
            get { return textspeedSliderText.text; }
            set { textspeedSliderText.text = value; }
        }

        protected override string autotextwaitSliderString
        {
            get { return autotextwaitSliderText.text; }
            set { autotextwaitSliderText.text = value; }
        }

        protected override string effectSkipToggleString
        {
            get { return effectSkipToggleText.text; }
            set { effectSkipToggleText.text = value; }
        }

        protected override string readTextSkipToggleString
        {
            get { return readTextSkipToggleText.text; }
            set { readTextSkipToggleText.text = value; }
        }

        protected override string initializeButtonString
        {
            get { return initializeButtonText.text; }
            set { initializeButtonText.text = value; }
        }

        [SerializeField]
        Text mastervolumeSliderText = null;
        [SerializeField]
        Text bgmvolumeSliderText = null;
        [SerializeField]
        Text sevolumeSliderText = null;
        [SerializeField]
        Text voicevolumeSliderText = null;
        [SerializeField]
        Text textspeedSliderText = null;
        [SerializeField]
        Text autotextwaitSliderText = null;
        [SerializeField]
        Text effectSkipToggleText = null;
        [SerializeField]
        Text readTextSkipToggleText = null;
        [SerializeField]
        Text initializeButtonText = null;
    }
}
