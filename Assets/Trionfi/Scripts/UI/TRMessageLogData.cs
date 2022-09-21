using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TRMessageLogData : MonoBehaviour
{
    public string logName;

    [SerializeField]
    public Image characterIcon;
    [SerializeField]
    public Button voicePlayButton;

    [SerializeField]
    public LetterWriter.Unity.Components.LetterWriterText/* Text*/ logText;

    [SerializeField]
    public Text nameText;

    [SerializeField]
    AudioSource audioSource;

    public AudioClip voice;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetLogData(string name, AudioClip _voice)
    {
        nameText.text = name;

        if (string.IsNullOrEmpty(name))
            characterIcon.gameObject.SetActive(false);
        //        else
        //        { /*ToDo*/}

        if (_voice != null)
        {
            voice = _voice;
            voicePlayButton.gameObject.SetActive(true);
        }
        else
            voicePlayButton.gameObject.SetActive(false);
    }

    public void PlayVoice()
    {
        if (voice != null)
        {
            Trionfi.Trionfi.Instance.audioInstance[Trionfi.TRAudioID.VOICE1].instance.clip = voice;
            Trionfi.Trionfi.Instance.audioInstance[Trionfi.TRAudioID.VOICE1].instance.Play();

            // audioSource.clip = voice;
            // audioSource.Play();
        }
    }
}
