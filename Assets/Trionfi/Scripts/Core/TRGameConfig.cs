using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TRGameConfig : SingletonMonoBehaviour<TRGameConfig> {
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
    
    class GameConfigData
    {
        public float mastervolume;
        public float bgmvolume;
        public float sevolume;
        public float voicevolume;
        public float textspeed;
        public float autotextWait;
        public bool effectSkip;
        public bool readtextSkip;
    }

    GameConfigData configData = new GameConfigData();

    public float masterVolume
    { get { return configData.mastervolume; }  }

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

    public void OnChangeSlider(int type)
    {
        switch(type)
        {
            case 0:
                configData.mastervolume = mastervolumeSlider.value;
                break;
            case 1:
                configData.bgmvolume = bgmvolumeSlider.value;
                break;
            case 2:
                configData.sevolume = sevolumeSlider.value;
                break;
            case 3:
                configData.voicevolume = voicevolumeSlider.value;
                break;
            case 4:
                configData.textspeed = textspeedSlider.value;
                break;
            case 5:
                configData.autotextWait= autotextwaitSlider.value;
                break;
        }
    }

    public void OnChangeEffectSkip()
    {
        configData.effectSkip = effectSkipToggle.isOn;
    }

    public void OnChangeReadTextSkip()
    {
        configData.readtextSkip = readTextSkipToggle.isOn;
    }

    void Save()
    {
        string jsonString = JsonUtility.ToJson(configData);
        PlayerPrefs.SetString(prefsKey, jsonString);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        string jsonString = PlayerPrefs.GetString(prefsKey);
        if(!string.IsNullOrEmpty(jsonString))
            configData = JsonUtility.FromJson<GameConfigData>(jsonString);
    }

    public void SetDefault()
    {
        configData.mastervolume = 0.7f;
        configData.bgmvolume = 0.7f;
        configData.sevolume = 1.0f;
        configData.voicevolume = 1.0f;
        configData.textspeed = 0.1f;
        configData.autotextWait = 5.0f;
        configData.effectSkip = false;
        configData.readtextSkip = false;
    }

    public void Close(GameObject window)
    {
        window.SetActive(false);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDestroy()
    {
        Save();
    }
}
