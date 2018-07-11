using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TRGameConfig : MonoBehaviour {
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

    class GameConfigData
    {
        public float mastervolume;
        public float bgmvolume;
        public float sevolume;
        public float voicevolume;
        public float textspeed;
        public float autotextWait;
    }

    GameConfigData configData = new GameConfigData();

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

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
