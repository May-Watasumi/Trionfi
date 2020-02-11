using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TRGameConfigWindow : SingletonMonoBehaviour<TRGameConfigWindow> {
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
    /*
    public float masterVolume
    { get { return TRGameConfig.Instance.mastervolume; }  }

    public float bgmvolue
    { get { return masterVolume * configData.bgmvolume; } }

    public float sevolume
    { get { return masterVolume * configData.sevolume; } }

    public float voiceVolume
    { get { return masterVolume * configData.voicevolume; } }

    public float textSpeed
    { get { return configData.textspeed; } }

    public float autotextWait
    { get { return configData.autotextWait; } }

    public bool iseffectSkip
    { get { return configData.effectSkip; } }

    public bool isreadtextSkip
    { get { return configData.readtextSkip; } }
    */
    public void OnChangeSlider(int type)
    {
        switch(type)
        {
            case 0:
                TRGameConfig.configData.mastervolume = mastervolumeSlider.value;
                break;
            case 1:
                TRGameConfig.configData.bgmvolume = bgmvolumeSlider.value;
                break;
            case 2:
                TRGameConfig.configData.sevolume = sevolumeSlider.value;
                break;
            case 3:
                TRGameConfig.configData.voicevolume = voicevolumeSlider.value;
                break;
            case 4:
                TRGameConfig.configData.textspeed = textspeedSlider.value;
                break;
            case 5:
                TRGameConfig.configData.autotextWait= autotextwaitSlider.value;
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

    public void LoadParam()
    {
        mastervolumeSlider.value = TRGameConfig.configData.mastervolume;
        bgmvolumeSlider.value = TRGameConfig.configData.bgmvolume;
        sevolumeSlider.value = TRGameConfig.configData.sevolume;
        voicevolumeSlider.value = TRGameConfig.configData.voicevolume;
        textspeedSlider.value = TRGameConfig.configData.textspeed;
        autotextwaitSlider.value = TRGameConfig.configData.autotextWait;
        effectSkipToggle.isOn = TRGameConfig.configData.effectSkip;
        readTextSkipToggle.isOn = TRGameConfig.configData.readtextSkip;
    }

    public void ResetDedault()
    {
        TRGameConfig.SetDefault();
        LoadParam();
    }
    
    void Start()
    {
        TRGameConfig.Load();
        LoadParam();
    }

    void Update() { }
}
