using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Trionfi
{
    public class TRGameConfigWindow : SingletonMonoBehaviour<TRGameConfigWindow>
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
        Slider textspeedSlider = null;
        [SerializeField]
        Slider autotextwaitSlider = null;
        [SerializeField]
        Toggle effectSkipToggle = null;
        [SerializeField]
        Toggle readTextSkipToggle = null;

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
        Text InitializeButtonText = null;

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
            mastervolumeSliderText.text = TRStageEnviroment.instance.GetUIText(TR_UITEXTID.CONFIG_MASTERVOLUME);
            bgmvolumeSliderText.text = TRStageEnviroment.instance.GetUIText(TR_UITEXTID.CONFIG_BGMVOLUME);
            sevolumeSliderText.text = TRStageEnviroment.instance.GetUIText(TR_UITEXTID.CONFIG_SEVOLUME);
            voicevolumeSliderText.text = TRStageEnviroment.instance.GetUIText(TR_UITEXTID.CONFIG_VOICEVOLUME);
            textspeedSliderText.text = TRStageEnviroment.instance.GetUIText(TR_UITEXTID.CONFIG_TEXTSPEED);
            autotextwaitSliderText.text = TRStageEnviroment.instance.GetUIText(TR_UITEXTID.CONFIG_AUTOWAIT);
            effectSkipToggleText.text = TRStageEnviroment.instance.GetUIText(TR_UITEXTID.CONFIG_EFFECTSKIP);
            readTextSkipToggleText.text = TRStageEnviroment.instance.GetUIText(TR_UITEXTID.CONFIG_READSKIP);
            InitializeButtonText.text = TRStageEnviroment.instance.GetUIText(TR_UITEXTID.CONFIG_INITIALIZE);

            UpdateAll(false);
        }
    }
}
