using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Trionfi
{
//Singleton
    public class TRGameConfig
    {
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
			public float systemvoicevolume = 1.0f;
			[Range(0.0f, 1.0f)]
            public float textspeed = 0.1f;
            [Range(0.0f, 10.0f)]
            public float autotextWait = 3.0f;
            public bool effectSkip = false;
            public bool readtextSkip = false;
        }

        public static GameConfigData configData = new GameConfigData();

        public static void Save()
        {
            string jsonString = JsonUtility.ToJson(configData);
            PlayerPrefs.SetString(Trionfi.Instance.titleName + "/" +  prefsKey, jsonString);
            PlayerPrefs.Save();
        }

        public static void Load()
        {
            string jsonString = PlayerPrefs.GetString(Trionfi.Instance.titleName + "/" + prefsKey);

            if (!string.IsNullOrEmpty(jsonString))
                configData = JsonUtility.FromJson<GameConfigData>(jsonString);
            else
                SetDefault();
        }

        public static void SetDefault()
        {
            configData.mastervolume = 1.0f;
            configData.bgmvolume = 0.6f;
            configData.sevolume = 0.75f;
            configData.voicevolume = 0.75f;
            configData.systemvoicevolume = 1.0f;
            configData.textspeed = 0.1f;
            configData.autotextWait = 5.0f;
            configData.effectSkip = false;
            configData.readtextSkip = false;
        }

        public static void Initialize() { Load(); }
    }
}
