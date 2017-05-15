using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NovelEx;

namespace NovelEx
{
    public class AudioObjectBase : MonoBehaviour
    {
        public float volume1;
        public float volume2;

        public virtual void Load(string resourceName) { }
        public virtual void Play(float time = 0.0f) { }
        public virtual void Stop(float time = 0.0f) { }
        public virtual IEnumerator FadeIn(float time) { return null; }
        public virtual IEnumerator FadeOut(float time) { return null; }
    }

    public class AudioObject : AudioObjectBase
	{
		public  AudioSource audioSource;
        private AudioClip audioClip;
		public bool isWait;
		public CompleteDelegate completeDelegate ;

        public override void Load(string resourceName)
		{
//			AudioClip audioClip = Resources.Load(resourceName, typeof(AudioClip)) as AudioClip;
//			AudioSource audioSource = this.gameObject.AddComponent<AudioSource>();
			audioSource.clip = audioClip;
//			audioSource = audioSource;
		}

        public override void Play(float time = 0.0f)
        {
            audioSource.Play();

            if(time > 0.0f)
                StartCoroutine(FadeIn(time));
		}

        public override void Stop(float time = 0.0f)
        {
            if(time > 0.0f)
                StartCoroutine(FadeOut(time));
            else
                audioSource.Stop();
		}
	}
}
