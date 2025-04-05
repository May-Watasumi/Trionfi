using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Trionfi
{
    abstract public class TRGameConfigWindowBase : SingletonMonoBehaviour<TRGameConfigWindowBase>
    {
        const string prefsKey = "GameConfigs";

        [SerializeField]
        Slider mastervolumeSlider = null;
        [SerializeField]
        Slider bgmvolumeSlider = null;
        [SerializeField]
        Slider sevolumeSlider = null;
        [SerializeField]
        Slider voicevolumeSlider = null;
		[SerializeField]
		Slider systemvoicevolumeSlider = null;
		[SerializeField]
        Slider textspeedSlider = null;
        [SerializeField]
        Slider autotextwaitSlider = null;
		[SerializeField]
        Toggle effectSkipToggle = null;
        [SerializeField]
        Toggle readTextSkipToggle = null;

        protected abstract string mastervolumeSliderString { get; set; }
        protected abstract string bgmvolumeSliderString { get; set; }
        protected abstract string sevolumeSliderString { get; set; }
		protected abstract string voicevolumeSliderString { get; set; }
		protected abstract string systemvoicevolumeSliderString { get; set; }
		protected abstract string textspeedSliderString { get; set; }
        protected abstract string autotextwaitSliderString { get; set; }
        protected abstract string effectSkipToggleString { get; set; }
        protected abstract string readTextSkipToggleString { get; set; }
        protected abstract string initializeButtonString { get; set; }

        public void OnChangeSlider(int type)
        {
            switch (type)
            {
                case 0:
                    TRGameConfig.configData.mastervolume = mastervolumeSlider.value;
                    Trionfi.instance.audioInstance[TRAudioID.BGM].mainVolume = TRGameConfig.configData.bgmvolume;// * TRGameConfig.configData.mastervolume;
                    Trionfi.instance.audioInstance[TRAudioID.SE1].mainVolume = TRGameConfig.configData.sevolume;// * TRGameConfig.configData.mastervolume;
                    Trionfi.instance.audioInstance[TRAudioID.VOICE1].mainVolume = TRGameConfig.configData.voicevolume;// * TRGameConfig.configData.mastervolume;
                    Trionfi.instance.audioInstance[TRAudioID.BGM].UpdateVolume();
                    Trionfi.instance.audioInstance[TRAudioID.SE1].UpdateVolume();
                    Trionfi.instance.audioInstance[TRAudioID.VOICE1].UpdateVolume();

                    break;
                case 1:
                    TRGameConfig.configData.bgmvolume = bgmvolumeSlider.value;
                    Trionfi.instance.audioInstance[TRAudioID.BGM].mainVolume = TRGameConfig.configData.bgmvolume;// * TRGameConfig.configData.mastervolume;
                    Trionfi.instance.audioInstance[TRAudioID.BGM].UpdateVolume();
                    break;
                case 2:
                    TRGameConfig.configData.sevolume = sevolumeSlider.value;
                    Trionfi.instance.audioInstance[TRAudioID.SE1].mainVolume = TRGameConfig.configData.sevolume;// * TRGameConfig.configData.mastervolume;
                    Trionfi.instance.audioInstance[TRAudioID.SE1].UpdateVolume();
                    break;
                case 3:
                    TRGameConfig.configData.voicevolume = voicevolumeSlider.value;
                    Trionfi.instance.audioInstance[TRAudioID.VOICE1].mainVolume = TRGameConfig.configData.voicevolume;// * TRGameConfig.configData.mastervolume;
                    Trionfi.instance.audioInstance[TRAudioID.VOICE1].UpdateVolume();
                    break;
                case 4:
                    TRGameConfig.configData.textspeed = textspeedSlider.value;
                    break;
                case 5:
                    TRGameConfig.configData.autotextWait = autotextwaitSlider.value;
                    break;
                case 6:
					TRGameConfig.configData.systemvoicevolume = systemvoicevolumeSlider.value;
					Trionfi.instance.systemVoice.volume = TRGameConfig.configData.systemvoicevolume * TRGameConfig.configData.mastervolume;
                    break;


			}
		}

        public void OnChangeEffectSkip()
        {
            TRGameConfig.configData.effectSkip = effectSkipToggle.isOn;
        }

        public void OnChangeReadTextSkip()
        {
            TRGameConfig.configData.readtextSkip = readTextSkipToggle.isOn;
        }

        public void UpdateAll(bool updateAudio = true)
        {
            mastervolumeSlider.value = TRGameConfig.configData.mastervolume;
            bgmvolumeSlider.value = TRGameConfig.configData.bgmvolume;
            sevolumeSlider.value = TRGameConfig.configData.sevolume;
            voicevolumeSlider.value = TRGameConfig.configData.voicevolume;
            textspeedSlider.value = TRGameConfig.configData.textspeed;
            autotextwaitSlider.value = TRGameConfig.configData.autotextWait;
            effectSkipToggle.isOn = TRGameConfig.configData.effectSkip;
            readTextSkipToggle.isOn = TRGameConfig.configData.readtextSkip;
            systemvoicevolumeSlider.value = TRGameConfig.configData.systemvoicevolume;

            if (updateAudio)
            {
                Trionfi.instance.audioInstance[TRAudioID.BGM].mainVolume = TRGameConfig.configData.bgmvolume * TRGameConfig.configData.mastervolume;
                Trionfi.instance.audioInstance[TRAudioID.SE1].mainVolume = TRGameConfig.configData.sevolume * TRGameConfig.configData.mastervolume;
                Trionfi.instance.audioInstance[TRAudioID.VOICE1].mainVolume = TRGameConfig.configData.voicevolume * TRGameConfig.configData.mastervolume;
            }
        }

        public void ResetDedault()
        {
            TRGameConfig.SetDefault();
            UpdateAll(true);
        }

        public void Exit()
        {
            Trionfi.Instance.PopWindow();
        }

        void Start()
        {
            mastervolumeSliderString = TRStageEnviroment.instance.GetUIText(TR_UITEXTID.CONFIG_MASTERVOLUME);
            bgmvolumeSliderString = TRStageEnviroment.instance.GetUIText(TR_UITEXTID.CONFIG_BGMVOLUME);
            sevolumeSliderString = TRStageEnviroment.instance.GetUIText(TR_UITEXTID.CONFIG_SEVOLUME);
            voicevolumeSliderString = TRStageEnviroment.instance.GetUIText(TR_UITEXTID.CONFIG_VOICEVOLUME);
            textspeedSliderString = TRStageEnviroment.instance.GetUIText(TR_UITEXTID.CONFIG_TEXTSPEED);
            autotextwaitSliderString= TRStageEnviroment.instance.GetUIText(TR_UITEXTID.CONFIG_AUTOWAIT);
            effectSkipToggleString = TRStageEnviroment.instance.GetUIText(TR_UITEXTID.CONFIG_EFFECTSKIP);
            readTextSkipToggleString= TRStageEnviroment.instance.GetUIText(TR_UITEXTID.CONFIG_READSKIP);
            initializeButtonString = TRStageEnviroment.instance.GetUIText(TR_UITEXTID.CONFIG_INITIALIZE);
                
            UpdateAll(false);
        }
    }
}
