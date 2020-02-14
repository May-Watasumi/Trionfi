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
        Slider mastervolumeSlider;
        [SerializeField]
        Slider bgmvolumeSlider;
        [SerializeField]
        Slider sevolumeSlider;
        [SerializeField]
        Slider voicevolumeSlider;
        [SerializeField]
        Slider textspeedSlider;
        [SerializeField]
        Slider autotextwaitSlider;
        [SerializeField]
        Toggle effectSkipToggle;
        [SerializeField]
        Toggle readTextSkipToggle;
  
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

        void Start()
        {
            UpdateAll(false);
        }
    }
}
