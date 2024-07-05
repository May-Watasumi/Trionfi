using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class TRMessageLogDataBase : MonoBehaviour
{
    protected abstract string nameString { get; set; }
    protected abstract string logString { get; set; }
    [SerializeField]
    public Image characterIcon;
    [SerializeField]
    public Button voicePlayButton;
    [SerializeField]
    public AudioClip voice;

    public void SetLogData(string sentence, string name, AudioClip _voice)
    {
        logString = sentence;
        nameString = name;

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
