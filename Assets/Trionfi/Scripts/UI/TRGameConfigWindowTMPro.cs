using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Trionfi
{
    public class TRGameConfigWindowTMPro : TRGameConfigWindowBase
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

		protected override string systemvoicevolumeSliderString
		{
			get { return systemvoicevolumeSliderText.text; }
			set { systemvoicevolumeSliderText.text = value; }
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
        TextMeshProUGUI mastervolumeSliderText = null;
        [SerializeField]
        TextMeshProUGUI bgmvolumeSliderText = null;
        [SerializeField]
        TextMeshProUGUI sevolumeSliderText = null;
        [SerializeField]
        TextMeshProUGUI voicevolumeSliderText = null;
		[SerializeField]
		TextMeshProUGUI systemvoicevolumeSliderText = null;
		[SerializeField]
        TextMeshProUGUI textspeedSliderText = null;
        [SerializeField]
        TextMeshProUGUI autotextwaitSliderText = null;
        [SerializeField]
        TextMeshProUGUI effectSkipToggleText = null;
        [SerializeField]
        TextMeshProUGUI readTextSkipToggleText = null;
        [SerializeField]
        TextMeshProUGUI initializeButtonText = null;
    }
}
