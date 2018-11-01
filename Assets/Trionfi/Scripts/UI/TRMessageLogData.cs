using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TRMessageLogData : MonoBehaviour {
    [SerializeField]
    public Image characterIcon;

    [SerializeField]
    public LetterWriter.Unity.Components.LetterWriterText/* Text*/ logText;

    [SerializeField]
    AudioSource audioSource;

    public AudioClip voice;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetIcon(string name)
    {
        if (string.IsNullOrEmpty(name))
            characterIcon.enabled = false;
//        else
//        { /*ToDo*/}
    }

    public void PlayVoice()
    {
        if (voice != null)
        {
            audioSource.clip = voice;
            audioSource.Play();
        }
    }
}
