﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TRGameConfig : SingletonMonoBehaviour<TRGameConfig> {
    const string prefsKey = "GameConfigs";
    
    [System.Serializable]
    public class GameConfigData
    {
        [Range(0.0f, 1.0f)]
        public float mastervolume = 1.0f;
        [Range(0.0f, 1.0f)]
        public float bgmvolume = 0.65f;
        [Range(0.0f, 1.0f)]
        public float sevolume = 0.8f;
        [Range(0.0f, 1.0f)]
        public float voicevolume = 0.8f;
        [Range(0.0f, 1.0f)]
        public float textspeed = 0.1f;
        [Range(0.0f, 10.0f)]
        public float autotextWait = 3.0f;
        public bool effectSkip = false;
        public bool readtextSkip = false;
    }

    [SerializeField]
    public GameConfigData configData = new GameConfigData();

    public void Save()
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
        configData.mastervolume = 1.0f;
        configData.bgmvolume = 0.6f;
        configData.sevolume = 0.75f;
        configData.voicevolume = 0.75f;
        configData.textspeed = 0.1f;
        configData.autotextWait = 5.0f;
        configData.effectSkip = false;
        configData.readtextSkip = false;
    }

    void Start()
    {
        Load();		
	}
	
	void Update ()
    {
		
	}

    private void OnDestroy()
    {
        Save();
    }
}
