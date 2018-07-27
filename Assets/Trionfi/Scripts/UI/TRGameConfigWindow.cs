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
                TRGameConfig.Instance.configData.mastervolume = mastervolumeSlider.value;
                break;
            case 1:
                TRGameConfig.Instance.configData.bgmvolume = bgmvolumeSlider.value;
                break;
            case 2:
                TRGameConfig.Instance.configData.sevolume = sevolumeSlider.value;
                break;
            case 3:
                TRGameConfig.Instance.configData.voicevolume = voicevolumeSlider.value;
                break;
            case 4:
                TRGameConfig.Instance.configData.textspeed = textspeedSlider.value;
                break;
            case 5:
                TRGameConfig.Instance.configData.autotextWait= autotextwaitSlider.value;
                break;
        }
    }

    public void OnChangeEffectSkip()
    {
        TRGameConfig.Instance.configData.effectSkip = effectSkipToggle.isOn;
    }

    public void OnChangeReadTextSkip()
    {
        TRGameConfig.Instance.configData.readtextSkip = readTextSkipToggle.isOn;
    }
    
    void Start() { }
    void Update() { }
}
